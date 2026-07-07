using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IOnboardingRepository
{
    Task<OnboardingDraft?> GetDraftByIdAsync(Guid tenantId, Guid draftId);
    Task<OnboardingDraft?> GetDraftByEmailAsync(Guid tenantId, string email);
    Task AddDraftAsync(OnboardingDraft draft);
    Task<WorkSchedule?> GetWorkScheduleAsync(Guid tenantId, Guid scheduleId);
    Task<ChecklistTemplate?> GetChecklistTemplateAsync(Guid tenantId, Guid templateId);
    Task<List<PositionAccessTemplate>> GetActivePositionAccessTemplatesAsync(Guid tenantId, Guid positionId);
    Task SaveChangesAsync();
}
