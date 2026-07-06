using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _db;

    public RoleRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Role>> GetByTenantAsync(Guid tenantId)
    {
        return await _db.Roles.AsNoTracking()
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _db.Roles
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetByNameAsync(Guid tenantId, string name)
    {
        return await _db.Roles
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Name == name);
    }

    public async Task<List<UserRole>> GetUserRolesAsync(Guid userId)
    {
        return await _db.UserRoles.AsNoTracking()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Role role)
    {
        await _db.Roles.AddAsync(role);
    }

    public async Task AddRolePermissionAsync(RolePermission rolePermission)
    {
        await _db.RolePermissions.AddAsync(rolePermission);
    }

    public async Task AddUserRoleAsync(UserRole userRole)
    {
        await _db.UserRoles.AddAsync(userRole);
    }

    public async Task ReplaceRolePermissionsAsync(Guid tenantId, Guid roleId, IReadOnlyCollection<Guid> permissionIds)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId && r.TenantId == tenantId);
        if (role is null)
        {
            return;
        }

        var existing = await _db.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        _db.RolePermissions.RemoveRange(existing);

        foreach (var permissionId in permissionIds)
        {
            await _db.RolePermissions.AddAsync(new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = permissionId
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
