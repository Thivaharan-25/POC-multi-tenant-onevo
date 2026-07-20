namespace OnevoHr.Api.Models.Agents;

// Minimal, purpose-built clock state for the tray app (see
// docs/superpowers/specs/2026-07-09-clock-in-app-usage-tracking-design.md).
// Deliberately NOT the schedule-aware AttendanceRecord/PresenceSession module -
// this is just the on/off gate that controls whether the agent is allowed to
// upload activity telemetry right now.
public class AgentClockState
{
    public Guid Id { get; set; }
    public Guid RegisteredAgentId { get; set; }
    public Guid TenantId { get; set; }
    public Guid? EmployeeId { get; set; }
    public bool IsClockedIn { get; set; }
    public DateTimeOffset? ClockedInAt { get; set; }
    public DateTimeOffset? ClockedOutAt { get; set; }
}
