namespace OnevoHr.Api.Services.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? EmployeeId { get; }
    IReadOnlyCollection<string> Permissions { get; }

    void SetUser(Guid userId, Guid tenantId, Guid? employeeId, IReadOnlyCollection<string> permissions);
    bool HasPermission(string permissionKey);
}
