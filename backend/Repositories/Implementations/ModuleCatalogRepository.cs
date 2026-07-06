using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Catalog;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class ModuleCatalogRepository : IModuleCatalogRepository
{
    private readonly AppDbContext _db;

    public ModuleCatalogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ModuleCatalog>> GetModulesAsync()
    {
        return await _db.ModuleCatalogs.AsNoTracking()
            .Include(m => m.Features)
            .OrderBy(m => m.ModuleKey)
            .ToListAsync();
    }

    public async Task<ModuleCatalog?> GetByKeyAsync(string moduleKey)
    {
        return await _db.ModuleCatalogs
            .Include(m => m.Features)
            .FirstOrDefaultAsync(m => m.ModuleKey == moduleKey);
    }

    public async Task<List<ModuleFeature>> GetFeaturesAsync()
    {
        return await _db.ModuleFeatures.AsNoTracking()
            .Include(f => f.ModuleCatalog)
            .OrderBy(f => f.FeatureKey)
            .ToListAsync();
    }

    public async Task<ModuleFeature?> GetFeatureByKeyAsync(string featureKey)
    {
        return await _db.ModuleFeatures
            .FirstOrDefaultAsync(f => f.FeatureKey == featureKey);
    }
}
