namespace OnevoHr.Api.Models.Demo;

public class DemoProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TrialDurationDays { get; set; }
    public bool AutoExpire { get; set; }
    public int MaxEmployees { get; set; }
    public int DemoStorageLimitGb { get; set; }
    public long DemoAiTokenLimit { get; set; }
    public bool IsActive { get; set; }
    public Guid? CreatedByPlatformUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<DemoProfileModuleAccess> ModuleAccess { get; set; } = new List<DemoProfileModuleAccess>();
    public DemoProfileUpgradeOption? UpgradeOptions { get; set; }
}
