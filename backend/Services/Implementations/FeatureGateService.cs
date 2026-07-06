using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// A feature is enabled only when its tenant feature entitlement is enabled
/// and its owning module entitlement is enabled. Runtime flags can disable
/// a feature but can never grant one outside the tenant's entitlements.
/// </summary>
public class FeatureGateService : IFeatureGateService
{
    private readonly ISubscriptionRepository _subscriptions;

    public FeatureGateService(ISubscriptionRepository subscriptions)
    {
        _subscriptions = subscriptions;
    }

    public async Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureKey)
    {
        var enabled = await GetEnabledFeatureKeysAsync(tenantId);
        return enabled.Contains(featureKey);
    }

    public async Task<IReadOnlyCollection<string>> GetEnabledFeatureKeysAsync(Guid tenantId)
    {
        // 1. Fetch entitlements and runtime flags from the repository
        var featureEntitlements = await _subscriptions.GetFeatureEntitlementsAsync(tenantId);
        var moduleEntitlements = await _subscriptions.GetModuleEntitlementsAsync(tenantId);
        var flags = await _subscriptions.GetRuntimeFlagsAsync(tenantId);

        // 2. Identify enabled modules
        var enabledModuleIds = new HashSet<Guid>();
        foreach (var m in moduleEntitlements)
        {
            if (m.IsEnabled)
            {
                enabledModuleIds.Add(m.ModuleCatalogId);
            }
        }

        // 3. Identify features disabled by runtime flags
        var disabledByFlag = new HashSet<string>();
        foreach (var f in flags)
        {
            if (!f.IsEnabled)
            {
                disabledByFlag.Add(f.FeatureKey);
            }
        }

        // 4. Filter features to those that are enabled and not disabled by flag
        var activeFeatures = new List<string>();
        foreach (var f in featureEntitlements)
        {
            if (f.IsEnabled && f.ModuleFeature is not null)
            {
                var featureKey = f.ModuleFeature.FeatureKey;
                var isModuleEnabled = enabledModuleIds.Contains(f.ModuleFeature.ModuleCatalogId);
                var isFlagDisabled = disabledByFlag.Contains(featureKey);

                if (isModuleEnabled && !isFlagDisabled)
                {
                    activeFeatures.Add(featureKey);
                }
            }
        }

        return activeFeatures.Distinct().ToList();
    }

    public async Task<IReadOnlyCollection<string>> GetEnabledModuleKeysAsync(Guid tenantId)
    {
        // 1. Fetch module entitlements from the repository
        var moduleEntitlements = await _subscriptions.GetModuleEntitlementsAsync(tenantId);

        // 2. Filter modules to those that are enabled and have resolved keys
        var activeModules = new List<string>();
        foreach (var m in moduleEntitlements)
        {
            if (m.IsEnabled && m.ModuleCatalog is not null)
            {
                activeModules.Add(m.ModuleCatalog.ModuleKey);
            }
        }

        return activeModules.Distinct().ToList();
    }
}
