using OnevoHr.Api.DTOs.Employees;

namespace OnevoHr.Api.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeListItemDto>> GetVisibleEmployeesAsync(EmployeeListQuery query);
    Task<EmployeeDto?> GetByIdAsync(Guid employeeId);
    Task<EmployeeDto?> CreateAsync(CreateEmployeeRequestDto request);
}
