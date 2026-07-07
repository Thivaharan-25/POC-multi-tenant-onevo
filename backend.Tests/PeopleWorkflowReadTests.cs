using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnevoHr.Api.Data;
using OnevoHr.Api.Data.Seed;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;
using Xunit;

namespace backend.Tests;

/// <summary>
/// Covers the additive read endpoints added for the People/Employees/Onboarding
/// rework: scoped+filterable employee list, drafts/mine isolation, and
/// checklist-template read/immutability.
/// </summary>
public class PeopleWorkflowReadTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public PeopleWorkflowReadTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });
        _client.DefaultRequestHeaders.Add("X-Tenant-Domain", "acme.test");
    }

    private async Task AuthenticateAsHrAdmin()
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
        var csrfToken = ExtractCsrfToken(loginResponse);
        _client.DefaultRequestHeaders.Remove("X-CSRF-Token");
        _client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);
    }

    private static string ExtractCsrfToken(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            throw new Exception("No Set-Cookie header on login response");
        }
        foreach (var cookie in cookies)
        {
            if (cookie.StartsWith("onevo_csrf=", StringComparison.OrdinalIgnoreCase))
            {
                var start = cookie.IndexOf('=') + 1;
                var end = cookie.IndexOf(';');
                return end > start ? cookie.Substring(start, end - start) : cookie.Substring(start);
            }
        }
        throw new Exception("No CSRF cookie found");
    }

    [Fact]
    public async Task GetEmployees_WithoutEmployeesReadPermission_Returns403()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seeded = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Tenant-Domain", $"{seeded.TenantSlug}.test");

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = seeded.UserEmail, password = seeded.UserPassword });
        var csrfToken = ExtractCsrfToken(loginResponse);
        client.DefaultRequestHeaders.Remove("X-CSRF-Token");
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);

        var response = await client.GetAsync("/api/v1/employees");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetEmployees_ReturnsDepartmentAndPositionNames()
    {
        await AuthenticateAsHrAdmin();

        var response = await _client.GetAsync("/api/v1/employees");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("departmentName", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("positionName", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetMyDrafts_ExcludesDraftsStartedByOtherUsers()
    {
        await AuthenticateAsHrAdmin();

        var uniqueName = $"Mine Only {Guid.NewGuid():N}";
        var saveResponse = await _client.PostAsJsonAsync("/api/v1/onboarding/drafts", new
        {
            employeeName = uniqueName,
            workEmail = $"mine-{Guid.NewGuid():N}@acme.test",
            lastSavedStep = "employee_details"
        });
        saveResponse.EnsureSuccessStatusCode();

        // Clone the HR admin's role onto a brand-new user in the same tenant, so the
        // second login has employees:write without depending on any other fixture user.
        Guid secondUserId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var hrAdmin = await db.Users.FirstAsync(u => u.Email == "hr.admin@acme.test");
            var hrAdminRole = await db.UserRoles.FirstAsync(ur => ur.UserId == hrAdmin.Id);

            var secondUser = new User
            {
                Id = Guid.NewGuid(),
                TenantId = hrAdmin.TenantId,
                Email = $"second-hr-{Guid.NewGuid():N}@acme.test",
                PasswordHash = hasher.Hash("Password123!"),
                DisplayName = "Second HR User",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            db.Users.Add(secondUser);
            db.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), UserId = secondUser.Id, RoleId = hrAdminRole.RoleId });
            await db.SaveChangesAsync();
            secondUserId = secondUser.Id;
        }

        string secondUserEmail;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            secondUserEmail = (await db.Users.FirstAsync(u => u.Id == secondUserId)).Email;
        }

        var secondLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = secondUserEmail, password = "Password123!" });
        var secondCsrfToken = ExtractCsrfToken(secondLoginResponse);
        _client.DefaultRequestHeaders.Remove("X-CSRF-Token");
        _client.DefaultRequestHeaders.Add("X-CSRF-Token", secondCsrfToken);

        var mineResponse = await _client.GetAsync("/api/v1/onboarding/drafts/mine");
        mineResponse.EnsureSuccessStatusCode();
        var body = await mineResponse.Content.ReadAsStringAsync();

        Assert.DoesNotContain(uniqueName, body);
    }

    [Fact]
    public async Task ChecklistTemplateDetail_UnchangedAfterDraftChecklistEdit()
    {
        await AuthenticateAsHrAdmin();

        var templatesResponse = await _client.GetAsync("/api/v1/onboarding/checklist-templates");
        templatesResponse.EnsureSuccessStatusCode();
        var listBody = await templatesResponse.Content.ReadFromJsonAsync<JsonElement>();
        var templates = listBody.GetProperty("templates").EnumerateArray().ToList();
        if (templates.Count == 0)
        {
            return; // no seeded templates in this tenant fixture — nothing to assert.
        }
        var templateId = templates[0].GetProperty("id").GetGuid();

        var beforeResponse = await _client.GetAsync($"/api/v1/onboarding/checklist-templates/{templateId}");
        var beforeBody = await beforeResponse.Content.ReadAsStringAsync();

        var saveResponse = await _client.PostAsJsonAsync("/api/v1/onboarding/drafts", new
        {
            employeeName = "Template Immutability Check",
            workEmail = $"template-check-{Guid.NewGuid():N}@acme.test",
            lastSavedStep = "employee_details"
        });
        var saved = await saveResponse.Content.ReadFromJsonAsync<JsonElement>();
        var draftId = saved.GetProperty("id").GetGuid();

        await _client.PostAsJsonAsync($"/api/v1/onboarding/drafts/{draftId}/checklist", new
        {
            selectedTemplateId = templateId,
            editedTasksJson = "[{\"title\":\"Mutated Task\",\"ownerType\":\"employee\"}]"
        });

        var afterResponse = await _client.GetAsync($"/api/v1/onboarding/checklist-templates/{templateId}");
        var afterBody = await afterResponse.Content.ReadAsStringAsync();

        Assert.Equal(beforeBody, afterBody);
    }
}
