using OnevoHr.Api.DTOs.Auth;

namespace OnevoHr.Api.Services.Interfaces;

/// <summary>
/// Scoped internal result holding resolved authentication state
/// for writing HttpOnly cookies. Never returned in public API JSON bodies.
/// </summary>
public sealed record AuthSessionResult(
    Guid UserId,
    Guid TenantId,
    Guid? EmployeeId,
    string DisplayName,
    string Email,
    string SessionToken,
    string CsrfToken,
    DateTime ExpiresAtUtc);

public interface IAuthService
{
    Task<AuthSessionResult?> LoginAsync(Guid tenantId, string email, string password);
    Task LogoutAsync(string sessionToken);
    Task<SessionValidationDto?> ValidateSessionAsync(string sessionToken);
    Task<AuthSessionResult?> RefreshSessionAsync(string sessionToken);
}
