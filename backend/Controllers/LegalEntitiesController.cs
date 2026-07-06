using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/org/legal-entities")]
public sealed class LegalEntitiesController : ControllerBase
{
    private readonly IOrgStructureService _orgStructure;
    private readonly ITenantContextService _tenantContext;

    public LegalEntitiesController(IOrgStructureService orgStructure, ITenantContextService tenantContext)
    {
        _orgStructure = orgStructure;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [RequirePermission("org:legal-entities:read")]
    public async Task<IActionResult> List()
    {
        var legalEntities = await _orgStructure.GetLegalEntitiesAsync(_tenantContext.TenantId!.Value);
        return Ok(legalEntities);
    }
}
