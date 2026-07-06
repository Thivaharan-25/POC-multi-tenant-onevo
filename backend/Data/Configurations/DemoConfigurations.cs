using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.Demo;

namespace OnevoHr.Api.Data.Configurations;

public class DemoProfileConfiguration : IEntityTypeConfiguration<DemoProfile>
{
    public void Configure(EntityTypeBuilder<DemoProfile> builder)
    {
        builder.ToTable("demo_profiles");
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.ModuleAccess)
            .WithOne(m => m.DemoProfile)
            .HasForeignKey(m => m.DemoProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpgradeOptions)
            .WithOne(o => o.DemoProfile)
            .HasForeignKey<DemoProfileUpgradeOption>(o => o.DemoProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DemoProfileModuleAccessConfiguration : IEntityTypeConfiguration<DemoProfileModuleAccess>
{
    public void Configure(EntityTypeBuilder<DemoProfileModuleAccess> builder)
    {
        builder.ToTable("demo_profile_modules");
        builder.HasIndex(x => new { x.DemoProfileId, x.ModuleCatalogId }).IsUnique();
        builder.Property(x => x.FeaturePermissions).HasColumnType("jsonb");

        builder.HasOne(x => x.ModuleCatalog)
            .WithMany()
            .HasForeignKey(x => x.ModuleCatalogId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DemoProfileUpgradeOptionConfiguration : IEntityTypeConfiguration<DemoProfileUpgradeOption>
{
    public void Configure(EntityTypeBuilder<DemoProfileUpgradeOption> builder)
    {
        builder.ToTable("demo_profile_upgrade_options");
        builder.HasIndex(x => x.DemoProfileId).IsUnique();
        builder.Property(x => x.AllowedPlanIds).HasColumnType("jsonb");
        builder.Property(x => x.AllowedAddonModuleKeys).HasColumnType("jsonb");
        builder.Property(x => x.HiddenAddonModuleKeys).HasColumnType("jsonb");
        builder.Property(x => x.AddonVisibility).HasColumnType("jsonb");
        builder.Property(x => x.AddonDemoLimits).HasColumnType("jsonb");
    }
}
