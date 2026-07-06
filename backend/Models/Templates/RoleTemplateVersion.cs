namespace OnevoHr.Api.Models.Templates;

public class RoleTemplateVersion
{
    public Guid Id { get; set; }
    public Guid RoleTemplateId { get; set; }
    public int Version { get; set; }

    /// <summary>jsonb</summary>
    public string PayloadJson { get; set; } = "{}";

    public DateTime CreatedAtUtc { get; set; }

    public RoleTemplate? RoleTemplate { get; set; }
}
