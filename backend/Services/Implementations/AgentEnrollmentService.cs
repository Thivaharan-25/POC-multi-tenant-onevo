using System.Security.Cryptography;
using OnevoHr.Api.DTOs.Agents;
using OnevoHr.Api.Models.Agents;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class AgentEnrollmentService : IAgentEnrollmentService
{
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
    private const int PollIntervalSeconds = 3;
    private const string UserCodeAlphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    private readonly IAgentPairingRepository _pairingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ITokenService _tokenService;
    private readonly IRlsBypassContext _rlsBypassContext;
    private readonly IConfiguration _configuration;

    public AgentEnrollmentService(
        IAgentPairingRepository pairingRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        ITokenService tokenService,
        IRlsBypassContext rlsBypassContext,
        IConfiguration configuration)
    {
        _pairingRepository = pairingRepository;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _tokenService = tokenService;
        _rlsBypassContext = rlsBypassContext;
        _configuration = configuration;
    }

    public async Task<EnrollStartResponseDto> StartAsync(EnrollStartRequestDto request)
    {
        Guid.TryParse(request.DeviceId, out var clientDeviceId);

        var userCode = GenerateUserCode();
        var enrollmentId = AuthService.GenerateToken();
        var now = DateTimeOffset.UtcNow;

        var pairing = new AgentPairingRequest
        {
            Id = Guid.NewGuid(),
            DeviceName = request.DeviceName,
            UserCodeHash = AuthService.HashToken(userCode),
            DeviceCodeHash = AuthService.HashToken(enrollmentId),
            ClientDeviceId = clientDeviceId == Guid.Empty ? null : clientDeviceId,
            Status = AgentPairingStatus.PendingConfirmation,
            CreatedAt = now,
            ExpiresAt = now.Add(CodeLifetime)
        };

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            await _pairingRepository.AddAsync(pairing);
            await _pairingRepository.SaveChangesAsync();
        }

        var frontendBaseUrl = _configuration["Email:FrontendBaseUrl"] ?? "http://localhost:4200";
        var authUrl = $"{frontendBaseUrl}/devices/confirm?code={Uri.EscapeDataString(userCode)}";

        return new EnrollStartResponseDto(
            enrollmentId,
            userCode,
            authUrl,
            pairing.ExpiresAt,
            PollIntervalSeconds);
    }

    public async Task<EnrollStatusResponseDto?> GetStatusAsync(string enrollmentId)
    {
        var hash = AuthService.HashToken(enrollmentId);

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            var pairing = await _pairingRepository.GetByDeviceCodeHashAsync(hash);
            if (pairing is null)
            {
                return null;
            }

            pairing = await ExpireIfPastDeadlineAsync(pairing);
            pairing.LastSeenAt = DateTimeOffset.UtcNow;

            string? employeeName = null;
            string? tenantName = null;
            string? authorizationCode = null;

            if (pairing.Status == AgentPairingStatus.Confirmed && pairing.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(pairing.UserId.Value);
                employeeName = user?.DisplayName;

                if (pairing.TenantId.HasValue)
                {
                    var tenant = await _tenantRepository.GetByIdAsync(pairing.TenantId.Value);
                    tenantName = tenant?.Name;
                }

                // Issue the one-time authorization code on first confirmed poll.
                authorizationCode = AuthService.GenerateToken();
                pairing.AuthorizationCodeHash = AuthService.HashToken(authorizationCode);
            }

            await _pairingRepository.SaveChangesAsync();

            return new EnrollStatusResponseDto(pairing.Status.ToString(), employeeName, tenantName, authorizationCode);
        }
    }

    public async Task<EnrollCompleteResponseDto?> CompleteAsync(EnrollCompleteRequestDto request)
    {
        var hash = AuthService.HashToken(request.EnrollmentId);

        using (_rlsBypassContext.BeginTrustedRlsBypass())
        {
            var pairing = await _pairingRepository.GetByDeviceCodeHashAsync(hash);
            pairing = pairing is null ? null : await ExpireIfPastDeadlineAsync(pairing);

            if (pairing is null
                || pairing.Status != AgentPairingStatus.Confirmed
                || !pairing.TenantId.HasValue
                || !pairing.UserId.HasValue
                || pairing.AuthorizationCodeHash is null)
            {
                return null;
            }

            if (AuthService.HashToken(request.AuthorizationCode) != pairing.AuthorizationCodeHash)
            {
                return null;
            }

            Guid.TryParse(request.DeviceId, out var deviceId);
            if (deviceId == Guid.Empty)
            {
                deviceId = pairing.ClientDeviceId ?? Guid.NewGuid();
            }

            var now = DateTimeOffset.UtcNow;
            var registeredAgent = new RegisteredAgent
            {
                Id = Guid.NewGuid(),
                TenantId = pairing.TenantId.Value,
                EmployeeId = pairing.EmployeeId,
                DeviceId = deviceId,
                DeviceName = pairing.DeviceName,
                OsVersion = "Windows",
                AgentVersion = "1.0.0",
                RegisteredAt = now,
                LastHeartbeatAt = now,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            };

            await _pairingRepository.AddRegisteredAgentAsync(registeredAgent);

            pairing.Status = AgentPairingStatus.Enrolled;
            pairing.ConsentAcceptedAt = now;
            pairing.RegisteredAgentId = registeredAgent.Id;
            await _pairingRepository.SaveChangesAsync();

            var deviceToken = _tokenService.GenerateDeviceToken(deviceId, pairing.TenantId.Value, out var tokenExpiresAt);

            var user = await _userRepository.GetByIdAsync(pairing.UserId.Value);
            var tenant = await _tenantRepository.GetByIdAsync(pairing.TenantId.Value);

            var policy = new Dictionary<string, object>
            {
                ["monitoring_enabled"] = false,
                ["screenshot_capture"] = false
            };

            return new EnrollCompleteResponseDto(
                registeredAgent.Id,
                pairing.TenantId.Value,
                pairing.EmployeeId,
                user?.DisplayName ?? string.Empty,
                deviceToken,
                tokenExpiresAt,
                policy);
        }
    }

    private async Task<AgentPairingRequest> ExpireIfPastDeadlineAsync(AgentPairingRequest pairing)
    {
        if (pairing.Status == AgentPairingStatus.PendingConfirmation && pairing.ExpiresAt < DateTimeOffset.UtcNow)
        {
            pairing.Status = AgentPairingStatus.Expired;
            await _pairingRepository.SaveChangesAsync();
        }

        return pairing;
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
