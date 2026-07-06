using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/platform-roles")]
public sealed class AdminPlatformRolesController : ControllerBase
{
    private readonly IPlatformAuthService _platformAuth;

    public AdminPlatformRolesController(IPlatformAuthService platformAuth)
    {
        _platformAuth = platformAuth;
    }

    [HttpGet]
    [RequirePermission("platform.roles.read")]
    public async Task<IActionResult> List()
    {
        var roles = await _platformAuth.GetPlatformRolesAsync();
        return Ok(roles);
    }
}
