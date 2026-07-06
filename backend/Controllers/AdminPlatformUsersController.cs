using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/platform-users")]
public sealed class AdminPlatformUsersController : ControllerBase
{
    private readonly IPlatformAuthService _platformAuth;

    public AdminPlatformUsersController(IPlatformAuthService platformAuth)
    {
        _platformAuth = platformAuth;
    }

    [HttpGet]
    [RequirePermission("platform.accounts.read")]
    public async Task<IActionResult> List()
    {
        var users = await _platformAuth.GetPlatformUsersAsync();
        return Ok(users);
    }
}
