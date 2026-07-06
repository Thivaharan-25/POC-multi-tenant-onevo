using OnevoHr.Api.Models.Catalog;

namespace OnevoHr.Api.Models.Subscriptions;

public class TenantModuleEntitlement
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ModuleCatalogId { get; set; }

    /// <summary>disabled, trial, subscription_included, purchased, quoted</summary>
    public string State { get; set; } = "disabled";

    public string? RuntimeOverride { get; set; }
    public bool IsEnabled { get; set; }

    public ModuleCatalog? ModuleCatalog { get; set; }
}
