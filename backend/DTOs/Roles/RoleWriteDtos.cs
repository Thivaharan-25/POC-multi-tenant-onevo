namespace OnevoHr.Api.DTOs.Roles;

public sealed record CreateRoleRequestDto(string Name, string Description);
public sealed record SetRolePermissionsRequestDto(IReadOnlyCollection<Guid> PermissionIds);
public sealed record AssignUserRoleRequestDto(Guid UserId, Guid RoleId);
public sealed record TenantUserDto(Guid Id, string Email, string DisplayName);
