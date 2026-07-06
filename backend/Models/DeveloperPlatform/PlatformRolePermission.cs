namespace OnevoHr.Api.Models.DeveloperPlatform;

public class PlatformRolePermission
{
    public Guid Id { get; set; }
    public Guid PlatformRoleId { get; set; }
    public Guid PlatformPermissionId { get; set; }

    public PlatformRole? PlatformRole { get; set; }
    public PlatformPermission? PlatformPermission { get; set; }
}
