namespace OnevoHr.Api.Models.Leave;

public class LeavePolicy
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public decimal EntitlementDays { get; set; }
    public bool CarryForwardAllowed { get; set; }
    public decimal CarryForwardLimit { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public LeaveType? LeaveType { get; set; }
}
