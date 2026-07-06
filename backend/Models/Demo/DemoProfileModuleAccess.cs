using OnevoHr.Api.Models.Catalog;

namespace OnevoHr.Api.Models.Demo;

public class DemoProfileModuleAccess
{
    public Guid Id { get; set; }
    public Guid DemoProfileId { get; set; }
    public Guid ModuleCatalogId { get; set; }

    /// <summary>full_access, view_only, or archive</summary>
    public string AccessLevel { get; set; } = "view_only";

    /// <summary>jsonb map of feature_key -> enabled bool</summary>
    public string FeaturePermissions { get; set; } = "{}";

    public DemoProfile? DemoProfile { get; set; }
    public ModuleCatalog? ModuleCatalog { get; set; }
}
