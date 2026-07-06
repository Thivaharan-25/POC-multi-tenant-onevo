using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.DTOs.Subscriptions;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/tenant")]
public sealed class TenantController : ControllerBase
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ITenantContextService _tenantContext;

    public TenantController(ISubscriptionRepository subscriptionRepository, ISubscriptionService subscriptionService, ITenantContextService tenantContext)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionService = subscriptionService;
        _tenantContext = tenantContext;
    }

    [HttpGet("resource-limits")]
    public async Task<IActionResult> GetResourceLimits()
    {
        if (HttpContext.Items[CurrentUserMiddleware.SessionItemKey] is not DTOs.Auth.SessionValidationDto)
        {
            return Unauthorized(new { error = "Authentication required." });
        }

        if (!_tenantContext.TenantId.HasValue)
        {
            return BadRequest(new { error = "Tenant context is missing." });
        }

        var limits = await _subscriptionRepository.GetResourceLimitsAsync(_tenantContext.TenantId.Value);
        return Ok(limits);
    }

    [HttpGet("entitlements")]
    [RequirePermission("billing:read")]
    public async Task<IActionResult> GetEntitlements()
    {
        var entitlements = await _subscriptionService.GetTenantEntitlementsAsync(_tenantContext.TenantId!.Value);
        return Ok(entitlements);
    }

    [HttpPut("modules/{moduleCatalogId:guid}/entitlement")]
    [RequirePermission("billing:manage")]
    public async Task<IActionResult> SetModuleEntitlement(Guid moduleCatalogId, [FromBody] ToggleEntitlementRequestDto request)
    {
        await _subscriptionService.SetModuleEntitlementAsync(_tenantContext.TenantId!.Value, moduleCatalogId, request.IsEnabled);
        return Ok(new { updated = true });
    }

    [HttpPut("features/{moduleFeatureId:guid}/entitlement")]
    [RequirePermission("billing:manage")]
    public async Task<IActionResult> SetFeatureEntitlement(Guid moduleFeatureId, [FromBody] ToggleEntitlementRequestDto request)
    {
        await _subscriptionService.SetFeatureEntitlementAsync(_tenantContext.TenantId!.Value, moduleFeatureId, request.IsEnabled);
        return Ok(new { updated = true });
    }
}
