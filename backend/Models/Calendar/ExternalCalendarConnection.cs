using OnevoHr.Api.Models.Auth;

namespace OnevoHr.Api.Models.Calendar;

public class ExternalCalendarConnection
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ExternalAccountEmail { get; set; } = string.Empty;
    public string? ExternalCalendarId { get; set; }
    public string? ExternalCalendarName { get; set; }
    public byte[]? AccessTokenEncrypted { get; set; }
    public byte[] RefreshTokenEncrypted { get; set; } = Array.Empty<byte>();
    public string ScopesJson { get; set; } = "[]";
    public string SyncDirection { get; set; } = "disabled";
    public string Status { get; set; } = "active";
    public byte[]? SyncTokenEncrypted { get; set; }
    public byte[]? DeltaLinkEncrypted { get; set; }
    public int FailureCount { get; set; }
    public DateTime? LastSyncedAtUtc { get; set; }
    public DateTime? LastSuccessfulSyncAtUtc { get; set; }
    public string? LastError { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Tenant.Tenant? Tenant { get; set; }
    public User? User { get; set; }
}
