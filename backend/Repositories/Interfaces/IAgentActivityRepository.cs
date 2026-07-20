using OnevoHr.Api.Models.Agents;
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IAgentActivityRepository
{
    Task<RegisteredAgent?> GetRegisteredAgentByDeviceTokenHashAsync(string deviceTokenHash);
    Task<RegisteredAgent?> GetRegisteredAgentByDeviceIdAsync(Guid deviceId);
    Task<AgentClockState?> GetClockStateAsync(Guid registeredAgentId);
    Task AddClockStateAsync(AgentClockState state);
    Task<ApplicationUsage?> GetApplicationUsageAsync(Guid tenantId, Guid employeeId, DateOnly date, string applicationName);
    Task AddApplicationUsageAsync(ApplicationUsage usage);
    Task SaveChangesAsync();
}
