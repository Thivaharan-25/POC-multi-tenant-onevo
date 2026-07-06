namespace OnevoHr.Api.DTOs.Permissions;

public sealed record PermissionDto(Guid Id, string Code, string Description, string Module, string? FeatureKey);
