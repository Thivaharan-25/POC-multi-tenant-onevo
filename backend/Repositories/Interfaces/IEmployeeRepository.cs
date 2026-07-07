using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetByTenantAsync(Guid tenantId);
    Task<List<Employee>> GetByIdsAsync(Guid tenantId, IReadOnlyCollection<Guid> employeeIds);
    Task<Employee?> GetByIdAsync(Guid id);
    Task<List<Guid>> GetDirectReportEmployeeIdsAsync(Guid tenantId, Guid managerEmployeeId);
    Task<Employee?> GetByWorkEmailAsync(Guid tenantId, string workEmail);
    Task<Employee?> GetByEmployeeNumberAsync(Guid tenantId, string employeeNumber);
    Task<int> CountOnboardingAndActiveAsync(Guid tenantId);
    Task AddAsync(Employee employee);
    Task AddLifecycleEventAsync(EmployeeLifecycleEvent lifecycleEvent);
    Task AddChecklistTaskAsync(EmployeeChecklistTask task);
    Task SaveChangesAsync();
}
