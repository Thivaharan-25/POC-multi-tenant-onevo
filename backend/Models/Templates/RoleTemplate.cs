namespace OnevoHr.Api.Models.Templates;

public class RoleTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>jsonb array of module keys</summary>
    public string ModuleKeysJson { get; set; } = "[]";

    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>jsonb array of permission codes</summary>
    public string PermissionCodesJson { get; set; } = "[]";
}
