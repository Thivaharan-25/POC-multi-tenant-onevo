using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Agents;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class AgentActivityRepository : IAgentActivityRepository
{
    private readonly AppDbContext _db;

    public AgentActivityRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<RegisteredAgent?> GetRegisteredAgentByDeviceTokenHashAsync(string deviceTokenHash)
    {
        return await _db.RegisteredAgents
            .FirstOrDefaultAsync(a => a.DeviceTokenHash == deviceTokenHash);
    }

    public async Task<RegisteredAgent?> GetRegisteredAgentByDeviceIdAsync(Guid deviceId)
    {
        return await _db.RegisteredAgents
            .Where(a => a.DeviceId == deviceId)
            .OrderByDescending(a => a.RegisteredAt)
            .FirstOrDefaultAsync();
    }

    public async Task<AgentClockState?> GetClockStateAsync(Guid registeredAgentId)
    {
        return await _db.AgentClockStates
            .FirstOrDefaultAsync(c => c.RegisteredAgentId == registeredAgentId);
    }

    public async Task AddClockStateAsync(AgentClockState state)
    {
        await _db.AgentClockStates.AddAsync(state);
    }

    public async Task<ApplicationUsage?> GetApplicationUsageAsync(Guid tenantId, Guid employeeId, DateOnly date, string applicationName)
    {
        return await _db.ApplicationUsages.FirstOrDefaultAsync(u =>
            u.TenantId == tenantId
            && u.EmployeeId == employeeId
            && u.Date == date
            && u.ApplicationName == applicationName);
    }

    public async Task AddApplicationUsageAsync(ApplicationUsage usage)
    {
        await _db.ApplicationUsages.AddAsync(usage);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
