using OnevoHr.Api.DTOs.Agents;

namespace OnevoHr.Api.Services.Interfaces;

// Docs-aligned device enrollment (2026-07-11-agent-service-split-jwt-design.md).
// The browser-confirm half is unchanged and still handled by IDevicePairingService
// (keyed by the user_code); this service owns the client-facing start/status/complete
// half and issues the JWT device credential at completion.
public interface IAgentEnrollmentService
{
    Task<EnrollStartResponseDto> StartAsync(EnrollStartRequestDto request);

    Task<EnrollStatusResponseDto?> GetStatusAsync(string enrollmentId);

    Task<EnrollCompleteResponseDto?> CompleteAsync(EnrollCompleteRequestDto request);
}
