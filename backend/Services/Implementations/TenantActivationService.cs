using System.Text.Json;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Services.Notifications;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// After payment succeeds, the tenant becomes active and receives its paid
/// subscription, paid entitlements, resource limits and setup templates.
/// </summary>
public class TenantActivationService : ITenantActivationService
{
    private readonly ITenantRepository _tenants;
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IModuleCatalogRepository _moduleCatalog;
    private readonly ITemplateApplicationService _templateApplication;
    private readonly IOutboxService _outbox;

    public TenantActivationService(
        ITenantRepository tenants,
        ISubscriptionRepository subscriptions,
        IModuleCatalogRepository moduleCatalog,
        ITemplateApplicationService templateApplication,
        IOutboxService outbox)
    {
        _tenants = tenants;
        _subscriptions = subscriptions;
        _moduleCatalog = moduleCatalog;
        _templateApplication = templateApplication;
        _outbox = outbox;
    }

    public async Task<bool> ActivateAsync(Guid tenantId)
    {
        var tenant = await _tenants.GetByIdAsync(tenantId);
        if (tenant is null || tenant.Status != "pending_payment")
        {
            return false;
        }

        var subscription = await _subscriptions.GetCurrentForTenantAsync(tenantId);
        if (subscription is null || subscription.Status != "pending_payment" || subscription.SubscriptionPlanId is null)
        {
            return false;
        }

        var plan = await _subscriptions.GetPlanByIdAsync(subscription.SubscriptionPlanId.Value);
        if (plan is null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        subscription.Status = "active";
        subscription.StartsAtUtc = now;

        tenant.Status = "active";
        tenant.ActivatedAtUtc = now;

        await _subscriptions.AddResourceLimitAsync(new TenantResourceLimit
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            StorageLimitGb = plan.SharedBaseStorageGb,
            AiTokenLimit = plan.SharedBaseAiTokenAllowance,
            EmployeeLimit = subscription.ConfirmedEmployeeCount,
            Source = "paid_plan",
            CreatedAtUtc = now
        });

        await ApplyPaidEntitlementsAsync(tenantId, subscription);

        if (subscription.ConfirmedEmployeeCount is int employeeCount)
        {
            await _templateApplication.ApplyTemplatesForEmployeeCountAsync(tenantId, employeeCount, null);
        }

        await _outbox.EnqueueAsync(tenantId, "tenant_activated", $"{{\"tenantId\":\"{tenantId}\"}}");
        await _subscriptions.SaveChangesAsync();
        return true;
    }

    private async Task ApplyPaidEntitlementsAsync(Guid tenantId, TenantSubscription subscription)
    {
        var selectedFeatureKeys = JsonSerializer.Deserialize<List<string>>(subscription.SelectedFeatureKeysJson)
            ?? new List<string>();

        var allFeatures = await _moduleCatalog.GetFeaturesAsync();
        var selectedFeatures = allFeatures
            .Where(f => selectedFeatureKeys.Contains(f.FeatureKey))
            .ToList();
        var selectedModuleIds = selectedFeatures.Select(f => f.ModuleCatalogId).Distinct().ToHashSet();

        var existingModuleEntitlements = await _subscriptions.GetModuleEntitlementsAsync(tenantId);
        var existingFeatureEntitlements = await _subscriptions.GetFeatureEntitlementsAsync(tenantId);

        foreach (var entitlement in existingModuleEntitlements)
        {
            entitlement.IsEnabled = selectedModuleIds.Contains(entitlement.ModuleCatalogId);
            entitlement.State = entitlement.IsEnabled ? "subscription_included" : "disabled";
        }

        var missingModuleIds = selectedModuleIds
            .Except(existingModuleEntitlements.Select(e => e.ModuleCatalogId));
        foreach (var moduleId in missingModuleIds)
        {
            await _subscriptions.AddModuleEntitlementAsync(new TenantModuleEntitlement
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ModuleCatalogId = moduleId,
                State = "subscription_included",
                IsEnabled = true
            });
        }

        var selectedFeatureIds = selectedFeatures.Select(f => f.Id).ToHashSet();
        foreach (var entitlement in existingFeatureEntitlements)
        {
            entitlement.IsEnabled = selectedFeatureIds.Contains(entitlement.ModuleFeatureId);
        }

        var missingFeatureIds = selectedFeatureIds
            .Except(existingFeatureEntitlements.Select(e => e.ModuleFeatureId));
        foreach (var featureId in missingFeatureIds)
        {
            await _subscriptions.AddFeatureEntitlementAsync(new TenantFeatureEntitlement
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ModuleFeatureId = featureId,
                IsEnabled = true
            });
        }
    }
}
