# People / Employees / Onboarding Rework Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Turn the People/Employees/Onboarding frontend into a real HR workflow — scoped Employees list, modal/drawer Add Employee flow with dropdowns instead of GUIDs/JSON, a UI checklist task editor, and a "My Drafts" resume feature — backed by a small set of additive, read-only backend endpoints.

**Architecture:** Backend stays additive: existing scoping (`ScopeResolverService`/`GetVisibleEmployeesAsync`), draft lifecycle (`OnboardingService`/`SendInviteAsync`), and template storage (`ChecklistTemplate.TasksJson`) are all reused untouched. New read endpoints are layered on top: enriched/filterable employee list, a checklist-template list+detail endpoint with department→company fallback resolution, a `drafts/mine` endpoint (using the already-present `OnboardingDraft.StartedById`), and an optional reporting-manager lookup. Frontend replaces the developer test form (`onboarding.component.ts`, 708 lines, raw Draft ID / Template ID / JSON textarea) with a 3-step wizard (Employee & Assignment → Checklist → Review & Send) hosted in a new shared `ov-modal` component, plus a real Employees table and a My Drafts drawer.

**Tech Stack:** ASP.NET Core + EF Core (backend, `OnevoHr.Api`), Angular 21 standalone components (frontend), xUnit + `CustomWebApplicationFactory` (backend tests).

## Global Constraints

- Backend code style (from `backend/CLAUDE.md`): no expression-bodied (`=>`) methods on repository/service/controller logic — explicit block bodies everywhere, including one-line async wrappers (`return await ...` inside a full method body). Explicit DTO constructors, not target-typed `new(...)`.
- Never show Draft ID, Checklist Template ID, or raw Edited Tasks JSON in any UI.
- Never let the frontend filter the employee list client-side for visibility — the scoped backend endpoint is the only source of truth.
- Never mutate `ChecklistTemplate.TasksJson` when HR edits onboarding tasks — edits only ever write to `OnboardingDraft.EditedTasksJson`.
- No user/employee/invite/checklist-task rows are created before `POST .../send-invite` — do not touch `OnboardingService.SendInviteAsync` or its transaction.
- `drafts/mine` must resolve tenant and user from `ICurrentUserService` (session), never from query/body.
- Do not touch accept-invite, SendGrid/email outbox, or session cookie/CSRF/JWT behavior.
- Position-specific checklist-template matching is deferred (per user decision 2026-07-07): `ChecklistTemplate` has no `PositionId` column today. This plan implements department-specific → company-wide fallback only. A `legalEntityId`/`positionId` query param is accepted on the new endpoint for forward compatibility but is not used in resolution yet — this is called out explicitly in code comments, not silently dropped.
- Keep the guarded direct route `/people/onboarding` (`employees:write`) working — it now renders the same wizard component inline instead of the old developer form.

---

## File Structure

**Backend (all under `C:\onevoNew\test\backend`):**
- Modify `Services/Interfaces/IEmployeeService.cs`, `Services/Implementations/EmployeeService.cs` — richer list query
- Modify `Controllers/EmployeesController.cs` — filters + permission gate
- Create `DTOs/Employees/EmployeeListItemDto.cs` — list-only DTO with names + query object
- Create `DTOs/Onboarding/OnboardingReadDtos.cs` — checklist-template / my-drafts read DTOs
- Modify `Repositories/Interfaces/IOnboardingRepository.cs`, `Repositories/Implementations/OnboardingRepository.cs` — 2 new read methods
- Modify `Services/Interfaces/IOnboardingService.cs`, `Services/Implementations/OnboardingService.cs` — 3 new read methods
- Create `Controllers/ChecklistTemplatesController.cs`
- Modify `Controllers/OnboardingController.cs` — `GET mine`, `:guid` constraint
- (Optional) Modify `Services/Interfaces/IOrgStructureService.cs`, `Services/Implementations/OrgStructureService.cs`, `Controllers/PositionsController.cs` — reporting-manager preview

**Frontend (all under `C:\onevoNew\test\onevo-tenant-app\src\app`):**
- Modify `core/api/endpoints/employees-api.service.ts`, `positions-api.service.ts`
- Create `core/api/endpoints/onboarding-api.service.ts`, `work-schedules-api.service.ts`
- Create `shared/ui/modal/modal.component.ts`
- Create `features/people/onboarding/onboarding-step1.component.ts`
- Create `features/people/onboarding/onboarding-step2.component.ts`
- Create `features/people/onboarding/onboarding-step3.component.ts`
- Create `features/people/onboarding/onboarding-wizard.component.ts`
- Create `features/people/onboarding/my-drafts-drawer.component.ts`
- Rewrite `features/people/people.component.ts`
- Rewrite `features/people/onboarding/onboarding.component.ts` (thin wrapper)

**Backend tests:** Create `backend.Tests/PeopleWorkflowReadTests.cs`

---

## Task 1: Backend — Employees list: names, search/filter, permission gate

**Files:**
- Create: `backend/DTOs/Employees/EmployeeListItemDto.cs`
- Modify: `backend/Services/Interfaces/IEmployeeService.cs`
- Modify: `backend/Services/Implementations/EmployeeService.cs`
- Modify: `backend/Controllers/EmployeesController.cs`

**Interfaces:**
- Produces: `EmployeeListItemDto(Guid Id, string EmployeeNumber, string FirstName, string LastName, string WorkEmail, string Status, DateOnly HireDate, Guid LegalEntityId, Guid? DepartmentId, string? DepartmentName, Guid? CurrentPositionId, string? PositionName)` and `EmployeeListQuery(string? Search, string? Status, Guid? DepartmentId, Guid? PositionId)`, both consumed by Task 12's Employees screen via `GET /api/v1/employees`.

- [ ] **Step 1: Confirm `GetVisibleEmployeesAsync` has no other callers**

Run: search the backend project for other call sites before changing its signature.

```bash
grep -rn "GetVisibleEmployeesAsync" backend --include=*.cs
```

Expected: only `EmployeeService.cs` (definition) and `EmployeesController.cs` (the one call in `List()`). If anything else calls it, stop and adapt this task instead of blindly changing the signature.

- [ ] **Step 2: Create the list DTO and query object**

```csharp
namespace OnevoHr.Api.DTOs.Employees;

public sealed record EmployeeListItemDto(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string WorkEmail,
    string Status,
    DateOnly HireDate,
    Guid LegalEntityId,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? CurrentPositionId,
    string? PositionName);

public sealed record EmployeeListQuery(
    string? Search,
    string? Status,
    Guid? DepartmentId,
    Guid? PositionId);
```

Save as `backend/DTOs/Employees/EmployeeListItemDto.cs`.

- [ ] **Step 3: Update `IEmployeeService`**

```csharp
using OnevoHr.Api.DTOs.Employees;

namespace OnevoHr.Api.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeListItemDto>> GetVisibleEmployeesAsync(EmployeeListQuery query);
    Task<EmployeeDto?> GetByIdAsync(Guid employeeId);
    Task<EmployeeDto?> CreateAsync(CreateEmployeeRequestDto request);
}
```

- [ ] **Step 4: Update `EmployeeService` — inject `IOrgRepository`, resolve names, filter**

```csharp
using OnevoHr.Api.DTOs.Employees;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// RLS-style filtering: employee reads are limited to the employees visible
/// under the current user's resolved scope.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employees;
    private readonly ICurrentUserService _currentUser;
    private readonly IScopeResolverService _scopeResolver;
    private readonly IOrgRepository _org;

    public EmployeeService(
        IEmployeeRepository employees,
        ICurrentUserService currentUser,
        IScopeResolverService scopeResolver,
        IOrgRepository org)
    {
        _employees = employees;
        _currentUser = currentUser;
        _scopeResolver = scopeResolver;
        _org = org;
    }

    public async Task<List<EmployeeListItemDto>> GetVisibleEmployeesAsync(EmployeeListQuery query)
    {
        if (_currentUser.TenantId is not Guid tenantId || _currentUser.UserId is not Guid userId)
        {
            return new List<EmployeeListItemDto>();
        }

        var scope = await _scopeResolver.ResolveScopeAsync(tenantId, userId);
        if (scope.VisibleEmployeeIds.Count == 0)
        {
            return new List<EmployeeListItemDto>();
        }

        var employees = scope.ScopeLevel == "Tenant"
            ? await _employees.GetByTenantAsync(tenantId)
            : await _employees.GetByIdsAsync(tenantId, scope.VisibleEmployeeIds);

        var filtered = ApplyFilters(employees, query);

        var departments = await _org.GetDepartmentsAsync(tenantId, null);
        var departmentNames = departments.ToDictionary(d => d.Id, d => d.Name);

        var positionIds = filtered
            .Where(e => e.CurrentPositionId.HasValue)
            .Select(e => e.CurrentPositionId!.Value)
            .Distinct()
            .ToList();
        var positionNames = new Dictionary<Guid, string>();
        foreach (var positionId in positionIds)
        {
            var position = await _org.GetPositionByIdAsync(positionId);
            if (position != null && position.TenantId == tenantId)
            {
                positionNames[positionId] = position.Name;
            }
        }

        return filtered.Select(e => MapListItem(e, departmentNames, positionNames)).ToList();
    }

    private static List<Employee> ApplyFilters(List<Employee> employees, EmployeeListQuery query)
    {
        var result = employees;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            result = result.Where(e =>
                e.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.LastName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.WorkEmail.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.EmployeeNumber.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            result = result.Where(e => string.Equals(e.Status, query.Status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (query.DepartmentId.HasValue)
        {
            result = result.Where(e => e.DepartmentId == query.DepartmentId.Value).ToList();
        }

        if (query.PositionId.HasValue)
        {
            result = result.Where(e => e.CurrentPositionId == query.PositionId.Value).ToList();
        }

        return result;
    }

    private static EmployeeListItemDto MapListItem(
        Employee e,
        Dictionary<Guid, string> departmentNames,
        Dictionary<Guid, string> positionNames)
    {
        string? departmentName = null;
        if (e.DepartmentId.HasValue && departmentNames.TryGetValue(e.DepartmentId.Value, out var deptName))
        {
            departmentName = deptName;
        }

        string? positionName = null;
        if (e.CurrentPositionId.HasValue && positionNames.TryGetValue(e.CurrentPositionId.Value, out var posName))
        {
            positionName = posName;
        }

        return new EmployeeListItemDto(
            e.Id, e.EmployeeNumber, e.FirstName, e.LastName, e.WorkEmail,
            e.Status, e.HireDate, e.LegalEntityId, e.DepartmentId, departmentName,
            e.CurrentPositionId, positionName);
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid employeeId)
    {
        if (_currentUser.TenantId is not Guid tenantId || _currentUser.UserId is not Guid userId)
        {
            return null;
        }

        var scope = await _scopeResolver.ResolveScopeAsync(tenantId, userId);
        if (!scope.VisibleEmployeeIds.Contains(employeeId))
        {
            return null;
        }

        var employee = await _employees.GetByIdAsync(employeeId);
        return employee is null || employee.TenantId != tenantId ? null : MapDto(employee);
    }

    public async Task<EmployeeDto?> CreateAsync(CreateEmployeeRequestDto request)
    {
        if (_currentUser.TenantId is not Guid tenantId)
        {
            return null;
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EmployeeNumber = request.EmployeeNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            WorkEmail = request.WorkEmail,
            Status = "active",
            HireDate = request.HireDate,
            LegalEntityId = request.LegalEntityId,
            DepartmentId = request.DepartmentId,
            CreatedAtUtc = DateTime.UtcNow
        };
        await _employees.AddAsync(employee);
        await _employees.SaveChangesAsync();
        return MapDto(employee);
    }

    private static EmployeeDto MapDto(Employee e)
    {
        return new EmployeeDto(
            e.Id, e.EmployeeNumber, e.FirstName, e.LastName, e.WorkEmail,
            e.Status, e.HireDate, e.LegalEntityId, e.DepartmentId, e.CurrentPositionId);
    }
}
```

