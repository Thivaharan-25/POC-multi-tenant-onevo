using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnevoHr.Api.Data;
using OnevoHr.Api.Data.Seed;
using OnevoHr.Api.Models.OrgStructure;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;
using Xunit;

namespace backend.Tests;

/// <summary>
/// Integration tests for the org lookup endpoints that back the onboarding
/// Step 2 Org Assignment dropdowns:
///   GET /api/v1/org/departments?legalEntityId=
///   GET /api/v1/org/positions?legalEntityId=&departmentId=
///   GET /api/v1/time-attendance/work-schedules?legalEntityId=
/// </summary>
public class OrgLookupTests : IClassFixture<CustomWebApplicationFactory>
{
    private sealed record DepartmentItem(Guid Id, Guid LegalEntityId, string Name, string Code, string Status);
    private sealed record PositionItem(Guid Id, Guid LegalEntityId, Guid DepartmentId, string Name, string Code, string Status);
    private sealed record WorkScheduleItem(Guid Id, Guid LegalEntityId, string Name, string Timezone, bool IsActive);

    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrgLookupTests(CustomWebApplicationFactory factory)
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
        if (!loginResponse.IsSuccessStatusCode)
        {
            var body = await loginResponse.Content.ReadAsStringAsync();
            throw new Exception($"HR Admin login failed: {loginResponse.StatusCode} — {body}");
        }
    }

    private AppDbContext GetDb()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    private async Task<(Guid TenantId, Guid LegalEntityId)> GetAcmeContextAsync()
    {
        var db = GetDb();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        var le = await db.LegalEntities.FirstAsync(e => e.TenantId == acme.Id);
        return (acme.Id, le.Id);
    }

    /// <summary>Seeds a second legal entity for acme with one department and one position.</summary>
    private async Task<(LegalEntity Le, Department Dept, Position Pos)> SeedSecondLegalEntityAsync(Guid tenantId)
    {
        var db = GetDb();
        var suffix = Guid.NewGuid().ToString("N").Substring(0, 8);
        var le = new LegalEntity
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = $"Acme Europe {suffix}",
            Code = $"ACME-EU-{suffix}",
            Status = "active",
            CreatedAtUtc = DateTime.UtcNow
        };
        var dept = new Department
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            LegalEntityId = le.Id,
            Name = $"EU Sales {suffix}",
            Code = $"EU_SALES_{suffix}",
            Status = "active",
            CreatedAtUtc = DateTime.UtcNow
        };
        var pos = new Position
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            LegalEntityId = le.Id,
            DepartmentId = dept.Id,
            Name = $"EU Sales Rep {suffix}",
            Code = $"EU-SALES-{suffix}",
            Capacity = 1,
            PositionType = "unique",
            Status = "active",
            CreatedAtUtc = DateTime.UtcNow
        };
        db.LegalEntities.Add(le);
        db.Departments.Add(dept);
        db.Positions.Add(pos);
        await db.SaveChangesAsync();
        return (le, dept, pos);
    }

    // ------------------------------------------------------------------
    // Departments
    // ------------------------------------------------------------------

    [Fact]
    public async Task Departments_FilteredByLegalEntity_ReturnsOnlyThatCompany()
    {
        await AuthenticateAsHrAdmin();
        var (tenantId, mainLeId) = await GetAcmeContextAsync();
        var (euLe, euDept, _) = await SeedSecondLegalEntityAsync(tenantId);

        var filtered = await _client.GetFromJsonAsync<DepartmentItem[]>(
            $"/api/v1/org/departments?legalEntityId={euLe.Id}");

        Assert.NotNull(filtered);
        Assert.NotEmpty(filtered);
        Assert.All(filtered, d => Assert.Equal(euLe.Id, d.LegalEntityId));
        Assert.Contains(filtered, d => d.Id == euDept.Id);

        var unfiltered = await _client.GetFromJsonAsync<DepartmentItem[]>("/api/v1/org/departments");
        Assert.NotNull(unfiltered);
        Assert.Contains(unfiltered, d => d.LegalEntityId == mainLeId);
        Assert.Contains(unfiltered, d => d.LegalEntityId == euLe.Id);
    }

    // ------------------------------------------------------------------
    // Positions
    // ------------------------------------------------------------------

    [Fact]
    public async Task Positions_FilteredByLegalEntityAndDepartment_ReturnsOnlyMatching()
    {
        await AuthenticateAsHrAdmin();
        var (tenantId, _) = await GetAcmeContextAsync();
        var (euLe, euDept, euPos) = await SeedSecondLegalEntityAsync(tenantId);

        var byLe = await _client.GetFromJsonAsync<PositionItem[]>(
            $"/api/v1/org/positions?legalEntityId={euLe.Id}");
        Assert.NotNull(byLe);
        Assert.NotEmpty(byLe);
        Assert.All(byLe, p => Assert.Equal(euLe.Id, p.LegalEntityId));
        Assert.Contains(byLe, p => p.Id == euPos.Id);

        var byDept = await _client.GetFromJsonAsync<PositionItem[]>(
            $"/api/v1/org/positions?legalEntityId={euLe.Id}&departmentId={euDept.Id}");
        Assert.NotNull(byDept);
        Assert.All(byDept, p => Assert.Equal(euDept.Id, p.DepartmentId));
        Assert.Contains(byDept, p => p.Id == euPos.Id);

        // Filtering by a department with no positions returns an empty list.
        var db = GetDb();
        var emptyDept = new Department
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            LegalEntityId = euLe.Id,
            Name = $"Empty Dept {Guid.NewGuid():N}",
            Code = $"EMPTY_{Guid.NewGuid().ToString("N").Substring(0, 6)}",
            Status = "active",
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Departments.Add(emptyDept);
        await db.SaveChangesAsync();

        var empty = await _client.GetFromJsonAsync<PositionItem[]>(
            $"/api/v1/org/positions?legalEntityId={euLe.Id}&departmentId={emptyDept.Id}");
        Assert.NotNull(empty);
        Assert.Empty(empty);
    }
}
