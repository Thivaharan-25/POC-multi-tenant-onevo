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
