using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<List<Permission>> GetTenantPermissionCatalogAsync();
    Task<IReadOnlyList<string>> GetEffectivePermissionKeysAsync(Guid tenantId, Guid userId);
    Task<List<Permission>> GetAllPermissionsAsync();
    Task<List<UserPermissionOverride>> GetActiveOverridesAsync(Guid tenantId, Guid userId, DateTimeOffset asOf);
    Task AddOverrideAsync(UserPermissionOverride entity);
    Task<UserPermissionOverride?> GetOverrideByIdAsync(Guid tenantId, Guid overrideId);
    Task RemoveOverrideAsync(UserPermissionOverride entity);
    Task<List<UserPermissionOverride>> GetOverridesForUserAsync(Guid tenantId, Guid userId);
    Task SaveChangesAsync();
}
