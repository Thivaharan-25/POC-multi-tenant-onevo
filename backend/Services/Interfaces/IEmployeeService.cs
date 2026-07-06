using OnevoHr.Api.DTOs.Employees;

namespace OnevoHr.Api.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetVisibleEmployeesAsync();
    Task<EmployeeDto?> GetByIdAsync(Guid employeeId);
    Task<EmployeeDto?> CreateAsync(CreateEmployeeRequestDto request);
}
