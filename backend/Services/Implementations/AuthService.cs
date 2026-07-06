using System.Security.Cryptography;
using System.Text;
using OnevoHr.Api.DTOs.Auth;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(8);

    private readonly IUserRepository _users;
    private readonly IPermissionRepository _permissions;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository users, IPermissionRepository permissions, IPasswordHasher passwordHasher)
    {
        _users = users;
        _permissions = permissions;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthSessionResult?> LoginAsync(Guid tenantId, string email, string password)
    {
        var user = await _users.GetByEmailAsync(tenantId, email);
        if (user is null || !user.IsActive || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            return null;
        }

        var sessionToken = GenerateToken();
        var csrfToken = GenerateToken();
        var expiresAtUtc = DateTime.UtcNow.Add(SessionLifetime);

        await _users.AddSessionAsync(new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TenantId = tenantId,
            SessionTokenHash = HashToken(sessionToken),
            CsrfTokenHash = HashToken(csrfToken),
            ExpiresAtUtc = expiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _users.SaveChangesAsync();

        return new AuthSessionResult(
            user.Id,
            user.TenantId,
            user.EmployeeId,
            user.DisplayName,
            user.Email,
            sessionToken,
            csrfToken,
            expiresAtUtc);
    }

    public async Task<AuthSessionResult?> RefreshSessionAsync(string oldSessionToken)
    {
        var oldSession = await _users.GetActiveSessionByTokenHashAsync(HashToken(oldSessionToken));
        if (oldSession is null)
        {
            return null;
        }

        var user = await _users.GetByIdAsync(oldSession.UserId);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        // Revoke the old session immediately
        oldSession.RevokedAtUtc = DateTime.UtcNow;

        // Generate rotated tokens
        var sessionToken = GenerateToken();
        var csrfToken = GenerateToken();
        var expiresAtUtc = DateTime.UtcNow.Add(SessionLifetime);

        await _users.AddSessionAsync(new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TenantId = user.TenantId,
            SessionTokenHash = HashToken(sessionToken),
            CsrfTokenHash = HashToken(csrfToken),
            ExpiresAtUtc = expiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _users.SaveChangesAsync();

        return new AuthSessionResult(
            user.Id,
            user.TenantId,
            user.EmployeeId,
            user.DisplayName,
            user.Email,
            sessionToken,
            csrfToken,
            expiresAtUtc);
    }

    public async Task LogoutAsync(string sessionToken)
    {
        var session = await _users.GetActiveSessionByTokenHashAsync(HashToken(sessionToken));
        if (session is not null)
        {
            session.RevokedAtUtc = DateTime.UtcNow;
            await _users.SaveChangesAsync();
        }
    }

    public async Task<SessionValidationDto?> ValidateSessionAsync(string sessionToken)
    {
        var session = await _users.GetActiveSessionByTokenHashAsync(HashToken(sessionToken));
        if (session is null)
        {
            return null;
        }

        var user = await _users.GetByIdAsync(session.UserId);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        var permissions = await _permissions.GetEffectivePermissionKeysAsync(user.TenantId, user.Id);
        return new SessionValidationDto(
            user.Id,
            user.TenantId,
            user.EmployeeId,
            user.Email,
            user.DisplayName,
            session.CsrfTokenHash,
            permissions);
    }

    internal static string GenerateToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
    }

    internal static string HashToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();
    }
}
