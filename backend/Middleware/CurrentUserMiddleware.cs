using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Middleware;

/// <summary>
/// Prepares the per-request tenant/user context from the HttpOnly session cookies.
/// Separates tenant requests (/api/*) and developer platform requests (/admin/*)
/// to prevent session crossing.
/// </summary>
public sealed class CurrentUserMiddleware
{
    public const string TenantSessionCookie = "onevo_session";
    public const string PlatformSessionCookie = "onevo_platform_session";
    public const string SessionItemKey = "SessionValidation";
    public const string PlatformSessionItemKey = "PlatformSessionValidation";

    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuthService authService,
        IPlatformAuthService platformAuthService,
        IPermissionService permissionService,
        ICurrentUserService currentUser,
        ITenantContextService tenantContext)
    {
        var path = context.Request.Path;

        // 1. Resolve tenant user session on /api/* endpoints
        if (path.StartsWithSegments("/api"))
        {
            if (context.Request.Cookies.TryGetValue(TenantSessionCookie, out var sessionToken))
            {
                var session = await authService.ValidateSessionAsync(sessionToken);
                if (session is not null 
                    && tenantContext.HasTenant 
                    && tenantContext.TenantId == session.TenantId)
                {
                    var permissions = await permissionService.GetEffectivePermissionsAsync(session.TenantId, session.UserId);
                    currentUser.SetUser(session.UserId, session.TenantId, session.EmployeeId, permissions);
                    context.Items[SessionItemKey] = session;
                }
            }
        }

        // 2. Resolve developer platform session on /admin/* endpoints
        if (path.StartsWithSegments("/admin"))
        {
            if (context.Request.Cookies.TryGetValue(PlatformSessionCookie, out var platformToken))
            {
                var platformSession = await platformAuthService.ValidateSessionAsync(platformToken);
                if (platformSession is not null)
                {
                    context.Items[PlatformSessionItemKey] = platformSession;
                }
            }
        }

        await _next(context);
    }
}
