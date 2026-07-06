using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/org/departments")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly IOrgStructureService _orgStructure;
    private readonly ITenantContextService _tenantContext;

    public DepartmentsController(IOrgStructureService orgStructure, ITenantContextService tenantContext)
    {
        _orgStructure = orgStructure;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [RequirePermission("org:departments:read")]
    public async Task<IActionResult> List()
    {
        var departments = await _orgStructure.GetDepartmentsAsync(_tenantContext.TenantId!.Value);
        return Ok(departments);
    }
}
