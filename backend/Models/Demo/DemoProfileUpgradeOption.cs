namespace OnevoHr.Api.Models.Demo;

/// <summary>
/// One row per demo profile, controlling which paid plans and add-ons a demo
/// tenant may upgrade to. Matches developer-platform/database/schema.md's
/// demo_profile_upgrade_options (array/jsonb columns, not a per-plan join table).
/// </summary>
public class DemoProfileUpgradeOption
{
    public Guid Id { get; set; }
    public Guid DemoProfileId { get; set; }

    /// <summary>jsonb array of allowed subscription_plans.id values</summary>
    public string AllowedPlanIds { get; set; } = "[]";

    /// <summary>jsonb array of allowed add-on module_key values</summary>
    public string AllowedAddonModuleKeys { get; set; } = "[]";

    /// <summary>jsonb array of hidden add-on module_key values</summary>
    public string HiddenAddonModuleKeys { get; set; } = "[]";

    /// <summary>jsonb map of module_key -> "enabled" | "show_only"</summary>
    public string AddonVisibility { get; set; } = "{}";

    /// <summary>jsonb map of demo-specific limits per add-on</summary>
    public string AddonDemoLimits { get; set; } = "{}";

    public DemoProfile? DemoProfile { get; set; }
}
