using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

/// <summary>
/// Platform System Config endpoints (backend-only, no frontend yet). Manages
/// the per-tenant email provider channel stored in notification_channels.
/// The API key is accepted once on POST, stored encrypted, and never returned.
/// </summary>
[ApiController]
[Route("admin/v1/system-config")]
public class AdminSystemConfigController : ControllerBase
{
    private const string SupportedProvider = "sendgrid";

    private readonly ISystemConfigService _systemConfigService;

    public AdminSystemConfigController(ISystemConfigService systemConfigService)
    {
        _systemConfigService = systemConfigService;
    }

    [HttpPost("email-channel")]
    [RequirePermission("platform.system_config.manage")]
    public async Task<IActionResult> UpsertEmailChannel([FromBody] UpsertEmailChannelRequest request, CancellationToken ct)
    {
        if (!string.Equals(request.Provider?.Trim(), SupportedProvider, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = "Only the 'sendgrid' email provider is supported." });
        }

        if (HttpContext.Items[CurrentUserMiddleware.PlatformSessionItemKey] is not PlatformSessionValidationDto platformSession)
        {
            return Unauthorized(new { error = "Platform authentication required." });
        }

        EmailChannelResponse? response;
        try
        {
            response = await _systemConfigService.UpsertEmailChannelAsync(request, platformSession.PlatformUserId, ct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        if (response is null)
        {
            return NotFound(new { error = "Tenant not found." });
        }

        return Ok(response);
    }

    [HttpGet("email-channel")]
    [RequirePermission("platform.system_config.read")]
    public async Task<IActionResult> GetEmailChannel([FromQuery] Guid tenantId, CancellationToken ct)
    {
        if (tenantId == Guid.Empty)
        {
            return BadRequest(new { error = "tenantId is required." });
        }

        var response = await _systemConfigService.GetEmailChannelAsync(tenantId, ct);
        if (response is null)
        {
            return NotFound(new { error = "No active email channel is configured for this tenant." });
        }

        return Ok(response);
    }
}
