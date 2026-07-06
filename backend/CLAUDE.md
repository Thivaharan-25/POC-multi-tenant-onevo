# CLAUDE.md

Guidance for Claude Code when working in this backend (OnevoHr.Api, ASP.NET Core + EF Core).

## Code style

This project is for learning — prefer explicit, readable code over compact code.

- Do NOT use expression-bodied (`=>`) methods for repository, service, or controller logic. Use normal block bodies.
- Async methods that wrap a single EF Core/repository call still get a block body with explicit `return await`:

```csharp
// Wrong
public Task<LeaveType?> GetLeaveTypeByCodeAsync(Guid tenantId, string code) =>
    _db.LeaveTypes.FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Code == code);

// Right
public async Task<LeaveType?> GetLeaveTypeByCodeAsync(Guid tenantId, string code)
{
    return await _db.LeaveTypes
        .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Code == code);
}
```

- Expression bodies are allowed only for very small pure computed properties or trivial one-line private helpers where they improve readability.
- Do not compress logic into clever one-liners; break dense expressions into explicit steps.
- Use LINQ only where EF Core queries require it; do not over-chain LINQ when simple sequential steps are clearer.
- Prefer explicit DTO type names at construction sites (`return new EmployeeDto(...)`) over target-typed `new(...)`.

## Build

- `dotnet build --nologo -v q` — quick build check from the backend root.

## ONEVO-HR coverage rule

Before deciding the final model list, read the relevant ONEVO-HR docs in:

`C:\onevoNew\OneVo-HR`

At minimum inspect:

- developer-platform/database/schema.md
- developer-platform/modules/demo-profiles/overview.md
- developer-platform/modules/requests-center/overview.md
- developer-platform/modules/subscription-manager/overview.md
- developer-platform/modules/module-catalog-manager/overview.md
- developer-platform/modules/role-template-manager/overview.md
- developer-platform/userflow/provisioning-flow.md
- modules/auth/overview.md
- modules/auth/authorization/overview.md
- modules/org-structure/overview.md
- modules/org-structure/positions/overview.md
- modules/core-hr/overview.md
- modules/leave/overview.md
- modules/shared-platform/subscriptions-billing/overview.md
- database/schemas/auth.md
- database/schemas/org-structure.md
- database/schemas/core-hr.md
- database/schemas/leave.md
- database/schemas/shared-platform.md
- database/schema-catalog.md

For every entity/table from those docs that belongs to this phase, either:

1. implement it as an EF Core model, or
2. explicitly list it in a "Deferred Entities" section with the reason.

Do not silently skip entities.

Deferred explanation format:

- Entity/table name
- Source doc path
- Why it is deferred
- What phase should add it later
- What dependency or feature requires it

This first backend phase should include all entities required for:

- Developer Platform demo request approval
- Demo Profile
- demo tenant creation
- demo-to-paid activation
- subscription plan and add-ons
- module catalog
- feature/permission entitlement
- tenant auth/session
- roles and permissions
- direct user roles with optional manual scope
- position-linked roles
- legal entities
- departments
- positions
- position assignments
- employees
- leave policy basics
- leave request basics
- workflow/approval basics
- notifications/outbox
- template application
- cookie auth
- permission middleware
- scope resolver foundation

If an entity from ONEVO-HR is not needed for those goals, defer it with a clear reason.
