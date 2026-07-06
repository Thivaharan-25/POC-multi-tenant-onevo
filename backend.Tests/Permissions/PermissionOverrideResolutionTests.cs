using Microsoft.Extensions.DependencyInjection;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;

namespace OnevoHr.Api.Tests.Permissions;

public class PermissionOverrideResolutionTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PermissionOverrideResolutionTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GrantOverride_AddsUngatedPermissionNotGrantedByAnyRole()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);

        var extraPermission = new Permission { Id = Guid.NewGuid(), Code = "widgets:delete", Description = "Delete widgets", Module = "widgets" };
        db.Permissions.Add(extraPermission);
        db.UserPermissionOverrides.Add(new UserPermissionOverride
        {
            Id = Guid.NewGuid(),
            TenantId = seed.TenantId,
            UserId = seed.UserId,
            PermissionId = extraPermission.Id,
            GrantType = "grant",
            Reason = "test grant",
            GrantedBy = seed.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync();

        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var effective = await permissionService.GetEffectivePermissionsAsync(seed.TenantId, seed.UserId);

        Assert.Contains("widgets:delete", effective);
    }

    [Fact]
    public async Task GrantOverride_ForGatedPermissionWithDisabledFeature_DoesNotApply()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);

        db.UserPermissionOverrides.Add(new UserPermissionOverride
        {
            Id = Guid.NewGuid(),
            TenantId = seed.TenantId,
            UserId = seed.UserId,
            PermissionId = seed.GatedDisabledPermissionId,
            GrantType = "grant",
            Reason = "attempt to grant a permission behind a disabled feature",
            GrantedBy = seed.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync();

        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var effective = await permissionService.GetEffectivePermissionsAsync(seed.TenantId, seed.UserId);

        // widgets_export has no TenantFeatureEntitlement row (disabled) — the grant must not surface it.
        Assert.DoesNotContain("widgets:export", effective);
    }

    [Fact]
    public async Task RevokeOverride_RemovesPermissionGrantedByRole()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);

        db.UserPermissionOverrides.Add(new UserPermissionOverride
        {
            Id = Guid.NewGuid(),
            TenantId = seed.TenantId,
            UserId = seed.UserId,
            PermissionId = seed.UngatedPermissionId,
            GrantType = "revoke",
            Reason = "test revoke",
            GrantedBy = seed.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync();

        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var effective = await permissionService.GetEffectivePermissionsAsync(seed.TenantId, seed.UserId);

        Assert.DoesNotContain("widgets:read", effective);
    }

    [Fact]
    public async Task ExpiredOverride_IsIgnored()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);

        db.UserPermissionOverrides.Add(new UserPermissionOverride
        {
            Id = Guid.NewGuid(),
            TenantId = seed.TenantId,
            UserId = seed.UserId,
            PermissionId = seed.UngatedPermissionId,
            GrantType = "revoke",
            Reason = "expired revoke",
            GrantedBy = seed.UserId,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1),
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-2)
        });
        await db.SaveChangesAsync();

        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var effective = await permissionService.GetEffectivePermissionsAsync(seed.TenantId, seed.UserId);

        Assert.Contains("widgets:read", effective);
    }
}
