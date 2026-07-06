using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Filters;

/// <summary>
/// Requires a specific permission key on the calling identity.
/// For tenant routes the entitlement-filtered effective permissions of the
/// current user are checked; for /admin routes the platform session
/// permissions are checked.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequirePermissionAttribute : Attribute, IAsyncActionFilter
{
    public RequirePermissionAttribute(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        if (httpContext.Request.Path.StartsWithSegments("/admin"))
        {
            if (httpContext.Items[CurrentUserMiddleware.PlatformSessionItemKey] is not PlatformSessionValidationDto platformSession)
            {
                context.Result = new UnauthorizedObjectResult(new { error = "Platform authentication required." });
                return;
            }

            if (!platformSession.Permissions.Contains(Permission))
            {
                context.Result = new ObjectResult(new { error = $"Missing platform permission '{Permission}'." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            await next();
            return;
        }

        var currentUser = httpContext.RequestServices.GetRequiredService<ICurrentUserService>();
        if (!currentUser.IsAuthenticated)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Authentication required." });
            return;
        }

        if (!currentUser.HasPermission(Permission))
        {
            context.Result = new ObjectResult(new { error = $"Missing permission '{Permission}'." })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        await next();
    }
}
