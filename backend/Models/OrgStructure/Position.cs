using OnevoHr.Api.Models.Auth;

namespace OnevoHr.Api.Models.OrgStructure;

public class Position
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LegalEntityId { get; set; }
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? ReportsToPositionId { get; set; }
    public int Capacity { get; set; } = 1;

    /// <summary>unique, pooled</summary>
    public string PositionType { get; set; } = "unique";

    public string Status { get; set; } = "active";
    public Guid? SuggestedRoleId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public LegalEntity? LegalEntity { get; set; }
    public Department? Department { get; set; }
    public Position? ReportsToPosition { get; set; }
    public ICollection<PositionAssignment> PositionAssignments { get; set; } = new List<PositionAssignment>();
}