Note: add `using System.Linq;` at the top if not already implied by existing usings in the file.

- [ ] **Step 5: Update `EmployeesController` — query params + permission gate**

```csharp
using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Employees;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employees;

    public EmployeesController(IEmployeeService employees)
    {
        _employees = employees;
    }

    [HttpGet]
    [RequirePermission("employees:read")]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? departmentId,
        [FromQuery] Guid? positionId)
    {
        // Visibility (own / direct reports / department / tenant) is scope-filtered in the service.
        var query = new EmployeeListQuery(search, status, departmentId, positionId);
        var employees = await _employees.GetVisibleEmployeesAsync(query);
        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _employees.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequestDto request)
    {
        var employee = await _employees.CreateAsync(request);
        if (employee is null)
        {
            return BadRequest(new { error = "Employee could not be created." });
        }

        return Ok(employee);
    }
}
```

- [ ] **Step 6: Build**

Run: `dotnet build --nologo -v q` from `backend/`. Expected: build succeeds, no errors.

- [ ] **Step 7: Commit**

```bash
git add backend/DTOs/Employees/EmployeeListItemDto.cs backend/Services/Interfaces/IEmployeeService.cs backend/Services/Implementations/EmployeeService.cs backend/Controllers/EmployeesController.cs
git commit -m "feat: enrich employee list with names, search/filter, employees:read gate"
```

---

## Task 2: Backend — Checklist templates list/detail endpoints

**Files:**
- Create: `backend/DTOs/Onboarding/OnboardingReadDtos.cs`
- Modify: `backend/Repositories/Interfaces/IOnboardingRepository.cs`
- Modify: `backend/Repositories/Implementations/OnboardingRepository.cs`
- Modify: `backend/Services/Interfaces/IOnboardingService.cs`
- Modify: `backend/Services/Implementations/OnboardingService.cs`
- Create: `backend/Controllers/ChecklistTemplatesController.cs`

**Interfaces:**
- Consumes: `OnboardingService`'s existing private `TryParseTasks(string?)` and nested `DraftTaskDefinition` (already defined in the same class — do not duplicate).
- Produces: `ChecklistTemplateListResponse(List<ChecklistTemplateSummaryDto> Templates, Guid? RecommendedTemplateId)` and `ChecklistTemplateDetailDto(Guid Id, string Name, string TemplateType, Guid? DepartmentId, bool IsActive, List<ChecklistTemplateTaskDto> Tasks)` via `GET /api/v1/onboarding/checklist-templates` and `GET /api/v1/onboarding/checklist-templates/{id}` — consumed by Task 8 (frontend Step 2).

- [ ] **Step 1: Create the read DTOs**

```csharp
namespace OnevoHr.Api.DTOs.Onboarding;

public sealed record ChecklistTemplateSummaryDto(
    Guid Id,
    string Name,
    string TemplateType,
    Guid? DepartmentId,
    bool IsActive);

public sealed record ChecklistTemplateTaskDto(
    string Title,
    string? OwnerType,
    int? Sequence,
    bool IsRequired,
    bool IsLocked);

public sealed record ChecklistTemplateDetailDto(
    Guid Id,
    string Name,
    string TemplateType,
    Guid? DepartmentId,
    bool IsActive,
    List<ChecklistTemplateTaskDto> Tasks);

public sealed record ChecklistTemplateListResponse(
    List<ChecklistTemplateSummaryDto> Templates,
    Guid? RecommendedTemplateId);

public sealed record MyDraftSummaryDto(
    Guid Id,
    string EmployeeName,
    string WorkEmail,
    string LastSavedStep,
    string DraftReason,
    DateTime UpdatedAtUtc);
```

Save as `backend/DTOs/Onboarding/OnboardingReadDtos.cs`.

- [ ] **Step 2: Add repository method to list active templates**

In `backend/Repositories/Interfaces/IOnboardingRepository.cs`, add to the interface:

```csharp
Task<List<ChecklistTemplate>> GetChecklistTemplatesAsync(Guid tenantId);
```

In `backend/Repositories/Implementations/OnboardingRepository.cs`, add the implementation (block body per repo style):

```csharp
public async Task<List<ChecklistTemplate>> GetChecklistTemplatesAsync(Guid tenantId)
{
    return await _db.ChecklistTemplates
        .Where(t => t.TenantId == tenantId && t.IsActive)
        .OrderBy(t => t.Name)
        .ToListAsync();
}
```

- [ ] **Step 3: Add service methods to `IOnboardingService`**

```csharp
Task<ChecklistTemplateListResponse> GetChecklistTemplatesAsync(Guid? departmentId, CancellationToken ct);
Task<ChecklistTemplateDetailDto?> GetChecklistTemplateDetailAsync(Guid templateId, CancellationToken ct);
```

Add `using OnevoHr.Api.DTOs.Onboarding;` to the top of `IOnboardingService.cs`.

- [ ] **Step 4: Implement the service methods in `OnboardingService`**

Add these two public methods to the `OnboardingService` class (near the other read methods; requires the existing `_currentUser` and `_onboardingRepository` fields already present in the class):

```csharp
public async Task<ChecklistTemplateListResponse> GetChecklistTemplatesAsync(Guid? departmentId, CancellationToken ct)
{
    if (_currentUser.TenantId is not Guid tenantId)
    {
        return new ChecklistTemplateListResponse(new List<ChecklistTemplateSummaryDto>(), null);
    }

    var templates = await _onboardingRepository.GetChecklistTemplatesAsync(tenantId);
    var summaries = templates
        .Select(t => new ChecklistTemplateSummaryDto(t.Id, t.Name, t.TemplateType, t.DepartmentId, t.IsActive))
        .ToList();

    // Recommended template: department-specific match first, then company-wide
    // fallback. Position-specific matching is deferred — ChecklistTemplate has
    // no PositionId column yet (see plan Global Constraints).
    ChecklistTemplate? recommended = null;
    if (departmentId.HasValue)
    {
        recommended = templates.FirstOrDefault(t => t.DepartmentId == departmentId.Value);
    }
    if (recommended == null)
    {
        recommended = templates.FirstOrDefault(t => t.DepartmentId == null);
    }

    return new ChecklistTemplateListResponse(summaries, recommended?.Id);
}

public async Task<ChecklistTemplateDetailDto?> GetChecklistTemplateDetailAsync(Guid templateId, CancellationToken ct)
{
    if (_currentUser.TenantId is not Guid tenantId)
    {
        return null;
    }

    var template = await _onboardingRepository.GetChecklistTemplateAsync(tenantId, templateId);
    if (template == null)
    {
        return null;
    }

    var parsedTasks = TryParseTasks(template.TasksJson) ?? new List<DraftTaskDefinition>();
    var tasks = parsedTasks
        .Where(t => !string.IsNullOrWhiteSpace(t.Title))
        .Select(t => new ChecklistTemplateTaskDto(
            t.Title!, t.OwnerType, t.Sequence, t.IsRequired == true, t.IsLocked == true))
        .ToList();

    return new ChecklistTemplateDetailDto(
        template.Id, template.Name, template.TemplateType, template.DepartmentId, template.IsActive, tasks);
}
```

Add `using OnevoHr.Api.DTOs.Onboarding;` to `OnboardingService.cs` if not already present.

- [ ] **Step 5: Add `GetMyDraftsAsync` (used by Task 3, implemented here to keep the file change together)**

Add to `IOnboardingService.cs`:

```csharp
Task<List<MyDraftSummaryDto>> GetMyDraftsAsync(CancellationToken ct);
```

Add to `OnboardingService.cs` (requires `_currentUser.UserId`, already used elsewhere in the class):

```csharp
public async Task<List<MyDraftSummaryDto>> GetMyDraftsAsync(CancellationToken ct)
{
    if (_currentUser.TenantId is not Guid tenantId || _currentUser.UserId is not Guid userId)
    {
        return new List<MyDraftSummaryDto>();
    }

    var drafts = await _onboardingRepository.GetDraftsByStartedByAsync(tenantId, userId);
    return drafts
        .Select(d => new MyDraftSummaryDto(d.Id, d.EmployeeName, d.WorkEmail, d.LastSavedStep, d.DraftReason, d.UpdatedAtUtc))
        .ToList();
}
```

- [ ] **Step 6: Add `GetDraftsByStartedByAsync` to the onboarding repository**

In `IOnboardingRepository.cs`:

```csharp
Task<List<OnboardingDraft>> GetDraftsByStartedByAsync(Guid tenantId, Guid startedById);
```

In `OnboardingRepository.cs`:

```csharp
public async Task<List<OnboardingDraft>> GetDraftsByStartedByAsync(Guid tenantId, Guid startedById)
{
    return await _db.OnboardingDrafts
        .Where(d => d.TenantId == tenantId && d.StartedById == startedById && d.Status == "draft")
        .OrderByDescending(d => d.UpdatedAtUtc)
        .ToListAsync();
}
```

- [ ] **Step 7: Create `ChecklistTemplatesController`**

```csharp
using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/onboarding/checklist-templates")]
public class ChecklistTemplatesController : ControllerBase
{
    private readonly IOnboardingService _onboardingService;

    public ChecklistTemplatesController(IOnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    /// <summary>
    /// GET /api/v1/onboarding/checklist-templates
    /// legalEntityId/positionId are accepted for forward compatibility with the
    /// canonical position -> department -> company matching order, but position-tier
    /// matching is deferred: ChecklistTemplate has no PositionId column yet.
    /// </summary>
    [HttpGet]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> List(
        [FromQuery] Guid? legalEntityId,
        [FromQuery] Guid? departmentId,
        [FromQuery] Guid? positionId,
        CancellationToken ct)
    {
        var result = await _onboardingService.GetChecklistTemplatesAsync(departmentId, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var detail = await _onboardingService.GetChecklistTemplateDetailAsync(id, ct);
        if (detail == null)
        {
            return NotFound();
        }

        return Ok(detail);
    }
}
```

- [ ] **Step 8: Build**

Run: `dotnet build --nologo -v q` from `backend/`. Expected: build succeeds.

- [ ] **Step 9: Commit**

```bash
git add backend/DTOs/Onboarding/OnboardingReadDtos.cs backend/Repositories/Interfaces/IOnboardingRepository.cs backend/Repositories/Implementations/OnboardingRepository.cs backend/Services/Interfaces/IOnboardingService.cs backend/Services/Implementations/OnboardingService.cs backend/Controllers/ChecklistTemplatesController.cs
git commit -m "feat: add checklist-template list/detail endpoints with department-to-company fallback"
```

