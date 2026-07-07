using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class WorkScheduleRepository : IWorkScheduleRepository
{
    private readonly AppDbContext _db;

    public WorkScheduleRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<WorkSchedule>> GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId)
    {
        var query = _db.WorkSchedules.AsNoTracking()
            .Where(s => s.TenantId == tenantId);

        if (legalEntityId.HasValue)
        {
            query = query.Where(s => s.LegalEntityId == legalEntityId.Value);
        }

        return await query
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task AddWorkScheduleAsync(WorkSchedule schedule)
    {
        await _db.WorkSchedules.AddAsync(schedule);
    }
}
