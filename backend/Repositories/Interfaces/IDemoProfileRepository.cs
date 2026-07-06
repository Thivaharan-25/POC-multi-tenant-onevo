using OnevoHr.Api.Models.Demo;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IDemoProfileRepository
{
    Task<List<DemoProfile>> GetAllAsync();
    Task<DemoProfile?> GetByIdAsync(Guid id);
    Task<DemoProfile?> GetByNameAsync(string name);
    Task<Dictionary<(Guid ModuleCatalogId, string FeatureKey), Guid>> GetModuleFeatureIdsAsync(IEnumerable<Guid> moduleCatalogIds);
}
