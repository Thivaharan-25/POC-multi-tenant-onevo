namespace OnevoHr.Api.Models.Leave;

public class LeaveType
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsActive { get; set; }
}
