namespace OnevoHr.Api.Services.Interfaces;

public interface IDeviceTokenAuthContext
{
    bool IsAuthenticated { get; }
    Guid? RegisteredAgentId { get; }
    Guid? TenantId { get; }
    Guid? EmployeeId { get; }

    void SetAgent(Guid registeredAgentId, Guid tenantId, Guid? employeeId);
}
