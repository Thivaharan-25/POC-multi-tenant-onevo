namespace OnevoHr.Api.Services.Interfaces;

public interface ITenantContextService
{
    bool HasTenant { get; }
    Guid? TenantId { get; }
    string? TenantStatus { get; }
    string? TenantSlug { get; }
    Guid? ActiveLegalEntityId { get; }

    void SetTenant(Guid tenantId, string status, string slug);
    void SetActiveLegalEntity(Guid legalEntityId);
}
