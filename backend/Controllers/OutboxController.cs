using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/outbox")]
public sealed class OutboxController : ControllerBase
{
    private readonly IOutboxRepository _outboxRepository;

    public OutboxController(IOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }

    [HttpGet("recent")]
    [RequirePermission("settings:read")]
    public async Task<IActionResult> GetRecent()
    {
        if (HttpContext.Items[CurrentUserMiddleware.SessionItemKey] is not DTOs.Auth.SessionValidationDto)
        {
            return Unauthorized(new { error = "Authentication required." });
        }

        var messages = await _outboxRepository.GetRecentAsync(50);

        return Ok(messages);
    }
}