---

## Task 3: Backend — `drafts/mine` endpoint

**Files:**
- Modify: `backend/Controllers/OnboardingController.cs`

**Interfaces:**
- Consumes: `IOnboardingService.GetMyDraftsAsync(CancellationToken)` (added in Task 2, Step 5).
- Produces: `GET /api/v1/onboarding/drafts/mine` → `List<MyDraftSummaryDto>`, consumed by Task 9 (My Drafts drawer).

- [ ] **Step 1: Add `:guid` constraint to the existing `GetDraft` route and add the `mine` action**

In `backend/Controllers/OnboardingController.cs`, change:

```csharp
[HttpGet("{id}")]
[RequirePermission("employees:write")]
public async Task<IActionResult> GetDraft(Guid id, CancellationToken ct)
```

to:

```csharp
[HttpGet("{id:guid}")]
[RequirePermission("employees:write")]
public async Task<IActionResult> GetDraft(Guid id, CancellationToken ct)
```

Then add a new action anywhere in the same controller (after `SaveDraft` is a good spot):

```csharp
/// <summary>
/// GET /api/v1/onboarding/drafts/mine
/// Returns only draft-status onboarding drafts started by the current session
/// user in the current tenant. Tenant and user are resolved server-side from
/// ICurrentUserService — never from query or body.
/// </summary>
[HttpGet("mine")]
[RequirePermission("employees:write")]
public async Task<IActionResult> GetMyDrafts(CancellationToken ct)
{
    var drafts = await _onboardingService.GetMyDraftsAsync(ct);
    return Ok(drafts);
}
```

- [ ] **Step 2: Build**

Run: `dotnet build --nologo -v q` from `backend/`. Expected: build succeeds.

- [ ] **Step 3: Commit**

```bash
git add backend/Controllers/OnboardingController.cs
git commit -m "feat: add GET /api/v1/onboarding/drafts/mine"
```

---

## Task 4 (Optional): Backend — reporting-manager preview

This task is optional. If skipped, Task 7's frontend "Reports to" preview simply never appears (it already degrades gracefully on any error/404).

**Files:**
- Modify: `backend/Services/Interfaces/IOrgStructureService.cs`
- Modify: `backend/Services/Implementations/OrgStructureService.cs`
- Modify: `backend/Controllers/PositionsController.cs`

**Interfaces:**
- Produces: `GET /api/v1/org/positions/{id}/reporting-manager` → `{ hasManager: bool, employeeId?: Guid, employeeName?: string }`, consumed by Task 7 (Step 1's `onPositionChange`).

- [ ] **Step 1: Add `GetReportingManagerAsync` to `IOrgStructureService`**

```csharp
Task<ReportingManagerDto?> GetReportingManagerAsync(Guid tenantId, Guid positionId);
```

Add alongside it in `backend/DTOs/OrgStructure/OrgDtos.cs`:

```csharp
public sealed record ReportingManagerDto(Guid EmployeeId, string EmployeeName);
```

- [ ] **Step 2: Implement in `OrgStructureService`**

Add `IEmployeeRepository` as a constructor dependency (DI resolves it automatically — no manual registration change needed):

```csharp
private readonly IOrgRepository _org;
private readonly IEmployeeRepository _employees;

public OrgStructureService(IOrgRepository org, IEmployeeRepository employees)
{
    _org = org;
    _employees = employees;
}
```

Add the method:

```csharp
public async Task<ReportingManagerDto?> GetReportingManagerAsync(Guid tenantId, Guid positionId)
{
    var position = await _org.GetPositionByIdAsync(positionId);
    if (position == null || position.TenantId != tenantId || position.ReportsToPositionId is not Guid managerPositionId)
    {
        return null;
    }

    var assignments = await _org.GetActiveAssignmentsForPositionAsync(managerPositionId);
    var assignment = assignments.FirstOrDefault(a => a.IsPrimary) ?? assignments.FirstOrDefault();
    if (assignment == null)
    {
        return null;
    }

    var employee = await _employees.GetByIdAsync(assignment.EmployeeId);
    if (employee == null || employee.TenantId != tenantId)
    {
        return null;
    }

    return new ReportingManagerDto(employee.Id, employee.FirstName + " " + employee.LastName);
}
```

- [ ] **Step 3: Add the controller action**

In `PositionsController.cs`:

```csharp
[HttpGet("{id:guid}/reporting-manager")]
[RequirePermission("org:positions:read")]
public async Task<IActionResult> GetReportingManager(Guid id, CancellationToken ct)
{
    var manager = await _orgStructure.GetReportingManagerAsync(_tenantContext.TenantId!.Value, id);
    if (manager == null)
    {
        return Ok(new { hasManager = false });
    }

    return Ok(new { hasManager = true, employeeId = manager.EmployeeId, employeeName = manager.EmployeeName });
}
```

- [ ] **Step 4: Build**

Run: `dotnet build --nologo -v q` from `backend/`. Expected: build succeeds.

- [ ] **Step 5: Commit**

```bash
git add backend/Services/Interfaces/IOrgStructureService.cs backend/Services/Implementations/OrgStructureService.cs backend/Controllers/PositionsController.cs backend/DTOs/OrgStructure/OrgDtos.cs
git commit -m "feat: add optional reporting-manager preview endpoint"
```

---

## Task 5: Frontend — API endpoint services

**Files:**
- Modify: `onevo-tenant-app/src/app/core/api/endpoints/positions-api.service.ts`
- Modify: `onevo-tenant-app/src/app/core/api/endpoints/employees-api.service.ts`
- Create: `onevo-tenant-app/src/app/core/api/endpoints/onboarding-api.service.ts`
- Create: `onevo-tenant-app/src/app/core/api/endpoints/work-schedules-api.service.ts`

**Interfaces:**
- Produces: `EmployeeListItem`, `EmployeeListFilters`, `SaveOnboardingDraftPayload`, `OnboardingDraft`, `MyDraftSummary`, `ChecklistTemplateSummary`, `ChecklistTemplateTask`, `ChecklistTemplateDetail`, `ChecklistTemplateListResponse`, `DraftValidationResult`, `SendInviteResult`, `WorkScheduleOption` — all consumed by Tasks 6–11.

- [ ] **Step 1: Extend `positions-api.service.ts`**

```typescript
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

export interface ReportingManagerPreview {
  hasManager: boolean;
  employeeId?: string;
  employeeName?: string;
}

@Injectable({ providedIn: 'root' })
export class PositionsApiService {
  private http = inject(HttpClient);

  list(legalEntityId: string, departmentId?: string) {
    const params: Record<string, string> = { legalEntityId };
    if (departmentId) {
      params['departmentId'] = departmentId;
    }
    return this.http.get('/api/v1/org/positions', { params });
  }

  getReportingManager(positionId: string) {
    return this.http.get<ReportingManagerPreview>(`/api/v1/org/positions/${positionId}/reporting-manager`);
  }
}
```

- [ ] **Step 2: Extend `employees-api.service.ts`**

```typescript
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

export interface EmployeeListItem {
  id: string;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  workEmail: string;
  status: string;
  hireDate: string;
  legalEntityId: string;
  departmentId: string | null;
  departmentName: string | null;
  currentPositionId: string | null;
  positionName: string | null;
}

export interface EmployeeListFilters {
  search?: string;
  status?: string;
  departmentId?: string;
  positionId?: string;
}

@Injectable({ providedIn: 'root' })
export class EmployeesApiService {
  private http = inject(HttpClient);

  list(filters: EmployeeListFilters = {}) {
    const params: Record<string, string> = {};
    if (filters.search) params['search'] = filters.search;
    if (filters.status) params['status'] = filters.status;
    if (filters.departmentId) params['departmentId'] = filters.departmentId;
    if (filters.positionId) params['positionId'] = filters.positionId;
    return this.http.get<EmployeeListItem[]>('/api/v1/employees', { params });
  }
}
```

- [ ] **Step 3: Create `work-schedules-api.service.ts`**

```typescript
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

export interface WorkScheduleOption {
  id: string;
  name: string;
  timezone: string;
}

@Injectable({ providedIn: 'root' })
export class WorkSchedulesApiService {
  private http = inject(HttpClient);

  list(legalEntityId: string) {
    return this.http.get<WorkScheduleOption[]>('/api/v1/time-attendance/work-schedules', {
      params: { legalEntityId }
    });
  }
}
```

- [ ] **Step 4: Create `onboarding-api.service.ts`**

```typescript
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

export interface SaveOnboardingDraftPayload {
  id?: string;
  employeeName: string;
  workEmail: string;
  legalEntityId: string | null;
  departmentId: string | null;
  positionId: string | null;
  employmentType: string | null;
  startDate: string | null;
  employeeNumber: string | null;
  scheduleId: string | null;
  lastSavedStep: string;
}

export interface OnboardingDraft {
  id: string;
  employeeName: string;
  workEmail: string;
  legalEntityId: string | null;
  departmentId: string | null;
  positionId: string | null;
  employmentType: string | null;
  startDate: string | null;
  employeeNumber: string | null;
  scheduleId: string | null;
  selectedTemplateId: string | null;
  editedTasksJson: string;
  status: string;
  draftReason: string;
  lastSavedStep: string;
}

export interface MyDraftSummary {
  id: string;
  employeeName: string;
  workEmail: string;
  lastSavedStep: string;
  draftReason: string;
  updatedAtUtc: string;
}

export interface ChecklistTemplateSummary {
  id: string;
  name: string;
  templateType: string;
  departmentId: string | null;
  isActive: boolean;
}

export interface ChecklistTemplateTask {
  title: string;
  ownerType: string | null;
  sequence: number | null;
  isRequired: boolean;
  isLocked: boolean;
}

export interface ChecklistTemplateDetail {
  id: string;
  name: string;
  templateType: string;
  departmentId: string | null;
  isActive: boolean;
  tasks: ChecklistTemplateTask[];
}

export interface ChecklistTemplateListResponse {
  templates: ChecklistTemplateSummary[];
  recommendedTemplateId: string | null;
}

export interface ValidationIssue {
  code: string;
  message: string;
  field?: string;
}

export interface DraftValidationResult {
  isValid: boolean;
  errors: ValidationIssue[];
  warnings: ValidationIssue[];
  actions: string[];
}

export interface SendInviteResult {
  status: 'completed' | 'blocked' | 'invalid' | 'not_found';
  draftReason?: string;
  validation?: DraftValidationResult;
  actions?: string[];
  employeeId?: string;
  userId?: string;
  dev_invite_url?: string;
  error?: string;
}

@Injectable({ providedIn: 'root' })
export class OnboardingApiService {
  private http = inject(HttpClient);

  saveDraft(payload: SaveOnboardingDraftPayload) {
    return this.http.post<{ id: string }>('/api/v1/onboarding/drafts', payload);
  }

  getDraft(id: string) {
    return this.http.get<OnboardingDraft>(`/api/v1/onboarding/drafts/${id}`);
  }

  getMyDrafts() {
    return this.http.get<MyDraftSummary[]>('/api/v1/onboarding/drafts/mine');
  }

  getChecklistTemplates(legalEntityId: string, departmentId?: string | null) {
    const params: Record<string, string> = { legalEntityId };
    if (departmentId) params['departmentId'] = departmentId;
    return this.http.get<ChecklistTemplateListResponse>('/api/v1/onboarding/checklist-templates', { params });
  }

  getChecklistTemplateDetail(templateId: string) {
    return this.http.get<ChecklistTemplateDetail>(`/api/v1/onboarding/checklist-templates/${templateId}`);
  }

  updateChecklist(draftId: string, selectedTemplateId: string | null, editedTasksJson: string) {
    return this.http.post(`/api/v1/onboarding/drafts/${draftId}/checklist`, { selectedTemplateId, editedTasksJson });
  }

  validateDraft(draftId: string) {
    return this.http.post<DraftValidationResult>(`/api/v1/onboarding/drafts/${draftId}/validate`, null);
  }

  sendInvite(draftId: string) {
    return this.http.post<SendInviteResult>(`/api/v1/onboarding/drafts/${draftId}/send-invite`, null);
  }

  requestSeat(draftId: string) {
    return this.http.post<{ status: string; message?: string }>(`/api/v1/onboarding/drafts/${draftId}/request-seat`, null);
  }

  submitPositionApproval(draftId: string) {
    return this.http.post<{ status: string; message?: string }>(`/api/v1/onboarding/drafts/${draftId}/submit-approval`, null);
  }
}
```

- [ ] **Step 5: Build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: build succeeds (these files aren't wired into any component yet, but must compile standalone).

- [ ] **Step 6: Commit**

```bash
git add onevo-tenant-app/src/app/core/api/endpoints/positions-api.service.ts onevo-tenant-app/src/app/core/api/endpoints/employees-api.service.ts onevo-tenant-app/src/app/core/api/endpoints/onboarding-api.service.ts onevo-tenant-app/src/app/core/api/endpoints/work-schedules-api.service.ts
git commit -m "feat: add onboarding/work-schedules api services, extend positions/employees services"
```

---

## Task 6: Frontend — shared `ov-modal` component

**Files:**
- Create: `onevo-tenant-app/src/app/shared/ui/modal/modal.component.ts`

**Interfaces:**
- Produces: `ov-modal` selector with `[open]`, `[title]`, `[variant]: 'dialog' | 'drawer'`, `[dismissible]` inputs and `(closed)` output — consumed by Task 9 (wizard) and Task 10 (My Drafts drawer).

- [ ] **Step 1: Create the component**

```typescript
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'ov-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="ov-modal-backdrop" *ngIf="open" (click)="onBackdropClick()">
      <div
        class="ov-modal-panel"
        [class.ov-modal-drawer]="variant === 'drawer'"
        (click)="$event.stopPropagation()"
      >
        <div class="ov-modal-header" *ngIf="title">
          <h2 class="ov-modal-title">{{ title }}</h2>
          <button type="button" class="ov-modal-close" (click)="close()" aria-label="Close">&times;</button>
        </div>
        <div class="ov-modal-body">
          <ng-content></ng-content>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .ov-modal-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 1000;
    }
    .ov-modal-panel {
      background: var(--content-bg);
      color: var(--content-fg);
      border: 1px solid var(--border-color);
      border-radius: 8px;
      width: 640px;
      max-width: calc(100vw - 32px);
      max-height: calc(100vh - 64px);
      display: flex;
      flex-direction: column;
      overflow: hidden;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.25);
    }
    .ov-modal-panel.ov-modal-drawer {
      position: fixed;
      top: 0;
      right: 0;
      height: 100vh;
      max-height: 100vh;
      width: 420px;
      border-radius: 0;
      border-right: none;
      border-top: none;
      border-bottom: none;
    }
    .ov-modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 20px 24px;
      border-bottom: 1px solid var(--border-color);
    }
    .ov-modal-title {
      font-size: 1.125rem;
      font-weight: 600;
      margin: 0;
    }
    .ov-modal-close {
      background: transparent;
      border: none;
      color: var(--content-fg);
      font-size: 1.5rem;
      line-height: 1;
      cursor: pointer;
      padding: 4px;
    }
    .ov-modal-body {
      padding: 24px;
      overflow-y: auto;
      flex: 1;
    }
  `]
})
export class ModalComponent {
  @Input() open = false;
  @Input() title?: string;
  @Input() variant: 'dialog' | 'drawer' = 'dialog';
  @Input() dismissible = true;
  @Output() closed = new EventEmitter<void>();

