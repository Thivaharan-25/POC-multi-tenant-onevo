using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.DeveloperPlatform;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class PlatformUserRepository : IPlatformUserRepository
{
    private readonly AppDbContext _db;

    public PlatformUserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PlatformUser?> GetByEmailAsync(string email)
    {
        return await _db.PlatformUsers
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<PlatformUser?> GetByIdAsync(Guid id)
    {
        return await _db.PlatformUsers
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<PlatformUser>> GetAllAsync()
    {
        return await _db.PlatformUsers.AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.PlatformRole)
            .OrderBy(u => u.Email)
            .ToListAsync();
    }

    public async Task<List<PlatformRole>> GetRolesAsync()
    {
        return await _db.PlatformRoles.AsNoTracking()
            .Include(r => r.RolePermissions)
            .ThenInclude(p => p.PlatformPermission)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetPermissionKeysForUserAsync(Guid platformUserId)
    {
        return await (
            from ur in _db.PlatformUserRoles.AsNoTracking()
            join rp in _db.PlatformRolePermissions.AsNoTracking() on ur.PlatformRoleId equals rp.PlatformRoleId
            join p in _db.PlatformPermissions.AsNoTracking() on rp.PlatformPermissionId equals p.Id
            where ur.PlatformUserId == platformUserId && p.IsActive
            select p.PermissionKey).Distinct().ToListAsync();
    }

    public async Task AddSessionAsync(PlatformUserSession session)
    {
        await _db.PlatformUserSessions.AddAsync(session);
    }

    public async Task<PlatformUserSession?> GetActiveSessionByTokenHashAsync(string sessionTokenHash)
    {
        return await _db.PlatformUserSessions.FirstOrDefaultAsync(s =>
            s.SessionTokenHash == sessionTokenHash &&
            s.RevokedAtUtc == null &&
            s.ExpiresAtUtc > DateTime.UtcNow);
    }

    public async Task AddAuthEventAsync(PlatformAuthEvent authEvent)
    {
        await _db.PlatformAuthEvents.AddAsync(authEvent);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
