# Onboarding Step 2 Org Assignment Dropdowns Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the four GUID text inputs in `/people/onboarding` Step 2 (Org Assignment) with name-based dropdowns backed by filtered backend lookup endpoints.

**Architecture:** Backend adds optional filter query params to the existing departments/positions list endpoints and introduces a new work-schedules lookup endpoint (controller → service → repository, tenant-scoped). Frontend Step 2 loads Company (legal entity) options, then cascades Department / Position / Work Schedule dropdowns filtered by the selected Company; selecting a Position auto-aligns Department. Save-draft payload still sends raw IDs.

**Tech Stack:** ASP.NET Core (.NET 10) + EF Core, xUnit integration tests via `CustomWebApplicationFactory`; Angular 21 standalone component with `FormsModule`/`ngModel`.

## Global Constraints

- HR must never see or paste GUIDs; backend still receives `legalEntityId`, `departmentId`, `positionId`, `scheduleId`.
- Company maps internally to `legal_entity_id`.
- Positions belong to one Company and one Department; Departments belong to one Company.
- Every backend query stays tenant-scoped (`TenantId` filter in repository).
- Do not change auth/session/CSRF; new endpoint uses `[RequirePermission]` like existing controllers.
- Backend code style (backend/CLAUDE.md): no expression-bodied service/repo/controller methods; explicit block bodies with `return await`; explicit DTO type names.
- Do not break existing onboarding draft/checklist/send-invite flow (all existing OnboardingTests must keep passing).
- Remove the text "Paste the GUID of each entity from your admin console."
- No hardcoded fake dropdown data.
- Work in repo `C:\onevoNew\test` on branch `feature/onboarding-org-dropdowns`.

---

### Task 1: Filtered departments endpoint (`GET /api/v1/org/departments?legalEntityId=`)

**Files:**
- Modify: `backend/Repositories/Interfaces/IOrgRepository.cs`
- Modify: `backend/Repositories/Implementations/OrgRepository.cs`
- Modify: `backend/Services/Interfaces/IOrgStructureService.cs`
- Modify: `backend/Services/Implementations/OrgStructureService.cs`
- Modify: `backend/Controllers/DepartmentsController.cs`
- Test: `backend.Tests/OrgLookupTests.cs` (new)

**Interfaces:**
- Produces: `IOrgStructureService.GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId)` returning `List<DepartmentDto>`; controller accepts `[FromQuery] Guid? legalEntityId`.

- [ ] **Step 1: Write the failing test**

Create `backend.Tests/OrgLookupTests.cs` with the same helper pattern as `OnboardingTests` (login `hr.admin@acme.test`, header `X-Tenant-Domain: acme.test`):

```csharp
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
using OnevoHr.Api.Models.Generated;
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
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test backend.Tests --nologo -v q --filter OrgLookupTests`
Expected: FAIL — filtered call returns departments from all legal entities (query param ignored), so `Assert.All(... LegalEntityId ...)` fails.

- [ ] **Step 3: Implement the filter**

`IOrgRepository.cs` — change:

```csharp
Task<List<Department>> GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId);
```

`OrgRepository.cs`:

```csharp
public async Task<List<Department>> GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId)
{
    var query = _db.Departments.AsNoTracking()
        .Where(d => d.TenantId == tenantId);

    if (legalEntityId.HasValue)
    {
        query = query.Where(d => d.LegalEntityId == legalEntityId.Value);
    }

    return await query
        .OrderBy(d => d.Name)
        .ToListAsync();
}
```

`IOrgStructureService.cs`:

```csharp
Task<List<DepartmentDto>> GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId);
```

`OrgStructureService.cs`:

```csharp
public async Task<List<DepartmentDto>> GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId)
{
    var departments = await _org.GetDepartmentsAsync(tenantId, legalEntityId);
    return departments.Select(d => new DepartmentDto(
        d.Id, d.LegalEntityId, d.Name, d.Code, d.ParentDepartmentId, d.HeadPositionId, d.Status)).ToList();
}
```

`DepartmentsController.cs` List action:

