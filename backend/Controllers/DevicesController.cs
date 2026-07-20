using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Auth;
using OnevoHr.Api.DTOs.Devices;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/devices")]
public sealed class DevicesController : ControllerBase
{
    private readonly IDevicePairingService _devicePairing;

    public DevicesController(IDevicePairingService devicePairing)
    {
        _devicePairing = devicePairing;
    }

    // Anonymous group - called by the tray app before it has any session.
    // Exempted from AuthBoundaryMiddleware by its "/api/v1/devices/pair" prefix.

    [HttpPost("pair")]
    public async Task<IActionResult> StartPairing([FromBody] PairRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.DeviceName))
        {
            return BadRequest(new { error = "deviceName is required." });
        }

        var result = await _devicePairing.StartPairingAsync(request.DeviceName);
        return Ok(result);
    }

    [HttpGet("pair/status")]
    public async Task<IActionResult> GetStatus([FromQuery] string deviceCode)
    {
        if (string.IsNullOrWhiteSpace(deviceCode))
        {
            return BadRequest(new { error = "deviceCode is required." });
        }

        var result = await _devicePairing.GetStatusAsync(deviceCode);
        if (result is null)
        {
            return NotFound(new { error = "Unknown device code." });
        }

        return Ok(result);
    }

    [HttpPost("pair/consent")]
    public async Task<IActionResult> AcceptConsent([FromBody] DeviceCodeRequestDto request)
    {
        var result = await _devicePairing.AcceptConsentAsync(request.DeviceCode);
        if (result is null)
        {
            return NotFound(new { error = "This device is not ready for enrollment." });
        }

        return Ok(result);
    }

    [HttpPost("pair/cancel")]
    public async Task<IActionResult> CancelPairing([FromBody] DeviceCodeRequestDto request)
    {
        var cancelled = await _devicePairing.CancelAsync(request.DeviceCode);
        if (!cancelled)
        {
            return NotFound(new { error = "Unknown device code." });
        }

        return NoContent();
    }

    // Authenticated group - called by the browser (Angular) confirm page from
    // the already-logged-in employee's session. Protected by AuthBoundaryMiddleware.

    [HttpGet("confirm")]
    public async Task<IActionResult> GetConfirmationInfo([FromQuery] string code)
    {
        if (HttpContext.Items[CurrentUserMiddleware.SessionItemKey] is not SessionValidationDto)
        {
            return Unauthorized(new { error = "Authentication required." });
        }

        var info = await _devicePairing.GetPairingInfoForConfirmationAsync(code);
        if (info is null)
        {
            return NotFound(new { error = "This code is invalid or has expired." });
        }

        return Ok(info);
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] UserCodeRequestDto request)
    {
        if (HttpContext.Items[CurrentUserMiddleware.SessionItemKey] is not SessionValidationDto session)
        {
            return Unauthorized(new { error = "Authentication required." });
        }

        var confirmed = await _devicePairing.ConfirmAsync(request.UserCode, session.TenantId, session.UserId, session.EmployeeId);
        if (!confirmed)
        {
            return NotFound(new { error = "This code is invalid or has expired." });
        }

        return NoContent();
    }

    [HttpPost("confirm/decline")]
    public async Task<IActionResult> Decline([FromBody] UserCodeRequestDto request)
    {
        if (HttpContext.Items[CurrentUserMiddleware.SessionItemKey] is not SessionValidationDto)
        {
            return Unauthorized(new { error = "Authentication required." });
        }

        var declined = await _devicePairing.DeclineAsync(request.UserCode);
        if (!declined)
        {
            return NotFound(new { error = "This code is invalid or has expired." });
        }

        return NoContent();
    }
}
