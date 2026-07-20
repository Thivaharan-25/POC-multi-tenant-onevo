using System.Security.Cryptography;
using OnevoHr.Api.DTOs.Devices;
using OnevoHr.Api.Models.Agents;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class DevicePairingService : IDevicePairingService
{
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
    private const int PollIntervalSeconds = 3;
    private const string UserCodeAlphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    private readonly IAgentPairingRepository _pairingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRlsBypassContext _rlsBypassContext;

    public DevicePairingService(
        IAgentPairingRepository pairingRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IRlsBypassContext rlsBypassContext)
    {
        _pairingRepository = pairingRepository;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _rlsBypassContext = rlsBypassContext;
    }

    public async Task<PairResponseDto> StartPairingAsync(string deviceName)
    {
        var userCode = GenerateUserCode();
        var deviceCode = AuthService.GenerateToken();
        var now = DateTimeOffset.UtcNow;

        var request = new AgentPairingRequest
        {
            Id = Guid.NewGuid(),
            DeviceName = deviceName,
            UserCodeHash = AuthService.HashToken(userCode),
            DeviceCodeHash = AuthService.HashToken(deviceCode),
            Status = AgentPairingStatus.PendingConfirmation,
            CreatedAt = now,
            ExpiresAt = now.Add(CodeLifetime)
        };

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            await _pairingRepository.AddAsync(request);
            await _pairingRepository.SaveChangesAsync();
        }

        return new PairResponseDto(userCode, deviceCode, (int)CodeLifetime.TotalSeconds, PollIntervalSeconds);
    }

    public async Task<PairStatusResponseDto?> GetStatusAsync(string deviceCode)
    {
        var hash = AuthService.HashToken(deviceCode);

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            var request = await _pairingRepository.GetByDeviceCodeHashAsync(hash);
            if (request is null)
            {
                return null;
            }

            request = await ExpireIfPastDeadlineAsync(request);
            request.LastSeenAt = DateTimeOffset.UtcNow;
            await _pairingRepository.SaveChangesAsync();

            string? employeeName = null;
            string? tenantName = null;

            if (request.Status == AgentPairingStatus.Confirmed && request.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId.Value);
                employeeName = user?.DisplayName;

                if (request.TenantId.HasValue)
                {
                    var tenant = await _tenantRepository.GetByIdAsync(request.TenantId.Value);
                    tenantName = tenant?.Name;
                }
            }

            return new PairStatusResponseDto(request.Status.ToString(), employeeName, tenantName);
        }
    }

    public async Task<PairConfirmInfoDto?> GetPairingInfoForConfirmationAsync(string userCode)
    {
        var hash = AuthService.HashToken(userCode);

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            var request = await _pairingRepository.GetByUserCodeHashAsync(hash);
            request = request is null ? null : await ExpireIfPastDeadlineAsync(request);

            if (request is null || request.Status != AgentPairingStatus.PendingConfirmation)
            {
                return null;
            }

            return new PairConfirmInfoDto(request.DeviceName, request.CreatedAt);
        }
    }

    public async Task<bool> ConfirmAsync(string userCode, Guid tenantId, Guid userId, Guid? employeeId)
    {
        var hash = AuthService.HashToken(userCode);

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            var request = await _pairingRepository.GetByUserCodeHashAsync(hash);
            request = request is null ? null : await ExpireIfPastDeadlineAsync(request);

            if (request is null || request.Status != AgentPairingStatus.PendingConfirmation)
            {
                return false;
            }

            request.TenantId = tenantId;
            request.UserId = userId;
            request.EmployeeId = employeeId;
            request.Status = AgentPairingStatus.Confirmed;
            request.ConfirmedAt = DateTimeOffset.UtcNow;
            await _pairingRepository.SaveChangesAsync();

            return true;
        }
    }

    public async Task<bool> DeclineAsync(string userCode)
    {
        var hash = AuthService.HashToken(userCode);

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            var request = await _pairingRepository.GetByUserCodeHashAsync(hash);
            if (request is null || request.Status is not (AgentPairingStatus.PendingConfirmation or AgentPairingStatus.Confirmed))
            {
                return false;
            }

            request.Status = AgentPairingStatus.Cancelled;
            await _pairingRepository.SaveChangesAsync();

            return true;
        }
    }

    public async Task<ConsentResponseDto?> AcceptConsentAsync(string deviceCode)
    {
        var hash = AuthService.HashToken(deviceCode);

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            var request = await _pairingRepository.GetByDeviceCodeHashAsync(hash);
            request = request is null ? null : await ExpireIfPastDeadlineAsync(request);

            if (request is null || request.Status != AgentPairingStatus.Confirmed
                || !request.TenantId.HasValue || !request.UserId.HasValue)
            {
                return null;
            }

            var deviceToken = AuthService.GenerateToken();
            var now = DateTimeOffset.UtcNow;

            var registeredAgent = new RegisteredAgent
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId.Value,
                EmployeeId = request.EmployeeId,
                DeviceId = Guid.NewGuid(),
                DeviceName = request.DeviceName,
                OsVersion = "Windows",
                AgentVersion = "1.0.0",
                RegisteredAt = now,
                LastHeartbeatAt = now,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now,
                DeviceTokenHash = AuthService.HashToken(deviceToken)
            };

            await _pairingRepository.AddRegisteredAgentAsync(registeredAgent);

            request.Status = AgentPairingStatus.Enrolled;
            request.ConsentAcceptedAt = now;
            request.RegisteredAgentId = registeredAgent.Id;
            await _pairingRepository.SaveChangesAsync();

            var user = await _userRepository.GetByIdAsync(request.UserId.Value);
            var tenant = await _tenantRepository.GetByIdAsync(request.TenantId.Value);

            return new ConsentResponseDto(
                deviceToken,
                user?.DisplayName ?? string.Empty,
                request.DeviceName,
                tenant?.Name ?? string.Empty);
        }
    }

    public async Task<bool> CancelAsync(string deviceCode)
    {
        var hash = AuthService.HashToken(deviceCode);

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            var request = await _pairingRepository.GetByDeviceCodeHashAsync(hash);
            if (request is null || request.Status is AgentPairingStatus.Enrolled or AgentPairingStatus.Cancelled)
            {
                return false;
            }

            request.Status = AgentPairingStatus.Cancelled;
            await _pairingRepository.SaveChangesAsync();

            return true;
        }
    }

    private async Task<AgentPairingRequest> ExpireIfPastDeadlineAsync(AgentPairingRequest request)
    {
        if (request.Status == AgentPairingStatus.PendingConfirmation && request.ExpiresAt < DateTimeOffset.UtcNow)
        {
            request.Status = AgentPairingStatus.Expired;
            await _pairingRepository.SaveChangesAsync();
        }

        return request;
    }

    private static string GenerateUserCode()
    {
        var chars = new char[8];
        var randomBytes = RandomNumberGenerator.GetBytes(8);

        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = UserCodeAlphabet[randomBytes[i] % UserCodeAlphabet.Length];
        }

        return $"{new string(chars, 0, 4)}-{new string(chars, 4, 4)}";
    }
}
