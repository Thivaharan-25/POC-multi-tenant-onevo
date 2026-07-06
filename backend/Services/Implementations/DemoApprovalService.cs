using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.DTOs.Demo;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Models.Tenant;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Services.Notifications;
using TenantEntity = OnevoHr.Api.Models.Tenant.Tenant;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Approving a demo request creates a demo workspace tenant with status "trial",
/// applying the demo profile's trial duration, limits and module/feature access.
/// </summary>
public class DemoApprovalService : IDemoApprovalService
{
    private readonly IDemoRequestRepository _demoRequests;
    private readonly IDemoProfileRepository _demoProfiles;
    private readonly ITenantRepository _tenants;
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IPermissionRepository _permissions;
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOutboxService _outbox;

    public DemoApprovalService(
        IDemoRequestRepository demoRequests,
        IDemoProfileRepository demoProfiles,
        ITenantRepository tenants,
        IUserRepository users,
        IRoleRepository roles,
        IPermissionRepository permissions,
        ISubscriptionRepository subscriptions,
        IPasswordHasher passwordHasher,
        IOutboxService outbox)
    {
        _demoRequests = demoRequests;
        _demoProfiles = demoProfiles;
        _tenants = tenants;
        _users = users;
        _roles = roles;
        _permissions = permissions;
        _subscriptions = subscriptions;
        _passwordHasher = passwordHasher;
        _outbox = outbox;
    }

    public async Task<TenantSummaryDto?> ApproveAsync(Guid demoRequestId, ApproveDemoRequestDto request, Guid reviewedByPlatformUserId)
    {
        var demoRequest = await _demoRequests.GetByIdAsync(demoRequestId);
        if (demoRequest is null || demoRequest.Status != "submitted")
        {
            return null;
        }

        var profile = await _demoProfiles.GetByIdAsync(request.DemoProfileId);
        if (profile is null || !profile.IsActive)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var tenant = new TenantEntity
        {
            Id = Guid.NewGuid(),
            Name = demoRequest.CompanyName,
            Slug = request.TenantSlug,
            Status = "trial",
            Source = "demo_request",
            SourceDemoRequestId = demoRequest.Id,
            DemoProfileId = profile.Id,
            TrialStartsAtUtc = now,
            TrialEndsAtUtc = now.AddDays(profile.TrialDurationDays),
            CreatedAtUtc = now
        };
        await _tenants.AddAsync(tenant);

        // Demo owner role with all tenant permissions and tenant-level scope.
        var ownerRole = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Name = "Tenant Owner",
            Description = "Demo workspace owner",
            IsSystemRole = true,
            CreatedAtUtc = now
        };
        await _roles.AddAsync(ownerRole);

        var tenantPermissions = await _permissions.GetTenantPermissionCatalogAsync();
        foreach (var permission in tenantPermissions)
        {
            await _roles.AddRolePermissionAsync(new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = ownerRole.Id,
                PermissionId = permission.Id
            });
        }

        var ownerUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = request.OwnerEmail,
            PasswordHash = _passwordHasher.Hash(request.OwnerPassword),
            DisplayName = request.OwnerDisplayName,
            IsActive = true,
            CreatedAtUtc = now
        };
        await _users.AddAsync(ownerUser);

        await _roles.AddUserRoleAsync(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = ownerUser.Id,
            RoleId = ownerRole.Id,
            ScopeType = "tenant",
            CreatedAtUtc = now
        });

        // Module/feature entitlements from the demo profile.
        foreach (var moduleAccess in profile.ModuleAccess)
        {
            await _subscriptions.AddModuleEntitlementAsync(new TenantModuleEntitlement
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                ModuleCatalogId = moduleAccess.ModuleCatalogId,
                State = moduleAccess.AccessLevel == "archive" ? "disabled" : "trial",
                IsEnabled = moduleAccess.AccessLevel != "archive"
            });
        }

        var moduleFeatureIds = await _demoProfiles.GetModuleFeatureIdsAsync(
            profile.ModuleAccess.Select(m => m.ModuleCatalogId));

        foreach (var moduleAccess in profile.ModuleAccess)
        {
            var featurePermissions = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(moduleAccess.FeaturePermissions)
                ?? new Dictionary<string, bool>();

            foreach (var (featureKey, isEnabled) in featurePermissions)
            {
                if (moduleFeatureIds.TryGetValue((moduleAccess.ModuleCatalogId, featureKey), out var moduleFeatureId))
                {
                    await _subscriptions.AddFeatureEntitlementAsync(new TenantFeatureEntitlement
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenant.Id,
                        ModuleFeatureId = moduleFeatureId,
                        IsEnabled = isEnabled
                    });
                }
            }
        }

        await _subscriptions.AddResourceLimitAsync(new TenantResourceLimit
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            StorageLimitGb = profile.DemoStorageLimitGb,
            AiTokenLimit = profile.DemoAiTokenLimit,
            EmployeeLimit = profile.MaxEmployees,
            Source = "demo_profile",
            CreatedAtUtc = now
        });

        await _subscriptions.AddSubscriptionAsync(new TenantSubscription
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Status = "demo",
            CreatedAtUtc = now
        });

        demoRequest.Status = "converted_to_demo";
        demoRequest.ReviewedAtUtc = now;
        demoRequest.ReviewedById = reviewedByPlatformUserId;
        demoRequest.CreatedTenantId = tenant.Id;

        await _outbox.EnqueueAsync(tenant.Id, "demo_tenant_created",
            $"{{\"tenantId\":\"{tenant.Id}\",\"demoRequestId\":\"{demoRequest.Id}\"}}");

        await _tenants.SaveChangesAsync();

        return new TenantSummaryDto(
            tenant.Id, tenant.Name, tenant.Slug, tenant.Status, tenant.Source,
            tenant.ConfirmedEmployeeCount, tenant.TrialEndsAtUtc, tenant.CreatedAtUtc);
    }

    public async Task<bool> RejectAsync(Guid demoRequestId, RejectDemoRequestDto request, Guid reviewedByPlatformUserId)
    {
        var demoRequest = await _demoRequests.GetByIdAsync(demoRequestId);
        if (demoRequest is null || demoRequest.Status != "submitted")
        {
            return false;
        }

        demoRequest.Status = "rejected";
        demoRequest.RejectionReason = request.Reason;
        demoRequest.ReviewedAtUtc = DateTime.UtcNow;
        demoRequest.ReviewedById = reviewedByPlatformUserId;
        await _demoRequests.SaveChangesAsync();
        return true;
    }
}
