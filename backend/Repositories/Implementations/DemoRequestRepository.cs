using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.DeveloperPlatform;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class DemoRequestRepository : IDemoRequestRepository
{
    private readonly AppDbContext _db;

    public DemoRequestRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<DemoRequest>> GetAllAsync()
    {
        return await _db.Set<DemoRequest>().AsNoTracking()
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<DemoRequest?> GetByIdAsync(Guid id)
    {
        return await _db.Set<DemoRequest>()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(DemoRequest request)
    {
        await _db.Set<DemoRequest>().AddAsync(request);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
