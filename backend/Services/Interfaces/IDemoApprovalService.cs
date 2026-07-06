using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.DTOs.Demo;

namespace OnevoHr.Api.Services.Interfaces;

public interface IDemoApprovalService
{
    Task<TenantSummaryDto?> ApproveAsync(Guid demoRequestId, ApproveDemoRequestDto request, Guid reviewedByPlatformUserId);
    Task<bool> RejectAsync(Guid demoRequestId, RejectDemoRequestDto request, Guid reviewedByPlatformUserId);
}
