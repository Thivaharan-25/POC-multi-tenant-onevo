namespace OnevoHr.Api.Models.Subscriptions;

/// <summary>
/// Explicit package classification for a plan's selected modules - source of
/// truth for base vs optional-addon. Matches developer-platform/database/
/// schema.md's subscription_plan_modules.
/// </summary>
public class SubscriptionPlanModule
{
    public Guid Id { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;

    /// <summary>base or optional_addon</summary>
    public string PackageType { get; set; } = "base";

    public int? StorageContributionGb { get; set; }
    public long? AiTokenContribution { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }
}
