using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("admin/v1/subscription-plans")]
public sealed class AdminSubscriptionPlansController : ControllerBase
{
    private readonly ISubscriptionService _subscriptions;

    public AdminSubscriptionPlansController(ISubscriptionService subscriptions)
    {
        _subscriptions = subscriptions;
    }

    [HttpGet]
    [RequirePermission("platform.subscriptions.read")]
    public async Task<IActionResult> List()
    {
        var plans = await _subscriptions.GetPlansAsync();
        return Ok(plans);
    }

    [HttpPost("invoices/{invoiceId:guid}/mark-paid")]
    [RequirePermission("platform.subscriptions.manage")]
    public async Task<IActionResult> MarkInvoicePaid(Guid invoiceId)
    {
        var paid = await _subscriptions.MarkInvoicePaidAsync(invoiceId);
        if (!paid)
        {
            return BadRequest(new { error = "Invoice could not be marked as paid." });
        }

        return Ok(new { paid = true });
    }
}
