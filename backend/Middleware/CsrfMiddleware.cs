using OnevoHr.Api.DTOs.Auth;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Services.Implementations;

namespace OnevoHr.Api.Middleware;

/// <summary>
/// Double-submit CSRF protection for cookie-authenticated tenant and platform requests.
/// Unsafe methods must carry an X-CSRF-Token header that hashes to the
/// CSRF token hash stored on the validated session.
/// </summary>
public sealed class CsrfMiddleware
{
    public const string CsrfHeader = "X-CSRF-Token";

    private static readonly string[] SafeMethods = { "GET", "HEAD", "OPTIONS", "TRACE" };

    private readonly RequestDelegate _next;

    public CsrfMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var isUnsafe = !SafeMethods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase);

        if (isUnsafe)
        {
            // Check tenant session CSRF
            if (context.Items[CurrentUserMiddleware.SessionItemKey] is SessionValidationDto tenantSession)
            {
                var csrfToken = context.Request.Headers[CsrfHeader].FirstOrDefault();
                if (csrfToken is null || AuthService.HashToken(csrfToken) != tenantSession.CsrfTokenHash)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing CSRF token." });
                    return;
                }
            }
            // Check platform session CSRF
            else if (context.Items[CurrentUserMiddleware.PlatformSessionItemKey] is PlatformSessionValidationDto platformSession)
            {
                var csrfToken = context.Request.Headers[CsrfHeader].FirstOrDefault();
                if (csrfToken is null || AuthService.HashToken(csrfToken) != platformSession.CsrfTokenHash)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing CSRF token." });
                    return;
                }
            }
        }

        await _next(context);
    }
}
