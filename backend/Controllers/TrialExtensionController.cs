using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Services.Notifications;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/trial-extension")]
public sealed class TrialExtensionController : ControllerBase
{
    private readonly IOutboxService _outbox;
    private readonly ITenantContextService _tenantContext;
    private readonly ICurrentUserService _currentUser;

    public TrialExtensionController(
        IOutboxService outbox,
        ITenantContextService tenantContext,
        ICurrentUserService currentUser)
    {
        _outbox = outbox;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
    }

    public sealed record TrialExtensionRequestDto(string? Reason);

    [HttpPost]
    public async Task<IActionResult> RequestExtension([FromBody] TrialExtensionRequestDto request)
    {
        if (_tenantContext.TenantStatus != "trial")
        {
            return BadRequest(new { error = "Trial extensions can only be requested by trial tenants." });
        }

        var payload = JsonSerializer.Serialize(new
        {
            tenantId = _tenantContext.TenantId,
            requestedByUserId = _currentUser.UserId,
            reason = request.Reason
        });

        await _outbox.EnqueueAsync(_tenantContext.TenantId, "trial_extension_requested", payload);
        return Accepted(new { requested = true });
    }
}
