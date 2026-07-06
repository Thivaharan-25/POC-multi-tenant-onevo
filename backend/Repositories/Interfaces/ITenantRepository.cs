using OnevoHr.Api.Models.Tenant;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id);
    Task<Tenant?> GetBySlugAsync(string slug);
    Task<Tenant?> GetByDomainAsync(string domain);
    Task<List<Tenant>> GetAllAsync();
    Task AddAsync(Tenant tenant);
    Task AddProvisioningStateAsync(TenantProvisioningState state);
    Task<TenantProvisioningState?> GetProvisioningStateAsync(Guid tenantId);
    Task SaveChangesAsync();
}
