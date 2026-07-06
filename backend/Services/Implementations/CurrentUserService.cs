using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Scoped per-request holder for the authenticated tenant user context.
/// Populated by cookie session validation in the middleware pipeline.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    public bool IsAuthenticated { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? TenantId { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public IReadOnlyCollection<string> Permissions { get; private set; } = Array.Empty<string>();

    public void SetUser(Guid userId, Guid tenantId, Guid? employeeId, IReadOnlyCollection<string> permissions)
    {
        IsAuthenticated = true;
        UserId = userId;
        TenantId = tenantId;
        EmployeeId = employeeId;
        Permissions = permissions;
    }

    public bool HasPermission(string permissionKey)
    {
        return IsAuthenticated && Permissions.Contains(permissionKey);
    }
}
