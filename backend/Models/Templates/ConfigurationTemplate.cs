namespace OnevoHr.Api.Models.Templates;

public class ConfigurationTemplate
{
    public Guid Id { get; set; }
    public string TemplateKey { get; set; } = string.Empty;

    /// <summary>configuration, position_template, leave_policy, monitoring_policy, app_allowlist, onboarding, data_import_mapping</summary>
    public string TemplateType { get; set; } = "configuration";

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; }
    public int? EmployeeRangeMin { get; set; }
    public int? EmployeeRangeMax { get; set; }
    public string? IndustryProfileTag { get; set; }

    /// <summary>jsonb typed payload</summary>
    public string PayloadJson { get; set; } = "{}";

    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
