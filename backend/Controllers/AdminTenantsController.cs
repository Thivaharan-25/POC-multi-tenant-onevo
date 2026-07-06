using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/tenants")]
public sealed class AdminTenantsController : ControllerBase
{
    private readonly ITenantProvisioningService _provisioning;
    private readonly ITenantActivationService _activation;

    public AdminTenantsController(ITenantProvisioningService provisioning, ITenantActivationService activation)
    {
        _provisioning = provisioning;
        _activation = activation;
    }

    [HttpGet]
    [RequirePermission("platform.tenants.read")]
    public async Task<IActionResult> List()
    {
        var tenants = await _provisioning.GetTenantsAsync();
        return Ok(tenants);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("platform.tenants.read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenant = await _provisioning.GetTenantAsync(id);
        if (tenant is null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    [HttpGet("{id:guid}/provisioning-state")]
    [RequirePermission("platform.tenants.read")]
    public async Task<IActionResult> GetProvisioningState(Guid id)
    {
        var state = await _provisioning.GetProvisioningStateAsync(id);
        if (state is null)
        {
            return NotFound();
        }

        return Ok(state);
    }

    [HttpPost]
    [RequirePermission("platform.tenants.manage")]
    public async Task<IActionResult> CreateDraft([FromBody] CreateTenantDraftDto request)
    {
        var tenant = await _provisioning.CreateDraftAsync(request);
        return Ok(tenant);
    }

    [HttpPost("{id:guid}/activate")]
    [RequirePermission("platform.tenants.activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var activated = await _activation.ActivateAsync(id);
        if (!activated)
        {
            return BadRequest(new { error = "Tenant could not be activated." });
        }

        return Ok(new { activated = true });
    }
}
