using OnevoHr.Api.Models.Subscriptions;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface ISubscriptionRepository
{
    Task<List<SubscriptionPlan>> GetPlansAsync();
    Task<SubscriptionPlan?> GetPlanByIdAsync(Guid id);
    Task<TenantSubscription?> GetCurrentForTenantAsync(Guid tenantId);
    Task<List<TenantModuleEntitlement>> GetModuleEntitlementsAsync(Guid tenantId);
    Task<List<TenantFeatureEntitlement>> GetFeatureEntitlementsAsync(Guid tenantId);
    Task<List<RuntimeFeatureFlag>> GetRuntimeFlagsAsync(Guid tenantId);
    Task<List<TenantResourceLimit>> GetResourceLimitsAsync(Guid tenantId);
    Task SetModuleEntitlementEnabledAsync(Guid tenantId, Guid moduleCatalogId, bool isEnabled);
    Task SetFeatureEntitlementEnabledAsync(Guid tenantId, Guid moduleFeatureId, bool isEnabled);
    Task<List<SubscriptionPlanResourceAddon>> GetResourceAddOnsAsync(Guid planId);
    Task AddSubscriptionAsync(TenantSubscription subscription);
    Task AddInvoiceAsync(SubscriptionInvoice invoice);
    Task<SubscriptionInvoice?> GetInvoiceByIdAsync(Guid id);
    Task AddModuleEntitlementAsync(TenantModuleEntitlement entitlement);
    Task AddFeatureEntitlementAsync(TenantFeatureEntitlement entitlement);
    Task AddResourceLimitAsync(TenantResourceLimit limit);
    Task SaveChangesAsync();
}
