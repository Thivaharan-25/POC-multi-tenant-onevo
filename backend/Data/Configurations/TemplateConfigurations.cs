using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.Templates;

namespace OnevoHr.Api.Data.Configurations;

public class RoleTemplateConfiguration : IEntityTypeConfiguration<RoleTemplate>
{
    public void Configure(EntityTypeBuilder<RoleTemplate> builder)
    {
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.ModuleKeysJson).HasColumnType("jsonb");
    }
}

public class ConfigurationTemplateConfiguration : IEntityTypeConfiguration<ConfigurationTemplate>
{
    public void Configure(EntityTypeBuilder<ConfigurationTemplate> builder)
    {
        builder.HasIndex(x => x.TemplateKey).IsUnique();
        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");
    }
}

public class TenantConfigurationTemplateApplicationConfiguration : IEntityTypeConfiguration<TenantConfigurationTemplateApplication>
{
    public void Configure(EntityTypeBuilder<TenantConfigurationTemplateApplication> builder)
    {
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.CustomPayloadJson).HasColumnType("jsonb");

        builder.HasOne(x => x.ConfigurationTemplate)
            .WithMany()
            .HasForeignKey(x => x.ConfigurationTemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
