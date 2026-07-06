using OnevoHr.Api.DTOs.Leave;

namespace OnevoHr.Api.Services.Interfaces;

public interface ILeaveService
{
    Task<List<LeaveTypeDto>> GetLeaveTypesAsync();
    Task<List<LeaveRequestDto>> GetVisibleRequestsAsync();
    Task<LeaveRequestDto?> CreateRequestAsync(CreateLeaveRequestDto request);
    Task<LeaveRequestDto?> ApproveRequestAsync(Guid requestId);
    Task<List<LeaveBalanceDto>> GetMyBalancesAsync(int year);
}
