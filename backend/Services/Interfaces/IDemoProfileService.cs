using OnevoHr.Api.DTOs.Admin;

namespace OnevoHr.Api.Services.Interfaces;

public interface IDemoProfileService
{
    Task<List<DemoProfileDto>> GetAllAsync();
    Task<DemoProfileDto?> GetByIdAsync(Guid id);
}
