namespace OnevoHr.Api.DTOs.Roles;

public sealed record RoleDto(
    Guid Id,
    string Name,
    string Description,
    bool IsSystemRole,
    IReadOnlyCollection<Guid> PermissionIds,
    IReadOnlyCollection<string> PermissionCodes);
