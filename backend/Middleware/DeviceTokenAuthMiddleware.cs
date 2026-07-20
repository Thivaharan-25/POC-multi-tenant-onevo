using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Middleware;

// Resolves the Authorization: Bearer <device JWT> header on /api/v1/agent/*
// requests into a DeviceTokenAuthContext, the tray-agent equivalent of what
// CurrentUserMiddleware does for browser cookie sessions. The token is a signed
// JWT (see JwtTokenService); the agent is resolved from its device_id claim.
// /api/v1/agent/enroll/* is exempt - the Service has no credential yet there.
public sealed class DeviceTokenAuthMiddleware
{
    private readonly RequestDelegate _next;

    public DeviceTokenAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAgentActivityRepository agentActivity,
        ITokenService tokenService,
        IDeviceTokenAuthContext deviceTokenAuthContext)
    {
        var path = context.Request.Path;

        if (path.StartsWithSegments("/api/v1/agent")
            && !path.StartsWithSegments("/api/v1/agent/enroll"))
        {
            var header = context.Request.Headers.Authorization.FirstOrDefault();
            var token = header is not null && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? header["Bearer ".Length..].Trim()
                : null;

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Device authentication required." });
                return;
            }

            var claims = tokenService.ValidateDeviceToken(token);
            if (claims is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid or expired device token." });
                return;
            }

            var agent = await agentActivity.GetRegisteredAgentByDeviceIdAsync(claims.DeviceId);
            if (agent is null || agent.TenantId != claims.TenantId)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unknown device." });
                return;
            }

            deviceTokenAuthContext.SetAgent(agent.Id, agent.TenantId, agent.EmployeeId);
        }

        await _next(context);
    }
}
