using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Permissions;
using OnevoHr.Api.Exceptions;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/permissions")]
public sealed class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissions;
    private readonly ICurrentUserService _currentUser;

    public PermissionsController(IPermissionService permissions, ICurrentUserService currentUser)
    {
        _permissions = permissions;
        _currentUser = currentUser;
    }

    [HttpGet("catalog")]
    [RequirePermission("permissions:read")]
    public async Task<IActionResult> Catalog()
    {
        var catalog = await _permissions.GetTenantPermissionCatalogAsync(_currentUser.TenantId!.Value);
        return Ok(catalog);
    }

    [HttpGet("overrides/{userId:guid}")]
    [RequirePermission("permissions:read")]
    public async Task<IActionResult> ListOverrides(Guid userId)
    {
        var overrides = await _permissions.GetOverridesForUserAsync(_currentUser.TenantId!.Value, userId);
        return Ok(overrides);
    }

    [HttpPost("overrides")]
    [RequirePermission("permissions:manage")]
    public async Task<IActionResult> CreateOverride([FromBody] OverrideRequestDto request)
    {
        try
        {
            var created = await _permissions.CreateOverrideAsync(_currentUser.TenantId!.Value, _currentUser.UserId!.Value, request);
            return Ok(created);
        }
        catch (UnassignablePermissionException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("overrides/{overrideId:guid}")]
    [RequirePermission("permissions:manage")]
    public async Task<IActionResult> RevokeOverride(Guid overrideId)
    {
        await _permissions.RevokeOverrideAsync(_currentUser.TenantId!.Value, overrideId);
        return Ok(new { revoked = true });
    }
}
