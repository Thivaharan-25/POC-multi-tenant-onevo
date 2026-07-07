using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(Guid tenantId, string email);
    Task<List<User>> GetByTenantAsync(Guid tenantId);
    Task AddAsync(User user);
    Task AddSessionAsync(UserSession session);
    Task<UserSession?> GetActiveSessionByTokenHashAsync(string sessionTokenHash);
    Task AddInvitationTokenAsync(InvitationToken token);
    Task<InvitationToken?> GetInvitationTokenByHashAsync(string tokenHash);
    Task SaveChangesAsync();
}
