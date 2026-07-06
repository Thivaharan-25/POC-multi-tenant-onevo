using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Data.Seed;
using OnevoHr.Api.Services.Implementations;

namespace OnevoHr.Api.Tests.Subscriptions;

public class SeedPhase2ExclusionTests
{
    private static readonly string[] Phase2FeatureKeys =
    {
        "work_management.sprints",
        "work_management.boards",
        "work_management.okrs",
        "work_management.roadmaps",
        "work_management.resource_planning",
        "work_management.work_analytics",
        "work_management.github_integration",
        "integrations.microsoft_teams",
        "integrations.github",
        "integrations.google_workspace",
        "integrations.webhooks",
        "integrations.api_access"
    };

    private static async Task<AppDbContext> SeedFreshDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        await DatabaseSeeder.SeedAsync(db, new PasswordHasher());
        return db;
    }

    [Fact]
    public async Task Seed_DoesNotContainPhase2FeatureKeys()
    {
        using var db = await SeedFreshDatabaseAsync();

        var seededFeatureKeys = await db.ModuleFeatures.Select(f => f.FeatureKey).ToListAsync();

        foreach (var phase2Key in Phase2FeatureKeys)
        {
            Assert.DoesNotContain(phase2Key, seededFeatureKeys);
        }

        // No workflow engine features or permissions either.
        Assert.DoesNotContain(seededFeatureKeys, k => k.StartsWith("workflow_engine."));
        var permissionCodes = await db.Permissions.Select(p => p.Code).ToListAsync();
        Assert.DoesNotContain(permissionCodes, c => c.StartsWith("workflows:"));
    }

    [Fact]
    public async Task Seed_DoesNotContainPhase2IntegrationsModule()
    {
        using var db = await SeedFreshDatabaseAsync();

        var moduleKeys = await db.ModuleCatalogs.Select(m => m.ModuleKey).ToListAsync();

        Assert.DoesNotContain("integrations", moduleKeys);
    }

    [Fact]
    public async Task Seed_KeepsPhase1WorkManagementBasics()
    {
        using var db = await SeedFreshDatabaseAsync();

        var seededFeatureKeys = await db.ModuleFeatures.Select(f => f.FeatureKey).ToListAsync();

        Assert.Contains("work_management.projects", seededFeatureKeys);
        Assert.Contains("work_management.tasks", seededFeatureKeys);
        Assert.Contains("work_management.time_tracking", seededFeatureKeys);
    }

    [Fact]
    public async Task Seed_TenantFeatureEntitlements_ContainNoPhase2Keys()
    {
        using var db = await SeedFreshDatabaseAsync();

        var entitledFeatureKeys = await db.TenantFeatureEntitlements
            .Join(db.ModuleFeatures, e => e.ModuleFeatureId, f => f.Id, (e, f) => f.FeatureKey)
            .ToListAsync();

        Assert.NotEmpty(entitledFeatureKeys);
        foreach (var phase2Key in Phase2FeatureKeys)
        {
            Assert.DoesNotContain(phase2Key, entitledFeatureKeys);
        }
    }
}
