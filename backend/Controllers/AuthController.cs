using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OnevoHr.Api.DTOs.Auth;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Services.Notifications;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITenantContextService _tenantContext;
    private readonly IPermissionService _permissionService;
    private readonly IFeatureGateService _featureGateService;
    private readonly ITenantProvisioningService _tenantProvisioningService;
    private readonly IWebHostEnvironment _env;
    private readonly IOutboxService _outbox;

    public AuthController(
        IAuthService authService,
        ITenantContextService tenantContext,
        IPermissionService permissionService,
        IFeatureGateService featureGateService,
        ITenantProvisioningService tenantProvisioningService,
        IWebHostEnvironment env,
        IOutboxService outbox)
    {
        _authService = authService;
        _tenantContext = tenantContext;
        _permissionService = permissionService;
        _featureGateService = featureGateService;
        _tenantProvisioningService = tenantProvisioningService;
        _env = env;
        _outbox = outbox;
    }

    private CookieOptions GetSessionCookieOptions(DateTime? expires = null)
    {
        var isDev = _env.IsDevelopment();
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            SameSite = SameSiteMode.Lax,
            Path = "/api",
            Expires = expires
        };
    }

    private CookieOptions GetCsrfCookieOptions(DateTime? expires = null)
    {
        var isDev = _env.IsDevelopment();
        return new CookieOptions
        {
            HttpOnly = false,
            Secure = !isDev,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = expires
        };
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!_tenantContext.HasTenant)
        {
            return BadRequest(new { error = "Tenant could not be resolved. Send X-Tenant-Domain or use a tenant domain." });
        }

        var result = await _authService.LoginAsync(_tenantContext.TenantId!.Value, request.Email, request.Password);
        if (result is null)
        {
            return Unauthorized(new { error = "Invalid email or password." });
        }

        // Set HttpOnly session cookie
        Response.Cookies.Append(CurrentUserMiddleware.TenantSessionCookie, result.SessionToken, GetSessionCookieOptions(result.ExpiresAtUtc));

        // Set readable CSRF cookie
        Response.Cookies.Append("onevo_csrf", result.CsrfToken, GetCsrfCookieOptions(result.ExpiresAtUtc));

        // Enqueue event to outbox
        await _outbox.EnqueueAsync(result.TenantId, "login_succeeded", $"{{\"userId\":\"{result.UserId}\",\"email\":\"{result.Email}\"}}");

        var sessionDto = await BuildSessionDtoAsync(
            result.TenantId,
            result.UserId,
            result.EmployeeId,
            result.Email,
            result.DisplayName);

        return Ok(sessionDto);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue(CurrentUserMiddleware.TenantSessionCookie, out var sessionToken))
        {
            await _authService.LogoutAsync(sessionToken);
        }

        Response.Cookies.Delete(CurrentUserMiddleware.TenantSessionCookie, GetSessionCookieOptions());
        Response.Cookies.Delete("onevo_csrf", GetCsrfCookieOptions());
        return Ok(new { loggedOut = true });
    }

    [HttpGet("session")]
    public async Task<IActionResult> Session()
    {
        if (HttpContext.Items[CurrentUserMiddleware.SessionItemKey] is not SessionValidationDto session)
        {
            if (Request.Cookies.ContainsKey(CurrentUserMiddleware.TenantSessionCookie))
            {
                return Unauthorized(new { error = "Session expired or invalid." });
            }

            // Unauthenticated: return SessionDto with null user
            return Ok(new SessionDto(null, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), false));
        }

        var sessionDto = await BuildSessionDtoAsync(
            session.TenantId,
            session.UserId,
            session.EmployeeId,
            session.Email,
            session.DisplayName);

        return Ok(sessionDto);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (Request.Cookies.TryGetValue(CurrentUserMiddleware.TenantSessionCookie, out var sessionToken))
        {
            var result = await _authService.RefreshSessionAsync(sessionToken);
            if (result is null)
            {
                return Unauthorized(new { error = "Session expired or invalid." });
            }

            // Set rotated session cookie
            Response.Cookies.Append(CurrentUserMiddleware.TenantSessionCookie, result.SessionToken, GetSessionCookieOptions(result.ExpiresAtUtc));

            // Set rotated readable CSRF cookie
            Response.Cookies.Append("onevo_csrf", result.CsrfToken, GetCsrfCookieOptions(result.ExpiresAtUtc));

            var sessionDto = await BuildSessionDtoAsync(
                result.TenantId,
                result.UserId,
                result.EmployeeId,
                result.Email,
                result.DisplayName);

            return Ok(sessionDto);
        }

        return Unauthorized(new { error = "No session cookie found." });
    }

    [HttpGet("invitations/validate")]
    public async Task<IActionResult> ValidateInvitation([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { error = "Token is required." });
        }

        var result = await _authService.ValidateInvitationTokenAsync(token);
        if (result == null)
        {
            return BadRequest(new { error = "Invalid, expired, or revoked invitation." });
        }

        return Ok(result);
    }

    [HttpPost("invitations/accept")]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInviteRequestDto request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new { error = "Passwords do not match." });
        }

        AuthSessionResult? result;
        try
        {
            result = await _authService.AcceptInvitationWithPasswordAsync(request.Token, request.Password);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        if (result == null)
        {
            return BadRequest(new { error = "Invalid or expired invitation, or password not allowed." });
        }

        // Set HttpOnly session cookie
        Response.Cookies.Append(CurrentUserMiddleware.TenantSessionCookie, result.SessionToken, GetSessionCookieOptions(result.ExpiresAtUtc));

        // Set readable CSRF cookie
        Response.Cookies.Append("onevo_csrf", result.CsrfToken, GetCsrfCookieOptions(result.ExpiresAtUtc));

        var sessionDto = await BuildSessionDtoAsync(
            result.TenantId,
            result.UserId,
            result.EmployeeId,
            result.Email,
            result.DisplayName);

        return Ok(sessionDto);
    }

    private async Task<SessionDto> BuildSessionDtoAsync(
        Guid tenantId,
        Guid userId,
        Guid? employeeId,
        string email,
        string displayName)
    {
        var permissions = await _permissionService.GetEffectivePermissionsAsync(tenantId, userId);
        var activeModules = await _featureGateService.GetEnabledModuleKeysAsync(tenantId);
        var activeFeatures = await _featureGateService.GetEnabledFeatureKeysAsync(tenantId);

        var provisioningState = await _tenantProvisioningService.GetProvisioningStateAsync(tenantId);
        var setupComplete = provisioningState?.CurrentStep == "completed" 
            || provisioningState?.ActivationReady == true;

        var userDto = new SessionUserDto(userId, tenantId, employeeId, displayName, email);
        return new SessionDto(userDto, permissions, activeModules, activeFeatures, setupComplete);
    }
}
