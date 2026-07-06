namespace OnevoHr.Api.Models.Subscriptions;

public class RuntimeFeatureFlag
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string FeatureKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
