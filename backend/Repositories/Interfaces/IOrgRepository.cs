using OnevoHr.Api.Models.OrgStructure;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IOrgRepository
{
    Task<List<LegalEntity>> GetLegalEntitiesAsync(Guid tenantId);
    Task<LegalEntity?> GetLegalEntityByIdAsync(Guid id);
    Task<List<Department>> GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId);
    Task<Department?> GetDepartmentByIdAsync(Guid id);
    Task<List<Position>> GetPositionsAsync(Guid tenantId);
    Task<Position?> GetPositionByIdAsync(Guid id);
    Task<List<PositionAssignment>> GetActiveAssignmentsForEmployeeAsync(Guid tenantId, Guid employeeId);
    Task<List<PositionAssignment>> GetActiveAssignmentsForPositionAsync(Guid positionId);
    Task AddLegalEntityAsync(LegalEntity legalEntity);
    Task AddDepartmentAsync(Department department);
    Task AddPositionAsync(Position position);
    Task AddPositionAssignmentAsync(PositionAssignment assignment);
    Task AddPositionReportingHistoryAsync(PositionReportingHistory history);
    Task SaveChangesAsync();
}
