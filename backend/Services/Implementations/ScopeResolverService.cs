using OnevoHr.Api.DTOs.Auth;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Resolves the effective data scope for a user.
/// Manual UserRole scope (ScopeType/ScopeTargetId) wins when present;
/// otherwise scope is derived from active position assignments,
/// department head positions, legal entity placement and the hierarchy closure.
/// Scope levels: Own, DirectReports, Department, DepartmentAndChildren,
/// LegalEntity, MultiLegalEntity, Tenant.
/// </summary>
public class ScopeResolverService : IScopeResolverService
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IOrgRepository _org;
    private readonly IEmployeeRepository _employees;

    public ScopeResolverService(
        IUserRepository users,
        IRoleRepository roles,
        IOrgRepository org,
        IEmployeeRepository employees)
    {
        _users = users;
        _roles = roles;
        _org = org;
        _employees = employees;
    }

    public async Task<ScopeResolutionDto> ResolveScopeAsync(Guid tenantId, Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user is null || user.TenantId != tenantId)
        {
            return new ScopeResolutionDto("Own", Array.Empty<Guid>());
        }

        var userRoles = await _roles.GetUserRolesAsync(userId);

        // Manual UserRole scope override (directly assigned roles only).
        if (userRoles.Any(r => r.ScopeType == "tenant"))
        {
            return await TenantScopeAsync(tenantId);
        }

        var manualLegalEntityIds = userRoles
            .Where(r => r.ScopeType == "legal_entity" && r.ScopeTargetId is not null)
            .Select(r => r.ScopeTargetId!.Value)
            .ToList();
        if (manualLegalEntityIds.Count > 0)
        {
            return await LegalEntityScopeAsync(tenantId, manualLegalEntityIds, "LegalEntity");
        }

        var manualDepartmentIds = userRoles
            .Where(r => r.ScopeType == "department" && r.ScopeTargetId is not null)
            .Select(r => r.ScopeTargetId!.Value)
            .ToList();
        if (manualDepartmentIds.Count > 0)
        {
            return await DepartmentScopeAsync(tenantId, manualDepartmentIds, user.EmployeeId);
        }

        // Position-derived scope.
        if (user.EmployeeId is not Guid employeeId)
        {
            return new ScopeResolutionDto("Own", Array.Empty<Guid>());
        }

        var assignments = await _org.GetActiveAssignmentsForEmployeeAsync(tenantId, employeeId);
        var positions = assignments
            .Where(a => a.Position is not null)
            .Select(a => a.Position!)
            .ToList();

        var legalEntityIds = positions.Select(p => p.LegalEntityId).Distinct().ToList();
        if (legalEntityIds.Count > 1)
        {
            return await LegalEntityScopeAsync(tenantId, legalEntityIds, "MultiLegalEntity");
        }

        if (positions.Any(p => p.ReportsToPositionId is null))
        {
            return await LegalEntityScopeAsync(tenantId, legalEntityIds, "LegalEntity");
        }

        var departments = await _org.GetDepartmentsAsync(tenantId);
        var positionIds = positions.Select(p => p.Id).ToHashSet();
        var headedDepartmentIds = departments
            .Where(d => d.HeadPositionId is not null && positionIds.Contains(d.HeadPositionId.Value))
            .Select(d => d.Id)
            .ToList();
        if (headedDepartmentIds.Count > 0)
        {
            return await DepartmentScopeAsync(tenantId, headedDepartmentIds, employeeId);
        }

        var directReportIds = await _employees.GetDirectReportEmployeeIdsAsync(tenantId, employeeId);
        if (directReportIds.Count > 0)
        {
            var visible = directReportIds.Append(employeeId).Distinct().ToList();
            return new ScopeResolutionDto("DirectReports", visible);
        }

        return new ScopeResolutionDto("Own", new[] { employeeId });
    }

    private async Task<ScopeResolutionDto> TenantScopeAsync(Guid tenantId)
    {
        var all = await _employees.GetByTenantAsync(tenantId);
        return new ScopeResolutionDto("Tenant", all.Select(e => e.Id).ToList());
    }

    private async Task<ScopeResolutionDto> LegalEntityScopeAsync(Guid tenantId, IReadOnlyCollection<Guid> legalEntityIds, string scopeLevel)
    {
        var all = await _employees.GetByTenantAsync(tenantId);
        var visible = all
            .Where(e => legalEntityIds.Contains(e.LegalEntityId))
            .Select(e => e.Id)
            .ToList();
        return new ScopeResolutionDto(scopeLevel, visible);
    }

    private async Task<ScopeResolutionDto> DepartmentScopeAsync(Guid tenantId, IReadOnlyCollection<Guid> departmentIds, Guid? selfEmployeeId)
    {
        var all = await _employees.GetByTenantAsync(tenantId);
        var visible = all
            .Where(e => e.DepartmentId is not null && departmentIds.Contains(e.DepartmentId.Value))
            .Select(e => e.Id)
            .ToList();
        if (selfEmployeeId is Guid self && !visible.Contains(self))
        {
            visible.Add(self);
        }
        return new ScopeResolutionDto("Department", visible);
    }
}
