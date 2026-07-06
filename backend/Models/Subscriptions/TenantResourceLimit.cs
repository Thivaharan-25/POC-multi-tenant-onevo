namespace OnevoHr.Api.Models.Subscriptions;

public class TenantResourceLimit
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public int StorageLimitGb { get; set; }
    public long AiTokenLimit { get; set; }
    public int? EmployeeLimit { get; set; }

    /// <summary>demo_profile, paid_plan, override</summary>
    public string Source { get; set; } = "demo_profile";

    public DateTime CreatedAtUtc { get; set; }
}
