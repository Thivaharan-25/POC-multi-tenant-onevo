using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.Tenant;
using TenantEntity = OnevoHr.Api.Models.Tenant.Tenant;

namespace OnevoHr.Api.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<TenantEntity>
{
    public void Configure(EntityTypeBuilder<TenantEntity> builder)
    {
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.Property(x => x.Slug).HasMaxLength(120);

        builder.HasMany(x => x.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Employees)
            .WithOne(e => e.Tenant)
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.LegalEntities)
            .WithOne(l => l.Tenant)
            .HasForeignKey(l => l.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TenantProvisioningStateConfiguration : IEntityTypeConfiguration<TenantProvisioningState>
{
    public void Configure(EntityTypeBuilder<TenantProvisioningState> builder)
    {
        builder.HasIndex(x => x.TenantId).IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithOne()
            .HasForeignKey<TenantProvisioningState>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
