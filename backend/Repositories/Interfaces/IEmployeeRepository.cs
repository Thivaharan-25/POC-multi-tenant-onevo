using OnevoHr.Api.Models.Employees;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetByTenantAsync(Guid tenantId);
    Task<List<Employee>> GetByIdsAsync(Guid tenantId, IReadOnlyCollection<Guid> employeeIds);
    Task<Employee?> GetByIdAsync(Guid id);
    Task<List<Guid>> GetDirectReportEmployeeIdsAsync(Guid tenantId, Guid managerEmployeeId);
    Task AddAsync(Employee employee);
    Task SaveChangesAsync();
}