```csharp
[HttpGet]
[RequirePermission("org:departments:read")]
public async Task<IActionResult> List([FromQuery] Guid? legalEntityId)
{
    var departments = await _orgStructure.GetDepartmentsAsync(_tenantContext.TenantId!.Value, legalEntityId);
    return Ok(departments);
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test backend.Tests --nologo -v q --filter OrgLookupTests`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add backend backend.Tests
git commit -m "feat(org): filter departments list by legalEntityId"
```

---

### Task 2: Filtered positions endpoint (`GET /api/v1/org/positions?legalEntityId=&departmentId=`)

**Files:**
- Modify: `backend/Repositories/Interfaces/IOrgRepository.cs`
- Modify: `backend/Repositories/Implementations/OrgRepository.cs`
- Modify: `backend/Services/Interfaces/IOrgStructureService.cs`
- Modify: `backend/Services/Implementations/OrgStructureService.cs`
- Modify: `backend/Controllers/PositionsController.cs`
- Test: `backend.Tests/OrgLookupTests.cs`

**Interfaces:**
- Consumes: `SeedSecondLegalEntityAsync` helper from Task 1.
- Produces: `IOrgStructureService.GetPositionsAsync(Guid tenantId, Guid? legalEntityId, Guid? departmentId)` returning `List<PositionDto>`.

- [ ] **Step 1: Write the failing test** (add to `OrgLookupTests.cs`)

```csharp
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
    var acmeEmptyDept = new Department
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        LegalEntityId = euLe.Id,
        Name = $"Empty Dept {Guid.NewGuid():N}",
        Code = $"EMPTY_{Guid.NewGuid().ToString("N").Substring(0, 6)}",
        Status = "active",
        CreatedAtUtc = DateTime.UtcNow
    };
    db.Departments.Add(acmeEmptyDept);
    await db.SaveChangesAsync();

    var empty = await _client.GetFromJsonAsync<PositionItem[]>(
        $"/api/v1/org/positions?legalEntityId={euLe.Id}&departmentId={acmeEmptyDept.Id}");
    Assert.NotNull(empty);
    Assert.Empty(empty);
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test backend.Tests --nologo -v q --filter Positions_FilteredByLegalEntityAndDepartment_ReturnsOnlyMatching`
Expected: FAIL — unfiltered list includes acme-global positions.

- [ ] **Step 3: Implement the filter**

`IOrgRepository.cs`:

```csharp
Task<List<Position>> GetPositionsAsync(Guid tenantId, Guid? legalEntityId, Guid? departmentId);
```

`OrgRepository.cs`:

```csharp
public async Task<List<Position>> GetPositionsAsync(Guid tenantId, Guid? legalEntityId, Guid? departmentId)
{
    var query = _db.Positions.AsNoTracking()
        .Include(p => p.PositionAssignments)
        .Where(p => p.TenantId == tenantId);

    if (legalEntityId.HasValue)
    {
        query = query.Where(p => p.LegalEntityId == legalEntityId.Value);
    }

    if (departmentId.HasValue)
    {
        query = query.Where(p => p.DepartmentId == departmentId.Value);
    }

    return await query
        .OrderBy(p => p.Name)
        .ToListAsync();
}
```

`IOrgStructureService.cs`:

```csharp
Task<List<PositionDto>> GetPositionsAsync(Guid tenantId, Guid? legalEntityId, Guid? departmentId);
```

`OrgStructureService.cs`:

```csharp
public async Task<List<PositionDto>> GetPositionsAsync(Guid tenantId, Guid? legalEntityId, Guid? departmentId)
{
    var positions = await _org.GetPositionsAsync(tenantId, legalEntityId, departmentId);
    return positions.Select(Map).ToList();
}
```

`PositionsController.cs` List action:

```csharp
[HttpGet]
[RequirePermission("org:positions:read")]
public async Task<IActionResult> List([FromQuery] Guid? legalEntityId, [FromQuery] Guid? departmentId)
{
    var positions = await _orgStructure.GetPositionsAsync(_tenantContext.TenantId!.Value, legalEntityId, departmentId);
    return Ok(positions);
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test backend.Tests --nologo -v q --filter OrgLookupTests`
Expected: PASS (both tests).

- [ ] **Step 5: Commit**

```bash
git add backend backend.Tests
git commit -m "feat(org): filter positions list by legalEntityId and departmentId"
```

---

### Task 3: Work-schedules lookup endpoint (`GET /api/v1/time-attendance/work-schedules?legalEntityId=`)

**Files:**
- Create: `backend/DTOs/TimeAttendance/WorkScheduleDtos.cs`
- Create: `backend/Repositories/Interfaces/IWorkScheduleRepository.cs`
- Create: `backend/Repositories/Implementations/WorkScheduleRepository.cs`
- Create: `backend/Services/Interfaces/IWorkScheduleService.cs`
- Create: `backend/Services/Implementations/WorkScheduleService.cs`
- Create: `backend/Controllers/WorkSchedulesController.cs`
- Modify: `backend/Program.cs` (DI registrations)
- Modify: `backend/Data/Seed/DatabaseSeeder.cs` (permission `attendance:read`, HR Admin template, seed one acme schedule)
- Test: `backend.Tests/OrgLookupTests.cs`

**Interfaces:**
- Produces: `WorkScheduleDto(Guid Id, Guid LegalEntityId, string Name, string Timezone, bool IsActive)`; `IWorkScheduleService.GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId)`; permission key `attendance:read` (module `time_attendance`, feature `time_attendance.work_schedules`).
- Consumes: `WorkSchedule` entity from `Models/Generated/GeneratedEntities.cs`; `AppDbContext.WorkSchedules`.

- [ ] **Step 1: Write the failing tests** (add to `OrgLookupTests.cs`)

```csharp
[Fact]
public async Task WorkSchedules_Unauthenticated_Returns401()
{
    // Ensure seed exists but do NOT log in on this client.
    using var scope = _factory.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!await db.PlatformUsers.AnyAsync())
    {
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await DatabaseSeeder.SeedAsync(db, hasher);
    }

    var anonymousClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        HandleCookies = true,
        BaseAddress = new Uri("https://localhost")
    });
    anonymousClient.DefaultRequestHeaders.Add("X-Tenant-Domain", "acme.test");

    var response = await anonymousClient.GetAsync("/api/v1/time-attendance/work-schedules");
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}