  onBackdropClick() {
    if (this.dismissible) {
      this.close();
    }
  }

  close() {
    this.closed.emit();
  }
}
```

- [ ] **Step 2: Build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: build succeeds.

- [ ] **Step 3: Commit**

```bash
git add onevo-tenant-app/src/app/shared/ui/modal/modal.component.ts
git commit -m "feat: add shared ov-modal component (dialog + drawer variants)"
```

---

## Task 7: Frontend — Onboarding Step 1 (Employee & Assignment)

**Files:**
- Create: `onevo-tenant-app/src/app/features/people/onboarding/onboarding-step1.component.ts`

**Interfaces:**
- Consumes: `LegalEntitiesApiService.list()`, `DepartmentsApiService.list(legalEntityId)`, `PositionsApiService.list(legalEntityId, departmentId?)`, `PositionsApiService.getReportingManager(positionId)`, `WorkSchedulesApiService.list(legalEntityId)` (Task 5).
- Produces: `Step1FormData` interface and `app-onboarding-step1` component with `[initialData]` input and `(next)`/`(cancel)` outputs — consumed by Task 10 (wizard host).

- [ ] **Step 1: Create the component**

```typescript
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { FormFieldComponent } from '../../../shared/ui/form-field/form-field.component';
import { LegalEntitiesApiService } from '../../../core/api/endpoints/legal-entities-api.service';
import { DepartmentsApiService } from '../../../core/api/endpoints/departments-api.service';
import { PositionsApiService } from '../../../core/api/endpoints/positions-api.service';
import { WorkSchedulesApiService } from '../../../core/api/endpoints/work-schedules-api.service';

export interface Step1FormData {
  employeeName: string;
  workEmail: string;
  employeeNumber: string;
  employmentType: string;
  startDate: string;
  legalEntityId: string | null;
  departmentId: string | null;
  positionId: string | null;
  scheduleId: string | null;
  selectedTemplateId: string | null;
  editedTasksJson: string;
  companyName: string | null;
  departmentName: string | null;
  positionName: string | null;
  scheduleName: string | null;
  reportingManagerName: string | null;
}

interface Option {
  id: string;
  name: string;
}

@Component({
  selector: 'app-onboarding-step1',
  standalone: true,
  imports: [CommonModule, FormsModule, FormFieldComponent],
  template: `
    <form class="wizard-form" (ngSubmit)="onNext()">
      <div class="form-grid">
        <ov-form-field label="Employee Name" [required]="true">
          <input type="text" [(ngModel)]="data.employeeName" name="employeeName" required />
        </ov-form-field>
        <ov-form-field label="Work Email" [required]="true">
          <input type="email" [(ngModel)]="data.workEmail" name="workEmail" required />
        </ov-form-field>
        <ov-form-field label="Employee Number" hint="Optional — auto-generated if left blank">
          <input type="text" [(ngModel)]="data.employeeNumber" name="employeeNumber" />
        </ov-form-field>
        <ov-form-field label="Employment Type" [required]="true">
          <select [(ngModel)]="data.employmentType" name="employmentType" required>
            <option value="full_time">Full-time</option>
            <option value="part_time">Part-time</option>
            <option value="contractor">Contractor</option>
            <option value="intern">Intern</option>
          </select>
        </ov-form-field>
        <ov-form-field label="Start Date" [required]="true">
          <input type="date" [(ngModel)]="data.startDate" name="startDate" required />
        </ov-form-field>
        <ov-form-field label="Company" [required]="true">
          <select [(ngModel)]="data.legalEntityId" name="legalEntityId" (ngModelChange)="onCompanyChange()" required>
            <option [ngValue]="null">Select company…</option>
            <option *ngFor="let c of companies" [ngValue]="c.id">{{ c.name }}</option>
          </select>
        </ov-form-field>
        <ov-form-field label="Department">
          <select
            [(ngModel)]="data.departmentId"
            name="departmentId"
            (ngModelChange)="onDepartmentChange()"
            [disabled]="!data.legalEntityId"
          >
            <option [ngValue]="null">Select department…</option>
            <option *ngFor="let d of departments" [ngValue]="d.id">{{ d.name }}</option>
          </select>
        </ov-form-field>
        <ov-form-field label="Position" [required]="true">
          <select
            [(ngModel)]="data.positionId"
            name="positionId"
            (ngModelChange)="onPositionChange()"
            [disabled]="!data.legalEntityId"
            required
          >
            <option [ngValue]="null">Select position…</option>
            <option *ngFor="let p of positions" [ngValue]="p.id">{{ p.name }}</option>
          </select>
        </ov-form-field>
        <ov-form-field label="Work Schedule">
          <select [(ngModel)]="data.scheduleId" name="scheduleId" [disabled]="!data.legalEntityId">
            <option [ngValue]="null">Select schedule…</option>
            <option *ngFor="let s of schedules" [ngValue]="s.id">{{ s.name }}</option>
          </select>
        </ov-form-field>
      </div>

      <div class="preview-banner" *ngIf="reportingManagerName">
        Reports to: <strong>{{ reportingManagerName }}</strong>
      </div>

      <div class="wizard-actions">
        <button type="button" class="btn btn-outline" (click)="cancel.emit()">Cancel</button>
        <button type="submit" class="btn btn-primary" [disabled]="!isValid()">Save & Next</button>
      </div>
    </form>
  `,
  styles: [`
    .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0 24px; }
    .preview-banner {
      background: rgba(59, 130, 246, 0.08);
      border: 1px solid var(--border-color);
      border-radius: 6px;
      padding: 10px 14px;
      margin-bottom: 16px;
      font-size: 0.875rem;
    }
    .wizard-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 8px; }
  `]
})
export class OnboardingStep1Component implements OnInit, OnChanges {
  private legalEntitiesApi = inject(LegalEntitiesApiService);
  private departmentsApi = inject(DepartmentsApiService);
  private positionsApi = inject(PositionsApiService);
  private schedulesApi = inject(WorkSchedulesApiService);

  @Input() initialData!: Step1FormData;
  @Output() next = new EventEmitter<Step1FormData>();
  @Output() cancel = new EventEmitter<void>();

  data: Step1FormData = this.emptyData();
  companies: Option[] = [];
  departments: Option[] = [];
  positions: Option[] = [];
  schedules: Option[] = [];
  reportingManagerName: string | null = null;

  private emptyData(): Step1FormData {
    return {
      employeeName: '',
      workEmail: '',
      employeeNumber: '',
      employmentType: 'full_time',
      startDate: '',
      legalEntityId: null,
      departmentId: null,
      positionId: null,
      scheduleId: null,
      selectedTemplateId: null,
      editedTasksJson: '[]',
      companyName: null,
      departmentName: null,
      positionName: null,
      scheduleName: null,
      reportingManagerName: null
    };
  }

