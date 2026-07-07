using OnevoHr.Api.DTOs.OrgStructure;
using OnevoHr.Api.Models.OrgStructure;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class OrgStructureService : IOrgStructureService
{
    private readonly IOrgRepository _org;
    private readonly IEmployeeRepository _employees;

    public OrgStructureService(IOrgRepository org, IEmployeeRepository employees)
    {
        _org = org;
        _employees = employees;
    }

    public async Task<List<LegalEntityDto>> GetLegalEntitiesAsync(Guid tenantId)
    {
        var entities = await _org.GetLegalEntitiesAsync(tenantId);
        return entities.Select(l => new LegalEntityDto(l.Id, l.Name, l.Code, l.Status)).ToList();
    }

    public async Task<List<DepartmentDto>> GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId)
    {
        var departments = await _org.GetDepartmentsAsync(tenantId, legalEntityId);
        return departments.Select(d => new DepartmentDto(
            d.Id, d.LegalEntityId, d.Name, d.Code, d.ParentDepartmentId, d.HeadPositionId, d.Status)).ToList();
    }

    public async Task<List<PositionDto>> GetPositionsAsync(Guid tenantId, Guid? legalEntityId, Guid? departmentId)
    {
        var positions = await _org.GetPositionsAsync(tenantId, legalEntityId, departmentId);
        return positions.Select(Map).ToList();
    }

    public async Task<PositionDto?> CreatePositionAsync(Guid tenantId, CreatePositionRequestDto request)
    {
        var department = await _org.GetDepartmentByIdAsync(request.DepartmentId);
        if (department is null || department.TenantId != tenantId || department.LegalEntityId != request.LegalEntityId)
        {
            return null;
        }

        if (request.ReportsToPositionId is Guid reportsToId)
        {
            var manager = await _org.GetPositionByIdAsync(reportsToId);
            // A position can only report to a non-pooled position in the same legal entity.
            if (manager is null || manager.TenantId != tenantId ||
                manager.LegalEntityId != request.LegalEntityId ||
                manager.PositionType == "pooled")
            {
                return null;
            }
        }

        var now = DateTime.UtcNow;
        var position = new Position
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            LegalEntityId = request.LegalEntityId,
            DepartmentId = request.DepartmentId,
            Name = request.Name,
            Code = request.Code,
            ReportsToPositionId = request.ReportsToPositionId,
            Capacity = request.Capacity,
            PositionType = request.PositionType,
            Status = "active",
            CreatedAtUtc = now
        };
        await _org.AddPositionAsync(position);

        await _org.AddPositionReportingHistoryAsync(new PositionReportingHistory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            PositionId = position.Id,
            ReportsToPositionId = request.ReportsToPositionId,
            EffectiveFromUtc = now
        });

        await _org.SaveChangesAsync();
        return Map(position);
    }

    public async Task<ReportingManagerDto?> GetReportingManagerAsync(Guid tenantId, Guid positionId)
    {
        var position = await _org.GetPositionByIdAsync(positionId);
        if (position == null || position.TenantId != tenantId || position.ReportsToPositionId is not Guid managerPositionId)
        {
            return null;
        }

        var assignments = await _org.GetActiveAssignmentsForPositionAsync(managerPositionId);
        var assignment = assignments.FirstOrDefault(a => a.IsPrimary) ?? assignments.FirstOrDefault();
        if (assignment == null)
        {
            return null;
        }

        var employee = await _employees.GetByIdAsync(assignment.EmployeeId);
        if (employee == null || employee.TenantId != tenantId)
        {
            return null;
        }

        return new ReportingManagerDto(employee.Id, employee.FirstName + " " + employee.LastName);
    }

    private static PositionDto Map(Position p)
    {
        return new PositionDto(
            p.Id, p.LegalEntityId, p.DepartmentId, p.Name, p.Code,
            p.ReportsToPositionId, p.Capacity, p.PositionType, p.Status);
    }
}
