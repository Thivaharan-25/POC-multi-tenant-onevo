using OnevoHr.Api.DTOs.Devices;

namespace OnevoHr.Api.Services.Interfaces;

public interface IDevicePairingService
{
    Task<PairResponseDto> StartPairingAsync(string deviceName);
    Task<PairStatusResponseDto?> GetStatusAsync(string deviceCode);
    Task<PairConfirmInfoDto?> GetPairingInfoForConfirmationAsync(string userCode);
    Task<bool> ConfirmAsync(string userCode, Guid tenantId, Guid userId, Guid? employeeId);
    Task<bool> DeclineAsync(string userCode);
    Task<ConsentResponseDto?> AcceptConsentAsync(string deviceCode);
    Task<bool> CancelAsync(string deviceCode);
}