  async ngOnInit() {
    const companies = (await firstValueFrom(this.legalEntitiesApi.list())) as any[];
    this.companies = companies.map(c => ({ id: c.id, name: c.name }));
  }

  async ngOnChanges() {
    if (this.initialData) {
      this.data = { ...this.initialData };
      if (this.data.legalEntityId) {
        await this.loadDependentOptions();
      }
    }
  }

  async onCompanyChange() {
    this.data.departmentId = null;
    this.data.positionId = null;
    this.data.scheduleId = null;
    this.reportingManagerName = null;
    await this.loadDependentOptions();
  }

  async onDepartmentChange() {
    this.data.positionId = null;
    this.reportingManagerName = null;
    if (this.data.legalEntityId) {
      const positions = (await firstValueFrom(
        this.positionsApi.list(this.data.legalEntityId, this.data.departmentId ?? undefined)
      )) as any[];
      this.positions = positions.map(p => ({ id: p.id, name: p.name }));
    }
  }

  async onPositionChange() {
    this.reportingManagerName = null;
    if (!this.data.positionId) {
      return;
    }
    try {
      const manager = await firstValueFrom(this.positionsApi.getReportingManager(this.data.positionId));
      this.reportingManagerName = manager.hasManager ? (manager.employeeName ?? null) : null;
    } catch {
      this.reportingManagerName = null;
    }
  }

  private async loadDependentOptions() {
    if (!this.data.legalEntityId) {
      this.departments = [];
      this.positions = [];
      this.schedules = [];
      return;
    }
    const legalEntityId = this.data.legalEntityId;
    const [depts, positions, schedules] = await Promise.all([
      firstValueFrom(this.departmentsApi.list(legalEntityId)) as Promise<any[]>,
      firstValueFrom(this.positionsApi.list(legalEntityId, this.data.departmentId ?? undefined)) as Promise<any[]>,
      firstValueFrom(this.schedulesApi.list(legalEntityId)) as Promise<any[]>
    ]);
    this.departments = depts.map(d => ({ id: d.id, name: d.name }));
    this.positions = positions.map(p => ({ id: p.id, name: p.name }));
    this.schedules = schedules.map(s => ({ id: s.id, name: s.name }));
  }

  isValid(): boolean {
    return !!(
      this.data.employeeName &&
      this.data.workEmail &&
      this.data.employmentType &&
      this.data.startDate &&
      this.data.legalEntityId &&
      this.data.positionId
    );
  }

  onNext() {
    if (!this.isValid()) {
      return;
    }
    const withNames: Step1FormData = {
      ...this.data,
      companyName: this.companies.find(c => c.id === this.data.legalEntityId)?.name ?? null,
      departmentName: this.departments.find(d => d.id === this.data.departmentId)?.name ?? null,
      positionName: this.positions.find(p => p.id === this.data.positionId)?.name ?? null,
      scheduleName: this.schedules.find(s => s.id === this.data.scheduleId)?.name ?? null,
      reportingManagerName: this.reportingManagerName
    };
    this.next.emit(withNames);
  }
}
```

- [ ] **Step 2: Build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: build succeeds.

- [ ] **Step 3: Commit**

```bash
git add onevo-tenant-app/src/app/features/people/onboarding/onboarding-step1.component.ts
git commit -m "feat: add onboarding step 1 (employee & assignment) component"
```

---

## Task 8: Frontend — Onboarding Step 2 (Checklist)

**Files:**
- Create: `onevo-tenant-app/src/app/features/people/onboarding/onboarding-step2.component.ts`

**Interfaces:**
- Consumes: `OnboardingApiService.getChecklistTemplates`, `getChecklistTemplateDetail` (Task 5).
- Produces: `EditableTask`, `Step2Result` interfaces and `app-onboarding-step2` component with `[draftId]`, `[legalEntityId]`, `[departmentId]`, `[initialTemplateId]`, `[initialTasksJson]` inputs and `(back)`/`(next)` outputs — consumed by Task 10.

- [ ] **Step 1: Create the component**

```typescript
import { Component, EventEmitter, Input, OnChanges, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { OnboardingApiService, ChecklistTemplateSummary, ChecklistTemplateTask } from '../../../core/api/endpoints/onboarding-api.service';

export interface EditableTask {
  title: string;
  ownerType: string;
  dueDate: string | null;
  isRequired: boolean;
  isLocked: boolean;
}

export interface Step2Result {
  selectedTemplateId: string | null;
  editedTasksJson: string;
  templateName: string;
  taskCount: number;
  requiredTaskTitles: string[];
}

@Component({
  selector: 'app-onboarding-step2',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="wizard-form">
      <label class="field-label">Checklist Template</label>
      <select [(ngModel)]="selectedTemplateId" (ngModelChange)="onTemplateChange()">
        <option [ngValue]="null">No template — start blank</option>
        <option *ngFor="let t of templates()" [ngValue]="t.id">
          {{ t.name }}{{ t.id === recommendedTemplateId() ? ' (Recommended)' : '' }}
        </option>
      </select>

      <div class="task-list">
        <div class="task-row" *ngFor="let task of tasks(); let i = index">
          <input
            type="text"
            [(ngModel)]="task.title"
            name="title{{ i }}"
            placeholder="Task title"
            [disabled]="task.isLocked"
          />
          <select [(ngModel)]="task.ownerType" name="owner{{ i }}">
            <option value="employee">Employee</option>
            <option value="manager">Manager</option>
            <option value="hr">HR</option>
            <option value="it">IT</option>
          </select>
          <input type="date" [(ngModel)]="task.dueDate" name="due{{ i }}" />
          <label class="required-toggle">
            <input type="checkbox" [(ngModel)]="task.isRequired" name="req{{ i }}" [disabled]="task.isLocked" />
            Required
          </label>
          <button
            type="button"
            class="btn-icon"
            (click)="removeTask(i)"
            [disabled]="task.isRequired || task.isLocked"
          >
            Remove
          </button>
        </div>
      </div>

      <button type="button" class="btn btn-outline btn-sm" (click)="addTask()">Add Task</button>

      <div class="wizard-actions">
        <button type="button" class="btn btn-outline" (click)="back.emit()">Back</button>
        <button type="button" class="btn btn-primary" (click)="onNext()">Save & Next</button>
      </div>
    </div>
  `,
  styles: [`
    .field-label { display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 6px; }
    .task-list { margin: 16px 0; display: flex; flex-direction: column; gap: 10px; }
    .task-row {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr auto auto;
      gap: 8px;
      align-items: center;
    }
    .required-toggle { display: flex; align-items: center; gap: 6px; font-size: 0.8125rem; }
    .wizard-actions { display: flex; justify-content: space-between; margin-top: 20px; }
  `]
})
export class OnboardingStep2Component implements OnChanges {
  private onboardingApi = inject(OnboardingApiService);

  @Input() draftId!: string;
  @Input() legalEntityId!: string;
  @Input() departmentId: string | null = null;
  @Input() initialTemplateId: string | null = null;
  @Input() initialTasksJson = '[]';
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<Step2Result>();

  templates = signal<ChecklistTemplateSummary[]>([]);
  recommendedTemplateId = signal<string | null>(null);
  tasks = signal<EditableTask[]>([]);
  selectedTemplateId: string | null = null;
  private loadedFromInitial = false;

  async ngOnChanges() {
    if (!this.legalEntityId || this.loadedFromInitial) {
      return;
    }
    this.loadedFromInitial = true;

    const list = await firstValueFrom(
      this.onboardingApi.getChecklistTemplates(this.legalEntityId, this.departmentId)
    );
    this.templates.set(list.templates);
    this.recommendedTemplateId.set(list.recommendedTemplateId);

    if (this.initialTemplateId) {
      this.selectedTemplateId = this.initialTemplateId;
    } else if (list.recommendedTemplateId) {
      this.selectedTemplateId = list.recommendedTemplateId;
    }

    if (this.initialTasksJson && this.initialTasksJson !== '[]') {
      this.tasks.set(this.parseTasksJson(this.initialTasksJson));
    } else if (this.selectedTemplateId) {
      await this.loadTasksFromTemplate(this.selectedTemplateId);
    }
  }

  async onTemplateChange() {
    if (this.selectedTemplateId) {
      await this.loadTasksFromTemplate(this.selectedTemplateId);
    } else {
      this.tasks.set([]);
    }
  }

  private async loadTasksFromTemplate(templateId: string) {
    const detail = await firstValueFrom(this.onboardingApi.getChecklistTemplateDetail(templateId));
    this.tasks.set(
      detail.tasks.map((t: ChecklistTemplateTask) => ({
        title: t.title,
        ownerType: t.ownerType || 'employee',
        dueDate: null,
        isRequired: t.isRequired,
        isLocked: t.isLocked
      }))
    );
  }

  private parseTasksJson(json: string): EditableTask[] {
    try {
      const parsed = JSON.parse(json) as any[];
      return parsed.map(t => ({
        title: t.title ?? '',
        ownerType: t.ownerType ?? 'employee',
        dueDate: t.dueDate ?? null,
        isRequired: !!t.isRequired,
        isLocked: !!t.isLocked
      }));
    } catch {
      return [];
    }
  }

  addTask() {
    this.tasks.update(list => [
      ...list,
      { title: '', ownerType: 'employee', dueDate: null, isRequired: false, isLocked: false }
    ]);
  }

  removeTask(index: number) {
    this.tasks.update(list => list.filter((_, i) => i !== index));
  }

  onNext() {
    const editedTasksJson = JSON.stringify(
      this.tasks()
        .filter(t => t.title.trim().length > 0)
        .map((t, i) => ({
          title: t.title,
          ownerType: t.ownerType,
          sequence: i + 1,
          dueDate: t.dueDate || null,
          isRequired: t.isRequired,
          isLocked: t.isLocked
        }))
    );
    const selectedTemplate = this.templates().find(t => t.id === this.selectedTemplateId);
    const requiredTaskTitles = this.tasks()
      .filter(t => t.isRequired || t.isLocked)
      .map(t => t.title);

    this.next.emit({
      selectedTemplateId: this.selectedTemplateId,
      editedTasksJson,
      templateName: selectedTemplate ? selectedTemplate.name : 'No template',
      taskCount: this.tasks().length,
      requiredTaskTitles
    });
  }
}
```

- [ ] **Step 2: Build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: build succeeds.

- [ ] **Step 3: Commit**

```bash
git add onevo-tenant-app/src/app/features/people/onboarding/onboarding-step2.component.ts
git commit -m "feat: add onboarding step 2 (checklist) component"
```

---

## Task 9: Frontend — Onboarding Step 3 (Review & Send Invite)

**Files:**
- Create: `onevo-tenant-app/src/app/features/people/onboarding/onboarding-step3.component.ts`

**Interfaces:**
- Consumes: `OnboardingApiService.validateDraft`, `sendInvite`, `requestSeat`, `submitPositionApproval` (Task 5); `Step1FormData` (Task 7, now includes `companyName`/`departmentName`/`positionName`/`scheduleName`/`reportingManagerName`).
- Produces: `app-onboarding-step3` component with `[draftId]`, `[formData]`, `[templateName]`, `[taskCount]`, `[requiredTaskTitles]` inputs and `(back)`/`(sent)` outputs — consumed by Task 10.

- [ ] **Step 1: Create the component**

```typescript
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { OnboardingApiService, SendInviteResult } from '../../../core/api/endpoints/onboarding-api.service';
import { Step1FormData } from './onboarding-step1.component';

@Component({
  selector: 'app-onboarding-step3',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="review">
      <dl class="review-grid">
        <dt>Employee Name</dt>
        <dd>{{ formData.employeeName }}</dd>
        <dt>Work Email</dt>
        <dd>{{ formData.workEmail }}</dd>
        <dt>Employee Number</dt>
        <dd>{{ formData.employeeNumber || 'Auto-generated' }}</dd>
        <dt>Employment Type</dt>
        <dd>{{ formData.employmentType }}</dd>
        <dt>Start Date</dt>
        <dd>{{ formData.startDate }}</dd>
        <dt>Company</dt>
        <dd>{{ formData.companyName || '—' }}</dd>
        <dt>Department</dt>
        <dd>{{ formData.departmentName || '—' }}</dd>
        <dt>Position</dt>
        <dd>{{ formData.positionName || '—' }}</dd>
        <dt>Work Schedule</dt>
        <dd>{{ formData.scheduleName || '—' }}</dd>
        <dt>Reporting Manager</dt>
        <dd>{{ formData.reportingManagerName || 'None' }}</dd>
        <dt>Checklist Template</dt>
        <dd>{{ templateName }}</dd>
        <dt>Checklist Tasks</dt>
        <dd>{{ taskCount }} task(s)</dd>
        <dt>Required Tasks</dt>
        <dd>{{ requiredTaskTitles.join(', ') || 'None' }}</dd>
        <dt>Invite will be sent to</dt>
        <dd>{{ formData.workEmail }}</dd>
      </dl>

      <div class="warning-banner" *ngIf="warnings().length > 0">
        <div class="warning-item" *ngFor="let w of warnings()">{{ w }}</div>
      </div>

      <div class="validation-errors" *ngIf="errors().length > 0">
        <div class="error-item" *ngFor="let e of errors()">{{ e }}</div>
      </div>

      <div class="blocked-banner" *ngIf="blockedReason()">
        <p>{{ blockedMessage() }}</p>
        <button
          type="button"
          class="btn btn-outline"
          *ngIf="blockedReason() === 'waiting_for_seat'"
          (click)="requestSeat()"
        >
          Request Seat Increase
        </button>
        <button
          type="button"
          class="btn btn-outline"
          *ngIf="blockedReason() === 'waiting_for_position_approval'"
          (click)="submitApproval()"
        >
          Submit Approval
        </button>
      </div>

      <div class="success-banner" *ngIf="completed()">
        Invite queued for {{ formData.workEmail }}.
      </div>

      <div class="wizard-actions">
        <button type="button" class="btn btn-outline" (click)="back.emit()" [disabled]="sending()">Back</button>
        <button type="button" class="btn btn-primary" (click)="sendInvite()" [disabled]="sending() || completed()">
          {{ sending() ? 'Sending…' : 'Send Invite' }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .review-grid { display: grid; grid-template-columns: 200px 1fr; row-gap: 10px; margin-bottom: 20px; }
    .review-grid dt { color: var(--shell-muted-fg); font-size: 0.8125rem; }
    .review-grid dd { margin: 0; }
    .validation-errors, .blocked-banner {
      background: rgba(239, 68, 68, 0.08);
      border: 1px solid var(--border-color);
      border-radius: 6px;
      padding: 12px 16px;
      margin-bottom: 16px;
    }
    .warning-banner {
      background: rgba(245, 158, 11, 0.1);
      border: 1px solid var(--border-color);
      border-radius: 6px;
      padding: 12px 16px;
      margin-bottom: 16px;
    }
    .success-banner {
      background: rgba(16, 185, 129, 0.1);
      border-radius: 6px;
      padding: 12px 16px;
      margin-bottom: 16px;
    }
    .wizard-actions { display: flex; justify-content: space-between; }
  `]
})
export class OnboardingStep3Component implements OnInit {
  private onboardingApi = inject(OnboardingApiService);

