namespace OnevoHr.Api.Models.Tenant;

public class TenantProvisioningState
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string CurrentStep { get; set; } = string.Empty;
    public DateTime? OrganizationInfoCompletedAtUtc { get; set; }
    public DateTime? AdminAccountCompletedAtUtc { get; set; }
    public DateTime? SubscriptionCompletedAtUtc { get; set; }
    public DateTime? ModuleSelectionCompletedAtUtc { get; set; }
    public DateTime? TemplateApplicationCompletedAtUtc { get; set; }
    public bool ActivationReady { get; set; }
    public string? ActivationBlockedReason { get; set; }
    public DateTime LastUpdatedAtUtc { get; set; }

    public Tenant? Tenant { get; set; }
}
