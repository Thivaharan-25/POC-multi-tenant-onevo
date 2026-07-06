using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.DTOs.Subscriptions;

namespace OnevoHr.Api.Services.Interfaces;

public interface ISubscriptionService
{
    Task<List<SubscriptionPlanDto>> GetPlansAsync();
    Task<bool> MarkInvoicePaidAsync(Guid invoiceId);
    Task<List<TenantModuleEntitlementViewDto>> GetTenantEntitlementsAsync(Guid tenantId);
    Task SetModuleEntitlementAsync(Guid tenantId, Guid moduleCatalogId, bool isEnabled);
    Task SetFeatureEntitlementAsync(Guid tenantId, Guid moduleFeatureId, bool isEnabled);
}
