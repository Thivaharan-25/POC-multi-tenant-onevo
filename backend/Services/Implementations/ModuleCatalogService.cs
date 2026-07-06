using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class ModuleCatalogService : IModuleCatalogService
{
    private readonly IModuleCatalogRepository _moduleCatalog;

    public ModuleCatalogService(IModuleCatalogRepository moduleCatalog)
    {
        _moduleCatalog = moduleCatalog;
    }

    public async Task<List<ModuleCatalogDto>> GetModulesAsync()
    {
        var modules = await _moduleCatalog.GetModulesAsync();
        return modules.Select(m => new ModuleCatalogDto(
            m.Id, m.ModuleKey, m.DisplayName, m.Description,
            m.IsFoundation, m.IsSellable, m.IsActive,
            m.Features
                .OrderBy(f => f.FeatureKey)
                .Select(f => new ModuleFeatureDto(f.Id, f.FeatureKey, f.IsActive))
                .ToList()))
            .ToList();
    }
}
