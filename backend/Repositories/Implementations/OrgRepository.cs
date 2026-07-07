using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.OrgStructure;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class OrgRepository : IOrgRepository
{
    private readonly AppDbContext _db;

    public OrgRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<LegalEntity>> GetLegalEntitiesAsync(Guid tenantId)
    {
        return await _db.LegalEntities.AsNoTracking()
            .Where(l => l.TenantId == tenantId)
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<LegalEntity?> GetLegalEntityByIdAsync(Guid id)
    {
        return await _db.LegalEntities
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Department>> GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId)
    {
        var query = _db.Departments.AsNoTracking()
            .Where(d => d.TenantId == tenantId);

        if (legalEntityId.HasValue)
        {
            query = query.Where(d => d.LegalEntityId == legalEntityId.Value);
        }

        return await query
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(Guid id)
    {
        return await _db.Departments
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Position>> GetPositionsAsync(Guid tenantId)
    {
        return await _db.Positions.AsNoTracking()
            .Include(p => p.PositionAssignments)
            .Where(p => p.TenantId == tenantId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Position?> GetPositionByIdAsync(Guid id)
    {
        return await _db.Positions
            .Include(p => p.PositionAssignments)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<PositionAssignment>> GetActiveAssignmentsForEmployeeAsync(Guid tenantId, Guid employeeId)
    {
        return await _db.PositionAssignments.AsNoTracking()
            .Include(a => a.Position)
            .Where(a => a.TenantId == tenantId && a.EmployeeId == employeeId && a.EndsAtUtc == null)
            .ToListAsync();
    }

    public async Task<List<PositionAssignment>> GetActiveAssignmentsForPositionAsync(Guid positionId)
    {
        return await _db.PositionAssignments.AsNoTracking()
            .Where(a => a.PositionId == positionId && a.EndsAtUtc == null)
            .ToListAsync();
    }

    public async Task AddLegalEntityAsync(LegalEntity legalEntity)
    {
        await _db.LegalEntities.AddAsync(legalEntity);
    }

    public async Task AddDepartmentAsync(Department department)
    {
        await _db.Departments.AddAsync(department);
    }

    public async Task AddPositionAsync(Position position)
    {
        await _db.Positions.AddAsync(position);
    }

    public async Task AddPositionAssignmentAsync(PositionAssignment assignment)
    {
        await _db.PositionAssignments.AddAsync(assignment);
    }

    public async Task AddPositionReportingHistoryAsync(PositionReportingHistory history)
    {
        await _db.PositionReportingHistories.AddAsync(history);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
