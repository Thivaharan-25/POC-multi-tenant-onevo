using OnevoHr.Api.DTOs.Leave;
using OnevoHr.Api.Models.Leave;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leave;
    private readonly ICurrentUserService _currentUser;
    private readonly IScopeResolverService _scopeResolver;

    public LeaveService(
        ILeaveRepository leave,
        ICurrentUserService currentUser,
        IScopeResolverService scopeResolver)
    {
        _leave = leave;
        _currentUser = currentUser;
        _scopeResolver = scopeResolver;
    }

    public async Task<List<LeaveTypeDto>> GetLeaveTypesAsync()
    {
        if (_currentUser.TenantId is not Guid tenantId)
        {
            return new List<LeaveTypeDto>();
        }

        var types = await _leave.GetLeaveTypesAsync(tenantId);
        return types.Select(t => new LeaveTypeDto(t.Id, t.Code, t.Name, t.IsPaid, t.RequiresApproval)).ToList();
    }

    public async Task<List<LeaveRequestDto>> GetVisibleRequestsAsync()
    {
        if (_currentUser.TenantId is not Guid tenantId || _currentUser.UserId is not Guid userId)
        {
            return new List<LeaveRequestDto>();
        }

        // RLS-style filtering: leave:read sees requests within scope, leave:read-own only own.
        if (_currentUser.HasPermission("leave:read"))
        {
            var scope = await _scopeResolver.ResolveScopeAsync(tenantId, userId);
            var requests = await _leave.GetRequestsForEmployeesAsync(tenantId, scope.VisibleEmployeeIds);
            return requests.Select(Map).ToList();
        }

        if (_currentUser.EmployeeId is Guid employeeId)
        {
            var own = await _leave.GetRequestsForEmployeesAsync(tenantId, new[] { employeeId });
            return own.Select(Map).ToList();
        }

        return new List<LeaveRequestDto>();
    }

    public async Task<LeaveRequestDto?> CreateRequestAsync(CreateLeaveRequestDto request)
    {
        if (_currentUser.TenantId is not Guid tenantId || _currentUser.EmployeeId is not Guid employeeId)
        {
            return null;
        }

        var leaveType = await _leave.GetLeaveTypeByIdAsync(request.LeaveTypeId);
        if (leaveType is null || leaveType.TenantId != tenantId || !leaveType.IsActive)
        {
            return null;
        }

        if (request.EndDate < request.StartDate)
        {
            return null;
        }

        var entity = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EmployeeId = employeeId,
            LeaveTypeId = request.LeaveTypeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = "submitted",
            CreatedAtUtc = DateTime.UtcNow
        };
        await _leave.AddRequestAsync(entity);
        await _leave.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<LeaveRequestDto?> ApproveRequestAsync(Guid requestId)
    {
        if (_currentUser.TenantId is not Guid tenantId || _currentUser.UserId is not Guid userId)
        {
            return null;
        }

        var request = await _leave.GetRequestByIdAsync(requestId);
        if (request is null || request.TenantId != tenantId || request.Status != "submitted")
        {
            return null;
        }

        var scope = await _scopeResolver.ResolveScopeAsync(tenantId, userId);
        if (!scope.VisibleEmployeeIds.Contains(request.EmployeeId))
        {
            return null;
        }

        request.Status = "approved";
        await _leave.SaveChangesAsync();
        return Map(request);
    }

    public async Task<List<LeaveBalanceDto>> GetMyBalancesAsync(int year)
    {
        if (_currentUser.TenantId is not Guid tenantId || _currentUser.EmployeeId is not Guid employeeId)
        {
            return new List<LeaveBalanceDto>();
        }

        var balances = await _leave.GetBalancesAsync(tenantId, employeeId, year);
        return balances.Select(b => new LeaveBalanceDto(
            b.LeaveTypeId, b.LeaveType?.Name ?? string.Empty,
            b.EntitledDays, b.UsedDays, b.RemainingDays, b.Year)).ToList();
    }

    private static LeaveRequestDto Map(LeaveRequest r)
    {
        return new LeaveRequestDto(r.Id, r.EmployeeId, r.StartDate, r.EndDate, r.Status);
    }
}
