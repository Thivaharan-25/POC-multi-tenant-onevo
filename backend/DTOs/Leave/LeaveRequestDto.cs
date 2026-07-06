namespace OnevoHr.Api.DTOs.Leave;

public sealed record LeaveRequestDto(Guid Id, Guid EmployeeId, DateOnly StartDate, DateOnly EndDate, string Status);
