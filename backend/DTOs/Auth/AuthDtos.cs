namespace OnevoHr.Api.DTOs.Auth;

public sealed record SessionUserDto(
    Guid Id,
    Guid TenantId,
    Guid? EmployeeId,
    string DisplayName,
    string Email);

public sealed record SessionDto(
    SessionUserDto? User,
    IReadOnlyCollection<string> Permissions,
    IReadOnlyCollection<string> ActiveModules,
    IReadOnlyCollection<string> ActiveFeatures,
    bool SetupComplete);

public sealed record SessionValidationDto(
    Guid UserId,
    Guid TenantId,
    Guid? EmployeeId,
    string Email,
    string DisplayName,
    string CsrfTokenHash,
    IReadOnlyCollection<string> Permissions);

public sealed record ScopeResolutionDto(string ScopeLevel, IReadOnlyCollection<Guid> VisibleEmployeeIds);

public sealed record SafeInvitationDto(
    string InvitedEmail,
    string InvitedFullName,
    string[] AllowedCompletionMethods,
    DateTimeOffset ExpiresAt,
    string? TenantName);

public sealed record AcceptInviteRequestDto(
    string Token,
    string Password,
    string ConfirmPassword);
