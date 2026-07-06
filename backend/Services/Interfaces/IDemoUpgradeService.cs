using OnevoHr.Api.DTOs.Demo;

namespace OnevoHr.Api.Services.Interfaces;

public interface IDemoUpgradeService
{
    Task<DemoUpgradeQuoteDto?> GetQuoteAsync(Guid tenantId, DemoUpgradeQuoteRequestDto request);
    Task<DemoUpgradeResultDto?> SubmitUpgradeAsync(Guid tenantId, DemoUpgradeSubmitRequestDto request);
}
