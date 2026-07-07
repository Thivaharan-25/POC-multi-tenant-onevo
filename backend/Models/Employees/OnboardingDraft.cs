using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnevoHr.Api.Models.Employees;

[Table("onboarding_drafts")]
public class OnboardingDraft
{
    [Key]
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string WorkEmail { get; set; } = string.Empty;
    public Guid? LegalEntityId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public string? EmploymentType { get; set; }
    public DateOnly? StartDate { get; set; }
    public string? EmployeeNumber { get; set; }
    public Guid? ScheduleId { get; set; }
    public Guid? SelectedTemplateId { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string EditedTasksJson { get; set; } = "[]";
    
    public string Status { get; set; } = "draft";
    public string DraftReason { get; set; } = string.Empty;
    public string LastSavedStep { get; set; } = string.Empty;
    
    public Guid StartedById { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
