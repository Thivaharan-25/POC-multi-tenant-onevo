using OnevoHr.Api.Models.Auth;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<List<Role>> GetByTenantAsync(Guid tenantId);
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(Guid tenantId, string name);
    Task<List<UserRole>> GetUserRolesAsync(Guid userId);
    Task AddAsync(Role role);
    Task AddRolePermissionAsync(RolePermission rolePermission);
    Task AddUserRoleAsync(UserRole userRole);
    Task ReplaceRolePermissionsAsync(Guid tenantId, Guid roleId, IReadOnlyCollection<Guid> permissionIds);
    Task SaveChangesAsync();
}