[Fact]
public async Task WorkSchedules_FilteredByLegalEntity_And_TenantIsolated()
{
    await AuthenticateAsHrAdmin();
    var (tenantId, mainLeId) = await GetAcmeContextAsync();

    var db = GetDb();
    var suffix = Guid.NewGuid().ToString("N").Substring(0, 8);

    var mainSchedule = new WorkSchedule
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        LegalEntityId = mainLeId,
        Name = $"Main LE Schedule {suffix}",
        Timezone = "UTC",
        IsActive = true,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow
    };
    // Schedule for a different legal entity in the same tenant — must be
    // excluded when filtering by mainLeId.
    var (euLe, _, _) = await SeedSecondLegalEntityAsync(tenantId);
    var otherLeSchedule = new WorkSchedule
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        LegalEntityId = euLe.Id,
        Name = $"EU Schedule {suffix}",
        Timezone = "UTC",
        IsActive = true,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow
    };
    // Schedule belonging to a completely different tenant — must never appear.
    var foreignSchedule = new WorkSchedule
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        LegalEntityId = Guid.NewGuid(),
        Name = $"Foreign Tenant Schedule {suffix}",
        Timezone = "UTC",
        IsActive = true,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow
    };
    db.WorkSchedules.AddRange(mainSchedule, otherLeSchedule, foreignSchedule);
    await db.SaveChangesAsync();

    var filtered = await _client.GetFromJsonAsync<WorkScheduleItem[]>(
        $"/api/v1/time-attendance/work-schedules?legalEntityId={mainLeId}");
    Assert.NotNull(filtered);
    Assert.All(filtered, s => Assert.Equal(mainLeId, s.LegalEntityId));
    Assert.Contains(filtered, s => s.Id == mainSchedule.Id);
    Assert.DoesNotContain(filtered, s => s.Id == otherLeSchedule.Id);
    Assert.DoesNotContain(filtered, s => s.Id == foreignSchedule.Id);

    var all = await _client.GetFromJsonAsync<WorkScheduleItem[]>(
        "/api/v1/time-attendance/work-schedules");
    Assert.NotNull(all);
    Assert.Contains(all, s => s.Id == otherLeSchedule.Id);
    Assert.DoesNotContain(all, s => s.Id == foreignSchedule.Id);
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test backend.Tests --nologo -v q --filter WorkSchedules`
Expected: FAIL — 404 (route does not exist), so `GetFromJsonAsync` throws / 401 test gets 404.

- [ ] **Step 3: Implement endpoint, DI, and seeding**

`backend/DTOs/TimeAttendance/WorkScheduleDtos.cs`:

```csharp
namespace OnevoHr.Api.DTOs.TimeAttendance;

public sealed record WorkScheduleDto(
    Guid Id,
    Guid LegalEntityId,
    string Name,
    string Timezone,
    bool IsActive);
