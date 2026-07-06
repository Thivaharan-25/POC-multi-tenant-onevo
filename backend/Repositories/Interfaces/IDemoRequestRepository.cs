using OnevoHr.Api.Models.DeveloperPlatform;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IDemoRequestRepository
{
    Task<List<DemoRequest>> GetAllAsync();
    Task<DemoRequest?> GetByIdAsync(Guid id);
    Task AddAsync(DemoRequest request);
    Task SaveChangesAsync();
}
