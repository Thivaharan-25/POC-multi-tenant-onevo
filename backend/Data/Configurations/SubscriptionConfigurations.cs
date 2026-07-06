using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.Subscriptions;

namespace OnevoHr.Api.Data.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.Status);
        builder.Property(x => x.SelectedFeatureKeysJson).HasColumnType("jsonb");
        builder.Property(x => x.SelectedAddOnsJson).HasColumnType("jsonb");

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SubscriptionPlan)
            .WithMany()
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TenantModuleEntitlementConfiguration : IEntityTypeConfiguration<TenantModuleEntitlement>
{
    public void Configure(EntityTypeBuilder<TenantModuleEntitlement> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.ModuleCatalogId }).IsUnique();

        builder.HasOne(x => x.ModuleCatalog)
            .WithMany()
            .HasForeignKey(x => x.ModuleCatalogId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TenantFeatureEntitlementConfiguration : IEntityTypeConfiguration<TenantFeatureEntitlement>
{
    public void Configure(EntityTypeBuilder<TenantFeatureEntitlement> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.ModuleFeatureId }).IsUnique();

        builder.HasOne(x => x.ModuleFeature)
            .WithMany()
            .HasForeignKey(x => x.ModuleFeatureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RuntimeFeatureFlagConfiguration : IEntityTypeConfiguration<RuntimeFeatureFlag>
{
    public void Configure(EntityTypeBuilder<RuntimeFeatureFlag> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.FeatureKey });
        builder.Property(x => x.FeatureKey).HasMaxLength(120);
    }
}

public class SubscriptionInvoiceConfiguration : IEntityTypeConfiguration<SubscriptionInvoice>
{
    public void Configure(EntityTypeBuilder<SubscriptionInvoice> builder)
    {
        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.Status);
        builder.Property(x => x.Amount).HasPrecision(18, 2);

        builder.HasOne(x => x.TenantSubscription)
            .WithMany()
            .HasForeignKey(x => x.TenantSubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentGatewayConfigConfiguration : IEntityTypeConfiguration<PaymentGatewayConfig>
{
    public void Configure(EntityTypeBuilder<PaymentGatewayConfig> builder)
    {
        builder.HasIndex(x => x.GatewayKey).IsUnique();
        builder.Property(x => x.GatewayKey).HasMaxLength(80);
    }
}

public class PaymentGatewayCredentialConfiguration : IEntityTypeConfiguration<PaymentGatewayCredential>
{
    public void Configure(EntityTypeBuilder<PaymentGatewayCredential> builder)
    {
        builder.HasOne(x => x.PaymentGatewayConfig)
            .WithMany()
            .HasForeignKey(x => x.PaymentGatewayConfigId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentGatewayCountryRouteConfiguration : IEntityTypeConfiguration<PaymentGatewayCountryRoute>
{
    public void Configure(EntityTypeBuilder<PaymentGatewayCountryRoute> builder)
    {
        builder.HasIndex(x => new { x.CountryCode, x.Environment });
        builder.Property(x => x.CountryCode).HasMaxLength(2);

        builder.HasOne(x => x.PaymentGatewayConfig)
            .WithMany()
            .HasForeignKey(x => x.GatewayConfigId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SubscriptionPlanPriceBracketConfiguration : IEntityTypeConfiguration<SubscriptionPlanPriceBracket>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlanPriceBracket> builder)
    {
        builder.HasIndex(x => new { x.SubscriptionPlanId, x.CompanySizeRange }).IsUnique();
        builder.Property(x => x.OptionalAddonPrices).HasColumnType("jsonb");
        builder.Property(x => x.ResourceAddonPrices).HasColumnType("jsonb");

        builder.HasOne(x => x.SubscriptionPlan)
            .WithMany(p => p.PriceBrackets)
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SubscriptionPlanModuleConfiguration : IEntityTypeConfiguration<SubscriptionPlanModule>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlanModule> builder)
    {
        builder.HasIndex(x => new { x.SubscriptionPlanId, x.ModuleKey }).IsUnique();

        builder.HasOne(x => x.SubscriptionPlan)
            .WithMany(p => p.PlanModules)
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SubscriptionPlanResourceAddonConfiguration : IEntityTypeConfiguration<SubscriptionPlanResourceAddon>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlanResourceAddon> builder)
    {
        builder.Property(x => x.PriceByEmployeeTier).HasColumnType("jsonb");

        builder.HasOne(x => x.SubscriptionPlan)
            .WithMany(p => p.ResourceAddOns)
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