```

`backend/Repositories/Interfaces/IWorkScheduleRepository.cs`:

```csharp
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IWorkScheduleRepository
{
    Task<List<WorkSchedule>> GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId);
}
```

`backend/Repositories/Implementations/WorkScheduleRepository.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class WorkScheduleRepository : IWorkScheduleRepository
{
    private readonly AppDbContext _db;

    public WorkScheduleRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<WorkSchedule>> GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId)
    {
        var query = _db.WorkSchedules.AsNoTracking()
            .Where(s => s.TenantId == tenantId);

        if (legalEntityId.HasValue)
        {
            query = query.Where(s => s.LegalEntityId == legalEntityId.Value);
        }

        return await query
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}
```

`backend/Services/Interfaces/IWorkScheduleService.cs`:

```csharp
using OnevoHr.Api.DTOs.TimeAttendance;

namespace OnevoHr.Api.Services.Interfaces;

public interface IWorkScheduleService
{
    Task<List<WorkScheduleDto>> GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId);
}
```

`backend/Services/Implementations/WorkScheduleService.cs`:

```csharp
using OnevoHr.Api.DTOs.TimeAttendance;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class WorkScheduleService : IWorkScheduleService
{
    private readonly IWorkScheduleRepository _workSchedules;

    public WorkScheduleService(IWorkScheduleRepository workSchedules)
    {
        _workSchedules = workSchedules;
    }

    public async Task<List<WorkScheduleDto>> GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId)
    {
        var schedules = await _workSchedules.GetWorkSchedulesAsync(tenantId, legalEntityId);
        return schedules.Select(s => new WorkScheduleDto(
            s.Id, s.LegalEntityId, s.Name, s.Timezone, s.IsActive)).ToList();
    }
}
```

`backend/Controllers/WorkSchedulesController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/time-attendance/work-schedules")]
public sealed class WorkSchedulesController : ControllerBase
{
    private readonly IWorkScheduleService _workSchedules;
    private readonly ITenantContextService _tenantContext;

