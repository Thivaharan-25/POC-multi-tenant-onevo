namespace OnevoHr.Api.Models.Auth;

public class UserRole
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    /// <summary>
    /// Optional manual scope for directly assigned roles only.
    /// null, department, legal_entity, tenant, own, direct_reports.
    /// Must not replace position-derived scope.
    /// </summary>
    public string? ScopeType { get; set; }
    public Guid? ScopeTargetId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User? User { get; set; }
    public Role? Role { get; set; }
}
