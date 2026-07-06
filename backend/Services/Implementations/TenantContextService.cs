using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Scoped per-request holder for the resolved tenant.
/// Populated by TenantResolutionMiddleware from X-Tenant-Domain or host.
/// </summary>
public class TenantContextService : ITenantContextService
{
    public bool HasTenant { get; private set; }
    public Guid? TenantId { get; private set; }
    public string? TenantStatus { get; private set; }
    public string? TenantSlug { get; private set; }
    public Guid? ActiveLegalEntityId { get; private set; }

    public void SetTenant(Guid tenantId, string status, string slug)
    {
        HasTenant = true;
        TenantId = tenantId;
        TenantStatus = status;
        TenantSlug = slug;
    }

    public void SetActiveLegalEntity(Guid legalEntityId)
    {
        ActiveLegalEntityId = legalEntityId;
    }
}
