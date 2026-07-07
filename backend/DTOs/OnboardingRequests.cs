using System;
using System.ComponentModel.DataAnnotations;

namespace OnevoHr.Api.DTOs;

public class SaveOnboardingDraftRequest
{
    [Required]
    public string EmployeeName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string WorkEmail { get; set; } = string.Empty;
    
    public Guid? LegalEntityId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    
    public string? EmploymentType { get; set; }
    public DateOnly? StartDate { get; set; }
    public string? EmployeeNumber { get; set; }
    public Guid? ScheduleId { get; set; }
    
    [Required]
    public string LastSavedStep { get; set; } = string.Empty;

    public Guid? SelectedTemplateId { get; set; }
    public string? EditedTasksJson { get; set; }
}

public class UpdateChecklistDraftRequest
{
    public Guid? SelectedTemplateId { get; set; }
    public string EditedTasksJson { get; set; } = "[]";
}

public class ValidationIssue
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Field { get; set; }
}

public class DraftValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationIssue> Errors { get; set; } = new List<ValidationIssue>();
    public List<ValidationIssue> Warnings { get; set; } = new List<ValidationIssue>();

    /// <summary>Follow-up actions the frontend can offer, e.g. "request_seat_increase".</summary>
    public List<string> Actions { get; set; } = new List<string>();
}

public class SendInviteResponse
{
    /// <summary>completed, blocked, invalid, not_found</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Set when Status = blocked: waiting_for_seat or waiting_for_position_approval.</summary>
    public string? DraftReason { get; set; }

    public DraftValidationResult? Validation { get; set; }
    public List<string> Actions { get; set; } = new List<string>();
    public Guid? EmployeeId { get; set; }
    public Guid? UserId { get; set; }
    public string? DevInviteUrl { get; set; }
    public string? Error { get; set; }
}

public class DraftActionResponse
{
    /// <summary>ok, invalid_state, not_found</summary>
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
}
