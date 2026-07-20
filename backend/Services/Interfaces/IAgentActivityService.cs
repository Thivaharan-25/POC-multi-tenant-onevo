using OnevoHr.Api.DTOs.Agents;

namespace OnevoHr.Api.Services.Interfaces;

public interface IAgentActivityService
{
    Task<ClockStateResponseDto?> ClockInAsync(Guid registeredAgentId, Guid tenantId, Guid? employeeId);
    Task<ClockStateResponseDto?> ClockOutAsync(Guid registeredAgentId);
    Task<bool> SubmitAppUsageAsync(Guid registeredAgentId, Guid tenantId, Guid? employeeId, IReadOnlyList<AppUsageSampleDto> samples);
    Task<bool> IngestAsync(Guid registeredAgentId, Guid tenantId, Guid? employeeId, IngestRequestDto request);
}
