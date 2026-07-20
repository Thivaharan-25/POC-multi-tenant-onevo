namespace OnevoHr.Api.Models.Agents;

public enum AgentPairingStatus
{
    PendingConfirmation,
    Confirmed,
    Enrolled,
    Expired,
    Cancelled
}

// Transient device-pairing handshake for the tray app (see
// docs/superpowers/specs/2026-07-09-device-pairing-flow-design.md).
// TenantId/EmployeeId are null until the browser-side confirm step binds them.
// Once consent is accepted, RegisteredAgentId points at the resulting row in
// the existing registered_agents table and this row is no longer read.
public class AgentPairingRequest
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? EmployeeId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string UserCodeHash { get; set; } = string.Empty;
    public string DeviceCodeHash { get; set; } = string.Empty;
    public AgentPairingStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public DateTimeOffset? ConsentAcceptedAt { get; set; }
    public Guid? RegisteredAgentId { get; set; }
    public int FailedConfirmAttempts { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }

    // Added for the docs-aligned /enroll flow (2026-07-11-agent-service-split-jwt-design.md).
    // The device_id the agent Service generates locally and sends in enroll/start;
    // it becomes RegisteredAgent.DeviceId and the JWT device_id claim at completion.
    public Guid? ClientDeviceId { get; set; }

    // One-time authorization_code, returned to the polling client only after the
    // browser confirms, and required by enroll/complete. Stored hashed.
    public string? AuthorizationCodeHash { get; set; }
}
