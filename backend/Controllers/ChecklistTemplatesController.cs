using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/onboarding/checklist-templates")]
public class ChecklistTemplatesController : ControllerBase
{
    private readonly IOnboardingService _onboardingService;

    public ChecklistTemplatesController(IOnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    /// <summary>
    /// GET /api/v1/onboarding/checklist-templates
    /// legalEntityId/positionId are accepted for forward compatibility with the
    /// canonical position -> department -> company matching order, but position-tier
    /// matching is deferred: ChecklistTemplate has no PositionId column yet.
    /// </summary>
    [HttpGet]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> List(
        [FromQuery] Guid? legalEntityId,
        [FromQuery] Guid? departmentId,
        [FromQuery] Guid? positionId,
        CancellationToken ct)
    {
        var result = await _onboardingService.GetChecklistTemplatesAsync(departmentId, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var detail = await _onboardingService.GetChecklistTemplateDetailAsync(id, ct);
        if (detail == null)
        {
            return NotFound();
        }

        return Ok(detail);
    }
}
