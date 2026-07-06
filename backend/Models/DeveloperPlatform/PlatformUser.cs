namespace OnevoHr.Api.Models.DeveloperPlatform;

public class PlatformUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool MfaEnabled { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public ICollection<PlatformUserRole> UserRoles { get; set; } = new List<PlatformUserRole>();
}
