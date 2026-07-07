using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Employee>> GetByTenantAsync(Guid tenantId)
    {
        return await _db.Employees.AsNoTracking()
            .Where(e => e.TenantId == tenantId)
            .OrderBy(e => e.EmployeeNumber)
            .ToListAsync();
    }

    public async Task<List<Employee>> GetByIdsAsync(Guid tenantId, IReadOnlyCollection<Guid> employeeIds)
    {
        return await _db.Employees.AsNoTracking()
            .Where(e => e.TenantId == tenantId && employeeIds.Contains(e.Id))
            .OrderBy(e => e.EmployeeNumber)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Guid>> GetDirectReportEmployeeIdsAsync(Guid tenantId, Guid managerEmployeeId)
    {
        return await _db.EmployeeHierarchyClosures.AsNoTracking()
            .Where(c => c.TenantId == tenantId && c.ManagerEmployeeId == managerEmployeeId && c.Depth == 1)
            .Select(c => c.ReportEmployeeId)
            .ToListAsync();
    }

    public async Task<Employee?> GetByWorkEmailAsync(Guid tenantId, string workEmail)
    {
        return await _db.Employees
            .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.WorkEmail == workEmail);
    }

    public async Task<Employee?> GetByEmployeeNumberAsync(Guid tenantId, string employeeNumber)
    {
        return await _db.Employees
            .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.EmployeeNumber == employeeNumber);
    }

    public async Task<int> CountOnboardingAndActiveAsync(Guid tenantId)
    {
        return await _db.Employees
            .CountAsync(e => e.TenantId == tenantId && (e.Status == "onboarding" || e.Status == "active"));
    }

    public async Task AddAsync(Employee employee)
    {
        await _db.Employees.AddAsync(employee);
    }

    public async Task AddLifecycleEventAsync(EmployeeLifecycleEvent lifecycleEvent)
    {
        await _db.EmployeeLifecycleEvents.AddAsync(lifecycleEvent);
    }

    public async Task AddChecklistTaskAsync(EmployeeChecklistTask task)
    {
        await _db.EmployeeChecklistTasks.AddAsync(task);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
