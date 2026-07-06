using OnevoHr.Api.DTOs.Permissions;
using OnevoHr.Api.DTOs.Roles;
using OnevoHr.Api.Exceptions;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Effective permission check: the user's roles must grant the permission,
/// and if the permission is mapped to a feature, that feature (and its module)
/// must be enabled for the tenant. Role permissions can never grant access
/// outside the tenant's entitlements.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissions;
    private readonly IRoleRepository _roles;
    private readonly IFeatureGateService _featureGate;
    private readonly IUserRepository _users;

    public PermissionService(IPermissionRepository permissions, IRoleRepository roles, IFeatureGateService featureGate, IUserRepository users)
    {
        _permissions = permissions;
        _roles = roles;
        _featureGate = featureGate;
        _users = users;
    }

    public async Task<List<RoleDto>> GetTenantRolesAsync(Guid tenantId)
    {
        var roles = await _roles.GetByTenantAsync(tenantId);
        return roles
            .Select(r => new RoleDto(
                r.Id,
                r.Name,
                r.Description ?? string.Empty,
                r.IsSystemRole,
                r.RolePermissions
                    .Where(rp => rp.Permission is not null)
                    .Select(rp => rp.Permission!.Id)
                    .OrderBy(id => id)
                    .ToList(),
                r.RolePermissions
                    .Where(rp => rp.Permission is not null)
                    .Select(rp => rp.Permission!.Code)
                    .OrderBy(k => k)
                    .ToList()))
            .ToList();
    }

    public async Task<List<PermissionDto>> GetTenantPermissionCatalogAsync(Guid tenantId)
    {
        var catalog = await _permissions.GetTenantPermissionCatalogAsync();
        var enabledModules = await _featureGate.GetEnabledModuleKeysAsync(tenantId);
        var enabledFeatures = await _featureGate.GetEnabledFeatureKeysAsync(tenantId);

        return catalog
            .Where(p => 
                (string.IsNullOrEmpty(p.Module) || enabledModules.Contains(p.Module)) &&
                (string.IsNullOrEmpty(p.FeatureKey) || enabledFeatures.Contains(p.FeatureKey)))
            .Select(p => new PermissionDto(p.Id, p.Code, p.Description, p.Module, p.FeatureKey))
            .ToList();
    }

    public async Task<IReadOnlyCollection<string>> GetEffectivePermissionsAsync(Guid tenantId, Guid userId)
    {
        // 1. Fetch user's raw role-permission keys
        var rolePermissionKeys = await _permissions.GetEffectivePermissionKeysAsync(tenantId, userId);

        // 2. Fetch all permissions to resolve gating features and override permission codes
        var permissions = await _permissions.GetAllPermissionsAsync();
        var permissionsById = permissions.ToDictionary(p => p.Id, p => p.Code);

        // 3. Fetch active overrides and split them into grant/revoke codes
        var overrides = await _permissions.GetActiveOverridesAsync(tenantId, userId, DateTimeOffset.UtcNow);
        var grantOverrideCodes = new List<string>();
        var revokeOverrideCodes = new List<string>();
        foreach (var o in overrides)
        {
            if (!permissionsById.TryGetValue(o.PermissionId, out var code))
            {
                continue;
            }

            if (o.GrantType == "grant")
            {
                grantOverrideCodes.Add(code);
            }
            else if (o.GrantType == "revoke")
            {
                revokeOverrideCodes.Add(code);
            }
        }

        // 4. Build the raw candidate set: role permissions plus grant overrides.
        //    Grant overrides still go through feature gating below (Decision 6) —
        //    they never bypass tenant entitlements, only role assignment.
        var rawCandidateKeys = new HashSet<string>(rolePermissionKeys);
        foreach (var code in grantOverrideCodes)
        {
            rawCandidateKeys.Add(code);
        }

        // 5. Fetch the set of features currently enabled for this tenant
        var enabledFeatureKeys = await _featureGate.GetEnabledFeatureKeysAsync(tenantId);

        // 6. Map permission keys to their gating features
        var permissionToFeatures = new Dictionary<string, List<string>>();
        foreach (var p in permissions)
        {
            if (string.IsNullOrEmpty(p.Code) || string.IsNullOrEmpty(p.FeatureKey))
            {
                continue;
            }

            if (!permissionToFeatures.TryGetValue(p.Code, out var featureList))
            {
                featureList = new List<string>();
                permissionToFeatures[p.Code] = featureList;
            }
            featureList.Add(p.FeatureKey);
        }

        // 7. Build the feature-gated effective set from the raw candidate set
        var effective = new HashSet<string>();
        foreach (var key in rawCandidateKeys)
        {
            if (!permissionToFeatures.TryGetValue(key, out var gatingFeatures))
            {
                effective.Add(key);
                continue;
            }

            var isGatedFeatureEnabled = false;
            foreach (var featureKey in gatingFeatures)
            {
                if (enabledFeatureKeys.Contains(featureKey))
                {
                    isGatedFeatureEnabled = true;
                    break;
                }
            }

            if (isGatedFeatureEnabled)
            {
                effective.Add(key);
            }
        }

        // 8. Apply revoke overrides last, unconditionally — revocation is always honored
        //    regardless of feature state (Decision 6).
        foreach (var code in revokeOverrideCodes)
        {
            effective.Remove(code);
        }

        return effective.ToList();
    }

    public async Task<bool> HasPermissionAsync(Guid tenantId, Guid userId, string permissionKey)
    {
        var effective = await GetEffectivePermissionsAsync(tenantId, userId);
        return effective.Contains(permissionKey);
    }

    public async Task<List<OverrideDto>> GetOverridesForUserAsync(Guid tenantId, Guid userId)
    {
        var overrides = await _permissions.GetOverridesForUserAsync(tenantId, userId);
        var permissions = await _permissions.GetAllPermissionsAsync();
        var codeById = permissions.ToDictionary(p => p.Id, p => p.Code);

        return overrides
            .Select(o => new OverrideDto(
                o.Id,
                o.UserId,
                o.PermissionId,
                codeById.TryGetValue(o.PermissionId, out var code) ? code : string.Empty,
                o.GrantType,
                o.Reason,
                o.ValidFrom,
                o.ExpiresAt,
                o.GrantedBy,
                o.CreatedAt))
            .ToList();
    }

    public async Task<OverrideDto> CreateOverrideAsync(Guid tenantId, Guid grantedByUserId, OverrideRequestDto request)
    {
        var catalog = await GetTenantPermissionCatalogAsync(tenantId);
        var allowableIds = new HashSet<Guid>(catalog.Select(p => p.Id));
        if (!allowableIds.Contains(request.PermissionId))
        {
            throw new UnassignablePermissionException("The permission is not assignable for this tenant.");
        }

        var entity = new UserPermissionOverride
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = request.UserId,
            PermissionId = request.PermissionId,
            GrantType = request.GrantType,
            Reason = request.Reason,
            ValidFrom = request.ValidFrom,
            ExpiresAt = request.ExpiresAt,
            GrantedBy = grantedByUserId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _permissions.AddOverrideAsync(entity);
        await _permissions.SaveChangesAsync();

        var permissions = await _permissions.GetAllPermissionsAsync();
        var code = permissions.FirstOrDefault(p => p.Id == entity.PermissionId)?.Code ?? string.Empty;

        return new OverrideDto(
            entity.Id,
            entity.UserId,
            entity.PermissionId,
            code,
            entity.GrantType,
            entity.Reason,
            entity.ValidFrom,
            entity.ExpiresAt,
            entity.GrantedBy,
            entity.CreatedAt);
    }

    public async Task RevokeOverrideAsync(Guid tenantId, Guid overrideId)
    {
        var entity = await _permissions.GetOverrideByIdAsync(tenantId, overrideId);
        if (entity is null)
        {
            return;
        }

        await _permissions.RemoveOverrideAsync(entity);
        await _permissions.SaveChangesAsync();
    }

    public async Task<RoleDto> CreateRoleAsync(Guid tenantId, CreateRoleRequestDto request)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name,
            Description = request.Description,
            IsSystemRole = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _roles.AddAsync(role);
        await _roles.SaveChangesAsync();

        return new RoleDto(role.Id, role.Name, role.Description ?? string.Empty, role.IsSystemRole, Array.Empty<Guid>(), Array.Empty<string>());
    }

    public async Task SetRolePermissionsAsync(Guid tenantId, Guid roleId, SetRolePermissionsRequestDto request)
    {
        var catalog = await GetTenantPermissionCatalogAsync(tenantId);
        var allowableIds = new HashSet<Guid>(catalog.Select(p => p.Id));
        if (request.PermissionIds.Any(id => !allowableIds.Contains(id)))
        {
            throw new UnassignablePermissionException("One or more permissions are not assignable for this tenant.");
        }

        await _roles.ReplaceRolePermissionsAsync(tenantId, roleId, request.PermissionIds);
    }

    public async Task AssignUserRoleAsync(Guid tenantId, AssignUserRoleRequestDto request)
    {
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            RoleId = request.RoleId
        };

        await _roles.AddUserRoleAsync(userRole);
        await _roles.SaveChangesAsync();
    }

    public async Task<List<TenantUserDto>> GetTenantUsersAsync(Guid tenantId)
    {
        var users = await _users.GetByTenantAsync(tenantId);
        return users
            .Select(u => new TenantUserDto(u.Id, u.Email, u.DisplayName))
            .ToList();
    }
}
