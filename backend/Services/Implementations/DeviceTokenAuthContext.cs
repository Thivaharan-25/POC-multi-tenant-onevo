using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

// Scoped per-request holder for the authenticated tray-app agent, resolved
// from the Authorization: Bearer <deviceToken> header by DeviceTokenAuthMiddleware.
// Mirrors CurrentUserService, which does the equivalent job for cookie sessions.
public class DeviceTokenAuthContext : IDeviceTokenAuthContext
{
    public bool IsAuthenticated { get; private set; }
    public Guid? RegisteredAgentId { get; private set; }
    public Guid? TenantId { get; private set; }
    public Guid? EmployeeId { get; private set; }

    public void SetAgent(Guid registeredAgentId, Guid tenantId, Guid? employeeId)
    {
        IsAuthenticated = true;
        RegisteredAgentId = registeredAgentId;
        TenantId = tenantId;
        EmployeeId = employeeId;
    }
}
