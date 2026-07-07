using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Data.Seed;
using OnevoHr.Api.DTOs;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Models.OrgStructure;
using OnevoHr.Api.Repositories.Implementations;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;
using Xunit;

namespace backend.Tests;

/// <summary>
/// Integration tests for the onboarding draft save/resume/checklist/validate/
/// finalize/send-invite/blocked-state/outbox flow, following the canonical
/// OneVo-HR docs:
///   - modules/core-hr/onboarding/overview.md
///   - modules/core-hr/onboarding/end-to-end-logic.md
///   - backend/notification-system.md
/// </summary>
public class OnboardingTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OnboardingTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });
        _client.DefaultRequestHeaders.Add("X-Tenant-Domain", "acme.test");
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    /// <summary>
    /// Seeds the database (idempotent) and logs in as the HR Admin user.
    /// Returns the CSRF token extracted from the Set-Cookie header.
    /// </summary>
    private async Task<string> AuthenticateAsHrAdmin()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!await db.PlatformUsers.AnyAsync())
        {
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await DatabaseSeeder.SeedAsync(db, hasher);
        }

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = "hr.admin@acme.test", password = "Password123!" });

        if (!loginResponse.IsSuccessStatusCode)
        {
            var body = await loginResponse.Content.ReadAsStringAsync();
            throw new Exception($"HR Admin login failed: {loginResponse.StatusCode} — {body}");
        }

        var csrfToken = ExtractCsrfToken(loginResponse);
        _client.DefaultRequestHeaders.Remove("X-CSRF-Token");
        _client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);
        return csrfToken;
    }

    private static string ExtractCsrfToken(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
            throw new Exception("No Set-Cookie header on login response");
        foreach (var cookie in cookies)
        {
            if (cookie.StartsWith("onevo_csrf=", StringComparison.OrdinalIgnoreCase))
                return cookie.Split(';')[0].Split('=')[1];
        }
        throw new Exception("onevo_csrf cookie not found in login response");
    }

    private async Task<AppDbContext> GetDbAsync()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    private async Task<(Guid TenantId, Guid LegalEntityId)> GetAcmeTenantContextAsync()
    {
        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);
        return (acme.Id, le.Id);
    }

    private async Task<OnboardingDraft> SeedDraftAsync(
        Guid tenantId,
        Guid startedById,
        string email,
        string draftReason = "saved_manually",
        string status = "draft",
        Guid? legalEntityId = null)
    {
        var db = await GetDbAsync();
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EmployeeName = "Test Employee",
            WorkEmail = email,
            Status = status,
            DraftReason = draftReason,
            LastSavedStep = "employee_details",
            StartedById = startedById,
            EditedTasksJson = "[]",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            LegalEntityId = legalEntityId
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();
        return draft;
    }

    // ------------------------------------------------------------------
    // 1. Auth / CSRF / Permission enforcement
    // ------------------------------------------------------------------

    [Fact]
    public async Task SaveDraft_Unauthenticated_Returns401()
    {
        // Fresh client with no session cookie
        var unauthClient = _factory.CreateClient();
        unauthClient.DefaultRequestHeaders.Add("X-Tenant-Domain", "acme.test");

        var response = await unauthClient.PostAsJsonAsync("/api/v1/onboarding/drafts",
            new { employeeName = "Test", workEmail = "unauth@test.test", lastSavedStep = "employee_details" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SaveDraft_AuthenticatedWithoutCsrfHeader_Returns403()
    {
        // Log in but do NOT add the CSRF header
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!await db.PlatformUsers.AnyAsync())
        {
            var h = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await DatabaseSeeder.SeedAsync(db, h);
        }

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Tenant-Domain", "acme.test");

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = "hr.admin@acme.test", password = "Password123!" });
        loginResponse.EnsureSuccessStatusCode();
        // Deliberately do NOT add X-CSRF-Token header

        var response = await client.PostAsJsonAsync("/api/v1/onboarding/drafts",
            new { employeeName = "Test", workEmail = "nocsrf@test.test", lastSavedStep = "employee_details" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SaveDraft_AuthenticatedUserWithoutEmployeesWrite_Returns403()
    {
        // Log in as an employee (not HR admin) who lacks employees:write
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!await db.PlatformUsers.AnyAsync())
        {
            var h = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await DatabaseSeeder.SeedAsync(db, h);
        }

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Tenant-Domain", "acme.test");

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = "employee@acme.test", password = "Password123!" });
        loginResponse.EnsureSuccessStatusCode();

        var csrfToken = ExtractCsrfToken(loginResponse);
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);

        var response = await client.PostAsJsonAsync("/api/v1/onboarding/drafts",
            new { employeeName = "Test", workEmail = "noperm@test.test", lastSavedStep = "employee_details" });

        // Employee role does not have employees:write — must be 403
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ------------------------------------------------------------------
    // 2. Draft save — creates only the draft row, nothing else
    // ------------------------------------------------------------------

    [Fact]
    public async Task SaveDraft_CreatesOnboardingDraftRow_AndDoesNotCreateEmployee()
    {
        await AuthenticateAsHrAdmin();

        var email = $"draft-{Guid.NewGuid():N}@acme.test";
        var request = new SaveOnboardingDraftRequest
        {
            EmployeeName = "Draft User",
            WorkEmail = email,
            EmploymentType = "full_time",
            LastSavedStep = "employee_details"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/onboarding/drafts", request);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var draftId = Guid.Parse(doc.RootElement.GetProperty("id").GetString()!);

        var db = await GetDbAsync();

        // Draft must exist with correct status
        var draft = await db.OnboardingDrafts.FindAsync(draftId);
        Assert.NotNull(draft);
        Assert.Equal("draft", draft!.Status);
        Assert.Equal("saved_manually", draft.DraftReason);
        Assert.Equal("employee_details", draft.LastSavedStep);

        // No employee, user, invite, email log, or outbox event must be created
        Assert.Null(await db.Employees.FirstOrDefaultAsync(e => e.WorkEmail == email));
        Assert.Null(await db.Users.FirstOrDefaultAsync(u => u.Email == email));
        Assert.Null(await db.InvitationTokens.FirstOrDefaultAsync(i => i.InvitedEmail == email));
        Assert.Null(await db.EmailDeliveryLogs.FirstOrDefaultAsync(e => e.RecipientEmail == email));
        Assert.False(await db.OutboxMessages.AnyAsync(m =>
            m.Type == "EmployeeOnboardingStarted" && m.PayloadJson.Contains(email)));
    }

    [Fact]
    public async Task SaveDraft_StoresSelectedTemplateId_EditedTasksJson_LastSavedStep()
    {
        await AuthenticateAsHrAdmin();

        var email = $"tasksave-{Guid.NewGuid():N}@acme.test";
        var tasks = "[{\"title\":\"IT setup\",\"ownerType\":\"hr\"}]";

        var request = new SaveOnboardingDraftRequest
        {
            EmployeeName = "Task Save User",
            WorkEmail = email,
            EmploymentType = "full_time",
            LastSavedStep = "checklist_review",
            EditedTasksJson = tasks
        };

        var response = await _client.PostAsJsonAsync("/api/v1/onboarding/drafts", request);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var draftId = Guid.Parse(doc.RootElement.GetProperty("id").GetString()!);

        var db = await GetDbAsync();
        var draft = await db.OnboardingDrafts.FindAsync(draftId);

        Assert.NotNull(draft);
        Assert.Equal(tasks, draft!.EditedTasksJson);
        Assert.Equal("checklist_review", draft.LastSavedStep);
        Assert.Null(draft.SelectedTemplateId); // not supplied in this request
    }

    // ------------------------------------------------------------------
    // 3. Tenant isolation — tenant B cannot access tenant A draft
    // ------------------------------------------------------------------

    [Fact]
    public async Task GetDraft_TenantB_CannotAccessTenantA_Draft()
    {
        // Authenticate as Acme (tenant A) HR admin
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");

        // Create a draft belonging to tenant A
        var email = $"isolation-{Guid.NewGuid():N}@acme.test";
        var draftA = await SeedDraftAsync(acme.Id, adminUser.Id, email);

        // Create a second fake tenant (tenant B) in the same DB
        // and try to load tenant A's draft using a forged slug
        var tenantB = new OnevoHr.Api.Models.Tenant.Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Other Corp",
            Slug = $"othercorp-{Guid.NewGuid():N}",
            Status = "active",
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Tenants.Add(tenantB);
        await db.SaveChangesAsync();

        // The current session is for Acme, so requesting a draft from tenant B's
        // perspective via the API is impossible without re-logging in as tenant B.
        // Instead, we verify that the REPOSITORY enforces tenant isolation:
        // the draft was seeded with acme.Id, so any service call that uses the
        // current user's tenantId cannot return it when running as tenant B.
        // We test this by calling the API endpoint with the current Acme session
        // and confirming the draft IS found (happy path), then creating a draft
        // for tenantB's ID directly and confirming it is NOT returned for Acme.

        var draftForOtherTenant = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB.Id,
            EmployeeName = "Other Corp Employee",
            WorkEmail = $"other-{Guid.NewGuid():N}@other.test",
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "employee_details",
            StartedById = Guid.NewGuid(), // random — tenant B user
            EditedTasksJson = "[]",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draftForOtherTenant);
        await db.SaveChangesAsync();

        // Tenant A session must NOT be able to retrieve tenant B's draft
        var response = await _client.GetAsync($"/api/v1/onboarding/drafts/{draftForOtherTenant.Id}");

        // The repository filters by (tenantId = currentTenantId AND id = draftId),
        // so this returns null → 404.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // But tenant A's own draft must be reachable
        var ownResponse = await _client.GetAsync($"/api/v1/onboarding/drafts/{draftA.Id}");
        Assert.Equal(HttpStatusCode.OK, ownResponse.StatusCode);
    }

    // ------------------------------------------------------------------
    // 4. Checklist update
    // ------------------------------------------------------------------

    [Fact]
    public async Task UpdateChecklist_Persists_EditedTasksJson_And_Sets_LastSavedStep()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var draft = await SeedDraftAsync(acme.Id, adminUser.Id, $"checklist-{Guid.NewGuid():N}@acme.test");

        var tasks = "[{\"title\":\"Laptop setup\",\"ownerType\":\"it\"},{\"title\":\"HR orientation\",\"ownerType\":\"hr\"}]";
        var request = new UpdateChecklistDraftRequest { EditedTasksJson = tasks };

        var response = await _client.PostAsJsonAsync($"/api/v1/onboarding/drafts/{draft.Id}/checklist", request);
        response.EnsureSuccessStatusCode();

        var updated = await db.OnboardingDrafts.FindAsync(draft.Id);
        Assert.NotNull(updated);
        Assert.Equal(tasks, updated!.EditedTasksJson);
        Assert.Equal("checklist_review", updated.LastSavedStep);
    }

    // ------------------------------------------------------------------
    // 5. Validation endpoint
    // ------------------------------------------------------------------

    [Fact]
    public async Task ValidateDraft_DuplicateWorkEmail_ReturnsError()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");

        // hr.admin@acme.test already exists as a user in seed data
        var draft = await SeedDraftAsync(acme.Id, adminUser.Id, "hr.admin@acme.test");

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/validate", null);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var isValid = doc.RootElement.GetProperty("isValid").GetBoolean();

        Assert.False(isValid);
        // Must contain at least one error about the duplicate email
        var errors = doc.RootElement.GetProperty("errors");
        var hasEmailError = false;
        foreach (var e in errors.EnumerateArray())
        {
            if (e.GetProperty("code").GetString()!.Contains("EMAIL", StringComparison.OrdinalIgnoreCase) ||
                e.GetProperty("field").GetString()!.Contains("workEmail", StringComparison.OrdinalIgnoreCase))
            {
                hasEmailError = true;
                break;
            }
        }
        Assert.True(hasEmailError, "Expected a validation error about duplicate work email");
    }

    [Fact]
    public async Task ValidateDraft_InvalidEditedTasksJson_ReturnsError()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");

        var email = $"badtasks-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Bad Tasks Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "checklist_review",
            // Intentionally invalid JSON
            EditedTasksJson = "NOT_VALID_JSON",
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/validate", null);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.False(doc.RootElement.GetProperty("isValid").GetBoolean());
        var errors = doc.RootElement.GetProperty("errors").GetArrayLength();
        Assert.True(errors > 0, "Expected at least one error for invalid tasks JSON");
    }

    [Fact]
    public async Task ValidateDraft_ValidDraft_ReturnsIsValidTrue()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        var email = $"valid-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Valid Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "checklist_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/validate", null);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.GetProperty("isValid").GetBoolean(),
            $"Expected isValid=true. Response: {body}");
    }

    // ------------------------------------------------------------------
    // 6. Blocking states: no seat
    // ------------------------------------------------------------------

    [Fact]
    public async Task SendInvite_WhenNoSeat_Sets_DraftReason_WaitingForSeat_CreatesNoDownstreamRows()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        // Seed a resource limit with EmployeeLimit=0 so the next finalize is always blocked
        // by no-seat. Use a uniquely identifiable limit so we can clean it up conceptually.
        var limit = new TenantResourceLimit
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeLimit = 0, // zero seats — always blocked
            StorageLimitGb = 100,
            AiTokenLimit = 1_000_000,
            Source = "test_no_seat",
            CreatedAtUtc = DateTime.UtcNow
        };
        // Seed the in-memory limits used by SubscriptionRepository
        SubscriptionRepository.SeedInMemoryLimits(new List<TenantResourceLimit> { limit });

        try
        {
            var email = $"noseat-{Guid.NewGuid():N}@acme.test";
            var draft = new OnboardingDraft
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                EmployeeName = "No Seat Employee",
                WorkEmail = email,
                Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        // Blocked returns 200 so frontend can read the draftReason
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("blocked", doc.RootElement.GetProperty("status").GetString());
        Assert.Equal("waiting_for_seat", doc.RootElement.GetProperty("draftReason").GetString());

        var db2 = await GetDbAsync();
        // Draft reason must be persisted
        var updated = await db2.OnboardingDrafts.FindAsync(draft.Id);
        Assert.Equal("waiting_for_seat", updated!.DraftReason);

        // No downstream rows must have been created
        Assert.Null(await db2.Employees.FirstOrDefaultAsync(e => e.WorkEmail == email));
        Assert.Null(await db2.Users.FirstOrDefaultAsync(u => u.Email == email));
        Assert.Null(await db2.InvitationTokens.FirstOrDefaultAsync(i => i.InvitedEmail == email));
        Assert.Null(await db2.EmailDeliveryLogs.FirstOrDefaultAsync(e => e.RecipientEmail == email));
        Assert.False(await db2.OutboxMessages.AnyAsync(m => m.Type == "EmployeeOnboardingStarted" && m.PayloadJson.Contains(email)));
        }
        finally
        {
            // Reset limit for other tests
            SubscriptionRepository.SeedInMemoryLimits(new List<TenantResourceLimit>
            {
                new TenantResourceLimit
                {
                    Id = Guid.NewGuid(), TenantId = acme.Id,
                    EmployeeLimit = null, Source = "test_reset",
                    CreatedAtUtc = DateTime.UtcNow.AddYears(1)
                }
            });
        }
    }

    [Fact]
    public async Task RequestSeat_OnlyWorks_When_DraftReason_Is_WaitingForSeat()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");

        // Draft with wrong reason (saved_manually) — should fail
        var wrongDraft = await SeedDraftAsync(acme.Id, adminUser.Id,
            $"wrong-{Guid.NewGuid():N}@acme.test", draftReason: "saved_manually");

        var wrongResponse = await _client.PostAsync(
            $"/api/v1/onboarding/drafts/{wrongDraft.Id}/request-seat", null);
        Assert.Equal(HttpStatusCode.BadRequest, wrongResponse.StatusCode);

        // Draft with correct reason — should succeed
        var correctDraft = await SeedDraftAsync(acme.Id, adminUser.Id,
            $"correct-{Guid.NewGuid():N}@acme.test", draftReason: "waiting_for_seat");

        var correctResponse = await _client.PostAsync(
            $"/api/v1/onboarding/drafts/{correctDraft.Id}/request-seat", null);
        correctResponse.EnsureSuccessStatusCode();

        var body = await correctResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("ok", doc.RootElement.GetProperty("status").GetString());
    }

    // ------------------------------------------------------------------
    // 7. Blocking states: sensitive position approval
    // ------------------------------------------------------------------

    [Fact]
    public async Task SendInvite_SensitivePosition_Sets_WaitingForPositionApproval_CreatesNoDownstreamRows()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        // Make sure seat is available: seed unlimited limit
        SubscriptionRepository.SeedInMemoryLimits(new List<TenantResourceLimit>
        {
            new TenantResourceLimit
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                EmployeeLimit = null, // unlimited
                StorageLimitGb = 100,
                AiTokenLimit = 1_000_000,
                Source = "test_unlimited",
                CreatedAtUtc = DateTime.UtcNow
            }
        });

        // Create a role to link to the position access template
        var role = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            Name = "Sensitive Role",
            Description = "Test sensitive role",
            IsSystemRole = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Roles.Add(role);

        // Create a dept and position
        var dept = await db.Departments.FirstAsync(d => d.TenantId == acme.Id);
        var sensitivePosition = new Position
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            LegalEntityId = le.Id,
            DepartmentId = dept.Id,
            Name = $"Sensitive Position {Guid.NewGuid():N}",
            Code = $"SENS-{Guid.NewGuid().ToString("N").Substring(0, 6)}",
            Capacity = 1,
            PositionType = "unique",
            Status = "active",
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Positions.Add(sensitivePosition);

        // Attach a PositionAccessTemplate that requires approval
        var pat = new PositionAccessTemplate
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            PositionId = sensitivePosition.Id,
            RoleId = role.Id,
            RequiresApproval = true,
            IsSensitive = true,
            IsActive = true,
            EffectiveFromRule = "hire_date",
            CreatedBy = adminUser.Id,
            CreatedAt = DateTimeOffset.UtcNow
        };
        db.PositionAccessTemplates.Add(pat);
        await db.SaveChangesAsync();

        var email = $"sensitive-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Sensitive Position Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            PositionId = sensitivePosition.Id,
            DepartmentId = dept.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("blocked", doc.RootElement.GetProperty("status").GetString());
        Assert.Equal("waiting_for_position_approval",
            doc.RootElement.GetProperty("draftReason").GetString());

        var db2 = await GetDbAsync();
        // Draft must have persisted the new reason
        var updated = await db2.OnboardingDrafts.FindAsync(draft.Id);
        Assert.Equal("waiting_for_position_approval", updated!.DraftReason);

        // No downstream rows
        Assert.Null(await db2.Employees.FirstOrDefaultAsync(e => e.WorkEmail == email));
        Assert.Null(await db2.Users.FirstOrDefaultAsync(u => u.Email == email));
        Assert.False(await db2.OutboxMessages.AnyAsync(m =>
            m.Type == "EmployeeOnboardingStarted" && m.PayloadJson.Contains(email)));
    }

    [Fact]
    public async Task SubmitApproval_OnlyWorks_When_DraftReason_Is_WaitingForPositionApproval()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");

        // Draft with wrong reason — must fail
        var wrongDraft = await SeedDraftAsync(acme.Id, adminUser.Id,
            $"wrong-approval-{Guid.NewGuid():N}@acme.test", draftReason: "saved_manually");

        var wrongResponse = await _client.PostAsync(
            $"/api/v1/onboarding/drafts/{wrongDraft.Id}/submit-approval", null);
        Assert.Equal(HttpStatusCode.BadRequest, wrongResponse.StatusCode);

        // Draft with correct reason — must succeed
        var correctDraft = await SeedDraftAsync(acme.Id, adminUser.Id,
            $"correct-approval-{Guid.NewGuid():N}@acme.test",
            draftReason: "waiting_for_position_approval");

        var correctResponse = await _client.PostAsync(
            $"/api/v1/onboarding/drafts/{correctDraft.Id}/submit-approval", null);
        correctResponse.EnsureSuccessStatusCode();

        var body = await correctResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("ok", doc.RootElement.GetProperty("status").GetString());
    }

    // ------------------------------------------------------------------
    // 8. Finalize / send-invite — happy path
    // ------------------------------------------------------------------

    [Fact]
    public async Task SendInvite_WhenClear_Creates_Employee_User_InviteToken_Lifecycle_EmailLog_OutboxEvent()
    {
        await AuthenticateAsHrAdmin();

        // Acme is seeded with EmployeeLimit = null (unlimited seats) — no extra seeding needed.
        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        var email = $"finalize-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Finalize Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[{\"title\":\"Complete HR forms\",\"ownerType\":\"hr\"}]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("completed", doc.RootElement.GetProperty("status").GetString());

        // Use a fresh scope so EF's change tracker doesn't return stale cached state.
        var db2 = await GetDbAsync();

        // Employee created with status=onboarding
        var employee = await db2.Employees.FirstOrDefaultAsync(e => e.WorkEmail == email);
        Assert.NotNull(employee);
        Assert.Equal("onboarding", employee!.Status);

        // User created and inactive (setup-required)
        var user = await db2.Users.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.False(user!.IsActive);

        // Invitation token: hash stored, not empty, and no raw token in DB
        var invite = await db2.InvitationTokens.FirstOrDefaultAsync(i => i.InvitedEmail == email);
        Assert.NotNull(invite);
        Assert.NotEmpty(invite!.TokenHash);
        Assert.Equal("pending", invite.Status);

        // Employee lifecycle event
        var lifecycle = await db2.EmployeeLifecycleEvents.FirstOrDefaultAsync(le2 =>
            le2.EmployeeId == employee.Id && le2.EventType == "hired");
        Assert.NotNull(lifecycle);

        // Checklist task
        var task = await db2.EmployeeChecklistTasks.FirstOrDefaultAsync(t => t.EmployeeId == employee.Id);
        Assert.NotNull(task);
        Assert.Equal("pending", task!.Status);
        Assert.Equal("onboarding", task.LifecycleType);

        // Email delivery log: queued before dispatch
        var emailLog = await db2.EmailDeliveryLogs.FirstOrDefaultAsync(e => e.RecipientEmail == email);
        Assert.NotNull(emailLog);
        Assert.Equal("queued", emailLog!.Status);
        Assert.Equal("local_dev", emailLog.Provider);

        // Draft status = completed after successful finalize
        var finalized = await db2.OnboardingDrafts.FirstOrDefaultAsync(d => d.Id == draft.Id);
        Assert.Equal("completed", finalized!.Status);

        // Dev invite URL in response (only returned in dev/local_dev mode)
        var hasDevUrl = doc.RootElement.TryGetProperty("dev_invite_url", out var urlProp);
        Assert.True(hasDevUrl && !string.IsNullOrWhiteSpace(urlProp.GetString()),
            "Expected dev_invite_url in response");
    }

    [Fact]
    public async Task SendInvite_InvitationToken_Stores_TokenHash_Not_RawToken()
    {
        await AuthenticateAsHrAdmin();

        // Acme is seeded with EmployeeLimit = null (unlimited) — no extra seeding needed.
        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        var email = $"tokentest-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Token Test Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        response.EnsureSuccessStatusCode();

        var invite = await db.InvitationTokens.FirstOrDefaultAsync(i => i.InvitedEmail == email);
        Assert.NotNull(invite);

        // TokenHash must be exactly 64 hex characters (SHA-256 output)
        Assert.Matches("^[0-9a-f]{64}$", invite!.TokenHash);

        // The raw token extracted from the dev URL must NOT match the stored hash
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var devUrl = doc.RootElement.GetProperty("dev_invite_url").GetString()!;
        var rawToken = Uri.UnescapeDataString(devUrl.Split("token=")[1]);

        // Confirm the stored value is the HASH, not the raw token itself
        Assert.NotEqual(rawToken, invite.TokenHash);
    }

    [Fact]
    public async Task SendInvite_User_IsInactive_SetupRequired_NoPassword()
    {
        await AuthenticateAsHrAdmin();

        // Acme is seeded with EmployeeLimit = null (unlimited) — no extra seeding needed.
        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        var email = $"inactive-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Inactive Test User",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var inviteResponse = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        inviteResponse.EnsureSuccessStatusCode();
        var inviteBody = await inviteResponse.Content.ReadAsStringAsync();
        using var inviteDoc = JsonDocument.Parse(inviteBody);
        Assert.Equal("completed", inviteDoc.RootElement.GetProperty("status").GetString());

        // Fresh scope to avoid stale EF tracking
        var db2 = await GetDbAsync();
        var user = await db2.Users.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        // Per docs: user is inactive/setup-required until the invite is accepted
        Assert.False(user!.IsActive);
        // No real password must be set — empty or placeholder only
        Assert.True(string.IsNullOrEmpty(user.PasswordHash),
            "Invited user must not have a real password hash set");
    }

    [Fact]
    public async Task SendInvite_DraftStatus_BecomesCompleted_Only_After_AllCreation_Succeeds()
    {
        await AuthenticateAsHrAdmin();

        // Acme is seeded with EmployeeLimit = null (unlimited) — no extra seeding needed.
        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        var email = $"commit-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Commit Test Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        response.EnsureSuccessStatusCode();

        // Fresh scope to avoid stale EF tracking
        var db2 = await GetDbAsync();
        var finalized = await db2.OnboardingDrafts.FirstOrDefaultAsync(d => d.Id == draft.Id);
        Assert.Equal("completed", finalized!.Status);
    }

    // ------------------------------------------------------------------
    // 9. Outbox events
    // ------------------------------------------------------------------

    [Fact]
    public async Task SendInvite_EmployeeOnboardingStarted_Persisted_After_Successful_Finalize()
    {
        await AuthenticateAsHrAdmin();

        // Acme is seeded with EmployeeLimit = null (unlimited) — no extra seeding needed.
        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        var email = $"outbox-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Outbox Test Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        response.EnsureSuccessStatusCode();

        // Fresh scope to avoid stale EF tracking
        var db2 = await GetDbAsync();
        var outboxEvent = await db2.OutboxMessages.FirstOrDefaultAsync(m => m.Type == "EmployeeOnboardingStarted");
        Assert.NotNull(outboxEvent);
        Assert.Equal("pending", outboxEvent!.Status);
        Assert.Equal(0, outboxEvent.RetryCount);
        Assert.Equal(acme.Id, outboxEvent.TenantId);

        // Payload must not contain the raw token
        Assert.DoesNotContain("rawToken", outboxEvent.PayloadJson, StringComparison.OrdinalIgnoreCase);
        // Payload must contain key identifiers
        Assert.Contains("employeeId", outboxEvent.PayloadJson, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("draftId", outboxEvent.PayloadJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendInvite_ValidationFails_NoOutboxEvent_Created()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");

        // Use a work email that already exists (hr.admin@acme.test) — will fail validation
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Duplicate Email Employee",
            WorkEmail = "hr.admin@acme.test", // duplicate
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var countBefore = await db.OutboxMessages.CountAsync(m => m.Type == "EmployeeOnboardingStarted");

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        // Invalid draft returns 400
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var countAfter = await db.OutboxMessages.CountAsync(m => m.Type == "EmployeeOnboardingStarted");
        Assert.Equal(countBefore, countAfter); // no new outbox event
    }



    // ------------------------------------------------------------------
    // 10. Email delivery logs
    // ------------------------------------------------------------------

    [Fact]
    public async Task SendInvite_EmailLog_Contains_Recipient_Subject_And_Body_With_InviteLink()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        var email = $"emailbody-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Email Body Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        response.EnsureSuccessStatusCode();

        var db2 = await GetDbAsync();
        var emailLog = await db2.EmailDeliveryLogs.FirstOrDefaultAsync(e => e.RecipientEmail == email);
        Assert.NotNull(emailLog);

        // Recipient, subject, and rendered bodies must all be snapshotted
        Assert.Equal(email, emailLog!.RecipientEmail);
        Assert.Equal("You're invited to OneVo", emailLog.SubjectSnapshot);
        Assert.False(string.IsNullOrWhiteSpace(emailLog.BodyHtmlSnapshot));
        Assert.False(string.IsNullOrWhiteSpace(emailLog.BodyTextSnapshot));

        // Both bodies must contain the accept-invite link
        Assert.Contains("/accept-invite?token=", emailLog.BodyHtmlSnapshot);
        Assert.Contains("/accept-invite?token=", emailLog.BodyTextSnapshot);
    }

    [Fact]
    public async Task SendInvite_RawToken_Only_In_EmailBody_And_DevUrl_Never_In_TokenRow_Or_Outbox()
    {
        await AuthenticateAsHrAdmin();

        var db = await GetDbAsync();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var adminUser = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);

        var email = $"rawtoken-{Guid.NewGuid():N}@acme.test";
        var draft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeName = "Raw Token Employee",
            WorkEmail = email,
            Status = "draft",
            DraftReason = "saved_manually",
            LastSavedStep = "final_review",
            EditedTasksJson = "[]",
            LegalEntityId = le.Id,
            StartedById = adminUser.Id,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        db.OnboardingDrafts.Add(draft);
        await db.SaveChangesAsync();

        var response = await _client.PostAsync($"/api/v1/onboarding/drafts/{draft.Id}/send-invite", null);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var devUrl = doc.RootElement.GetProperty("dev_invite_url").GetString()!;
        var rawToken = Uri.UnescapeDataString(devUrl.Split("token=")[1]);

        var db2 = await GetDbAsync();

        // invitation_tokens stores only the hash, never the raw token
        var invite = await db2.InvitationTokens.FirstOrDefaultAsync(i => i.InvitedEmail == email);
        Assert.NotNull(invite);
        Assert.NotEqual(rawToken, invite!.TokenHash);
        Assert.DoesNotContain(rawToken, invite.TokenHash);

        // Outbox payload must not contain the raw token
        var outboxEvent = await db2.OutboxMessages.FirstOrDefaultAsync(m =>
            m.Type == "EmployeeOnboardingStarted" && m.PayloadJson.Contains(email));
        Assert.NotNull(outboxEvent);
        Assert.DoesNotContain(rawToken, outboxEvent!.PayloadJson);

        // The email body snapshots ARE allowed to contain the raw token —
        // that is how the recipient receives the accept-invite link.
        var emailLog = await db2.EmailDeliveryLogs.FirstOrDefaultAsync(e => e.RecipientEmail == email);
        Assert.NotNull(emailLog);
        Assert.Contains(Uri.EscapeDataString(rawToken), emailLog!.BodyTextSnapshot);
    }

    [Fact]
    public async Task ProcessEmails_Requires_Auth_Permission()
    {
        // Unauthenticated — must be 401
        var unauthClient = _factory.CreateClient();
        unauthClient.DefaultRequestHeaders.Add("X-Tenant-Domain", "acme.test");
        var response = await unauthClient.PostAsync("/api/v1/outbox/process-emails", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProcessEmails_HrAdmin_WithNotificationsManage_Succeeds()
    {
        // HR Admin has notifications:manage in the seeded role definitions
        await AuthenticateAsHrAdmin();
        var response = await _client.PostAsync("/api/v1/outbox/process-emails", null);
        response.EnsureSuccessStatusCode();
    }
}
