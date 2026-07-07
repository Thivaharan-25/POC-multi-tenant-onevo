namespace OnevoHr.Api.DTOs.Onboarding;

public sealed record ChecklistTemplateSummaryDto(
    Guid Id,
    string Name,
    string TemplateType,
    Guid? DepartmentId,
    bool IsActive);

public sealed record ChecklistTemplateTaskDto(
    string Title,
    string? OwnerType,
    int? Sequence,
    bool IsRequired,
    bool IsLocked);

public sealed record ChecklistTemplateDetailDto(
    Guid Id,
    string Name,
    string TemplateType,
    Guid? DepartmentId,
    bool IsActive,
    List<ChecklistTemplateTaskDto> Tasks);

public sealed record ChecklistTemplateListResponse(
    List<ChecklistTemplateSummaryDto> Templates,
    Guid? RecommendedTemplateId);

public sealed record MyDraftSummaryDto(
    Guid Id,
    string EmployeeName,
    string WorkEmail,
    string LastSavedStep,
    string DraftReason,
    DateTime UpdatedAtUtc);
