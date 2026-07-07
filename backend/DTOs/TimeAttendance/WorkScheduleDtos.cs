namespace OnevoHr.Api.DTOs.TimeAttendance;

public sealed record WorkScheduleDto(
    Guid Id,
    Guid LegalEntityId,
    string Name,
    string Timezone,
    bool IsActive);
