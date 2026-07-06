namespace OnevoHr.Api.Models.Catalog;

public class ModuleCatalog
{
    public Guid Id { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsFoundation { get; set; }
    public bool IsSellable { get; set; }
    public bool IsActive { get; set; }
    public bool SupportsStorage { get; set; }
    public bool SupportsAi { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<ModuleFeature> Features { get; set; } = new List<ModuleFeature>();
    public ICollection<ModulePermissionOwnership> PermissionOwnerships { get; set; } = new List<ModulePermissionOwnership>();
}
