using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/time-attendance/work-schedules")]
public sealed class WorkSchedulesController : ControllerBase
{
    private readonly IWorkScheduleService _workSchedules;
    private readonly ITenantContextService _tenantContext;

    public WorkSchedulesController(IWorkScheduleService workSchedules, ITenantContextService tenantContext)
    {
        _workSchedules = workSchedules;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [RequirePermission("attendance:read")]
    public async Task<IActionResult> List([FromQuery] Guid? legalEntityId)
    {
        var schedules = await _workSchedules.GetWorkSchedulesAsync(_tenantContext.TenantId!.Value, legalEntityId);
        return Ok(schedules);
    }
}
