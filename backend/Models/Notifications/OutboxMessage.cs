namespace OnevoHr.Api.Models.Notifications;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Type { get; set; } = string.Empty;

    /// <summary>jsonb</summary>
    public string PayloadJson { get; set; } = "{}";

    /// <summary>pending, published, failed</summary>
    public string Status { get; set; } = "pending";

    public int RetryCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
}
