namespace OnevoHr.Api.Models.Templates;

public class TenantConfigurationTemplateApplication
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ConfigurationTemplateId { get; set; }
    public string TemplateType { get; set; } = string.Empty;
    public int AppliedVersion { get; set; }

    /// <summary>applied, superseded, customized</summary>
    public string Status { get; set; } = "applied";

    public DateTime AppliedAtUtc { get; set; }
    public Guid? AppliedByPlatformUserId { get; set; }

    /// <summary>jsonb</summary>
    public string? CustomPayloadJson { get; set; }

    public ConfigurationTemplate? ConfigurationTemplate { get; set; }
}
