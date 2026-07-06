using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Leave;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/leave")]
public sealed class LeaveController : ControllerBase
{
    private readonly ILeaveService _leave;

    public LeaveController(ILeaveService leave)
    {
        _leave = leave;
    }

    [HttpGet("types")]
    public async Task<IActionResult> Types()
    {
        var types = await _leave.GetLeaveTypesAsync();
        return Ok(types);
    }

    [HttpGet("requests")]
    public async Task<IActionResult> Requests()
    {
        // Visibility (own vs. approvable reports) is scope-filtered in the service.
        var requests = await _leave.GetVisibleRequestsAsync();
        return Ok(requests);
    }

    [HttpPost("requests")]
    [RequirePermission("leave:create")]
    public async Task<IActionResult> CreateRequest([FromBody] CreateLeaveRequestDto request)
    {
        var created = await _leave.CreateRequestAsync(request);
        if (created is null)
        {
            return BadRequest(new { error = "Leave request could not be created." });
        }

        return Ok(created);
    }

    [HttpPost("requests/{id:guid}/approve")]
    [RequirePermission("leave:approve")]
    public async Task<IActionResult> ApproveRequest(Guid id)
    {
        var approved = await _leave.ApproveRequestAsync(id);
        if (approved is null)
        {
            return NotFound(new { error = "Leave request not found or not approvable." });
        }

        return Ok(approved);
    }

    [HttpGet("balances")]
    public async Task<IActionResult> Balances([FromQuery] int? year)
    {
        var balances = await _leave.GetMyBalancesAsync(year ?? DateTime.UtcNow.Year);
        return Ok(balances);
    }
}
