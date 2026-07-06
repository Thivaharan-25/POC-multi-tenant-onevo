using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Catalog;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Models.Tenant;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Tests.TestInfrastructure;

public sealed record SeededTenant(
    Guid TenantId,
    Guid UserId,
    Guid RoleId,
    Guid UngatedPermissionId,
    Guid GatedEnabledPermissionId,
    Guid GatedDisabledPermissionId,
    Guid DisabledModulePermissionId,
    Guid ModuleCatalogId,
    Guid EnabledModuleFeatureId,
    Guid DisabledModuleFeatureId,
    string TenantSlug,
    string UserEmail,
    string UserPassword);

public static class TestDataSeeder
{
    public static async Task<SeededTenant> SeedMinimalTenantAsync(AppDbContext db, IPasswordHasher hasher)
    {
        var uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Slug = $"acme-test-{uniqueSuffix}",
            Name = "Acme Test",
            Status = "active",
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Tenants.Add(tenant);

        var moduleCatalog = new ModuleCatalog
        {
            Id = Guid.NewGuid(),
            ModuleKey = "widgets",
            DisplayName = "Widgets",
            IsFoundation = false,
            IsSellable = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.ModuleCatalogs.Add(moduleCatalog);

        var enabledFeature = new ModuleFeature { Id = Guid.NewGuid(), ModuleCatalogId = moduleCatalog.Id, FeatureKey = "widgets_pro", IsActive = true };
        var disabledFeature = new ModuleFeature { Id = Guid.NewGuid(), ModuleCatalogId = moduleCatalog.Id, FeatureKey = "widgets_export", IsActive = true };
        db.ModuleFeatures.AddRange(enabledFeature, disabledFeature);

        // A second module with NO TenantModuleEntitlement row at all — the module is
        // disabled for the tenant, so its permission must never be assignable.
        var disabledModuleCatalog = new ModuleCatalog
        {
            Id = Guid.NewGuid(),
            ModuleKey = "gadgets",
            DisplayName = "Gadgets",
            IsFoundation = false,
            IsSellable = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.ModuleCatalogs.Add(disabledModuleCatalog);

        var ungatedPermission = new Permission { Id = Guid.NewGuid(), Code = "widgets:read", Description = "Read widgets", Module = "widgets" };
        var disabledModulePermission = new Permission { Id = Guid.NewGuid(), Code = "gadgets:read", Description = "Read gadgets", Module = "gadgets" };
        db.Permissions.Add(disabledModulePermission);
        var gatedEnabledPermission = new Permission { Id = Guid.NewGuid(), Code = "widgets:manage", Description = "Manage widgets", Module = "widgets", FeatureKey = "widgets_pro" };
        var gatedDisabledPermission = new Permission { Id = Guid.NewGuid(), Code = "widgets:export", Description = "Export widgets", Module = "widgets", FeatureKey = "widgets_export" };
        var permissionsManagePermission = new Permission { Id = Guid.NewGuid(), Code = "permissions:manage", Description = "Manage permissions", Module = "roles" };
        var permissionsReadPermission = new Permission { Id = Guid.NewGuid(), Code = "permissions:read", Description = "Read permissions", Module = "roles" };
        db.Permissions.AddRange(ungatedPermission, gatedEnabledPermission, gatedDisabledPermission, permissionsManagePermission, permissionsReadPermission);

        var role = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Name = "Tester",
            Description = "Tester",
            IsSystemRole = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Roles.Add(role);
        db.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = role.Id, PermissionId = ungatedPermission.Id });
        db.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = role.Id, PermissionId = gatedEnabledPermission.Id });
        db.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = role.Id, PermissionId = permissionsManagePermission.Id });
        db.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = role.Id, PermissionId = permissionsReadPermission.Id });
        
        var rolesManagePermission = new Permission { Id = Guid.NewGuid(), Code = "roles:manage", Description = "Manage roles", Module = "roles" };
        var rolesReadPermission = new Permission { Id = Guid.NewGuid(), Code = "roles:read", Description = "Read roles", Module = "roles" };
        var billingReadPermission = new Permission { Id = Guid.NewGuid(), Code = "billing:read", Description = "Read billing", Module = "configuration" };
        var billingManagePermission = new Permission { Id = Guid.NewGuid(), Code = "billing:manage", Description = "Manage billing", Module = "configuration" };
        db.Permissions.AddRange(rolesManagePermission, rolesReadPermission, billingReadPermission, billingManagePermission);
        db.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = role.Id, PermissionId = rolesManagePermission.Id });
        db.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = role.Id, PermissionId = rolesReadPermission.Id });
        db.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = role.Id, PermissionId = billingReadPermission.Id });
        db.RolePermissions.Add(new RolePermission { Id = Guid.NewGuid(), RoleId = role.Id, PermissionId = billingManagePermission.Id });
        
        // Deliberately NOT granting gatedDisabledPermission via role — Task 4's discriminating
        // override test grants it directly to prove a disabled feature still blocks it.

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = $"tester-{uniqueSuffix}@acme-test.test",
            PasswordHash = hasher.Hash("Password123!"),
            DisplayName = "Test User",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Users.Add(user);
        db.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), UserId = user.Id, RoleId = role.Id });

        db.TenantModuleEntitlements.Add(new TenantModuleEntitlement
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            ModuleCatalogId = moduleCatalog.Id,
            State = "trial",
            IsEnabled = true
        });
        // Only the "widgets_pro" feature is entitled/enabled; "widgets_export" has
        // no TenantFeatureEntitlement row at all, so FeatureGateService treats it as disabled.
        db.TenantFeatureEntitlements.Add(new TenantFeatureEntitlement
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            ModuleFeatureId = enabledFeature.Id,
            IsEnabled = true
        });

        await db.SaveChangesAsync();

        return new SeededTenant(
            tenant.Id,
            user.Id,
            role.Id,
            ungatedPermission.Id,
            gatedEnabledPermission.Id,
            gatedDisabledPermission.Id,
            disabledModulePermission.Id,
            moduleCatalog.Id,
            enabledFeature.Id,
            disabledFeature.Id,
            tenant.Slug,
            user.Email,
            "Password123!");
    }
}
