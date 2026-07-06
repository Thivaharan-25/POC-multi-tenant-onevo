namespace OnevoHr.Api.Models.Catalog;

public class ModuleFeature
{
    public Guid Id { get; set; }
    public Guid ModuleCatalogId { get; set; }
    public string FeatureKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public ModuleCatalog? ModuleCatalog { get; set; }
}
