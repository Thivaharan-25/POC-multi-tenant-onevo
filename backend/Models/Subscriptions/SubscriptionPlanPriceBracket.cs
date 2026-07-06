namespace OnevoHr.Api.Models.Subscriptions;

/// <summary>
/// Company-size pricing tier for a plan. Matches developer-platform/database/
/// schema.md's subscription_plan_price_brackets.
/// </summary>
public class SubscriptionPlanPriceBracket
{
    public Guid Id { get; set; }
    public Guid SubscriptionPlanId { get; set; }

    /// <summary>e.g. "1-50", "51-200", "1001+"</summary>
    public string CompanySizeRange { get; set; } = string.Empty;

    public decimal BasePlanMonthlyPrice { get; set; }

    /// <summary>
    /// Not in schema.md (which derives annual pricing from subscription_plans.
    /// annual_discount_pct); kept here since SubscriptionPlan does not yet
    /// carry that column and callers need a concrete annual price today.
    /// </summary>
    public decimal AnnualPrice { get; set; }

    /// <summary>jsonb map of module_key -> monthly_price</summary>
    public string OptionalAddonPrices { get; set; } = "{}";

    /// <summary>jsonb map of addon_id -> unit_price</summary>
    public string ResourceAddonPrices { get; set; } = "{}";

    public string Currency { get; set; } = "USD";
    public DateTime CreatedAtUtc { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }
}
