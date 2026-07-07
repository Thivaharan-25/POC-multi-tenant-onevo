using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Filters;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/outbox")]
public class OutboxProcessorController : ControllerBase
{
    private readonly IEmailOutboxProcessorService _processorService;

    public OutboxProcessorController(IEmailOutboxProcessorService processorService)
    {
        _processorService = processorService;
    }

    /// <summary>
    /// POST /api/v1/outbox/process-emails
    /// Sends queued email_delivery_logs rows. When the tenant has an active
    /// SendGrid notification channel (configured via the platform System
    /// Config endpoint), rows move queued → sending → sent (or failed).
    /// Without an active channel the local_dev fallback marks them dev_logged
    /// and no real email is sent. Requires notifications:manage permission so
    /// this endpoint is never inadvertently exposed to employees without
    /// admin access.
    /// </summary>
    [HttpPost("process-emails")]
    [RequirePermission("notifications:manage")]
    public async Task<IActionResult> ProcessEmails(CancellationToken ct)
    {
        var processedCount = await _processorService.ProcessPendingEmailsAsync(ct);
        return Ok(new { processed = processedCount });
    }
}
