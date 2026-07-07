using System.ComponentModel.DataAnnotations;

namespace OnevoHr.Api.DTOs.OrgStructure;

public class CreateLegalEntityRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Timezone { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "active";
}
