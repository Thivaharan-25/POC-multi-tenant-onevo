using OnevoHr.Api.Models.Templates;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface ITemplateRepository
{
    Task<List<ConfigurationTemplate>> GetConfigurationTemplatesAsync();
    Task<ConfigurationTemplate?> GetConfigurationTemplateByIdAsync(Guid id);
    Task<ConfigurationTemplate?> FindByTypeAndEmployeeCountAsync(string templateType, int employeeCount);
    Task<List<RoleTemplate>> GetRoleTemplatesAsync();
    Task<RoleTemplate?> GetRoleTemplateByIdAsync(Guid id);
    Task AddApplicationAsync(TenantConfigurationTemplateApplication application);
    Task SaveChangesAsync();
}
