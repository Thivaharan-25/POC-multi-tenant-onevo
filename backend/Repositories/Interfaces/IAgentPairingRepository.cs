using OnevoHr.Api.Models.Agents;
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IAgentPairingRepository
{
    Task AddAsync(AgentPairingRequest request);
    Task<AgentPairingRequest?> GetByUserCodeHashAsync(string userCodeHash);
    Task<AgentPairingRequest?> GetByDeviceCodeHashAsync(string deviceCodeHash);
    Task AddRegisteredAgentAsync(RegisteredAgent agent);
    Task SaveChangesAsync();
}
