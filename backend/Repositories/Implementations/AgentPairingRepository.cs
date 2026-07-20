using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Agents;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class AgentPairingRepository : IAgentPairingRepository
{
    private readonly AppDbContext _db;

    public AgentPairingRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AgentPairingRequest request)
    {
        await _db.AgentPairingRequests.AddAsync(request);
    }

    public async Task<AgentPairingRequest?> GetByUserCodeHashAsync(string userCodeHash)
    {
        return await _db.AgentPairingRequests
            .FirstOrDefaultAsync(r => r.UserCodeHash == userCodeHash);
    }

    public async Task<AgentPairingRequest?> GetByDeviceCodeHashAsync(string deviceCodeHash)
    {
        return await _db.AgentPairingRequests
            .FirstOrDefaultAsync(r => r.DeviceCodeHash == deviceCodeHash);
    }

    public async Task AddRegisteredAgentAsync(RegisteredAgent agent)
    {
        await _db.RegisteredAgents.AddAsync(agent);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
