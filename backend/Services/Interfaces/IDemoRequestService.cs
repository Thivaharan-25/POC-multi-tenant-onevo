using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.DTOs.Demo;

namespace OnevoHr.Api.Services.Interfaces;

public interface IDemoRequestService
{
    Task<DemoRequestDto> CreateAsync(CreateDemoRequestDto request);
    Task<List<DemoRequestDto>> GetAllAsync();
    Task<DemoRequestDto?> GetByIdAsync(Guid id);
}
