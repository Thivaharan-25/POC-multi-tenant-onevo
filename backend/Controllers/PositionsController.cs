using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.OrgStructure;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/org/positions")]
public sealed class PositionsController : ControllerBase
{
    private readonly IOrgStructureService _orgStructure;
    private readonly ITenantContextService _tenantContext;

    public PositionsController(IOrgStructureService orgStructure, ITenantContextService tenantContext)
    {
        _orgStructure = orgStructure;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [RequirePermission("org:positions:read")]
    public async Task<IActionResult> List([FromQuery] Guid? legalEntityId, [FromQuery] Guid? departmentId)
    {
        var positions = await _orgStructure.GetPositionsAsync(_tenantContext.TenantId!.Value, legalEntityId, departmentId);
        return Ok(positions);
    }

    [HttpPost]
    [RequirePermission("org:positions:manage")]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequestDto request)
    {
        var position = await _orgStructure.CreatePositionAsync(_tenantContext.TenantId!.Value, request);
        if (position is null)
        {
            return BadRequest(new { error = "Position could not be created." });
        }

        return Ok(position);
    }

    [HttpGet("{id:guid}/reporting-manager")]
    [RequirePermission("org:positions:read")]
    public async Task<IActionResult> GetReportingManager(Guid id, CancellationToken ct)
    {
        var manager = await _orgStructure.GetReportingManagerAsync(_tenantContext.TenantId!.Value, id);
        if (manager == null)
        {
            return Ok(new { hasManager = false });
        }

        return Ok(new { hasManager = true, employeeId = manager.EmployeeId, employeeName = manager.EmployeeName });
    }
}
