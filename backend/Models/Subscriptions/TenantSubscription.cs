namespace OnevoHr.Api.Models.Subscriptions;

public class TenantSubscription
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? SubscriptionPlanId { get; set; }

    /// <summary>demo, pending_payment, active, cancelled</summary>
    public string Status { get; set; } = "demo";

    /// <summary>monthly, annual</summary>
    public string? BillingCycle { get; set; }

    public int? ConfirmedEmployeeCount { get; set; }

    /// <summary>jsonb array of selected feature keys</summary>
    public string SelectedFeatureKeysJson { get; set; } = "[]";

    /// <summary>jsonb array of selected add-on keys</summary>
    public string SelectedAddOnsJson { get; set; } = "[]";

    public DateTime? StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Tenant.Tenant? Tenant { get; set; }
    public SubscriptionPlan? SubscriptionPlan { get; set; }
}
