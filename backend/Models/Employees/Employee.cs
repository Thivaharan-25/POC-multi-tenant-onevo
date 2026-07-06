using OnevoHr.Api.Models.OrgStructure;

namespace OnevoHr.Api.Models.Employees;

public class Employee
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string WorkEmail { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public DateOnly HireDate { get; set; }
    public Guid LegalEntityId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CurrentPositionId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Tenant.Tenant? Tenant { get; set; }
    public LegalEntity? LegalEntity { get; set; }
    public ICollection<PositionAssignment> PositionAssignments { get; set; } = new List<PositionAssignment>();
}
