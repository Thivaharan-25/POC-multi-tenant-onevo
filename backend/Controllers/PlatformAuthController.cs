using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/auth")]
public sealed class PlatformAuthController : ControllerBase
{
    private readonly IPlatformAuthService _platformAuth;

    public PlatformAuthController(IPlatformAuthService platformAuth)
    {
        _platformAuth = platformAuth;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] PlatformLoginRequestDto request)
    {
        var result = await _platformAuth.LoginAsync(
            request.Email,
            request.Password,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers.UserAgent.FirstOrDefault());

        if (result is null)
        {
            return Unauthorized(new { error = "Invalid email or password." });
        }

        // Set HttpOnly session cookie scoped to /admin
        Response.Cookies.Append(CurrentUserMiddleware.PlatformSessionCookie, result.SessionToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/admin",
            Expires = result.ExpiresAtUtc
        });

        // Set readable CSRF cookie scoped to /admin
        Response.Cookies.Append("onevo_platform_csrf", result.CsrfToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/admin",
            Expires = result.ExpiresAtUtc
        });

        // Safe platform response (no tokens)
        var responseUser = new PlatformLoginResponseDto(result.PlatformUserId, result.Email, result.DisplayName);
        return Ok(responseUser);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue(CurrentUserMiddleware.PlatformSessionCookie, out var sessionToken))
        {
            await _platformAuth.LogoutAsync(sessionToken);
        }

        Response.Cookies.Delete(CurrentUserMiddleware.PlatformSessionCookie, new CookieOptions { Path = "/admin" });
        Response.Cookies.Delete("onevo_platform_csrf", new CookieOptions { Path = "/admin" });
        return Ok(new { loggedOut = true });
    }

    [HttpGet("session")]
    public IActionResult Session()
    {
        if (HttpContext.Items[CurrentUserMiddleware.PlatformSessionItemKey] is not PlatformSessionValidationDto session)
        {
            return Ok(new { authenticated = false });
        }

        return Ok(new
        {
            authenticated = true,
            platformUserId = session.PlatformUserId,
            email = session.Email,
            displayName = session.DisplayName,
            permissions = session.Permissions
        });
    }
}
