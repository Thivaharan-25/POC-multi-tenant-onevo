using OnevoHr.Api.Models.Leave;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface ILeaveRepository
{
    Task<List<LeaveType>> GetLeaveTypesAsync(Guid tenantId);
    Task<LeaveType?> GetLeaveTypeByIdAsync(Guid id);
    Task<List<LeaveRequest>> GetRequestsAsync(Guid tenantId);
    Task<List<LeaveRequest>> GetRequestsForEmployeesAsync(Guid tenantId, IReadOnlyCollection<Guid> employeeIds);
    Task<LeaveRequest?> GetRequestByIdAsync(Guid id);
    Task<List<LeaveBalance>> GetBalancesAsync(Guid tenantId, Guid employeeId, int year);
    Task AddRequestAsync(LeaveRequest request);
    Task<LeaveType?> GetLeaveTypeByCodeAsync(Guid tenantId, string code);
    Task AddLeaveTypeAsync(LeaveType leaveType);
    Task AddLeavePolicyAsync(LeavePolicy policy);
    Task AddLeavePolicyAssignmentAsync(LeavePolicyAssignment assignment);
    Task SaveChangesAsync();
}
