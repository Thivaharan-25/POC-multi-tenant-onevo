using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Models.Tenant;

namespace OnevoHr.Api.Services.Interfaces;

public interface ITenantProvisioningService
{
    Task<TenantSummaryDto> CreateDraftAsync(CreateTenantDraftDto request);
    Task<TenantProvisioningState?> GetProvisioningStateAsync(Guid tenantId);
    Task<List<TenantSummaryDto>> GetTenantsAsync();
    Task<TenantSummaryDto?> GetTenantAsync(Guid tenantId);
}
