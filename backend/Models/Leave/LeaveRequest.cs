namespace OnevoHr.Api.Models.Leave;

public class LeaveRequest
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    /// <summary>draft, submitted, approved, rejected, cancelled</summary>
    public string Status { get; set; } = "draft";

    /// <summary>jsonb</summary>
    public string? ConflictSnapshotJson { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public LeaveType? LeaveType { get; set; }
}
