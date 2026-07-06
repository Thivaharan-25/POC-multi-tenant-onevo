using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Middleware;

/// <summary>
/// Resolves the tenant for /api requests from the X-Tenant-Domain header
/// (dev convenience) or the request host, and stores it in the scoped
/// tenant context. Admin (/admin) requests are not tenant-scoped.
/// </summary>
public sealed class TenantResolutionMiddleware
{
    public const string TenantDomainHeader = "X-Tenant-Domain";
    public const string LegalEntityHeader = "X-Legal-Entity-Id";

    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantRepository tenants,
        ITenantContextService tenantContext)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            var domain = context.Request.Headers[TenantDomainHeader].FirstOrDefault()
                ?? context.Request.Host.Host;

            var tenant = await tenants.GetByDomainAsync(domain);
            if (tenant is not null)
            {
                tenantContext.SetTenant(tenant.Id, tenant.Status, tenant.Slug);

                var legalEntityHeader = context.Request.Headers[LegalEntityHeader].FirstOrDefault();
                if (Guid.TryParse(legalEntityHeader, out var legalEntityId))
                {
                    tenantContext.SetActiveLegalEntity(legalEntityId);
                }
            }
        }

        await _next(context);
    }
}