  @Input() draftId!: string;
  @Input() formData!: Step1FormData;
  @Input() templateName = '';
  @Input() taskCount = 0;
  @Input() requiredTaskTitles: string[] = [];
  @Output() back = new EventEmitter<void>();
  @Output() sent = new EventEmitter<void>();

  sending = signal(false);
  errors = signal<string[]>([]);
  warnings = signal<string[]>([]);
  blockedReason = signal<string | null>(null);
  completed = signal(false);

  async ngOnInit() {
    // Surface seat-availability / sensitive-position warnings proactively, before
    // the user clicks Send Invite, using the existing validate endpoint.
    try {
      const result = await firstValueFrom(this.onboardingApi.validateDraft(this.draftId));
      this.warnings.set((result.warnings ?? []).map(w => w.message));
    } catch {
      this.warnings.set([]);
    }
  }

  blockedMessage(): string {
    return this.blockedReason() === 'waiting_for_seat'
      ? 'No seats are available on the current plan.'
      : 'This position requires approval before an invite can be sent.';
  }

  async sendInvite() {
    this.sending.set(true);
    this.errors.set([]);
    this.blockedReason.set(null);
    try {
      const result: SendInviteResult = await firstValueFrom(this.onboardingApi.sendInvite(this.draftId));
      if (result.status === 'completed') {
        this.completed.set(true);
        this.sent.emit();
      } else if (result.status === 'blocked') {
        this.blockedReason.set(result.draftReason ?? null);
      } else if (result.status === 'invalid') {
        this.errors.set((result.validation?.errors ?? []).map(e => e.message));
      } else {
        this.errors.set(['Draft could not be found.']);
      }
    } finally {
      this.sending.set(false);
    }
  }

  async requestSeat() {
    await firstValueFrom(this.onboardingApi.requestSeat(this.draftId));
    this.blockedReason.set(null);
    this.errors.set(['Seat increase requested. You will be notified when a seat is available.']);
  }

  async submitApproval() {
    await firstValueFrom(this.onboardingApi.submitPositionApproval(this.draftId));
    this.blockedReason.set(null);
    this.errors.set(['Approval submitted. You will be notified once it is reviewed.']);
  }
}
```

- [ ] **Step 2: Build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: build succeeds.

- [ ] **Step 3: Commit**

```bash
git add onevo-tenant-app/src/app/features/people/onboarding/onboarding-step3.component.ts
git commit -m "feat: add onboarding step 3 (review & send invite) component"
```

---

## Task 10: Frontend — Onboarding wizard host

**Files:**
- Create: `onevo-tenant-app/src/app/features/people/onboarding/onboarding-wizard.component.ts`

**Interfaces:**
- Consumes: `OnboardingApiService.saveDraft/getDraft/updateChecklist` (Task 5), `LegalEntitiesApiService`/`DepartmentsApiService`/`PositionsApiService`/`WorkSchedulesApiService` (name resolution on draft resume), `OnboardingStep1Component`/`Step1FormData` (Task 7), `OnboardingStep2Component`/`Step2Result` (Task 8), `OnboardingStep3Component` (Task 9), `ModalComponent` (Task 6).
- Produces: `app-onboarding-wizard` component with `[mode]: 'modal' | 'inline'`, `[initialDraftId]` inputs and `(closed)`/`(saved)` outputs — consumed by Task 11 (Employees screen) and Task 12 (thin route wrapper).

- [ ] **Step 1: Create the component**

```typescript
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { ModalComponent } from '../../../shared/ui/modal/modal.component';
import { OnboardingApiService } from '../../../core/api/endpoints/onboarding-api.service';
import { LegalEntitiesApiService } from '../../../core/api/endpoints/legal-entities-api.service';
import { DepartmentsApiService } from '../../../core/api/endpoints/departments-api.service';
import { PositionsApiService } from '../../../core/api/endpoints/positions-api.service';
import { WorkSchedulesApiService } from '../../../core/api/endpoints/work-schedules-api.service';
import { OnboardingStep1Component, Step1FormData } from './onboarding-step1.component';
import { OnboardingStep2Component, Step2Result } from './onboarding-step2.component';
import { OnboardingStep3Component } from './onboarding-step3.component';

@Component({
  selector: 'app-onboarding-wizard',
  standalone: true,
  imports: [CommonModule, ModalComponent, OnboardingStep1Component, OnboardingStep2Component, OnboardingStep3Component],
  template: `
    <ov-modal
      *ngIf="mode === 'modal'"
      [open]="true"
      [title]="wizardTitle"
      [dismissible]="false"
      (closed)="onClose()"
    >
      <ng-container *ngTemplateOutlet="wizardBody"></ng-container>
    </ov-modal>

    <div class="wizard-inline" *ngIf="mode === 'inline'">
      <ng-container *ngTemplateOutlet="wizardBody"></ng-container>
    </div>

    <ng-template #wizardBody>
      <div class="wizard-steps">
        <span [class.active]="step() === 1">1. Employee &amp; Assignment</span>
        <span [class.active]="step() === 2">2. Checklist</span>
        <span [class.active]="step() === 3">3. Review &amp; Send</span>
      </div>

      <div *ngIf="loadingDraft()" class="state-message">Loading draft…</div>

      <app-onboarding-step1
        *ngIf="!loadingDraft() && step() === 1"
        [initialData]="formData"
        (next)="onStep1Next($event)"
        (cancel)="onClose()"
      ></app-onboarding-step1>

      <app-onboarding-step2
        *ngIf="!loadingDraft() && step() === 2"
        [draftId]="draftId()!"
        [legalEntityId]="formData.legalEntityId!"
        [departmentId]="formData.departmentId"
        [initialTemplateId]="formData.selectedTemplateId"
        [initialTasksJson]="formData.editedTasksJson"
        (back)="step.set(1)"
        (next)="onStep2Next($event)"
      ></app-onboarding-step2>

      <app-onboarding-step3
        *ngIf="!loadingDraft() && step() === 3"
        [draftId]="draftId()!"
        [formData]="formData"
        [templateName]="selectedTemplateName"
        [taskCount]="taskCount"
        [requiredTaskTitles]="requiredTaskTitles"
        (back)="step.set(2)"
        (sent)="onSent()"
      ></app-onboarding-step3>
      <!-- formData (Step1FormData) already carries companyName/departmentName/positionName/
           scheduleName/reportingManagerName set by Step 1's onNext(); Step 3 reads them
           directly off the [formData] input, no separate bindings needed. -->
    </ng-template>
  `,
  styles: [`
    .wizard-steps {
      display: flex;
      gap: 20px;
      margin-bottom: 20px;
      font-size: 0.8125rem;
      color: var(--shell-muted-fg);
    }
    .wizard-steps .active { color: var(--content-fg); font-weight: 600; }
    .state-message { color: var(--shell-muted-fg); padding: 24px 0; text-align: center; }
    .wizard-inline { max-width: 900px; }
  `]
})
export class OnboardingWizardComponent implements OnInit {
  private onboardingApi = inject(OnboardingApiService);
  private legalEntitiesApi = inject(LegalEntitiesApiService);
  private departmentsApi = inject(DepartmentsApiService);
  private positionsApi = inject(PositionsApiService);
  private schedulesApi = inject(WorkSchedulesApiService);

