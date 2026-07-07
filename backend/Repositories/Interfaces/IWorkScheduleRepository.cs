using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IWorkScheduleRepository
{
    Task<List<WorkSchedule>> GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId);
    Task AddWorkScheduleAsync(WorkSchedule schedule);
}
