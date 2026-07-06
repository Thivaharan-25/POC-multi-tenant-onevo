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

    public EmployeeService(
        IEmployeeRepository employees,
        ICurrentUserService currentUser,
        IScopeResolverService scopeResolver)
    {
        _employees = employees;
        _currentUser = currentUser;
        _scopeResolver = scopeResolver;
    }

    public async Task<List<EmployeeDto>> GetVisibleEmployeesAsync()
    {
        if (_currentUser.TenantId is not Guid tenantId || _currentUser.UserId is not Guid userId)
        {
            return new List<EmployeeDto>();
        }

        var scope = await _scopeResolver.ResolveScopeAsync(tenantId, userId);
        if (scope.VisibleEmployeeIds.Count == 0)
        {
            return new List<EmployeeDto>();
        }

        var employees = scope.ScopeLevel == "Tenant"
            ? await _employees.GetByTenantAsync(tenantId)
            : await _employees.GetByIdsAsync(tenantId, scope.VisibleEmployeeIds);
        return employees.Select(Map).ToList();
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
        return employee is null || employee.TenantId != tenantId ? null : Map(employee);
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
        return Map(employee);
    }

    private static EmployeeDto Map(Employee e)
    {
        return new EmployeeDto(
            e.Id, e.EmployeeNumber, e.FirstName, e.LastName, e.WorkEmail,
            e.Status, e.HireDate, e.LegalEntityId, e.DepartmentId, e.CurrentPositionId);
    }
}
