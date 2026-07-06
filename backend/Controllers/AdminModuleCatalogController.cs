using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/module-catalog")]
public sealed class AdminModuleCatalogController : ControllerBase
{
    private readonly IModuleCatalogService _moduleCatalog;

    public AdminModuleCatalogController(IModuleCatalogService moduleCatalog)
    {
        _moduleCatalog = moduleCatalog;
    }

    [HttpGet]
    [RequirePermission("platform.module_catalog.read")]
    public async Task<IActionResult> List()
    {
        var modules = await _moduleCatalog.GetModulesAsync();
        return Ok(modules);
    }
}
