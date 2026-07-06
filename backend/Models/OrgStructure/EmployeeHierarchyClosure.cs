namespace OnevoHr.Api.Models.OrgStructure;

public class EmployeeHierarchyClosure
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ManagerEmployeeId { get; set; }
    public Guid ReportEmployeeId { get; set; }
    public int Depth { get; set; }
}
