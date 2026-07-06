using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Catalog;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Models.Tenant;
using OnevoHr.Api.Repositories.Implementations;

namespace OnevoHr.Api.Tests.Subscriptions;

public class TenantFeatureEntitlementPersistenceTests
{
    private static AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task FeatureEntitlement_PersistsThroughDbContext_NotStaticState()
    {
        await using var db = CreateInMemoryDb();

        var tenant = new Tenant { Id = Guid.NewGuid(), Slug = "efb-test", Name = "EF Backing Test", Status = "active", CreatedAtUtc = DateTime.UtcNow };
        var moduleCatalog = new ModuleCatalog { Id = Guid.NewGuid(), ModuleKey = "widgets", DisplayName = "Widgets", IsActive = true, CreatedAtUtc = DateTime.UtcNow };
        var moduleFeature = new ModuleFeature { Id = Guid.NewGuid(), ModuleCatalogId = moduleCatalog.Id, FeatureKey = "widgets_pro", IsActive = true };
        db.Tenants.Add(tenant);
        db.ModuleCatalogs.Add(moduleCatalog);
        db.ModuleFeatures.Add(moduleFeature);
        await db.SaveChangesAsync();

        var repository = new SubscriptionRepository(db);
        await repository.AddFeatureEntitlementAsync(new TenantFeatureEntitlement
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            ModuleFeatureId = moduleFeature.Id,
            IsEnabled = true
        });
        await repository.SaveChangesAsync();

        var entitlements = await repository.GetFeatureEntitlementsAsync(tenant.Id);
        Assert.Single(entitlements);
        Assert.True(entitlements[0].IsEnabled);

        // Confirm it round-trips through the real DbSet, not a static in-process bag
        var directCount = await db.TenantFeatureEntitlements.CountAsync(e => e.TenantId == tenant.Id);
        Assert.Equal(1, directCount);
    }
}
