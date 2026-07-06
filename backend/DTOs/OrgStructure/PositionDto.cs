namespace OnevoHr.Api.DTOs.OrgStructure;

public sealed record PositionDto(
    Guid Id,
    Guid LegalEntityId,
    Guid DepartmentId,
    string Name,
    string Code,
    Guid? ReportsToPositionId,
    int Capacity,
    string PositionType,
    string Status);
