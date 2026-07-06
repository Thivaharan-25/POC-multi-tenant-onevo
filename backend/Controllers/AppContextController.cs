using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.AppContext;
using OnevoHr.Api.DTOs.Auth;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/app-context")]
public sealed class AppContextController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly ITenantContextService _tenantContext;
    private readonly ITenantProvisioningService _tenants;
    private readonly IFeatureGateService _featureGate;
    private readonly IScopeResolverService _scopeResolver;

    public AppContextController(
        ICurrentUserService currentUser,
        ITenantContextService tenantContext,
        ITenantProvisioningService tenants,
        IFeatureGateService featureGate,
        IScopeResolverService scopeResolver)
    {
        _currentUser = currentUser;
        _tenantContext = tenantContext;
        _tenants = tenants;
        _featureGate = featureGate;
        _scopeResolver = scopeResolver;
    }

    [HttpGet]
    [HttpGet("/api/v1/me/app-context")]
    public async Task<IActionResult> Get()
    {
        if (HttpContext.Items[CurrentUserMiddleware.SessionItemKey] is not SessionValidationDto session)
        {
            return Unauthorized(new { error = "Authentication required." });
        }

        var tenantId = _tenantContext.TenantId!.Value;
        var tenant = await _tenants.GetTenantAsync(tenantId);
        var enabledModuleKeys = await _featureGate.GetEnabledModuleKeysAsync(tenantId);
        var enabledFeatureKeys = await _featureGate.GetEnabledFeatureKeysAsync(tenantId);
        var scope = await _scopeResolver.ResolveScopeAsync(tenantId, session.UserId);

        return Ok(new AppContextDto(
            tenantId,
            tenant?.Name ?? _tenantContext.TenantSlug ?? string.Empty,
            _tenantContext.TenantStatus ?? string.Empty,
            session.UserId,
            session.Email,
            session.DisplayName,
            session.EmployeeId,
            _currentUser.Permissions,
            enabledModuleKeys,
            enabledFeatureKeys,
            scope.ScopeLevel));
    }
}
