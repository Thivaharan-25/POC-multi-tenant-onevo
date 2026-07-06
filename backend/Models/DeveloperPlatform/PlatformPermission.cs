namespace OnevoHr.Api.Models.DeveloperPlatform;

public class PlatformPermission
{
    public Guid Id { get; set; }
    public string PermissionKey { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
