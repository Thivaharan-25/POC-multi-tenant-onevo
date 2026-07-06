namespace OnevoHr.Api.Models.Subscriptions;

public class PaymentGatewayConfig
{
    public Guid Id { get; set; }
    public string GatewayKey { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? PublicKey { get; set; }
    public string? MerchantId { get; set; }
    public string WebhookUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
