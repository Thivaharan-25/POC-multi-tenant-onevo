namespace OnevoHr.Api.DTOs.Leave;

public sealed record LeaveTypeDto(Guid Id, string Code, string Name, bool IsPaid, bool RequiresApproval);

public sealed record LeaveBalanceDto(Guid LeaveTypeId, string LeaveTypeName, decimal EntitledDays, decimal UsedDays, decimal RemainingDays, int Year);

public sealed record CreateLeaveRequestDto(Guid LeaveTypeId, DateOnly StartDate, DateOnly EndDate);
