using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Agents;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

// Client-facing enrollment endpoints for the agent Service. All anonymous - the
// Service has no credential until enrollment completes. Exempted from both
// AuthBoundaryMiddleware (cookie) and DeviceTokenAuthMiddleware (bearer) by the
// /api/v1/agent/enroll path prefix. The browser-confirm half is handled
// separately by DevicesController (cookie-authenticated, keyed by user_code).
[ApiController]
[Route("api/v1/agent/enroll")]
public sealed class AgentEnrollmentController : ControllerBase
{
    private readonly IAgentEnrollmentService _enrollment;

    public AgentEnrollmentController(IAgentEnrollmentService enrollment)
    {
        _enrollment = enrollment;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] EnrollStartRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.DeviceId)
            || string.IsNullOrWhiteSpace(request.DeviceName)
            || string.IsNullOrWhiteSpace(request.OsVersion)
            || string.IsNullOrWhiteSpace(request.AgentVersion))
        {
            return UnprocessableEntity(new { error = "device_id, device_name, os_version and agent_version are required." });
        }

        var result = await _enrollment.StartAsync(request);
        return Ok(result);
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status([FromQuery(Name = "enrollment_id")] string enrollmentId)
    {
        if (string.IsNullOrWhiteSpace(enrollmentId))
        {
            return BadRequest(new { error = "enrollment_id is required." });
        }

        var result = await _enrollment.GetStatusAsync(enrollmentId);
        if (result is null)
        {
            return NotFound(new { error = "Unknown enrollment_id." });
        }

        return Ok(result);
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete([FromBody] EnrollCompleteRequestDto request)
    {
        var result = await _enrollment.CompleteAsync(request);
        if (result is null)
        {
            return BadRequest(new { error = "Enrollment could not be completed. It may be unconfirmed, expired, or the authorization code is invalid." });
        }

        return Created(string.Empty, result);
    }
}
