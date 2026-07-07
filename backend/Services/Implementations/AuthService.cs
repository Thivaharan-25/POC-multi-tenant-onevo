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

    public async Task<SafeInvitationDto?> ValidateInvitationTokenAsync(string token)
    {
        var tokenHash = HashToken(token);
        var invite = await _users.GetInvitationTokenByHashAsync(tokenHash);

        if (invite == null || invite.Status != "pending" || invite.UsedAt.HasValue || invite.RevokedAt.HasValue)
        {
            return null;
        }

        if (invite.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return null; // Expired
        }

        string[] completionMethods = string.IsNullOrWhiteSpace(invite.CompletionMethodsJson) 
            ? Array.Empty<string>() 
            : System.Text.Json.JsonSerializer.Deserialize<string[]>(invite.CompletionMethodsJson) ?? Array.Empty<string>();

        // We can get TenantName if we load the tenant, but for now we just return null or if the repository already loaded it, we use it.
        // IUserRepository might not load Tenant. We can leave it null.
        return new SafeInvitationDto(
            invite.InvitedEmail,
            invite.InvitedFullName,
            completionMethods,
            invite.ExpiresAt,
            null);
    }

    public async Task<AuthSessionResult?> AcceptInvitationWithPasswordAsync(string token, string password)
    {
        var tokenHash = HashToken(token);
        var invite = await _users.GetInvitationTokenByHashAsync(tokenHash);

        if (invite == null || invite.Status != "pending" || invite.UsedAt.HasValue || invite.RevokedAt.HasValue)
        {
            return null;
        }

        if (invite.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return null; // Expired
        }

        string[] completionMethods = string.IsNullOrWhiteSpace(invite.CompletionMethodsJson) 
            ? Array.Empty<string>() 
            : System.Text.Json.JsonSerializer.Deserialize<string[]>(invite.CompletionMethodsJson) ?? Array.Empty<string>();

        if (!completionMethods.Contains("password"))
        {
            return null; // Password not allowed
        }

        if (!ValidatePasswordPolicy(password))
        {
            throw new ArgumentException("Password does not meet complexity requirements."); // Or return null/custom result
        }

        var user = await _users.GetByIdAsync(invite.UserId);
        if (user == null)
        {
            return null;
        }

        // Validate email matches
        if (!string.Equals(user.Email, invite.InvitedEmail, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // Update user
        user.PasswordHash = _passwordHasher.Hash(password);
        user.IsActive = true;
        user.PasswordSetupRequired = false;
        user.PasswordSetupExpiresAt = null;

        // Update invite
        invite.Status = "completed";
        invite.CompletedWith = "password";
        invite.UsedAt = DateTimeOffset.UtcNow;

        await _users.SaveChangesAsync();

        // Create immediate session
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

    private bool ValidatePasswordPolicy(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;
        
        bool hasUpper = false;
        bool hasLower = false;
        bool hasDigit = false;
        bool hasSpecial = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else hasSpecial = true; // non-alphanumeric
        }

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
