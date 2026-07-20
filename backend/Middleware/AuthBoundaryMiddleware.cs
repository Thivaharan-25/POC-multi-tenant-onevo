using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Middleware;

/// <summary>
/// Enforces authentication for protected route groups:
/// /api/v1/* requires a valid tenant user session (except /api/v1/auth),
/// /admin/v1/* requires a valid platform session (except /admin/v1/auth).
/// Ensures strict separation between tenant and platform user access.
/// </summary>
public sealed class AuthBoundaryMiddleware
{
    private readonly RequestDelegate _next;

    public AuthBoundaryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUser)
    {
        var path = context.Request.Path;

        // 1. Enforce tenant authentication for /api requests
        if (path.StartsWithSegments("/api"))
        {
            // Allow anonymous access to auth endpoints, the device-pairing
            // handshake (the tray app has no session before it is enrolled;
            // /api/v1/devices/confirm/* is the authenticated counterpart and
            // stays protected), and the agent-authenticated routes (these use
            // DeviceTokenAuthMiddleware's Bearer deviceToken instead of a
            // cookie session).
            if (!path.StartsWithSegments("/api/v1/auth")
                && !path.StartsWithSegments("/api/v1/devices/pair")
                && !path.StartsWithSegments("/api/v1/agent"))
            {
                // Reject if not authenticated or if a platform session is somehow mixed in
                if (!currentUser.IsAuthenticated || !context.Items.ContainsKey(CurrentUserMiddleware.SessionItemKey))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Tenant authentication required." });
                    return;
                }
            }
        }

        // 2. Enforce platform authentication for /admin requests
        if (path.StartsWithSegments("/admin"))
        {
            // Allow anonymous access to platform auth endpoints
            if (!path.StartsWithSegments("/admin/v1/auth"))
            {
                if (!context.Items.ContainsKey(CurrentUserMiddleware.PlatformSessionItemKey))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Platform authentication required." });
                    return;
                }
            }
        }

        await _next(context);
    }
}
