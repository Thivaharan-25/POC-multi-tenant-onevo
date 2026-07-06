using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Roles;
using OnevoHr.Api.Exceptions;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/roles")]
public sealed class RolesController : ControllerBase
{
    private readonly IPermissionService _permissions;
    private readonly ITenantContextService _tenantContext;

    public RolesController(IPermissionService permissions, ITenantContextService tenantContext)
    {
        _permissions = permissions;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [RequirePermission("roles:read")]
    public async Task<IActionResult> List()
    {
        var roles = await _permissions.GetTenantRolesAsync(_tenantContext.TenantId!.Value);
        return Ok(roles);
    }

    [HttpGet("users")]
    [RequirePermission("roles:read")]
    public async Task<IActionResult> Users()
    {
        var users = await _permissions.GetTenantUsersAsync(_tenantContext.TenantId!.Value);
        return Ok(users);
    }

    [HttpPost]
    [RequirePermission("roles:manage")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto request)
    {
        var role = await _permissions.CreateRoleAsync(_tenantContext.TenantId!.Value, request);
        return Ok(role);
    }

    [HttpPut("{roleId:guid}/permissions")]
    [RequirePermission("roles:manage")]
    public async Task<IActionResult> SetPermissions(Guid roleId, [FromBody] SetRolePermissionsRequestDto request)
    {
        try
        {
            await _permissions.SetRolePermissionsAsync(_tenantContext.TenantId!.Value, roleId, request);
        }
        catch (UnassignablePermissionException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        return Ok(new { updated = true });
    }

    [HttpPost("assignments")]
    [RequirePermission("roles:manage")]
    public async Task<IActionResult> AssignUserRole([FromBody] AssignUserRoleRequestDto request)
    {
        await _permissions.AssignUserRoleAsync(_tenantContext.TenantId!.Value, request);
        return Ok(new { assigned = true });
    }
}
