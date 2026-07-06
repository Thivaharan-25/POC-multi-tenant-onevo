using OnevoHr.Api.DTOs.Admin;

namespace OnevoHr.Api.Services.Interfaces;

public interface ITemplateApplicationService
{
    Task<List<ConfigurationTemplateDto>> GetConfigurationTemplatesAsync();
    Task<List<RoleTemplateDto>> GetRoleTemplatesAsync();
    Task<bool> ApplyConfigurationTemplateAsync(Guid tenantId, Guid configurationTemplateId, Guid? appliedByPlatformUserId);
    Task ApplyTemplatesForEmployeeCountAsync(Guid tenantId, int employeeCount, Guid? appliedByPlatformUserId);
}
