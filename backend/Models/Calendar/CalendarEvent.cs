namespace OnevoHr.Api.Models.Calendar;

public class CalendarEvent
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartAtUtc { get; set; }
    public DateTime EndAtUtc { get; set; }

    /// <summary>tenant, department, team, individual</summary>
    public string AudienceType { get; set; } = "tenant";

    public Guid? AudienceId { get; set; }
}
