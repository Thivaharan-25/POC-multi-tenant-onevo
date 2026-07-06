namespace OnevoHr.Api.DTOs.AppContext;

public sealed record AppContextDto(
    Guid TenantId,
    string TenantName,
    string TenantStatus,
    Guid UserId,
    string Email,
    string DisplayName,
    Guid? EmployeeId,
    IReadOnlyCollection<string> Permissions,
    IReadOnlyCollection<string> EnabledModuleKeys,
    IReadOnlyCollection<string> EnabledFeatureKeys,
    string ScopeLevel);
