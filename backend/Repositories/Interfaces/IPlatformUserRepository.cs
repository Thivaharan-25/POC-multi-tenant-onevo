using OnevoHr.Api.Models.DeveloperPlatform;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IPlatformUserRepository
{
    Task<PlatformUser?> GetByEmailAsync(string email);
    Task<PlatformUser?> GetByIdAsync(Guid id);
    Task<List<PlatformUser>> GetAllAsync();
    Task<List<PlatformRole>> GetRolesAsync();
    Task<IReadOnlyList<string>> GetPermissionKeysForUserAsync(Guid platformUserId);
    Task AddSessionAsync(PlatformUserSession session);
    Task<PlatformUserSession?> GetActiveSessionByTokenHashAsync(string sessionTokenHash);
    Task AddAuthEventAsync(PlatformAuthEvent authEvent);
    Task SaveChangesAsync();
}
