using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.OrgStructure;

namespace OnevoHr.Api.Models.Tenant;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    /// <summary>provisioning, trial, trial_expired, pending_payment, active, suspended, cancelled</summary>
    public string Status { get; set; } = "provisioning";

    /// <summary>operator_provisioning, demo_request</summary>
    public string Source { get; set; } = "operator_provisioning";

    public Guid? SourceDemoRequestId { get; set; }
    public Guid? DemoProfileId { get; set; }
    public int? EstimatedEmployeeCount { get; set; }
    public int? ConfirmedEmployeeCount { get; set; }
    public DateTime? TrialStartsAtUtc { get; set; }
    public DateTime? TrialEndsAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ActivatedAtUtc { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<LegalEntity> LegalEntities { get; set; } = new List<LegalEntity>();
}
