using OnevoHr.Api.Models.Employees;

namespace OnevoHr.Api.Models.OrgStructure;

public class PositionAssignment
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PositionId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
    public bool IsPrimary { get; set; }

    public Position? Position { get; set; }
    public Employee? Employee { get; set; }
}
