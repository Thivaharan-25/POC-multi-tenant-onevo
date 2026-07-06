namespace OnevoHr.Api.Models.DeveloperPlatform;

public class PlatformAuthEvent
{
    public Guid Id { get; set; }
    public Guid? PlatformUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
