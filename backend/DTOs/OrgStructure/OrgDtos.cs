namespace OnevoHr.Api.DTOs.OrgStructure;

public sealed record LegalEntityDto(
    Guid Id, 
    string Name, 
    string Code, 
    string Status,
    string Country,
    string Currency,
    string Timezone,
    string Address);

public sealed record DepartmentDto(
    Guid Id,
    Guid LegalEntityId,
    string Name,
    string Code,
    Guid? ParentDepartmentId,
    Guid? HeadPositionId,
    string Status);

public sealed record CreatePositionRequestDto(
    Guid LegalEntityId,
    Guid DepartmentId,
    string Name,
    string Code,
    Guid? ReportsToPositionId,
    int Capacity,
    string PositionType);

public sealed record ReportingManagerDto(Guid EmployeeId, string EmployeeName);
