namespace OnevoHr.Api.DTOs.Admin;

public sealed record PlatformLoginRequestDto(string Email, string Password);

public sealed record PlatformLoginResponseDto(Guid PlatformUserId, string Email, string DisplayName);

public sealed record PlatformSessionValidationDto(
    Guid PlatformUserId,
    string Email,
    string DisplayName,
    string CsrfTokenHash,
    IReadOnlyCollection<string> Permissions);

public sealed record TenantSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    string Source,
    int? ConfirmedEmployeeCount,
    DateTime? TrialEndsAtUtc,
    DateTime CreatedAtUtc);

public sealed record DemoRequestDto(
    Guid Id,
    string CompanyName,
    string RequesterName,
    string RequesterEmail,
    string CountryCode,
    string RequestedCompanySizeRange,
    string Status,
    string Source,
    DateTime CreatedAtUtc,
    Guid? CreatedTenantId);

public sealed record DemoProfileDto(
    Guid Id,
    string Name,
    string? Description,
    int TrialDurationDays,
    bool AutoExpire,
    int MaxEmployees,
    int DemoStorageLimitGb,
    long DemoAiTokenLimit,
    bool IsActive);

public sealed record PriceBracketDto(string CompanySizeRange, decimal BasePlanMonthlyPrice, decimal AnnualPrice, string Currency);

public sealed record SubscriptionPlanDto(
    Guid Id,
    string Name,
    string Code,
    string BillingCycle,
    bool IsActive,
    int SharedBaseStorageGb,
    long SharedBaseAiTokenAllowance,
    IReadOnlyCollection<PriceBracketDto> PriceBrackets);

public sealed record ModuleFeatureDto(Guid Id, string FeatureKey, bool IsActive);

public sealed record ModuleCatalogDto(
    Guid Id,
    string ModuleKey,
    string DisplayName,
    string? Description,
    bool IsFoundation,
    bool IsSellable,
    bool IsActive,
    IReadOnlyCollection<ModuleFeatureDto> Features);

public sealed record ConfigurationTemplateDto(
    Guid Id,
    string TemplateKey,
    string TemplateType,
    string Name,
    int Version,
    int? EmployeeRangeMin,
    int? EmployeeRangeMax,
    bool IsActive);

public sealed record RoleTemplateDto(Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, IReadOnlyCollection<string> PermissionKeys);

public sealed record PlatformUserDto(Guid Id, string Email, string DisplayName, bool IsActive, IReadOnlyCollection<string> Roles);

public sealed record PlatformRoleDto(Guid Id, string Name, string? Description, bool IsSystemRole, IReadOnlyCollection<string> PermissionKeys);

public sealed record CreateTenantDraftDto(string Name, string Slug, string Domain, int? EstimatedEmployeeCount);
