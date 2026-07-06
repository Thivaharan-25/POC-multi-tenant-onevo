namespace OnevoHr.Api.Services.Interfaces;

public interface ITenantActivationService
{
    Task<bool> ActivateAsync(Guid tenantId);
}
