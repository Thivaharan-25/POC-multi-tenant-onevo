using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Templates;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class TemplateRepository : ITemplateRepository
{
    private readonly AppDbContext _db;

    public TemplateRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ConfigurationTemplate>> GetConfigurationTemplatesAsync()
    {
        return await _db.ConfigurationTemplates.AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.TemplateKey)
            .ToListAsync();
    }

    public async Task<ConfigurationTemplate?> GetConfigurationTemplateByIdAsync(Guid id)
    {
        return await _db.ConfigurationTemplates
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<ConfigurationTemplate?> FindByTypeAndEmployeeCountAsync(string templateType, int employeeCount)
    {
        return await _db.ConfigurationTemplates
            .Where(t => t.IsActive
                && t.TemplateType == templateType
                && t.EmployeeRangeMin != null
                && t.EmployeeRangeMin <= employeeCount
                && (t.EmployeeRangeMax == null || t.EmployeeRangeMax >= employeeCount))
            .OrderBy(t => t.EmployeeRangeMin)
            .FirstOrDefaultAsync();
    }

    public async Task<List<RoleTemplate>> GetRoleTemplatesAsync()
    {
        return await _db.RoleTemplates.AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<RoleTemplate?> GetRoleTemplateByIdAsync(Guid id)
    {
        return await _db.RoleTemplates
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddApplicationAsync(TenantConfigurationTemplateApplication application)
    {
        await _db.TenantConfigurationTemplateApplications.AddAsync(application);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
