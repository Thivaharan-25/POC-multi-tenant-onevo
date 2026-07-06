namespace OnevoHr.Api.Models.Subscriptions;

/// <summary>
/// Resource-only add-on for a specific plan (not a module) - increases the
/// tenant's shared storage pool and/or AI token allowance. Matches
/// developer-platform/database/schema.md's subscription_plan_resource_addons.
/// </summary>
public class SubscriptionPlanResourceAddon
{
    public Guid Id { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public string Label { get; set; } = string.Empty;
    public int? StorageContributionGb { get; set; }
    public long? AiTokenContribution { get; set; }

    /// <summary>jsonb map of employee_count_tier -> unit_price</summary>
    public string PriceByEmployeeTier { get; set; } = "{}";

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }
}
