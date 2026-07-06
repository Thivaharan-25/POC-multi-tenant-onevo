using OnevoHr.Api.DTOs.Admin;

namespace OnevoHr.Api.Services.Interfaces;

/// <summary>
/// Scoped internal result holding platform authentication state
/// for writing HttpOnly cookies. Never returned in public API JSON bodies.
/// </summary>
public sealed record PlatformAuthSessionResult(
    Guid PlatformUserId,
    string Email,
    string DisplayName,
    string SessionToken,
    string CsrfToken,
    DateTime ExpiresAtUtc);

public interface IPlatformAuthService
{
    Task<PlatformAuthSessionResult?> LoginAsync(string email, string password, string? ipAddress, string? userAgent);
    Task LogoutAsync(string sessionToken);
    Task<PlatformSessionValidationDto?> ValidateSessionAsync(string sessionToken);
    Task<List<PlatformUserDto>> GetPlatformUsersAsync();
    Task<List<PlatformRoleDto>> GetPlatformRolesAsync();
}
