namespace OnevoHr.Api.Models.Calendar;

public class ExternalCalendarEventLink
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CalendarEventId { get; set; }
    public Guid ExternalCalendarConnectionId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ExternalCalendarId { get; set; } = string.Empty;
    public string ExternalEventId { get; set; } = string.Empty;
    public string ExternalEtag { get; set; } = string.Empty;
    public string SyncDirection { get; set; } = "inbound";
    public string SyncStatus { get; set; } = "synced";
    public DateTime? LastSyncedAtUtc { get; set; }
    public string? LastError { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Tenant.Tenant? Tenant { get; set; }
    public CalendarEvent? CalendarEvent { get; set; }
    public ExternalCalendarConnection? ExternalCalendarConnection { get; set; }
}
