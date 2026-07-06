using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Models.DeveloperPlatform;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class PlatformAuthService : IPlatformAuthService
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(8);

    private readonly IPlatformUserRepository _platformUsers;
    private readonly IPasswordHasher _passwordHasher;

    public PlatformAuthService(IPlatformUserRepository platformUsers, IPasswordHasher passwordHasher)
    {
        _platformUsers = platformUsers;
        _passwordHasher = passwordHasher;
    }

    public async Task<PlatformAuthSessionResult?> LoginAsync(string email, string password, string? ipAddress, string? userAgent)
    {
        var user = await _platformUsers.GetByEmailAsync(email);
        var success = user is not null && user.IsActive && _passwordHasher.Verify(password, user.PasswordHash);

        await _platformUsers.AddAuthEventAsync(new PlatformAuthEvent
        {
            Id = Guid.NewGuid(),
            PlatformUserId = user?.Id,
            Email = email,
            EventType = success ? "login_success" : "login_failed",
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAtUtc = DateTime.UtcNow
        });

        if (!success)
        {
            await _platformUsers.SaveChangesAsync();
            return null;
        }

        var sessionToken = AuthService.GenerateToken();
        var csrfToken = AuthService.GenerateToken();
        var expiresAtUtc = DateTime.UtcNow.Add(SessionLifetime);

        await _platformUsers.AddSessionAsync(new PlatformUserSession
        {
            Id = Guid.NewGuid(),
            PlatformUserId = user!.Id,
            SessionTokenHash = AuthService.HashToken(sessionToken),
            CsrfTokenHash = AuthService.HashToken(csrfToken),
            ExpiresAtUtc = expiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _platformUsers.SaveChangesAsync();

        return new PlatformAuthSessionResult(
            user.Id,
            user.Email,
            user.DisplayName,
            sessionToken,
            csrfToken,
            expiresAtUtc);
    }

    public async Task LogoutAsync(string sessionToken)
    {
        var session = await _platformUsers.GetActiveSessionByTokenHashAsync(AuthService.HashToken(sessionToken));
        if (session is not null)
        {
            session.RevokedAtUtc = DateTime.UtcNow;
            await _platformUsers.SaveChangesAsync();
        }
    }

    public async Task<PlatformSessionValidationDto?> ValidateSessionAsync(string sessionToken)
    {
        var session = await _platformUsers.GetActiveSessionByTokenHashAsync(AuthService.HashToken(sessionToken));
        if (session is null)
        {
            return null;
        }

        var user = await _platformUsers.GetByIdAsync(session.PlatformUserId);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        var permissions = await _platformUsers.GetPermissionKeysForUserAsync(user.Id);
        return new PlatformSessionValidationDto(user.Id, user.Email, user.DisplayName, session.CsrfTokenHash, permissions);
    }

    public async Task<List<PlatformUserDto>> GetPlatformUsersAsync()
    {
        var users = await _platformUsers.GetAllAsync();
        return users
            .Select(u => new PlatformUserDto(
                u.Id,
                u.Email,
                u.DisplayName,
                u.IsActive,
                u.UserRoles
                    .Where(ur => ur.PlatformRole is not null)
                    .Select(ur => ur.PlatformRole!.Name)
                    .OrderBy(n => n)
                    .ToList()))
            .ToList();
    }

    public async Task<List<PlatformRoleDto>> GetPlatformRolesAsync()
    {
        var roles = await _platformUsers.GetRolesAsync();
        return roles
            .Select(r => new PlatformRoleDto(
                r.Id,
                r.Name,
                r.Description,
                r.IsSystemRole,
                r.RolePermissions
                    .Where(rp => rp.PlatformPermission is not null)
                    .Select(rp => rp.PlatformPermission!.PermissionKey)
                    .OrderBy(k => k)
                    .ToList()))
            .ToList();
    }
}
