using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Demo;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class DemoProfileRepository : IDemoProfileRepository
{
    private readonly AppDbContext _db;

    public DemoProfileRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<DemoProfile>> GetAllAsync()
    {
        return await _db.Set<DemoProfile>().AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<DemoProfile?> GetByIdAsync(Guid id)
    {
        return await _db.Set<DemoProfile>()
            .Include(p => p.ModuleAccess)
            .Include(p => p.UpgradeOptions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<DemoProfile?> GetByNameAsync(string name)
    {
        return await _db.Set<DemoProfile>()
            .Include(p => p.ModuleAccess)
            .Include(p => p.UpgradeOptions)
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<Dictionary<(Guid ModuleCatalogId, string FeatureKey), Guid>> GetModuleFeatureIdsAsync(IEnumerable<Guid> moduleCatalogIds)
    {
        var features = await _db.ModuleFeatures
            .Where(f => moduleCatalogIds.Contains(f.ModuleCatalogId))
            .ToListAsync();

        return features.ToDictionary(f => (f.ModuleCatalogId, f.FeatureKey), f => f.Id);
    }
}
