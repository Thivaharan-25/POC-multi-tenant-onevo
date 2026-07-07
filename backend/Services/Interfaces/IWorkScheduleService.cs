using OnevoHr.Api.DTOs.TimeAttendance;

namespace OnevoHr.Api.Services.Interfaces;

public interface IWorkScheduleService
{
    Task<List<WorkScheduleDto>> GetWorkSchedulesAsync(Guid tenantId, Guid? legalEntityId);
}
