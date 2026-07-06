using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.DTOs.Demo;
using OnevoHr.Api.Models.DeveloperPlatform;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class DemoRequestService : IDemoRequestService
{
    private readonly IDemoRequestRepository _demoRequests;

    public DemoRequestService(IDemoRequestRepository demoRequests)
    {
        _demoRequests = demoRequests;
    }

    public async Task<DemoRequestDto> CreateAsync(CreateDemoRequestDto request)
    {
        var entity = new DemoRequest
        {
            Id = Guid.NewGuid(),
            CompanyName = request.CompanyName,
            RequesterName = request.RequesterName,
            RequesterEmail = request.RequesterEmail,
            CountryCode = request.CountryCode,
            RequestedCompanySizeRange = request.RequestedCompanySizeRange,
            Status = "submitted",
            Source = "landing_demo_form",
            CreatedAtUtc = DateTime.UtcNow
        };

        await _demoRequests.AddAsync(entity);
        await _demoRequests.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<List<DemoRequestDto>> GetAllAsync()
    {
        var requests = await _demoRequests.GetAllAsync();
        return requests.Select(Map).ToList();
    }

    public async Task<DemoRequestDto?> GetByIdAsync(Guid id)
    {
        var request = await _demoRequests.GetByIdAsync(id);
        return request is null ? null : Map(request);
    }

    private static DemoRequestDto Map(DemoRequest r)
    {
        return new DemoRequestDto(
            r.Id, r.CompanyName, r.RequesterName, r.RequesterEmail, r.CountryCode,
            r.RequestedCompanySizeRange, r.Status, r.Source, r.CreatedAtUtc, r.CreatedTenantId);
    }
}
