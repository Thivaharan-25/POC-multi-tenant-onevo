using System.Linq;
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

        if (query.LegalEntityId.HasValue)
        {
            result = result.Where(e => e.LegalEntityId == query.LegalEntityId.Value).ToList();
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
