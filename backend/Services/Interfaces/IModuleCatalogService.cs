using OnevoHr.Api.DTOs.Admin;

namespace OnevoHr.Api.Services.Interfaces;

public interface IModuleCatalogService
{
    Task<List<ModuleCatalogDto>> GetModulesAsync();
}
