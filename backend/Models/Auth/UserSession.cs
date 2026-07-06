namespace OnevoHr.Api.Models.Auth;

public class UserSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string SessionTokenHash { get; set; } = string.Empty;
    public string CsrfTokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }

    public User? User { get; set; }
}
