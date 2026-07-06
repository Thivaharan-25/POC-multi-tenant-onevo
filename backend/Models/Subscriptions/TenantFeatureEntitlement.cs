using OnevoHr.Api.Models.Catalog;

namespace OnevoHr.Api.Models.Subscriptions;

public class TenantFeatureEntitlement
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ModuleFeatureId { get; set; }
    public bool IsEnabled { get; set; }

    public ModuleFeature? ModuleFeature { get; set; }
}
