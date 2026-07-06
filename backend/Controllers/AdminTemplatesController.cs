using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/templates")]
public sealed class AdminTemplatesController : ControllerBase
{
    private readonly ITemplateApplicationService _templates;

    public AdminTemplatesController(ITemplateApplicationService templates)
    {
        _templates = templates;
    }

    [HttpGet("configuration")]
    [RequirePermission("platform.templates.read")]
    public async Task<IActionResult> ConfigurationTemplates()
    {
        var templates = await _templates.GetConfigurationTemplatesAsync();
        return Ok(templates);
    }

    [HttpGet("roles")]
    [RequirePermission("platform.templates.read")]
    public async Task<IActionResult> RoleTemplates()
    {
        var templates = await _templates.GetRoleTemplatesAsync();
        return Ok(templates);
    }

    public sealed record ApplyTemplateRequestDto(Guid TenantId, Guid ConfigurationTemplateId);

    [HttpPost("apply")]
    [RequirePermission("platform.templates.manage")]
    public async Task<IActionResult> Apply([FromBody] ApplyTemplateRequestDto request)
    {
        var platformUserId =
            (HttpContext.Items[CurrentUserMiddleware.PlatformSessionItemKey] as PlatformSessionValidationDto)?.PlatformUserId;

        var applied = await _templates.ApplyConfigurationTemplateAsync(
            request.TenantId, request.ConfigurationTemplateId, platformUserId);

        if (!applied)
        {
            return BadRequest(new { error = "Template could not be applied." });
        }

        return Ok(new { applied = true });
    }
}
