using OnevoHr.Api.Models.Catalog;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IModuleCatalogRepository
{
    Task<List<ModuleCatalog>> GetModulesAsync();
    Task<ModuleCatalog?> GetByKeyAsync(string moduleKey);
    Task<List<ModuleFeature>> GetFeaturesAsync();
    Task<ModuleFeature?> GetFeatureByKeyAsync(string featureKey);
}