    public WorkSchedulesController(IWorkScheduleService workSchedules, ITenantContextService tenantContext)
    {
        _workSchedules = workSchedules;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [RequirePermission("attendance:read")]
    public async Task<IActionResult> List([FromQuery] Guid? legalEntityId)
    {
        var schedules = await _workSchedules.GetWorkSchedulesAsync(_tenantContext.TenantId!.Value, legalEntityId);
        return Ok(schedules);
    }
}
```

`backend/Program.cs` — add next to the other repository/service registrations:

```csharp
builder.Services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
builder.Services.AddScoped<IWorkScheduleService, WorkScheduleService>();
```

`backend/Data/Seed/DatabaseSeeder.cs` — three changes:

1. In `permissionDefinitions`, after the `org:*` entries add:

```csharp
("attendance:read", "time_attendance", "time_attendance.work_schedules"),
```

2. In the HR Admin role template definition, add `"time_attendance"` to its module keys array and `"attendance:read"` to its permission keys array (Tenant Owner already receives all permissions automatically).

3. After the acme positions block, seed one work schedule for the acme legal entity:

```csharp
db.WorkSchedules.Add(new WorkSchedule
{
    Id = Guid.NewGuid(),
    TenantId = acme.Id,
    LegalEntityId = acmeGlobal.Id,
    Name = "Standard Weekday (Mon–Fri)",
    Timezone = "UTC",
    DefaultForNewEmployee = true,
    IsActive = true,
    CreatedAt = acme.CreatedAtUtc,
    UpdatedAt = acme.CreatedAtUtc
});
```

(Requires `using OnevoHr.Api.Models.Generated;` if not already imported.)

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test backend.Tests --nologo -v q --filter OrgLookupTests`
Expected: PASS (4 tests). Then run the full suite to confirm nothing broke:
`dotnet test backend.Tests --nologo -v q`
Expected: all tests PASS.

- [ ] **Step 5: Commit**

```bash
git add backend backend.Tests
git commit -m "feat(time-attendance): add tenant-scoped work-schedules lookup endpoint"
```

---

### Task 4: Frontend Step 2 dropdowns with cascading Company → Department/Position/Schedule

**Files:**
- Modify: `onevo-tenant-app/src/app/features/people/onboarding/onboarding.component.ts`

**Interfaces:**
- Consumes: `GET /api/v1/org/legal-entities` → `{ id, name, code, status }[]`; `GET /api/v1/org/departments?legalEntityId=` → `{ id, legalEntityId, name, code, ... }[]`; `GET /api/v1/org/positions?legalEntityId=&departmentId=` → `{ id, legalEntityId, departmentId, name, code, status }[]`; `GET /api/v1/time-attendance/work-schedules?legalEntityId=` → `{ id, legalEntityId, name, timezone, isActive }[]`.
- Produces: unchanged save-draft payload (`legalEntityId`, `departmentId`, `positionId`, `scheduleId` as GUID strings).

Implementation detail (single component change, no separate test cycle — Angular unit test infra is a default `app.spec.ts` scaffold only; verification is `npm run build` plus backend integration tests):

- [ ] **Step 1: Add option interfaces and lookup state**

Add after the existing DTO interfaces:

```ts
interface LegalEntityOption { id: string; name: string; code: string; status: string; }
interface DepartmentOption { id: string; legalEntityId: string; name: string; code: string; status: string; }
interface PositionOption { id: string; legalEntityId: string; departmentId: string; name: string; code: string; status: string; }
interface WorkScheduleOption { id: string; legalEntityId: string; name: string; timezone: string; isActive: boolean; }
```

Component fields:

```ts
legalEntities: LegalEntityOption[] = [];
departments: DepartmentOption[] = [];
positions: PositionOption[] = [];
schedules: WorkScheduleOption[] = [];
lookupError: string | null = null;
```

- [ ] **Step 2: Add lookup loading + cascade methods**

Component implements `OnInit`; `ngOnInit()` calls `loadLegalEntities()`. Methods:

```ts
async ngOnInit() {
  await this.loadLegalEntities();
}

private async loadLegalEntities() {
  try {
    this.legalEntities = await firstValueFrom(
      this.http.get<LegalEntityOption[]>('/api/v1/org/legal-entities'));
    this.lookupError = null;
  } catch {
    this.lookupError = 'Could not load companies. Check that your role has org read permissions.';
  }
}

private async loadCompanyScopedLookups() {
  if (!this.draft.legalEntityId) {
    this.departments = [];
    this.positions = [];
    this.schedules = [];
    return;
  }
  try {
    const le = encodeURIComponent(this.draft.legalEntityId);
    const deptFilter = this.draft.departmentId
      ? `&departmentId=${encodeURIComponent(this.draft.departmentId)}` : '';
    [this.departments, this.positions, this.schedules] = await Promise.all([
      firstValueFrom(this.http.get<DepartmentOption[]>(`/api/v1/org/departments?legalEntityId=${le}`)),
      firstValueFrom(this.http.get<PositionOption[]>(`/api/v1/org/positions?legalEntityId=${le}${deptFilter}`)),
      firstValueFrom(this.http.get<WorkScheduleOption[]>(`/api/v1/time-attendance/work-schedules?legalEntityId=${le}`)),
    ]);
    this.schedules = this.schedules.filter(s => s.isActive);
    this.lookupError = null;
  } catch {
    this.lookupError = 'Could not load org data for the selected company.';
  }
}

async onCompanyChange() {
  this.draft.departmentId = '';
  this.draft.positionId = '';
  this.draft.scheduleId = '';
  await this.loadCompanyScopedLookups();
}

async onDepartmentChange() {
  const pos = this.positions.find(p => p.id === this.draft.positionId);
  if (pos && this.draft.departmentId && pos.departmentId !== this.draft.departmentId) {
    this.draft.positionId = '';
  }
  await this.loadCompanyScopedLookups();
}

async onPositionChange() {
  const pos = this.positions.find(p => p.id === this.draft.positionId);
  if (!pos) return;
  // Positions belong to exactly one department; keep the department aligned.
  if (this.draft.departmentId !== pos.departmentId) {
    this.draft.departmentId = pos.departmentId;
    await this.loadCompanyScopedLookups();
  }
}
```

Name helpers for the review table:

```ts
companyName(): string {
  return this.legalEntities.find(le => le.id === this.draft.legalEntityId)?.name ?? '—';
}
departmentName(): string {
  return this.departments.find(d => d.id === this.draft.departmentId)?.name ?? '—';
}
positionName(): string {
  return this.positions.find(p => p.id === this.draft.positionId)?.name ?? '—';
}
scheduleName(): string {
  return this.schedules.find(s => s.id === this.draft.scheduleId)?.name ?? '—';
}
positionOptionLabel(p: PositionOption): string {
  const dept = this.departments.find(d => d.id === p.departmentId);
  return dept ? `${p.name} (${p.code}) — ${dept.name}` : `${p.name} (${p.code})`;
}
```

In `resumeDraft()`, after restoring fields, add `await this.loadCompanyScopedLookups();` so a resumed draft shows names.

- [ ] **Step 3: Replace Step 2 template**

Remove the hint line `Paste the GUID of each entity from your admin console.` and the four text inputs; replace with:

```html
<div class="step-card" *ngIf="currentStep === 'org_assignment'">
  <h3>Step 2 — Org Assignment</h3>
  <p *ngIf="lookupError" class="error-text">{{ lookupError }}</p>
  <div class="field-row">
    <label for="company">Company <span class="req">*</span></label>
    <select id="company" [(ngModel)]="draft.legalEntityId" (ngModelChange)="onCompanyChange()">
      <option value="">— Select a company —</option>
      <option *ngFor="let le of legalEntities" [value]="le.id">{{ le.name }}</option>
    </select>
  </div>
  <div class="field-row">
    <label for="dept">Department</label>
    <select id="dept" [(ngModel)]="draft.departmentId" (ngModelChange)="onDepartmentChange()"
            [disabled]="!draft.legalEntityId">
      <option value="">— Optional —</option>
      <option *ngFor="let d of departments" [value]="d.id">{{ d.name }} ({{ d.code }})</option>
    </select>
  </div>
  <div class="field-row">
    <label for="pos">Position</label>
    <select id="pos" [(ngModel)]="draft.positionId" (ngModelChange)="onPositionChange()"
            [disabled]="!draft.legalEntityId">
      <option value="">— Optional — sensitive positions trigger approval —</option>
      <option *ngFor="let p of positions" [value]="p.id">{{ positionOptionLabel(p) }}</option>
    </select>
  </div>
  <div class="field-row">
    <label for="sched">Work Schedule</label>
    <select id="sched" [(ngModel)]="draft.scheduleId" [disabled]="!draft.legalEntityId">
      <option value="">— Optional —</option>
      <option *ngFor="let s of schedules" [value]="s.id">{{ s.name }} ({{ s.timezone }})</option>
    </select>
  </div>
  <div class="step-actions">
    <button class="btn btn-secondary" (click)="currentStep = 'employee_details'">Back</button>
    <button class="btn btn-primary" (click)="saveDraftAndAdvance('checklist_review')">Save & Next</button>
  </div>
</div>
```

Update the Step 4 review table rows from IDs to names:

```html
<tr><th>Company</th><td>{{ companyName() }}</td></tr>
<tr><th>Department</th><td>{{ departmentName() }}</td></tr>
<tr><th>Position</th><td>{{ positionName() }}</td></tr>
<tr><th>Work Schedule</th><td>{{ scheduleName() }}</td></tr>
```

Leave `saveDraftAndAdvance` payload logic untouched (still sends the four IDs).

- [ ] **Step 4: Build**

Run: `cd onevo-tenant-app && npm run build`
Expected: build succeeds with no errors.

- [ ] **Step 5: Commit**

```bash
git add onevo-tenant-app/src/app/features/people/onboarding/onboarding.component.ts
git commit -m "feat(onboarding): replace Step 2 GUID inputs with company-scoped dropdowns"
```

---

### Task 5: Full verification

- [ ] **Step 1: Full backend test suite**

Run: `dotnet test backend.Tests --nologo -v q`
Expected: all tests PASS (existing OnboardingTests unaffected — draft/checklist/validate/send-invite flow untouched).

- [ ] **Step 2: Backend build clean**

Run: `dotnet build backend --nologo -v q`
Expected: 0 errors.

- [ ] **Step 3: Frontend production build**

Run: `cd onevo-tenant-app && npm run build`
Expected: success.

- [ ] **Step 4: Commit plan doc**

```bash
git add docs/superpowers/plans/2026-07-07-onboarding-org-assignment-dropdowns.md
git commit -m "docs: onboarding org assignment dropdowns plan"
```

## Self-Review Notes

- Spec coverage: Company/Department/Position/Schedule dropdowns (Task 4), filtered endpoints (Tasks 1–3), GUID hint removed (Task 4 Step 3), payload unchanged (Task 4), position→department auto-align (Task 4 `onPositionChange`), tenant isolation preserved (repo `TenantId` filters + Task 3 test), auth/CSRF untouched, backend tests added (Tasks 1–3), builds verified (Task 5).
- `attendance:read` permission is new; seeded and granted to HR Admin template + acme HR Admin role via existing `roleTemplateDefinitions` reuse (`tenantRoleDefinitions["HR Admin"]` reads the same array).
- Existing dev databases seeded before this change will lack `attendance:read`; the test suite uses a freshly seeded DB. For an existing local DB, re-seed or grant manually — noted as a known limitation.
