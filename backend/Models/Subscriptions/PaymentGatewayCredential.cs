namespace OnevoHr.Api.Models.Subscriptions;

public class PaymentGatewayCredential
{
    public Guid Id { get; set; }
    public Guid PaymentGatewayConfigId { get; set; }
    public byte[] SecretEncrypted { get; set; } = Array.Empty<byte>();
    public byte[]? WebhookSecretEncrypted { get; set; }
    public string EncryptionKeyVersion { get; set; } = string.Empty;
    public int CredentialVersion { get; set; }
    public bool IsActive { get; set; }
    public Guid RotatedById { get; set; }
    public DateTime RotatedAtUtc { get; set; }
    public Guid? DeactivatedById { get; set; }
    public DateTime? DeactivatedAtUtc { get; set; }

    public PaymentGatewayConfig? PaymentGatewayConfig { get; set; }
}
