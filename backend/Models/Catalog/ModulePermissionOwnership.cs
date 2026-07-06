using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Models.Catalog;

public class ModulePermissionOwnership
{
    public Guid Id { get; set; }
    public Guid ModuleCatalogId { get; set; }
    public Guid PermissionId { get; set; }

    public ModuleCatalog? ModuleCatalog { get; set; }
    public Permission? Permission { get; set; }
}
