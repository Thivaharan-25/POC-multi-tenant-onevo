namespace OnevoHr.Api.DTOs.Subscriptions;

public sealed record TenantFeatureEntitlementViewDto(
    Guid ModuleFeatureId,
    string FeatureKey,
    bool IsEnabled);

public sealed record TenantModuleEntitlementViewDto(
    Guid ModuleCatalogId,
    string ModuleKey,
    string DisplayName,
    bool IsFoundation,
    bool IsEnabled,
    IReadOnlyCollection<TenantFeatureEntitlementViewDto> Features);
