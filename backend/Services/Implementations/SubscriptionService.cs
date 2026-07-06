using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.DTOs.Subscriptions;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptions;
    private readonly ITenantActivationService _activation;
    private readonly IModuleCatalogRepository _moduleCatalog;

    public SubscriptionService(ISubscriptionRepository subscriptions, ITenantActivationService activation, IModuleCatalogRepository moduleCatalog)
    {
        _subscriptions = subscriptions;
        _activation = activation;
        _moduleCatalog = moduleCatalog;
    }

    public async Task<List<SubscriptionPlanDto>> GetPlansAsync()
    {
        var plans = await _subscriptions.GetPlansAsync();
        return plans.Select(p => new SubscriptionPlanDto(
            p.Id, p.Name, p.Code, p.BillingCycle, p.IsActive,
            p.SharedBaseStorageGb, p.SharedBaseAiTokenAllowance,
            p.PriceBrackets
                .OrderBy(b => b.CompanySizeRange)
                .Select(b => new PriceBracketDto(b.CompanySizeRange, b.BasePlanMonthlyPrice, b.AnnualPrice, b.Currency))
                .ToList()))
            .ToList();
    }

    public async Task<bool> MarkInvoicePaidAsync(Guid invoiceId)
    {
        var invoice = await _subscriptions.GetInvoiceByIdAsync(invoiceId);
        if (invoice is null || invoice.Status != "open")
        {
            return false;
        }

        invoice.Status = "paid";
        invoice.PaidAtUtc = DateTime.UtcNow;
        await _subscriptions.SaveChangesAsync();

        return await _activation.ActivateAsync(invoice.TenantId);
    }

    public async Task<List<TenantModuleEntitlementViewDto>> GetTenantEntitlementsAsync(Guid tenantId)
    {
        var modules = await _moduleCatalog.GetModulesAsync();
        var moduleEntitlements = await _subscriptions.GetModuleEntitlementsAsync(tenantId);
        var featureEntitlements = await _subscriptions.GetFeatureEntitlementsAsync(tenantId);

        var moduleEnabledById = new Dictionary<Guid, bool>();
        foreach (var entitlement in moduleEntitlements)
        {
            moduleEnabledById[entitlement.ModuleCatalogId] = entitlement.IsEnabled;
        }

        var featureEnabledById = new Dictionary<Guid, bool>();
        foreach (var entitlement in featureEntitlements)
        {
            featureEnabledById[entitlement.ModuleFeatureId] = entitlement.IsEnabled;
        }

        var result = new List<TenantModuleEntitlementViewDto>();
        foreach (var module in modules.OrderBy(m => m.DisplayName))
        {
            var featureViews = new List<TenantFeatureEntitlementViewDto>();
            foreach (var feature in module.Features.OrderBy(f => f.FeatureKey))
            {
                var isFeatureEnabled = featureEnabledById.TryGetValue(feature.Id, out var featureEnabled) && featureEnabled;
                featureViews.Add(new TenantFeatureEntitlementViewDto(feature.Id, feature.FeatureKey, isFeatureEnabled));
            }

            var isModuleEnabled = moduleEnabledById.TryGetValue(module.Id, out var moduleEnabled) && moduleEnabled;
            result.Add(new TenantModuleEntitlementViewDto(
                module.Id,
                module.ModuleKey,
                module.DisplayName,
                module.IsFoundation,
                isModuleEnabled,
                featureViews));
        }

        return result;
    }

    public async Task SetModuleEntitlementAsync(Guid tenantId, Guid moduleCatalogId, bool isEnabled)
    {
        await _subscriptions.SetModuleEntitlementEnabledAsync(tenantId, moduleCatalogId, isEnabled);
    }

    public async Task SetFeatureEntitlementAsync(Guid tenantId, Guid moduleFeatureId, bool isEnabled)
    {
        await _subscriptions.SetFeatureEntitlementEnabledAsync(tenantId, moduleFeatureId, isEnabled);
    }
}
