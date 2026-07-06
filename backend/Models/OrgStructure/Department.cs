namespace OnevoHr.Api.Models.OrgStructure;

public class Department
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LegalEntityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? ParentDepartmentId { get; set; }
    public Guid? HeadPositionId { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreatedAtUtc { get; set; }

    public LegalEntity? LegalEntity { get; set; }
    public Department? ParentDepartment { get; set; }
    public Position? HeadPosition { get; set; }
    public ICollection<Position> Positions { get; set; } = new List<Position>();
}
