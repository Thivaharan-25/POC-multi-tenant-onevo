using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.DTOs.Demo;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/demo-requests")]
public sealed class AdminDemoRequestsController : ControllerBase
{
    private readonly IDemoRequestService _demoRequests;
    private readonly IDemoApprovalService _demoApproval;

    public AdminDemoRequestsController(IDemoRequestService demoRequests, IDemoApprovalService demoApproval)
    {
        _demoRequests = demoRequests;
        _demoApproval = demoApproval;
    }

    [HttpGet]
    [RequirePermission("platform.requests.read")]
    public async Task<IActionResult> List()
    {
        var requests = await _demoRequests.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("platform.requests.read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var request = await _demoRequests.GetByIdAsync(id);
        if (request is null)
        {
            return NotFound();
        }

        return Ok(request);
    }

    [HttpPost]
    [RequirePermission("platform.requests.manage")]
    public async Task<IActionResult> Create([FromBody] CreateDemoRequestDto request)
    {
        var created = await _demoRequests.CreateAsync(request);
        return Ok(created);
    }

    [HttpPost("{id:guid}/approve")]
    [RequirePermission("platform.requests.manage")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveDemoRequestDto request)
    {
        var platformUserId = GetPlatformUserId();
        var tenant = await _demoApproval.ApproveAsync(id, request, platformUserId);
        if (tenant is null)
        {
            return BadRequest(new { error = "Demo request could not be approved." });
        }

        return Ok(tenant);
    }

    [HttpPost("{id:guid}/reject")]
    [RequirePermission("platform.requests.manage")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectDemoRequestDto request)
    {
        var platformUserId = GetPlatformUserId();
        var rejected = await _demoApproval.RejectAsync(id, request, platformUserId);
        if (!rejected)
        {
            return BadRequest(new { error = "Demo request could not be rejected." });
        }

        return Ok(new { rejected = true });
    }

    private Guid GetPlatformUserId()
    {
        var session = (PlatformSessionValidationDto)HttpContext.Items[CurrentUserMiddleware.PlatformSessionItemKey]!;
        return session.PlatformUserId;
    }
}