  @Input() mode: 'modal' | 'inline' = 'modal';
  @Input() initialDraftId: string | null = null;
  @Output() closed = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  step = signal<1 | 2 | 3>(1);
  draftId = signal<string | null>(null);
  loadingDraft = signal(false);

  formData: Step1FormData = {
    employeeName: '',
    workEmail: '',
    employeeNumber: '',
    employmentType: 'full_time',
    startDate: '',
    legalEntityId: null,
    departmentId: null,
    positionId: null,
    scheduleId: null,
    selectedTemplateId: null,
    editedTasksJson: '[]',
    companyName: null,
    departmentName: null,
    positionName: null,
    scheduleName: null,
    reportingManagerName: null
  };

  selectedTemplateName = '';
  taskCount = 0;
  requiredTaskTitles: string[] = [];

  get wizardTitle(): string {
    if (this.step() === 1) return 'Add Employee — Employee & Assignment';
    if (this.step() === 2) return 'Add Employee — Checklist';
    return 'Add Employee — Review & Send Invite';
  }

  async ngOnInit() {
    if (this.initialDraftId) {
      await this.loadExistingDraft(this.initialDraftId);
    }
  }

  private async loadExistingDraft(id: string) {
    this.loadingDraft.set(true);
    try {
      const draft = await firstValueFrom(this.onboardingApi.getDraft(id));
      this.draftId.set(draft.id);
      this.formData = {
        employeeName: draft.employeeName,
        workEmail: draft.workEmail,
        employeeNumber: draft.employeeNumber ?? '',
        employmentType: draft.employmentType ?? 'full_time',
        startDate: draft.startDate ?? '',
        legalEntityId: draft.legalEntityId,
        departmentId: draft.departmentId,
        positionId: draft.positionId,
        scheduleId: draft.scheduleId,
        selectedTemplateId: draft.selectedTemplateId,
        editedTasksJson: draft.editedTasksJson || '[]',
        companyName: null,
        departmentName: null,
        positionName: null,
        scheduleName: null,
        reportingManagerName: null
      };
      await this.resolveDisplayNames();
      this.step.set(this.stepFromLastSaved(draft.lastSavedStep));
    } finally {
      this.loadingDraft.set(false);
    }
  }

  /// Resuming a draft can land directly on Step 2 or 3, skipping Step 1's onNext
  /// (which is what normally fills in the display names) — so resolve them here
  /// from the stored IDs whenever a draft is loaded with org assignment already set.
  private async resolveDisplayNames() {
    const legalEntityId = this.formData.legalEntityId;
    if (!legalEntityId) {
      return;
    }
    const [companies, departments, positions, schedules] = await Promise.all([
      firstValueFrom(this.legalEntitiesApi.list()) as Promise<any[]>,
      firstValueFrom(this.departmentsApi.list(legalEntityId)) as Promise<any[]>,
      firstValueFrom(this.positionsApi.list(legalEntityId)) as Promise<any[]>,
      firstValueFrom(this.schedulesApi.list(legalEntityId)) as Promise<any[]>
    ]);
    this.formData.companyName = companies.find(c => c.id === legalEntityId)?.name ?? null;
    this.formData.departmentName = departments.find((d: any) => d.id === this.formData.departmentId)?.name ?? null;
    this.formData.positionName = positions.find((p: any) => p.id === this.formData.positionId)?.name ?? null;
    this.formData.scheduleName = schedules.find((s: any) => s.id === this.formData.scheduleId)?.name ?? null;

    if (this.formData.positionId) {
      try {
        const manager = await firstValueFrom(this.positionsApi.getReportingManager(this.formData.positionId));
        this.formData.reportingManagerName = manager.hasManager ? (manager.employeeName ?? null) : null;
      } catch {
        this.formData.reportingManagerName = null;
      }
    }
  }

  private stepFromLastSaved(lastSavedStep: string): 1 | 2 | 3 {
    if (lastSavedStep === 'checklist_review') return 2;
    if (lastSavedStep === 'final_review') return 3;
    return 1;
  }

  async onStep1Next(data: Step1FormData) {
    this.formData = { ...this.formData, ...data };
    const payload = {
      id: this.draftId() ?? undefined,
      employeeName: this.formData.employeeName,
      workEmail: this.formData.workEmail,
      legalEntityId: this.formData.legalEntityId,
      departmentId: this.formData.departmentId,
      positionId: this.formData.positionId,
      employmentType: this.formData.employmentType,
      startDate: this.formData.startDate || null,
      employeeNumber: this.formData.employeeNumber || null,
      scheduleId: this.formData.scheduleId,
      lastSavedStep: 'org_assignment'
    };
    const result = await firstValueFrom(this.onboardingApi.saveDraft(payload));
    this.draftId.set(result.id);
    this.step.set(2);
  }

  async onStep2Next(result: Step2Result) {
    this.formData.selectedTemplateId = result.selectedTemplateId;
    this.formData.editedTasksJson = result.editedTasksJson;
    this.selectedTemplateName = result.templateName;
    this.taskCount = result.taskCount;
    this.requiredTaskTitles = result.requiredTaskTitles;

    await firstValueFrom(
      this.onboardingApi.updateChecklist(this.draftId()!, result.selectedTemplateId, result.editedTasksJson)
    );
    this.step.set(3);
  }

  onSent() {
    this.saved.emit();
  }

  onClose() {
    this.closed.emit();
  }
}
```

- [ ] **Step 2: Build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: build succeeds.

- [ ] **Step 3: Commit**

```bash
git add onevo-tenant-app/src/app/features/people/onboarding/onboarding-wizard.component.ts
git commit -m "feat: add onboarding wizard host component"
```

---

## Task 11: Frontend — My Drafts drawer

**Files:**
- Create: `onevo-tenant-app/src/app/features/people/onboarding/my-drafts-drawer.component.ts`

**Interfaces:**
- Consumes: `OnboardingApiService.getMyDrafts()` (Task 5), `ModalComponent` (Task 6).
- Produces: `app-my-drafts-drawer` component with `(closed)`/`(continueDraft: string)` outputs — consumed by Task 12.

- [ ] **Step 1: Create the component**

```typescript
import { Component, EventEmitter, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { ModalComponent } from '../../../shared/ui/modal/modal.component';
import { OnboardingApiService, MyDraftSummary } from '../../../core/api/endpoints/onboarding-api.service';

@Component({
  selector: 'app-my-drafts-drawer',
  standalone: true,
  imports: [CommonModule, ModalComponent],
  template: `
    <ov-modal [open]="true" variant="drawer" title="My Drafts" (closed)="closed.emit()">
      <div *ngIf="loading()" class="state-message">Loading your drafts…</div>
      <div *ngIf="!loading() && drafts().length === 0" class="state-message">
        You have no saved onboarding drafts.
      </div>
      <div class="draft-card" *ngFor="let d of drafts()">
        <div class="draft-name">{{ d.employeeName || 'Unnamed employee' }}</div>
        <div class="draft-email">{{ d.workEmail }}</div>
        <div class="draft-meta">
          <span>Step: {{ stepLabel(d.lastSavedStep) }}</span>
          <span>Reason: {{ reasonLabel(d.draftReason) }}</span>
          <span>Updated: {{ d.updatedAtUtc | date: 'short' }}</span>
        </div>
        <button type="button" class="btn btn-primary btn-sm" (click)="continueDraft.emit(d.id)">Continue</button>
      </div>
    </ov-modal>
  `,
  styles: [`
    .state-message { color: var(--shell-muted-fg); padding: 12px 0; }
    .draft-card {
      border: 1px solid var(--border-color);
      border-radius: 8px;
      padding: 16px;
      margin-bottom: 12px;
    }
    .draft-name { font-weight: 600; }
    .draft-email { color: var(--shell-muted-fg); font-size: 0.875rem; margin-bottom: 8px; }
    .draft-meta {
      display: flex;
      flex-wrap: wrap;
      gap: 4px 12px;
      font-size: 0.75rem;
      color: var(--shell-muted-fg);
      margin-bottom: 12px;
    }
  `]
})
export class MyDraftsDrawerComponent implements OnInit {
  private onboardingApi = inject(OnboardingApiService);

  @Output() closed = new EventEmitter<void>();
  @Output() continueDraft = new EventEmitter<string>();

  drafts = signal<MyDraftSummary[]>([]);
  loading = signal(true);

  async ngOnInit() {
    try {
      const result = await firstValueFrom(this.onboardingApi.getMyDrafts());
      this.drafts.set(result);
    } finally {
      this.loading.set(false);
    }
  }

  stepLabel(step: string): string {
    switch (step) {
      case 'employee_details':
      case 'org_assignment':
        return 'Employee & Assignment';
      case 'checklist_review':
        return 'Checklist';
      case 'final_review':
        return 'Review & Send';
      default:
        return step;
    }
  }

  reasonLabel(reason: string): string {
    switch (reason) {
      case 'saved_manually':
        return 'Saved manually';
      case 'waiting_for_seat':
        return 'Waiting for seat';
      case 'waiting_for_position_approval':
        return 'Waiting for approval';
      default:
        return reason || '—';
    }
  }
}
```

- [ ] **Step 2: Build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: build succeeds.

- [ ] **Step 3: Commit**

```bash
git add onevo-tenant-app/src/app/features/people/onboarding/my-drafts-drawer.component.ts
git commit -m "feat: add my drafts drawer component"
```

---

## Task 12: Frontend — Employees screen + thin onboarding route wrapper

**Files:**
- Rewrite: `onevo-tenant-app/src/app/features/people/people.component.ts`
- Rewrite: `onevo-tenant-app/src/app/features/people/onboarding/onboarding.component.ts`

**Interfaces:**
- Consumes: `EmployeesApiService.list` (Task 5), `DepartmentsApiService.list`, `LegalEntitiesApiService.list`, `OnboardingWizardComponent` (Task 10), `MyDraftsDrawerComponent` (Task 11), `HasPermissionDirective`, `PermissionService`.
- No route changes required — `app.routes.ts` already points `/people` at `PeopleComponent` and `/people/onboarding` (guarded `employees:write`) at `OnboardingComponent`.

- [ ] **Step 1: Read the current `onboarding.component.ts` one more time only to confirm no other file imports symbols from it besides the route**

```bash
grep -rn "onboarding.component" onevo-tenant-app/src/app --include=*.ts
```

Expected: only `app.routes.ts`. Proceed to fully replace its content.

- [ ] **Step 2: Rewrite `people.component.ts`**

```typescript
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { PageShellComponent } from '../../shared/ui/page-shell/page-shell.component';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header.component';
import { CardComponent } from '../../shared/ui/card/card.component';
import { StatusPillComponent } from '../../shared/ui/status-pill/status-pill.component';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';
import { EmployeesApiService, EmployeeListItem } from '../../core/api/endpoints/employees-api.service';
import { DepartmentsApiService } from '../../core/api/endpoints/departments-api.service';
import { LegalEntitiesApiService } from '../../core/api/endpoints/legal-entities-api.service';
import { OnboardingWizardComponent } from './onboarding/onboarding-wizard.component';
import { MyDraftsDrawerComponent } from './onboarding/my-drafts-drawer.component';

interface DepartmentOption {
  id: string;
  name: string;
}

@Component({
  selector: 'app-people',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    PageShellComponent,
    PageHeaderComponent,
    CardComponent,
    StatusPillComponent,
    HasPermissionDirective,
    OnboardingWizardComponent,
    MyDraftsDrawerComponent
  ],
  template: `
    <ov-page-shell>
      <ov-page-header title="Employees" subtitle="People visible to you based on your role and coverage.">
        <div actions>
          <button type="button" class="btn btn-outline" *hasPermission="'employees:write'" (click)="openMyDrafts()">
            My Drafts
          </button>
          <button type="button" class="btn btn-primary" *hasPermission="'employees:write'" (click)="openAddEmployee()">
            Add Employee
          </button>
        </div>
      </ov-page-header>

      <ov-card>
        <div class="filters-row">
          <input
            type="text"
            class="search-input"
            placeholder="Search by name, email, or employee number"
            [(ngModel)]="search"
            (ngModelChange)="onFiltersChanged()"
          />
          <select [(ngModel)]="statusFilter" (ngModelChange)="onFiltersChanged()">
            <option value="">All statuses</option>
            <option value="draft">Draft</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
          </select>
          <select [(ngModel)]="departmentFilter" (ngModelChange)="onFiltersChanged()">
            <option value="">All departments</option>
            <option *ngFor="let d of departments()" [value]="d.id">{{ d.name }}</option>
          </select>
        </div>

        <div *ngIf="loading()" class="state-message">Loading employees…</div>
        <div *ngIf="!loading() && error()" class="state-message error">{{ error() }}</div>
        <div *ngIf="!loading() && !error() && employees().length === 0" class="state-message">
          No employees match your current view.
        </div>

        <table class="employees-table" *ngIf="!loading() && employees().length > 0">
          <thead>
            <tr>
              <th>Name</th>
              <th>Work Email</th>
              <th>Employee #</th>
              <th>Position</th>
              <th>Department</th>
              <th>Status</th>
              <th>Start Date</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let e of employees()">
              <td>{{ e.firstName }} {{ e.lastName }}</td>
              <td>{{ e.workEmail }}</td>
              <td>{{ e.employeeNumber }}</td>
              <td>{{ e.positionName || '—' }}</td>
              <td>{{ e.departmentName || '—' }}</td>
              <td><ov-status-pill [status]="statusPillFor(e.status)">{{ e.status }}</ov-status-pill></td>
              <td>{{ e.hireDate | date: 'mediumDate' }}</td>
              <td></td>
            </tr>
          </tbody>
        </table>
      </ov-card>
    </ov-page-shell>

    <app-onboarding-wizard
      *ngIf="wizardOpen()"
      mode="modal"
      [initialDraftId]="wizardDraftId()"
      (closed)="closeWizard()"
      (saved)="onWizardSaved()"
    ></app-onboarding-wizard>

    <app-my-drafts-drawer
      *ngIf="myDraftsOpen()"
      (closed)="myDraftsOpen.set(false)"
      (continueDraft)="continueDraft($event)"
    ></app-my-drafts-drawer>
  `,
  styles: [`
    .filters-row { display: flex; gap: 12px; margin-bottom: 20px; }
    .search-input {
      flex: 1;
      padding: 8px 12px;
      border: 1px solid var(--border-color);
      border-radius: 6px;
      background: transparent;
      color: var(--content-fg);
    }
    .filters-row select {
      padding: 8px 12px;
      border: 1px solid var(--border-color);
      border-radius: 6px;
      background: transparent;
      color: var(--content-fg);
    }
    .state-message { color: var(--shell-muted-fg); padding: 24px 0; text-align: center; }
    .state-message.error { color: #dc2626; }
    .employees-table { width: 100%; border-collapse: collapse; }
    .employees-table th {
      text-align: left;
      font-size: 0.75rem;
      text-transform: uppercase;
      color: var(--shell-muted-fg);
      padding: 8px 12px;
      border-bottom: 1px solid var(--border-color);
    }
    .employees-table td {
      padding: 12px;
      border-bottom: 1px solid var(--border-color);
      font-size: 0.875rem;
    }
  `]
})
export class PeopleComponent implements OnInit {
  private employeesApi = inject(EmployeesApiService);
  private departmentsApi = inject(DepartmentsApiService);
  private legalEntitiesApi = inject(LegalEntitiesApiService);

