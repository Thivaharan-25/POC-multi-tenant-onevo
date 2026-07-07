using OnevoHr.Api.DTOs.TimeAttendance;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class WorkScheduleService : IWorkScheduleService
{
    private readonly IWorkScheduleRepository _workSchedules;

    public WorkScheduleService(IWorkScheduleRepository workSchedules)
    {
        _workSchedules = workSchedules;
    }

    public async Task<List<WorkScheduleDto>> GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId)
    {
        var schedules = await _workSchedules.GetWorkSchedulesAsync(tenantId, legalEntityId);
        return schedules.Select(s => new WorkScheduleDto(
            s.Id, s.LegalEntityId, s.Name, s.Timezone, s.IsActive, s.DefaultForNewEmployee)).ToList();
    }
}
