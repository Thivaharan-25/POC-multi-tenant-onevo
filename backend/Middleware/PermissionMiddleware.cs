using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Middleware;

/// <summary>
/// Enforces authentication for protected route groups:
/// /api/v1/* requires a valid tenant user session (except /api/v1/auth),
/// /admin/v1/* requires a valid platform session (except /admin/v1/auth).
/// Ensures strict separation between tenant and platform user access.
/// </summary>
public sealed class PermissionMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUser)
    {
        var path = context.Request.Path;

        // 1. Enforce tenant authentication for /api requests
        if (path.StartsWithSegments("/api"))
        {
            // Allow anonymous access to auth endpoints
            if (!path.StartsWithSegments("/api/v1/auth"))
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
