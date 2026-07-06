using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Leave;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext _db;

    public LeaveRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<LeaveType>> GetLeaveTypesAsync(Guid tenantId)
    {
        return await _db.LeaveTypes.AsNoTracking()
            .Where(t => t.TenantId == tenantId && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<LeaveType?> GetLeaveTypeByIdAsync(Guid id)
    {
        return await _db.LeaveTypes
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<LeaveRequest>> GetRequestsAsync(Guid tenantId)
    {
        return await _db.LeaveRequests.AsNoTracking()
            .Include(r => r.LeaveType)
            .Where(r => r.TenantId == tenantId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<List<LeaveRequest>> GetRequestsForEmployeesAsync(Guid tenantId, IReadOnlyCollection<Guid> employeeIds)
    {
        return await _db.LeaveRequests.AsNoTracking()
            .Include(r => r.LeaveType)
            .Where(r => r.TenantId == tenantId && employeeIds.Contains(r.EmployeeId))
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<LeaveRequest?> GetRequestByIdAsync(Guid id)
    {
        return await _db.LeaveRequests
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<LeaveBalance>> GetBalancesAsync(Guid tenantId, Guid employeeId, int year)
    {
        return await _db.LeaveBalances.AsNoTracking()
            .Include(b => b.LeaveType)
            .Where(b => b.TenantId == tenantId && b.EmployeeId == employeeId && b.Year == year)
            .ToListAsync();
    }

    public async Task AddRequestAsync(LeaveRequest request)
    {
        await _db.LeaveRequests.AddAsync(request);
    }

    public async Task<LeaveType?> GetLeaveTypeByCodeAsync(Guid tenantId, string code)
    {
        return await _db.LeaveTypes
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Code == code);
    }

    public async Task AddLeaveTypeAsync(LeaveType leaveType)
    {
        await _db.LeaveTypes.AddAsync(leaveType);
    }

    public async Task AddLeavePolicyAsync(LeavePolicy policy)
    {
        await _db.LeavePolicies.AddAsync(policy);
    }

    public async Task AddLeavePolicyAssignmentAsync(LeavePolicyAssignment assignment)
    {
        await _db.LeavePolicyAssignments.AddAsync(assignment);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
