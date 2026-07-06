using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/demo-profiles")]
public sealed class AdminDemoProfilesController : ControllerBase
{
    private readonly IDemoProfileService _demoProfiles;

    public AdminDemoProfilesController(IDemoProfileService demoProfiles)
    {
        _demoProfiles = demoProfiles;
    }

    [HttpGet]
    [RequirePermission("platform.demo_profiles.read")]
    public async Task<IActionResult> List()
    {
        var profiles = await _demoProfiles.GetAllAsync();
        return Ok(profiles);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("platform.demo_profiles.read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var profile = await _demoProfiles.GetByIdAsync(id);
        if (profile is null)
        {
            return NotFound();
        }

        return Ok(profile);
    }
}
