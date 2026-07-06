using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _db;

    public PermissionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Permission>> GetTenantPermissionCatalogAsync()
    {
        return await _db.Permissions.AsNoTracking()
            .OrderBy(p => p.Code)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetEffectivePermissionKeysAsync(Guid tenantId, Guid userId)
    {
        var user = await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId);
        if (user is null)
        {
            return Array.Empty<string>();
        }

        var roleIds = await _db.UserRoles.AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (roleIds.Count == 0)
        {
            return Array.Empty<string>();
        }

        return await (
            from rp in _db.RolePermissions.AsNoTracking()
            join p in _db.Permissions.AsNoTracking() on rp.PermissionId equals p.Id
            where roleIds.Contains(rp.RoleId)
            select p.Code).Distinct().ToListAsync();
    }

    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
        return await _db.Permissions.AsNoTracking().ToListAsync();
    }

    public async Task<List<UserPermissionOverride>> GetActiveOverridesAsync(Guid tenantId, Guid userId, DateTimeOffset asOf)
    {
        return await _db.UserPermissionOverrides.AsNoTracking()
            .Where(o => o.TenantId == tenantId && o.UserId == userId)
            .Where(o => o.ValidFrom == null || o.ValidFrom <= asOf)
            .Where(o => o.ExpiresAt == null || o.ExpiresAt > asOf)
            .ToListAsync();
    }

    public async Task AddOverrideAsync(UserPermissionOverride entity)
    {
        await _db.UserPermissionOverrides.AddAsync(entity);
    }

    public async Task<UserPermissionOverride?> GetOverrideByIdAsync(Guid tenantId, Guid overrideId)
    {
        return await _db.UserPermissionOverrides
            .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.Id == overrideId);
    }

    public async Task RemoveOverrideAsync(UserPermissionOverride entity)
    {
        _db.UserPermissionOverrides.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<List<UserPermissionOverride>> GetOverridesForUserAsync(Guid tenantId, Guid userId)
    {
        return await _db.UserPermissionOverrides.AsNoTracking()
            .Where(o => o.TenantId == tenantId && o.UserId == userId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
