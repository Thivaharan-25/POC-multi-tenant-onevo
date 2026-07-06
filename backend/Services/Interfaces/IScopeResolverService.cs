using OnevoHr.Api.DTOs.Auth;

namespace OnevoHr.Api.Services.Interfaces;

public interface IScopeResolverService
{
    Task<ScopeResolutionDto> ResolveScopeAsync(Guid tenantId, Guid userId);
}
