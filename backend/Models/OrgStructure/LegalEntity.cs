namespace OnevoHr.Api.Models.OrgStructure;

public class LegalEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public string OfficeAddressLabel { get; set; } = string.Empty;
    public decimal? OfficeLatitude { get; set; }
    public decimal? OfficeLongitude { get; set; }
    public int? OfficeAllowedRadiusMeters { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Tenant.Tenant? Tenant { get; set; }
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Position> Positions { get; set; } = new List<Position>();
}
