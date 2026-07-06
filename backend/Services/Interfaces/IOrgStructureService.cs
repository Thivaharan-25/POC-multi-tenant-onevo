using OnevoHr.Api.DTOs.OrgStructure;

namespace OnevoHr.Api.Services.Interfaces;

public interface IOrgStructureService
{
    Task<List<LegalEntityDto>> GetLegalEntitiesAsync(Guid tenantId);
    Task<List<DepartmentDto>> GetDepartmentsAsync(Guid tenantId);
    Task<List<PositionDto>> GetPositionsAsync(Guid tenantId);
    Task<PositionDto?> CreatePositionAsync(Guid tenantId, CreatePositionRequestDto request);
}
