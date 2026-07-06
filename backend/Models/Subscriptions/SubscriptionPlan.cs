namespace OnevoHr.Api.Models.Subscriptions;

public class SubscriptionPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    /// <summary>monthly, annual</summary>
    public string BillingCycle { get; set; } = "monthly";

    public bool IsActive { get; set; }
    public int SharedBaseStorageGb { get; set; }
    public long SharedBaseAiTokenAllowance { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<SubscriptionPlanModule> PlanModules { get; set; } = new List<SubscriptionPlanModule>();
    public ICollection<SubscriptionPlanResourceAddon> ResourceAddOns { get; set; } = new List<SubscriptionPlanResourceAddon>();
    public ICollection<SubscriptionPlanPriceBracket> PriceBrackets { get; set; } = new List<SubscriptionPlanPriceBracket>();
}
