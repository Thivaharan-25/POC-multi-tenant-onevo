using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _db;

    // In-memory stores for tables not yet wired to real EF persistence
    private static readonly ConcurrentBag<TenantResourceLimit> _resourceLimits = new();
    private static readonly ConcurrentBag<SubscriptionPlanResourceAddon> _resourceAddOns = new();
    private static readonly List<SubscriptionPlanPriceBracket> _priceBrackets = new();
    private static readonly List<SubscriptionPlanModule> _planModules = new();

    public SubscriptionRepository(AppDbContext db)
    {
        _db = db;
    }

    public static void SeedInMemoryLimits(IEnumerable<TenantResourceLimit> limits)
    {
        foreach (var l in limits)
        {
            _resourceLimits.Add(l);
        }
    }

    public static void SeedInMemoryResourceAddOns(IEnumerable<SubscriptionPlanResourceAddon> addOns)
    {
        foreach (var a in addOns)
        {
            _resourceAddOns.Add(a);
        }
    }

    public static void SeedInMemoryBrackets(IEnumerable<SubscriptionPlanPriceBracket> brackets)
    {
        lock (_priceBrackets)
        {
            _priceBrackets.AddRange(brackets);
        }
    }

    public static void SeedInMemoryPlanModules(IEnumerable<SubscriptionPlanModule> modules)
    {
        lock (_planModules)
        {
            _planModules.AddRange(modules);
        }
    }

    public async Task<List<SubscriptionPlan>> GetPlansAsync()
    {
        var plans = await _db.SubscriptionPlans.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        lock (_priceBrackets)
        {
            lock (_planModules)
            {
                foreach (var p in plans)
                {
                    p.PriceBrackets = _priceBrackets.Where(b => b.SubscriptionPlanId == p.Id).ToList();
                    p.PlanModules = _planModules.Where(m => m.SubscriptionPlanId == p.Id).ToList();
                    p.ResourceAddOns = _resourceAddOns.Where(a => a.SubscriptionPlanId == p.Id).ToList();
                }
            }
        }
        return plans;
    }

    public async Task<SubscriptionPlan?> GetPlanByIdAsync(Guid id)
    {
        var plan = await _db.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan != null)
        {
            lock (_priceBrackets)
            {
                lock (_planModules)
                {
                    plan.PriceBrackets = _priceBrackets.Where(b => b.SubscriptionPlanId == plan.Id).ToList();
                    plan.PlanModules = _planModules.Where(m => m.SubscriptionPlanId == plan.Id).ToList();
                    plan.ResourceAddOns = _resourceAddOns.Where(a => a.SubscriptionPlanId == plan.Id).ToList();
                }
            }
        }
        return plan;
    }

    public async Task<TenantSubscription?> GetCurrentForTenantAsync(Guid tenantId)
    {
        return await _db.TenantSubscriptions
            .Include(s => s.SubscriptionPlan)
            .Where(s => s.TenantId == tenantId)
            .OrderByDescending(s => s.CreatedAtUtc)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TenantModuleEntitlement>> GetModuleEntitlementsAsync(Guid tenantId)
    {
        return await _db.TenantModuleEntitlements
            .Include(e => e.ModuleCatalog)
            .Where(e => e.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<List<TenantFeatureEntitlement>> GetFeatureEntitlementsAsync(Guid tenantId)
    {
        return await _db.TenantFeatureEntitlements
            .Include(e => e.ModuleFeature)
            .Where(e => e.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<List<RuntimeFeatureFlag>> GetRuntimeFlagsAsync(Guid tenantId)
    {
        return await _db.RuntimeFeatureFlags.AsNoTracking()
            .Where(f => f.TenantId == tenantId || f.TenantId == null)
            .ToListAsync();
    }

    public Task<List<TenantResourceLimit>> GetResourceLimitsAsync(Guid tenantId)
    {
        var limits = _resourceLimits.Where(l => l.TenantId == tenantId)
            .OrderByDescending(l => l.CreatedAtUtc)
            .ToList();
        return Task.FromResult(limits);
    }

    public Task<List<SubscriptionPlanResourceAddon>> GetResourceAddOnsAsync(Guid planId)
    {
        var active = _resourceAddOns.Where(a => a.IsActive && a.SubscriptionPlanId == planId).ToList();
        return Task.FromResult(active);
    }

    public async Task AddSubscriptionAsync(TenantSubscription subscription)
    {
        await _db.TenantSubscriptions.AddAsync(subscription);
    }

    public async Task AddInvoiceAsync(SubscriptionInvoice invoice)
    {
        await _db.SubscriptionInvoices.AddAsync(invoice);
    }

    public async Task<SubscriptionInvoice?> GetInvoiceByIdAsync(Guid id)
    {
        return await _db.SubscriptionInvoices
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task AddModuleEntitlementAsync(TenantModuleEntitlement entitlement)
    {
        await _db.TenantModuleEntitlements.AddAsync(entitlement);
    }

    public async Task AddFeatureEntitlementAsync(TenantFeatureEntitlement entitlement)
    {
        await _db.TenantFeatureEntitlements.AddAsync(entitlement);
    }

    public Task AddResourceLimitAsync(TenantResourceLimit limit)
    {
        _resourceLimits.Add(limit);
        return Task.CompletedTask;
    }

    public async Task SetModuleEntitlementEnabledAsync(Guid tenantId, Guid moduleCatalogId, bool isEnabled)
    {
        var existing = await _db.TenantModuleEntitlements
            .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.ModuleCatalogId == moduleCatalogId);

        if (existing is not null)
        {
            existing.IsEnabled = isEnabled;
        }
        else
        {
            await _db.TenantModuleEntitlements.AddAsync(new TenantModuleEntitlement
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ModuleCatalogId = moduleCatalogId,
                State = isEnabled ? "trial" : "disabled",
                IsEnabled = isEnabled
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task SetFeatureEntitlementEnabledAsync(Guid tenantId, Guid moduleFeatureId, bool isEnabled)
    {
        var existing = await _db.TenantFeatureEntitlements
            .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.ModuleFeatureId == moduleFeatureId);

        if (existing is not null)
        {
            existing.IsEnabled = isEnabled;
        }
        else
        {
            await _db.TenantFeatureEntitlements.AddAsync(new TenantFeatureEntitlement
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ModuleFeatureId = moduleFeatureId,
                IsEnabled = isEnabled
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
