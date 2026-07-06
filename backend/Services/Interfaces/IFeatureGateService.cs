namespace OnevoHr.Api.Services.Interfaces;

public interface IFeatureGateService
{
    Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureKey);
    Task<IReadOnlyCollection<string>> GetEnabledFeatureKeysAsync(Guid tenantId);
    Task<IReadOnlyCollection<string>> GetEnabledModuleKeysAsync(Guid tenantId);
}
