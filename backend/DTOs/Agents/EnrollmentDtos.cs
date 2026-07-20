using System.Text.Json.Serialization;

namespace OnevoHr.Api.DTOs.Agents;

// Docs-aligned enrollment contract (2026-07-11-agent-service-split-jwt-design.md).
// JSON uses snake_case to match the agent-server-protocol docs; the rest of the
// API stays camelCase, so these opt in explicitly via [JsonPropertyName].

public sealed record EnrollStartRequestDto(
    [property: JsonPropertyName("device_id")] string DeviceId,
    [property: JsonPropertyName("device_name")] string DeviceName,
    [property: JsonPropertyName("os_version")] string OsVersion,
    [property: JsonPropertyName("agent_version")] string AgentVersion,
    [property: JsonPropertyName("enrollment_method")] string EnrollmentMethod);

public sealed record EnrollStartResponseDto(
    [property: JsonPropertyName("enrollment_id")] string EnrollmentId,
    [property: JsonPropertyName("user_code")] string UserCode,
    [property: JsonPropertyName("auth_url")] string AuthUrl,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt,
    [property: JsonPropertyName("poll_interval_seconds")] int PollIntervalSeconds);

public sealed record EnrollStatusResponseDto(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("employee_name")] string? EmployeeName,
    [property: JsonPropertyName("tenant_name")] string? TenantName,
    [property: JsonPropertyName("authorization_code")] string? AuthorizationCode);

public sealed record EnrollCompleteRequestDto(
    [property: JsonPropertyName("enrollment_id")] string EnrollmentId,
    [property: JsonPropertyName("device_id")] string DeviceId,
    [property: JsonPropertyName("authorization_code")] string AuthorizationCode);

public sealed record EnrollCompleteResponseDto(
    [property: JsonPropertyName("agent_id")] Guid AgentId,
    [property: JsonPropertyName("tenant_id")] Guid TenantId,
    [property: JsonPropertyName("employee_id")] Guid? EmployeeId,
    [property: JsonPropertyName("employee_name")] string EmployeeName,
    [property: JsonPropertyName("device_token")] string DeviceToken,
    [property: JsonPropertyName("token_expires_at")] DateTimeOffset TokenExpiresAt,
    [property: JsonPropertyName("policy")] object Policy);
