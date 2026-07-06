using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(Guid tenantId, string email)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Email == email);
    }

    public async Task<List<User>> GetByTenantAsync(Guid tenantId)
    {
        return await _db.Users.AsNoTracking()
            .Where(u => u.TenantId == tenantId && u.IsActive)
            .OrderBy(u => u.DisplayName)
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public async Task AddSessionAsync(UserSession session)
    {
        await _db.UserSessions.AddAsync(session);
    }

    public async Task<UserSession?> GetActiveSessionByTokenHashAsync(string sessionTokenHash)
    {
        return await _db.UserSessions.FirstOrDefaultAsync(s =>
            s.SessionTokenHash == sessionTokenHash &&
            s.RevokedAtUtc == null &&
            s.ExpiresAtUtc > DateTime.UtcNow);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
