using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Models.Tenant;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;
using TenantEntity = OnevoHr.Api.Models.Tenant.Tenant;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Operator-created paid drafts only. The main demo flow does not use
/// provisioning status: DemoRequest -> Trial -> Pending Payment -> Active.
/// </summary>
public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly ITenantRepository _tenants;

    public TenantProvisioningService(ITenantRepository tenants)
    {
        _tenants = tenants;
    }

    public async Task<TenantSummaryDto> CreateDraftAsync(CreateTenantDraftDto request)
    {
        var now = DateTime.UtcNow;
        var tenant = new TenantEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            Status = "provisioning",
            Source = "operator_provisioning",
            EstimatedEmployeeCount = request.EstimatedEmployeeCount,
            CreatedAtUtc = now
        };
        await _tenants.AddAsync(tenant);

        await _tenants.AddProvisioningStateAsync(new TenantProvisioningState
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            CurrentStep = "organization_info",
            ActivationReady = false,
            LastUpdatedAtUtc = now
        });

        await _tenants.SaveChangesAsync();
        return Map(tenant);
    }

    public async Task<TenantProvisioningState?> GetProvisioningStateAsync(Guid tenantId)
    {
        return await _tenants.GetProvisioningStateAsync(tenantId);
    }

    public async Task<List<TenantSummaryDto>> GetTenantsAsync()
    {
        var tenants = await _tenants.GetAllAsync();
        return tenants.Select(Map).ToList();
    }

    public async Task<TenantSummaryDto?> GetTenantAsync(Guid tenantId)
    {
        var tenant = await _tenants.GetByIdAsync(tenantId);
        return tenant is null ? null : Map(tenant);
    }

    private static TenantSummaryDto Map(TenantEntity t)
    {
        return new TenantSummaryDto(
            t.Id, t.Name, t.Slug, t.Status, t.Source,
            t.ConfirmedEmployeeCount, t.TrialEndsAtUtc, t.CreatedAtUtc);
    }
}
