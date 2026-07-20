namespace OnevoHr.Api.DTOs.Devices;

public sealed record PairRequestDto(string DeviceName);

public sealed record DeviceCodeRequestDto(string DeviceCode);

public sealed record UserCodeRequestDto(string UserCode);

public sealed record PairResponseDto(
    string UserCode,
    string DeviceCode,
    int ExpiresInSeconds,
    int PollIntervalSeconds);

public sealed record PairStatusResponseDto(
    string Status,
    string? EmployeeName,
    string? TenantName);

public sealed record PairConfirmInfoDto(
    string DeviceName,
    DateTimeOffset RequestedAt);

public sealed record ConsentResponseDto(
    string DeviceToken,
    string EmployeeName,
    string DeviceName,
    string TenantName);
