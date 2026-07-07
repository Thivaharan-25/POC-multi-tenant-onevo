using OnevoHr.Api.DTOs.OrgStructure;

namespace OnevoHr.Api.Services.Interfaces;

public interface IOrgStructureService
{
    Task<List<LegalEntityDto>> GetLegalEntitiesAsync(Guid tenantId);
    Task<List<DepartmentDto>> GetDepartmentsAsync(Guid tenantId, Guid? legalEntityId);
    Task<List<PositionDto>> GetPositionsAsync(Guid tenantId, Guid? legalEntityId, Guid? departmentId);
    Task<PositionDto?> CreatePositionAsync(Guid tenantId, CreatePositionRequestDto request);
}
