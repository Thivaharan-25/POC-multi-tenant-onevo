using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Tenant;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _db;

    public TenantRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id)
    {
        return await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tenant?> GetBySlugAsync(string slug)
    {
        return await _db.Tenants
            .FirstOrDefaultAsync(t => t.Slug == slug);
    }

    // Tenant resolution happens before the tenant context is established. ONEVO
    // owns the parent domain and issues only {slug}.onevo.com subdomains (see
    // ADE-START-HERE.md); there is no per-tenant custom domain table, so the
    // subdomain portion of the request host (or the X-Tenant-Domain dev header)
    // is matched against tenants.slug directly.
    public async Task<Tenant?> GetByDomainAsync(string domain)
    {
        var slug = domain.Split('.')[0];
        if (slug == "localhost" || slug == "127") slug = "acme";
        return await GetBySlugAsync(slug);
    }

    public async Task<List<Tenant>> GetAllAsync()
    {
        return await _db.Tenants.AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Tenant tenant)
    {
        await _db.Tenants.AddAsync(tenant);
    }

    public async Task AddProvisioningStateAsync(TenantProvisioningState state)
    {
        await _db.TenantProvisioningStates.AddAsync(state);
    }

    public async Task<TenantProvisioningState?> GetProvisioningStateAsync(Guid tenantId)
    {
        return await _db.TenantProvisioningStates
            .FirstOrDefaultAsync(s => s.TenantId == tenantId);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
