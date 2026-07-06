namespace OnevoHr.Api.DTOs.Permissions;

public sealed record OverrideRequestDto(
    Guid UserId,
    Guid PermissionId,
    string GrantType,
    string Reason,
    DateTimeOffset? ValidFrom,
    DateTimeOffset? ExpiresAt);

public sealed record OverrideDto(
    Guid Id,
    Guid UserId,
    Guid PermissionId,
    string PermissionCode,
    string GrantType,
    string Reason,
    DateTimeOffset? ValidFrom,
    DateTimeOffset? ExpiresAt,
    Guid GrantedBy,
    DateTimeOffset CreatedAt);
