namespace OnevoHr.Api.Models.Leave;

public class LeaveBalance
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public decimal EntitledDays { get; set; }
    public decimal UsedDays { get; set; }
    public decimal RemainingDays { get; set; }
    public int Year { get; set; }

    public LeaveType? LeaveType { get; set; }
}
