using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Demo;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/demo/upgrade")]
public sealed class DemoUpgradeController : ControllerBase
{
    private readonly IDemoUpgradeService _demoUpgrade;
    private readonly ITenantContextService _tenantContext;

    public DemoUpgradeController(IDemoUpgradeService demoUpgrade, ITenantContextService tenantContext)
    {
        _demoUpgrade = demoUpgrade;
        _tenantContext = tenantContext;
    }

    [HttpPost("quote")]
    public async Task<IActionResult> Quote([FromBody] DemoUpgradeQuoteRequestDto request)
    {
        var quote = await _demoUpgrade.GetQuoteAsync(_tenantContext.TenantId!.Value, request);
        if (quote is null)
        {
            return BadRequest(new { error = "Quote could not be produced for the selected plan." });
        }

        return Ok(quote);
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] DemoUpgradeSubmitRequestDto request)
    {
        var result = await _demoUpgrade.SubmitUpgradeAsync(_tenantContext.TenantId!.Value, request);
        if (result is null)
        {
            return BadRequest(new { error = "Upgrade could not be submitted." });
        }

        return Ok(result);
    }
}
