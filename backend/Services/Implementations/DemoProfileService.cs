using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Models.Demo;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class DemoProfileService : IDemoProfileService
{
    private readonly IDemoProfileRepository _demoProfiles;

    public DemoProfileService(IDemoProfileRepository demoProfiles)
    {
        _demoProfiles = demoProfiles;
    }

    public async Task<List<DemoProfileDto>> GetAllAsync()
    {
        var profiles = await _demoProfiles.GetAllAsync();
        return profiles.Select(Map).ToList();
    }

    public async Task<DemoProfileDto?> GetByIdAsync(Guid id)
    {
        var profile = await _demoProfiles.GetByIdAsync(id);
        return profile is null ? null : Map(profile);
    }

    private static DemoProfileDto Map(DemoProfile p)
    {
        return new DemoProfileDto(
            p.Id, p.Name, p.Description, p.TrialDurationDays, p.AutoExpire,
            p.MaxEmployees, p.DemoStorageLimitGb, p.DemoAiTokenLimit, p.IsActive);
    }
}
