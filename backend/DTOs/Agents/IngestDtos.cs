using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnevoHr.Api.DTOs.Agents;

// Docs-aligned batch ingest contract (agent-server-protocol.md).
// Only batch items with type "app_usage" are handled today; other documented
// types (meeting, screenshot_capture, ...) are accepted-but-ignored for now.

public sealed record IngestRequestDto(
    [property: JsonPropertyName("device_id")] string? DeviceId,
    [property: JsonPropertyName("employee_id")] string? EmployeeId,
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("batch")] List<IngestItemDto> Batch);

public sealed record IngestItemDto(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("data")] JsonElement Data);

public sealed record AppUsageDataDto(
    [property: JsonPropertyName("application_name")] string ApplicationName,
    [property: JsonPropertyName("process_name")] string ProcessName,
    [property: JsonPropertyName("window_title_hash")] string WindowTitleHash,
    [property: JsonPropertyName("date")] DateOnly Date,
    [property: JsonPropertyName("seconds")] int Seconds);
