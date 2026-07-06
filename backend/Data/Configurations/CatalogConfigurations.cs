using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.Catalog;

namespace OnevoHr.Api.Data.Configurations;

public class ModuleCatalogConfiguration : IEntityTypeConfiguration<ModuleCatalog>
{
    public void Configure(EntityTypeBuilder<ModuleCatalog> builder)
    {
        builder.HasIndex(x => x.ModuleKey).IsUnique();
        builder.Property(x => x.ModuleKey).HasMaxLength(120);

        builder.HasMany(x => x.Features)
            .WithOne(f => f.ModuleCatalog)
            .HasForeignKey(f => f.ModuleCatalogId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PermissionOwnerships)
            .WithOne(o => o.ModuleCatalog)
            .HasForeignKey(o => o.ModuleCatalogId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ModuleFeatureConfiguration : IEntityTypeConfiguration<ModuleFeature>
{
    public void Configure(EntityTypeBuilder<ModuleFeature> builder)
    {
        builder.HasIndex(x => x.FeatureKey).IsUnique();
        builder.Property(x => x.FeatureKey).HasMaxLength(120);
    }
}

public class ModulePermissionOwnershipConfiguration : IEntityTypeConfiguration<ModulePermissionOwnership>
{
    public void Configure(EntityTypeBuilder<ModulePermissionOwnership> builder)
    {
        builder.HasIndex(x => new { x.ModuleCatalogId, x.PermissionId }).IsUnique();

        builder.HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
