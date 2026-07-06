namespace OnevoHr.Api.Models.Subscriptions;

public class SubscriptionInvoice
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid TenantSubscriptionId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>draft, open, paid, void, failed</summary>
    public string Status { get; set; } = "draft";

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? DueAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public TenantSubscription? TenantSubscription { get; set; }
}
