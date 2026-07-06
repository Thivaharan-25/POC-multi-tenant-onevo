namespace OnevoHr.Api.Models.Subscriptions;

public class PaymentGatewayCountryRoute
{
    public Guid Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryNameSnapshot { get; set; } = string.Empty;
    public Guid GatewayConfigId { get; set; }
    public string Environment { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public PaymentGatewayConfig? PaymentGatewayConfig { get; set; }
}
