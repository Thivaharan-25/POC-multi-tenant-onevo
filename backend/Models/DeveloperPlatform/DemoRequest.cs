namespace OnevoHr.Api.Models.DeveloperPlatform;

/// <summary>
/// Public/demo inquiry request requiring platform-side approval. Physical
/// table demo_access_requests matches developer-platform/database/schema.md.
/// </summary>
public class DemoRequest
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public string? RequesterPhone { get; set; }
    public string? RequestedSubdomain { get; set; }
    public string? CompanyWebsite { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string RequestedCompanySizeRange { get; set; } = string.Empty;
    public Guid? RequestedDemoProfileId { get; set; }

    /// <summary>jsonb array of requested module keys</summary>
    public string RequestedModuleKeys { get; set; } = "[]";

    public string? RequestedAccessNotes { get; set; }

    /// <summary>submitted, approved, rejected, converted_to_demo</summary>
    public string Status { get; set; } = "submitted";

    /// <summary>landing_demo_form</summary>
    public string Source { get; set; } = "landing_demo_form";

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public Guid? ReviewedById { get; set; }
    public string? RejectionReason { get; set; }
    public string? AdminNotes { get; set; }
    public string? TenantVisibleNote { get; set; }
    public Guid? CreatedTenantId { get; set; }

    /// <summary>jsonb; campaign/source context, UTM, sales notes</summary>
    public string? Metadata { get; set; }
}
