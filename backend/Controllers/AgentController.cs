using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Agents;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

// All routes here are authenticated via DeviceTokenAuthMiddleware's Bearer
// deviceToken (not the cookie session) - see IDeviceTokenAuthContext.
[ApiController]
[Route("api/v1/agent")]
public sealed class AgentController : ControllerBase
{
    private readonly IAgentActivityService _agentActivity;
    private readonly IDeviceTokenAuthContext _deviceAuth;

    public AgentController(IAgentActivityService agentActivity, IDeviceTokenAuthContext deviceAuth)
    {
        _agentActivity = agentActivity;
        _deviceAuth = deviceAuth;
    }

    [HttpPost("clock-in")]
    public async Task<IActionResult> ClockIn()
    {
        var result = await _agentActivity.ClockInAsync(_deviceAuth.RegisteredAgentId!.Value, _deviceAuth.TenantId!.Value, _deviceAuth.EmployeeId);
        if (result is null)
        {
            return Conflict(new { error = "Already clocked in." });
        }

        return Ok(result);
    }

    [HttpPost("clock-out")]
    public async Task<IActionResult> ClockOut()
    {
        var result = await _agentActivity.ClockOutAsync(_deviceAuth.RegisteredAgentId!.Value);
        if (result is null)
        {
            return Conflict(new { error = "Not currently clocked in." });
        }

        return Ok(result);
    }

    [HttpPost("app-usage")]
    public async Task<IActionResult> SubmitAppUsage([FromBody] AppUsageBatchRequestDto request)
    {
        var accepted = await _agentActivity.SubmitAppUsageAsync(
            _deviceAuth.RegisteredAgentId!.Value,
            _deviceAuth.TenantId!.Value,
            _deviceAuth.EmployeeId,
            request.Samples);

        if (!accepted)
        {
            return Conflict(new { error = "Not currently clocked in." });
        }

        return NoContent();
    }

    // Docs-aligned batch ingest (agent-server-protocol.md). Returns 202 Accepted.
    [HttpPost("ingest")]
    public async Task<IActionResult> Ingest([FromBody] IngestRequestDto request)
    {
        var accepted = await _agentActivity.IngestAsync(
            _deviceAuth.RegisteredAgentId!.Value,
            _deviceAuth.TenantId!.Value,
            _deviceAuth.EmployeeId,
            request);

        if (!accepted)
        {
            return Conflict(new { error = "Not currently clocked in." });
        }

        return Accepted();
    }
}