  employees = signal<EmployeeListItem[]>([]);
  departments = signal<DepartmentOption[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  search = '';
  statusFilter = '';
  departmentFilter = '';

  wizardOpen = signal(false);
  wizardDraftId = signal<string | null>(null);
  myDraftsOpen = signal(false);

  async ngOnInit() {
    await this.loadDepartments();
    await this.loadEmployees();
  }

  private async loadDepartments() {
    try {
      const legalEntities = (await firstValueFrom(this.legalEntitiesApi.list())) as any[];
      const allDepartments: DepartmentOption[] = [];
      for (const le of legalEntities) {
        const depts = (await firstValueFrom(this.departmentsApi.list(le.id))) as any[];
        for (const d of depts) {
          allDepartments.push({ id: d.id, name: d.name });
        }
      }
      this.departments.set(allDepartments);
    } catch {
      this.departments.set([]);
    }
  }

  async loadEmployees() {
    this.loading.set(true);
    this.error.set(null);
    try {
      const result = await firstValueFrom(
        this.employeesApi.list({
          search: this.search || undefined,
          status: this.statusFilter || undefined,
          departmentId: this.departmentFilter || undefined
        })
      );
      this.employees.set(result);
    } catch {
      this.error.set('Could not load employees. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  onFiltersChanged() {
    this.loadEmployees();
  }

  statusPillFor(status: string): 'success' | 'warning' | 'danger' | 'neutral' | 'info' {
    switch (status) {
      case 'active':
        return 'success';
      case 'draft':
        return 'warning';
      case 'inactive':
        return 'neutral';
      default:
        return 'info';
    }
  }

  openAddEmployee() {
    this.wizardDraftId.set(null);
    this.wizardOpen.set(true);
  }

  openMyDrafts() {
    this.myDraftsOpen.set(true);
  }

  continueDraft(draftId: string) {
    this.myDraftsOpen.set(false);
    this.wizardDraftId.set(draftId);
    this.wizardOpen.set(true);
  }

  closeWizard() {
    this.wizardOpen.set(false);
    this.wizardDraftId.set(null);
  }

  onWizardSaved() {
    this.closeWizard();
    this.loadEmployees();
  }
}
```

- [ ] **Step 3: Rewrite `onboarding.component.ts` as a thin wrapper for the guarded direct route**

```typescript
import { Component } from '@angular/core';
import { PageShellComponent } from '../../../shared/ui/page-shell/page-shell.component';
import { OnboardingWizardComponent } from './onboarding-wizard.component';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [PageShellComponent, OnboardingWizardComponent],
  template: `
    <ov-page-shell>
      <app-onboarding-wizard mode="inline"></app-onboarding-wizard>
    </ov-page-shell>
  `
})
export class OnboardingComponent {}
```

- [ ] **Step 4: Build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: build succeeds with no errors.

- [ ] **Step 5: Commit**

```bash
git add onevo-tenant-app/src/app/features/people/people.component.ts onevo-tenant-app/src/app/features/people/onboarding/onboarding.component.ts
git commit -m "feat: rewrite Employees screen with scoped table + Add Employee/My Drafts, simplify onboarding route to wizard wrapper"
```

---

## Task 13: Backend — tests for the new read endpoints

**Files:**
- Create: `backend.Tests/PeopleWorkflowReadTests.cs`

**Interfaces:**
- Consumes: `CustomWebApplicationFactory`, `DatabaseSeeder.SeedAsync`, `TestDataSeeder.SeedMinimalTenantAsync` (existing test infrastructure), the endpoints added in Tasks 1–3.

- [ ] **Step 1: Read the existing test file for conventions before writing new tests**

Read `backend.Tests/OnboardingTests.cs` in full (login/CSRF helper pattern, `AuthenticateAsHrAdmin`) and skim `backend.Tests/TestInfrastructure/TestDataSeeder.cs` for `SeedMinimalTenantAsync`'s returned `SeededTenant` fields, since the tests below reuse both.

- [ ] **Step 2: Write the test file**

```csharp
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

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = seeded.UserEmail, password = seeded.UserPassword });
        var csrfToken = ExtractCsrfToken(loginResponse);
        _client.DefaultRequestHeaders.Remove("X-CSRF-Token");
        _client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);

        var response = await _client.GetAsync("/api/v1/employees");

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
```

- [ ] **Step 3: Run the new tests**

Run: `dotnet test backend.Tests/backend.Tests.csproj --no-restore --filter PeopleWorkflowReadTests`. Expected: all tests pass. If `hrAdminRole.RoleId` or `SeededTenant.UserEmail`/`UserPassword` field names differ from what Step 1's reading found, fix the test to match the actual property names before re-running — do not guess a second time.

- [ ] **Step 4: Run the full backend test suite**

Run: `dotnet test backend.Tests/backend.Tests.csproj --no-restore`. Expected: all tests pass, including the pre-existing `OnboardingTests.cs`.

- [ ] **Step 5: Commit**

```bash
git add backend.Tests/PeopleWorkflowReadTests.cs
git commit -m "test: cover employee list scoping/filters, drafts/mine isolation, template immutability"
```

---

## Task 14: Final verification

**Files:** none (verification only).

- [ ] **Step 1: Full backend build**

Run: `dotnet build --nologo -v q` from `backend/`. Expected: 0 errors.

- [ ] **Step 2: Full backend test suite**

Run: `dotnet test backend.Tests/backend.Tests.csproj --no-restore`. Expected: all tests pass.

- [ ] **Step 3: Full frontend build**

Run: `npm.cmd run build` from `onevo-tenant-app/`. Expected: 0 errors.

- [ ] **Step 4: Manual smoke check (if a dev server is available)**

Start the app, navigate to `/people`, confirm: the Employees table loads with department/position names; "Add Employee" opens the modal wizard starting at Step 1 with dropdowns (no GUID/Draft ID/JSON fields anywhere); completing Step 1 advances to Step 2 with a template picker and editable task rows; Step 3 shows a full review and a working Send Invite; after a successful invite the Employees list refreshes. Open "My Drafts", confirm a draft saved earlier appears and "Continue" resumes at the correct step.

- [ ] **Step 5: Final report**

Summarize: files changed, routes changed (none — `/people` and `/people/onboarding` unchanged), APIs added (`GET /api/v1/employees` filters, `GET /api/v1/onboarding/checklist-templates[/{id}]`, `GET /api/v1/onboarding/drafts/mine`, optional `GET /api/v1/org/positions/{id}/reporting-manager`), how the employee list is scoped (unchanged `ScopeResolverService` + new server-side search/status/department/position filters), how draftId/templateId/editedTasksJson stay hidden from HR (component-internal signals only, never rendered), how checklist editing avoids mutating the template (`OnboardingDraft.EditedTasksJson` only, template read-only), and build/test results.
