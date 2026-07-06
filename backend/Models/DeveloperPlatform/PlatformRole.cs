namespace OnevoHr.Api.Models.DeveloperPlatform;

public class PlatformRole
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<PlatformRolePermission> RolePermissions { get; set; } = new List<PlatformRolePermission>();
}
