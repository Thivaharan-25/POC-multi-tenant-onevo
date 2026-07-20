namespace OnevoHr.Api.DTOs.Agents;

public sealed record ClockStateResponseDto(
    bool IsClockedIn,
    DateTimeOffset? ClockedInAt,
    DateTimeOffset? ClockedOutAt);

public sealed record AppUsageSampleDto(
    string ApplicationName,
    string ProcessName,
    string WindowTitleHash,
    DateOnly Date,
    int Seconds);

public sealed record AppUsageBatchRequestDto(List<AppUsageSampleDto> Samples);
