namespace OnevoHr.Api.DTOs.Employees;

public sealed record EmployeeDto(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string WorkEmail,
    string Status,
    DateOnly HireDate,
    Guid LegalEntityId,
    Guid? DepartmentId,
    Guid? CurrentPositionId);

public sealed record CreateEmployeeRequestDto(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string WorkEmail,
    DateOnly HireDate,
    Guid LegalEntityId,
    Guid? DepartmentId);
