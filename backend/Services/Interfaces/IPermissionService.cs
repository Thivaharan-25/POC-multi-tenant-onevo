using OnevoHr.Api.DTOs.Permissions;
using OnevoHr.Api.DTOs.Roles;

namespace OnevoHr.Api.Services.Interfaces;

public interface IPermissionService
{
    Task<List<PermissionDto>> GetTenantPermissionCatalogAsync(Guid tenantId);
    Task<List<RoleDto>> GetTenantRolesAsync(Guid tenantId);
    Task<IReadOnlyCollection<string>> GetEffectivePermissionsAsync(Guid tenantId, Guid userId);
    Task<bool> HasPermissionAsync(Guid tenantId, Guid userId, string permissionKey);
    Task<List<OverrideDto>> GetOverridesForUserAsync(Guid tenantId, Guid userId);
    Task<OverrideDto> CreateOverrideAsync(Guid tenantId, Guid grantedByUserId, OverrideRequestDto request);
    Task RevokeOverrideAsync(Guid tenantId, Guid overrideId);
    Task<RoleDto> CreateRoleAsync(Guid tenantId, CreateRoleRequestDto request);
    Task SetRolePermissionsAsync(Guid tenantId, Guid roleId, SetRolePermissionsRequestDto request);
    Task AssignUserRoleAsync(Guid tenantId, AssignUserRoleRequestDto request);
    Task<List<TenantUserDto>> GetTenantUsersAsync(Guid tenantId);
}
