using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class OnboardingRepository : IOnboardingRepository
{
    private readonly AppDbContext _db;

    public OnboardingRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<OnboardingDraft?> GetDraftByIdAsync(Guid tenantId, Guid draftId)
    {
        // Every draft read is tenant-scoped so one tenant can never load
        // another tenant's draft by guessing its id.
        return await _db.OnboardingDrafts
            .FirstOrDefaultAsync(d => d.TenantId == tenantId && d.Id == draftId);
    }

    public async Task<OnboardingDraft?> GetDraftByEmailAsync(Guid tenantId, string email)
    {
        return await _db.OnboardingDrafts
            .FirstOrDefaultAsync(d => d.TenantId == tenantId && d.WorkEmail == email && d.Status == "draft");
    }

    public async Task AddDraftAsync(OnboardingDraft draft)
    {
        await _db.OnboardingDrafts.AddAsync(draft);
    }

    public async Task<WorkSchedule?> GetWorkScheduleAsync(Guid tenantId, Guid scheduleId)
    {
        return await _db.WorkSchedules
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == scheduleId);
    }

    public async Task<ChecklistTemplate?> GetChecklistTemplateAsync(Guid tenantId, Guid templateId)
    {
        return await _db.ChecklistTemplates
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Id == templateId);
    }

    public async Task<List<PositionAccessTemplate>> GetActivePositionAccessTemplatesAsync(Guid tenantId, Guid positionId)
    {
        return await _db.PositionAccessTemplates.AsNoTracking()
            .Where(t => t.TenantId == tenantId && t.PositionId == positionId && t.IsActive)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
