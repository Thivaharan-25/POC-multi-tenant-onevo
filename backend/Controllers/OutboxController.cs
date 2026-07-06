using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Repositories.Implementations;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/outbox")]
public sealed class OutboxController : ControllerBase
{
    private readonly AppDbContext _db;

    public OutboxController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("recent")]
    [RequirePermission("settings:read")]
    public async Task<IActionResult> GetRecent()
    {
        if (HttpContext.Items[CurrentUserMiddleware.SessionItemKey] is not DTOs.Auth.SessionValidationDto)
        {
            return Unauthorized(new { error = "Authentication required." });
        }

        var messages = OutboxRepository.GetRecentMessages();

        return Ok(messages);
    }
}
