namespace OnevoHr.Api.Models.Employees;

public class EmployeeAssignmentHistory
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    public string? Reason { get; set; }

    public Employee? Employee { get; set; }
}
