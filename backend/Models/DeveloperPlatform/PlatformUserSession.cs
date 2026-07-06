namespace OnevoHr.Api.Models.DeveloperPlatform;

public class PlatformUserSession
{
    public Guid Id { get; set; }
    public Guid PlatformUserId { get; set; }
    public string SessionTokenHash { get; set; } = string.Empty;
    public string CsrfTokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }

    public PlatformUser? PlatformUser { get; set; }
}
