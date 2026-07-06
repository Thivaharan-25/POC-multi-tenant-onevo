namespace OnevoHr.Api.Models.DeveloperPlatform;

public class PlatformUserRole
{
    public Guid Id { get; set; }
    public Guid PlatformUserId { get; set; }
    public Guid PlatformRoleId { get; set; }

    public PlatformUser? PlatformUser { get; set; }
    public PlatformRole? PlatformRole { get; set; }
}
