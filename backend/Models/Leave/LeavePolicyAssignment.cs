namespace OnevoHr.Api.Models.Leave;

public class LeavePolicyAssignment
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LeavePolicyId { get; set; }

    /// <summary>tenant, department, position</summary>
    public string AssignmentScope { get; set; } = "tenant";

    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }

    public LeavePolicy? LeavePolicy { get; set; }
}
