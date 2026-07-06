namespace OnevoHr.Api.Models.OrgStructure;

public class PositionReportingHistory
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PositionId { get; set; }
    public Guid? ReportsToPositionId { get; set; }
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }

    public Position? Position { get; set; }
}
