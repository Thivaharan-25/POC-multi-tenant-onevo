// Auto-generated Phase 1 Entities
using System;
using System.Text.Json;

namespace OnevoHr.Api.Models.Generated;

public class Country
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string PhoneCode { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
}

public class FileRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string SafeFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public class FileUploadReservation
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public long ReservedBytes { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ReservedByUserId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public Guid? CompletedFileRecordId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class TenantStorageStat
{
    public Guid TenantId { get; set; }
    public long UsedR2Bytes { get; set; }
    public long UsedDbBytes { get; set; }
    public long ReservedR2Bytes { get; set; }
    public DateTimeOffset LastCalculatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class EntityAsset
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string OwnerType { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public string AssetPurpose { get; set; } = string.Empty;
    public Guid FileRecordId { get; set; }
    public bool IsPrimary { get; set; }
    public int? SortOrder { get; set; }
    public string Metadata { get; set; } = string.Empty;
    public string CreatedByType { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string OldValuesJson { get; set; } = string.Empty;
    public string NewValuesJson { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class FeatureAccessGrant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string GranteeType { get; set; } = string.Empty;
    public Guid GranteeId { get; set; }
    public string Module { get; set; } = string.Empty;
    public string? FeatureKey { get; set; }
    public bool IsEnabled { get; set; }
    public Guid GrantedBy { get; set; }
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class LegalAcceptanceRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentVersion { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public bool Required { get; set; }
    public DateTimeOffset DecidedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}

public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? FeatureKey { get; set; }
}

public class InvitationToken
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoleId { get; set; }
    public string InvitedEmail { get; set; } = string.Empty;
    public string InvitedFullName { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CompletionMethodsJson { get; set; } = string.Empty;
    public string? CompletedWith { get; set; }
    public bool AllowGoogleEmailMismatch { get; set; }
    public string AllowedEmailDomainsJson { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? RevokedByUserId { get; set; }
    public Guid? RevokedByPlatformUserId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? CreatedByPlatformUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class TenantAuthPolicy
{
    public Guid TenantId { get; set; }
    public bool PasswordLoginEnabled { get; set; }
    public bool GoogleLoginEnabled { get; set; }
    public bool InviteGoogleEmailMismatchAllowed { get; set; }
    public string AllowedLoginDomainsJson { get; set; } = string.Empty;
    public bool MfaRequired { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class UserExternalIdentity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderSubject { get; set; } = string.Empty;
    public string ProviderEmail { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public DateTimeOffset LinkedAt { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
}

public class UserPermissionOverride
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public string GrantType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public Guid GrantedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class AccessGrantRequest
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public Guid TargetPositionId { get; set; }
    public Guid TargetDepartmentId { get; set; }
    public Guid PositionAccessTemplateId { get; set; }
    public Guid RequestedRoleId { get; set; }
    public string ApprovalStatus { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
    public DateTimeOffset EffectiveFrom { get; set; }
    public DateTimeOffset? EffectiveTo { get; set; }
    public string? DecisionComment { get; set; }
}

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public Guid ReplacedById { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class UserMfa
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Method { get; set; } = string.Empty;
    public string SecretEncrypted { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class MfaRecoveryCode
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public DateTimeOffset? UsedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class PositionAccessTemplate
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PositionId { get; set; }
    public Guid RoleId { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsSensitive { get; set; }
    public string EffectiveFromRule { get; set; } = string.Empty;
    public string? EffectiveToRule { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class ManagementCoverageRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LegalEntityId { get; set; }
    public Guid OwnerPositionId { get; set; }
    public string CoveredTargetType { get; set; } = string.Empty;
    public Guid? CoveredPositionId { get; set; }
    public Guid? CoveredDepartmentId { get; set; }
    public int OwnerOrder { get; set; }
    public string Source { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class EmployeeAddresse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string AddressJson { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class EmployeeBankDetail
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public byte[] AccountNumberEncrypted { get; set; } = Array.Empty<byte>();
    public string RoutingNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class EmployeeCustomField
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldValue { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
}

public class EmployeeDependent
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public bool IsEmergencyContact { get; set; }
    public string Phone { get; set; } = string.Empty;
}

public class EmployeeEmergencyContact
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class EmployeeLifecycleEvent
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateOnly EventDate { get; set; }
    public string DetailsJson { get; set; } = string.Empty;
    public Guid PerformedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class EmployeeQualification
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string QualificationType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public int YearObtained { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public Guid DocumentFileId { get; set; }
}

public class EmployeeSalaryHistory
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public decimal BaseSalary { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string ChangeReason { get; set; } = string.Empty;
    public Guid ApprovedById { get; set; }
}

public class EmployeeWorkHistory
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string ReasonForLeaving { get; set; } = string.Empty;
}

public class EmployeeTransfer
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? FromDepartmentId { get; set; }
    public Guid? ToDepartmentId { get; set; }
    public Guid? FromPositionId { get; set; }
    public Guid? ToPositionId { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Guid RequestedById { get; set; }
    public Guid? ApprovedById { get; set; }
}

public class OffboardingRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateOnly LastWorkingDate { get; set; }
    public string KnowledgeRiskLevel { get; set; } = string.Empty;
    public string ExitInterviewNotes { get; set; } = string.Empty;
    public string PenaltiesJson { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class EmployeeChecklistTask
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? TemplateId { get; set; }
    public string LifecycleType { get; set; } = string.Empty;
    public string TaskTitle { get; set; } = string.Empty;
    public string OwnerType { get; set; } = string.Empty;
    public int? Sequence { get; set; }
    public Guid AssignedToId { get; set; }
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CompletedAt { get; set; }
}

public class ChecklistTemplate
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string TasksJson { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class TimeOffPolicyRule
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PolicyId { get; set; }
    public Guid TimeOffTypeId { get; set; }
    public int EntitlementMinutes { get; set; }
    public string AccrualMethod { get; set; } = string.Empty;
    public string ProrationMethod { get; set; } = string.Empty;
    public bool CarryForwardAllowed { get; set; }
    public int? CarryForwardLimitMinutes { get; set; }
    public string? CarryForwardExpiry { get; set; }
    public string RolloverPeriod { get; set; } = string.Empty;
    public int? MinimumRequestMinutes { get; set; }
    public int? MaxConsecutiveMinutes { get; set; }
    public int? NoticePeriodDays { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class TimeOffBalancesAudit
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid TimeOffTypeId { get; set; }
    public Guid? PolicyId { get; set; }
    public Guid? EntitlementId { get; set; }
    public Guid? TimeOffRequestId { get; set; }
    public Guid? AttendanceRecordId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public int MinutesChanged { get; set; }
    public int BalanceAfterMinutes { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? CalculationSnapshotJson { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid? CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CalendarEventParticipant
{
    public Guid EventId { get; set; }
    public Guid EmployeeId { get; set; }
    public string ResponseStatus { get; set; } = string.Empty;
    public string? ResponseReason { get; set; }
}

public class HolidayCalendarSetting
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LegalEntityId { get; set; }
    public string DefaultCountryCode { get; set; } = string.Empty;
    public string? OverrideCountryCode { get; set; }
    public string EffectiveCountryCode { get; set; } = string.Empty;
    public bool HolidaySyncEnabled { get; set; }
    public string Provider { get; set; } = string.Empty;
    public int? LastSyncedYear { get; set; }
    public DateTimeOffset? LastSyncedAt { get; set; }
    public Guid UpdatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class TenantSetting
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string DateFormat { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string WorkWeekDaysJson { get; set; } = string.Empty;
    public string WorkHoursStart { get; set; } = string.Empty;
    public string WorkHoursEnd { get; set; } = string.Empty;
    public string PrivacyMode { get; set; } = string.Empty;
    public string DataRetentionDaysJson { get; set; } = string.Empty;
    public string SettingsJson { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MonitoringFeatureToggle
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool ActivityMonitoring { get; set; }
    public bool ApplicationTracking { get; set; }
    public bool DocumentTracking { get; set; }
    public bool CommunicationTracking { get; set; }
    public bool ScreenshotCapture { get; set; }
    public bool AutoScreenshotCapture { get; set; }
    public bool MeetingDetection { get; set; }
    public bool DeviceTracking { get; set; }
    public bool WorkLocationVerification { get; set; }
    public bool IdentityVerification { get; set; }
    public bool Biometric { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MonitoringAlertPolicy
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string MonitoringAlertRecipientResolver { get; set; } = string.Empty;
    public int MonitoringAlertWaitForScheduledRecipientGraceMinutes { get; set; }
    public bool MonitoringAlertFallbackToManagementCoverageChain { get; set; }
    public string MonitoringAlertUnresolvedRoutingAction { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AppAllowlist
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ScopeType { get; set; } = string.Empty;
    public Guid ScopeId { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsAllowed { get; set; }
    public string Source { get; set; } = string.Empty;
    public Guid? GlobalCatalogId { get; set; }
    public Guid SetById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AppAllowlistAudit
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid AllowlistId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid ChangedById { get; set; }
    public string OldValueJson { get; set; } = string.Empty;
    public string NewValueJson { get; set; } = string.Empty;
    public DateTimeOffset ChangedAt { get; set; }
}

public class ObservedApplication
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public Guid? GlobalCatalogId { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public int EmployeeCount { get; set; }
    public long TotalSecondsObserved { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class EmployeeMonitoringOverride
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public bool? ActivityMonitoring { get; set; }
    public bool? ApplicationTracking { get; set; }
    public bool? DocumentTracking { get; set; }
    public bool? CommunicationTracking { get; set; }
    public bool? ScreenshotCapture { get; set; }
    public bool? AutoScreenshotCapture { get; set; }
    public bool? MeetingDetection { get; set; }
    public bool? DeviceTracking { get; set; }
    public bool? WorkLocationVerification { get; set; }
    public bool? IdentityVerification { get; set; }
    public bool? Biometric { get; set; }
    public string OverrideReason { get; set; } = string.Empty;
    public Guid SetById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MonitoringPolicyOverride
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ScopeType { get; set; } = string.Empty;
    public Guid ScopeId { get; set; }
    public bool? ActivityMonitoring { get; set; }
    public bool? ApplicationTracking { get; set; }
    public bool? DocumentTracking { get; set; }
    public bool? CommunicationTracking { get; set; }
    public bool? ScreenshotCapture { get; set; }
    public bool? AutoScreenshotCapture { get; set; }
    public bool? MeetingDetection { get; set; }
    public bool? DeviceTracking { get; set; }
    public bool? WorkLocationVerification { get; set; }
    public bool? IdentityVerification { get; set; }
    public bool? Biometric { get; set; }
    public string OverrideReason { get; set; } = string.Empty;
    public Guid SetById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class EmployeeWorkLocationSetting
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string WorkMode { get; set; } = string.Empty;
    public bool WorkLocationVerificationEnabled { get; set; }
    public int? GracePeriodMinutes { get; set; }
    public bool PhotoChallengeOnMismatch { get; set; }
    public Guid SetById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class EmployeeRemoteWorkProfile
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CapturedAt { get; set; }
    public string? PublicIp { get; set; }
    public string? WifiSsid { get; set; }
    public string? WifiBssidHash { get; set; }
    public string? GatewayMacHash { get; set; }
    public bool VpnDetected { get; set; }
    public string? CoarseLocationJson { get; set; }
    public Guid VerificationRecordId { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
}

public class RemoteWorkLocationChangeRequest
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? CurrentProfileId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; set; }
    public Guid? ReviewedById { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }
    public Guid? NewProfileId { get; set; }
}

public class ActivitySnapshot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTimeOffset CapturedAt { get; set; }
    public int KeyboardEventsCount { get; set; }
    public int MouseEventsCount { get; set; }
    public int ActiveSeconds { get; set; }
    public int IdleSeconds { get; set; }
    public decimal IntensityScore { get; set; }
    public string ForegroundProcessName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class ActivityRawBuffer
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid AgentDeviceId { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
}

public class ActivityDailySummary
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public int TotalActiveMinutes { get; set; }
    public int TotalIdleMinutes { get; set; }
    public int TotalMeetingMinutes { get; set; }
    public decimal ActivePercentage { get; set; }
    public int ProductiveAppMinutes { get; set; }
    public int PersonalAppMinutes { get; set; }
    public int UnknownAppMinutes { get; set; }
    public int FocusMinutes { get; set; }
    public decimal ActivityScore { get; set; }
    public decimal DataCoveragePercentage { get; set; }
    public string TopAppsJson { get; set; } = string.Empty;
    public decimal IntensityAvg { get; set; }
    public int KeyboardTotal { get; set; }
    public int MouseTotal { get; set; }
    public int? BrowserActiveMinutes { get; set; }
    public int WorkBrowserMinutes { get; set; }
    public int PersonalBrowserMinutes { get; set; }
    public int DocumentTimeMinutes { get; set; }
    public int DeepFocusSessionsCount { get; set; }
    public string DataSource { get; set; } = string.Empty;
}

public class ApplicationCategory
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ApplicationNamePattern { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool? IsProductive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class ApplicationUsage
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string ApplicationCategory { get; set; } = string.Empty;
    public string WindowTitleHash { get; set; } = string.Empty;
    public int TotalSeconds { get; set; }
    public bool? IsProductive { get; set; }
    public bool? IsAllowed { get; set; }
    public string AppCategoryType { get; set; } = string.Empty;
    public string? BrowserDomain { get; set; }
}

public class DeviceTracking
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public int LaptopActiveMinutes { get; set; }
    public int EstimatedMobileMinutes { get; set; }
    public decimal LaptopPercentage { get; set; }
    public string DetectionMethod { get; set; } = string.Empty;
}

public class MeetingSession
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTimeOffset MeetingStart { get; set; }
    public DateTimeOffset MeetingEnd { get; set; }
    public string Platform { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public bool HadCameraOn { get; set; }
    public bool HadMicActivity { get; set; }
}

public class MonitoringEvidenceAsset
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? AgentDeviceId { get; set; }
    public Guid? ActivitySnapshotId { get; set; }
    public Guid? ActivityEventId { get; set; }
    public DateTimeOffset CapturedAt { get; set; }
    public Guid FileRecordId { get; set; }
    public string EvidenceType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string TriggerType { get; set; } = string.Empty;
    public Guid? RetentionPolicyId { get; set; }
    public Guid? LegalHoldId { get; set; }
    public string Metadata { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class BrowserActivity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string DomainClassification { get; set; } = string.Empty;
    public int TotalSeconds { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class DiscrepancyEvent
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public int HrActiveMinutes { get; set; }
    public int WorkManagementLoggedMinutes { get; set; }
    public int CalendarMinutes { get; set; }
    public int UnaccountedMinutes { get; set; }
    public string Severity { get; set; } = string.Empty;
    public int ThresholdMinutes { get; set; }
    public bool NotifiedOwner { get; set; }
    public DateTimeOffset? NotifiedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public decimal? ZScore { get; set; }
    public decimal? BaselineAvgMinutes { get; set; }
    public decimal? BaselineStddevMinutes { get; set; }
    public string SeverityMethod { get; set; } = string.Empty;
}

public class WorkManagementDailyTimeLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public int TotalLoggedMinutes { get; set; }
    public DateTimeOffset? ActiveTaskAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class EmployeeDiscrepancyBaseline
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly ComputedAt { get; set; }
    public int WindowDays { get; set; }
    public decimal AvgUnaccountedMinutes { get; set; }
    public decimal StddevUnaccountedMinutes { get; set; }
    public int SampleCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class PresenceSession
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public int TotalPresentMinutes { get; set; }
    public int TotalBreakMinutes { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AttendanceRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public Guid? WorkScheduleId { get; set; }
    public bool ExpectedWorkingDay { get; set; }
    public string WorkTimeType { get; set; } = string.Empty;
    public string? ScheduledStart { get; set; }
    public string? ScheduledEnd { get; set; }
    public int? RequiredWorkMinutes { get; set; }
    public string ExpectedWorkArea { get; set; } = string.Empty;
    public string ScheduleTimezone { get; set; } = string.Empty;
    public bool IsHoliday { get; set; }
    public string? HolidayName { get; set; }
    public DateTimeOffset? ActualStart { get; set; }
    public DateTimeOffset? ActualEnd { get; set; }
    public int WorkedMinutes { get; set; }
    public int BreakMinutes { get; set; }
    public int? LateMinutes { get; set; }
    public int? ShortMinutes { get; set; }
    public string? DetectedWorkArea { get; set; }
    public string AttendanceSource { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AttendanceCorrection
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LegalEntityId { get; set; }
    public Guid? PresenceSessionId { get; set; }
    public Guid? AttendanceRecordId { get; set; }
    public string CorrectionType { get; set; } = string.Empty;
    public DateTimeOffset? OriginalClockInAt { get; set; }
    public DateTimeOffset? OriginalClockOutAt { get; set; }
    public DateTimeOffset? RequestedClockInAt { get; set; }
    public DateTimeOffset? RequestedClockOutAt { get; set; }
    public string? OriginalBreakJson { get; set; }
    public string? RequestedBreakJson { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid RequestedById { get; set; }
    public Guid? ReviewedById { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class BreakRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTimeOffset BreakStart { get; set; }
    public DateTimeOffset BreakEnd { get; set; }
    public string BreakType { get; set; } = string.Empty;
    public bool AutoDetected { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class DeviceSession
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid DeviceId { get; set; }
    public DateTimeOffset SessionStart { get; set; }
    public DateTimeOffset SessionEnd { get; set; }
    public int ActiveMinutes { get; set; }
    public int IdleMinutes { get; set; }
    public decimal ActivePercentage { get; set; }
}

public class WorkSchedule
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LegalEntityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public bool PullPublicHolidays { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public bool DefaultForNewEmployee { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class WorkScheduleDay
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid WorkScheduleId { get; set; }
    public int DayOfWeek { get; set; }
    public bool IsWorkingDay { get; set; }
    public string WorkTimeType { get; set; } = string.Empty;
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public int? RequiredWorkMinutes { get; set; }
    public string BreakType { get; set; } = string.Empty;
    public string? BreakStartTime { get; set; }
    public string? BreakEndTime { get; set; }
    public int? BreakDurationMinutes { get; set; }
    public string? ExpectedWorkArea { get; set; }
    public bool IsOvernight { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class WorkScheduleHoliday
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid WorkScheduleId { get; set; }
    public Guid? PublicHolidayId { get; set; }
    public DateOnly Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class ScheduleAssignment
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LegalEntityId { get; set; }
    public Guid WorkScheduleId { get; set; }
    public string AssignmentType { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsDefaultForNewEmployee { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class PublicHoliday
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid CountryId { get; set; }
    public DateOnly Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
}

public class Shift
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int BreakMinutes { get; set; }
    public bool IsOvernight { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class ShiftAssignment
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ShiftId { get; set; }
    public DateOnly Date { get; set; }
    public string? ExpectedWorkArea { get; set; }
    public bool IsOverride { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class RosterPeriod
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class RosterEntry
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid RosterPeriodId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ShiftId { get; set; }
    public DateOnly Date { get; set; }
    public string? ExpectedWorkArea { get; set; }
}

public class WorkAreaChangeRequest
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LegalEntityId { get; set; }
    public DateOnly Date { get; set; }
    public Guid? ShiftAssignmentId { get; set; }
    public string CurrentExpectedWorkArea { get; set; } = string.Empty;
    public string RequestedWorkArea { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; set; }
    public Guid? ReviewedById { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }
}

public class ClockInPolicy
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LegalEntityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ScopeType { get; set; } = string.Empty;
    public Guid? DepartmentIds { get; set; }
    public Guid? PositionIds { get; set; }
    public Guid? EmployeeIds { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool ClockInRequired { get; set; }
    public bool LocationVerificationRequired { get; set; }
    public int? AllowedRadiusMeters { get; set; }
    public bool OnsiteBiometricEnabled { get; set; }
    public bool OnsiteWebEnabled { get; set; }
    public bool OnsiteTrayEnabled { get; set; }
    public bool OnsitePhotoRequired { get; set; }
    public bool RemoteBiometricEnabled { get; set; }
    public bool RemoteWebEnabled { get; set; }
    public bool RemoteTrayEnabled { get; set; }
    public bool RemotePhotoRequired { get; set; }
    public bool EitherBiometricEnabled { get; set; }
    public bool EitherWebEnabled { get; set; }
    public bool EitherTrayEnabled { get; set; }
    public bool EitherPhotoRequired { get; set; }
    public bool EitherLocationCheckRequired { get; set; }
    public string EitherSourceRule { get; set; } = string.Empty;
    public bool FieldBiometricEnabled { get; set; }
    public bool FieldWebEnabled { get; set; }
    public bool FieldTrayEnabled { get; set; }
    public string? FieldPhotoRequirement { get; set; }
    public bool AllowUnscheduledWorkClockIn { get; set; }
    public bool AllowHolidayWorkClockIn { get; set; }
    public bool CorrectionRequiresApproval { get; set; }
    public bool OutageFallbackEnabled { get; set; }
    public string NotificationRecipientResolver { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class ClockInLateDeductionRule
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClockInPolicyId { get; set; }
    public int LateArrivalMinute { get; set; }
    public decimal Multiplier { get; set; }
    public Guid TimeOffTypeId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class VerificationPolicy
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool RequirePhotoClockIn { get; set; }
    public bool RequirePhotoClockOut { get; set; }
    public bool CameraPhotoVerificationEnabled { get; set; }
    public bool AbsencePhotoCaptureEnabled { get; set; }
    public string PhotoCaptureContextScope { get; set; } = string.Empty;
    public decimal MatchThreshold { get; set; }
    public string ReferenceEnrollmentMode { get; set; } = string.Empty;
    public bool BlockMonitoringUntilReferenceApproved { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class VerificationRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTimeOffset VerifiedAt { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal MatchConfidence { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? AgentId { get; set; }
    public Guid? BiometricDeviceId { get; set; }
    public string? FailureReason { get; set; }
    public string Trigger { get; set; } = string.Empty;
    public Guid? RequestedById { get; set; }
    public Guid? AlertId { get; set; }
    public DateTimeOffset? RequestedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public int? ResponseDurationSeconds { get; set; }
    public Guid? ReviewedById { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public string? ReviewStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class VerificationEvidenceAsset
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? VerificationRecordId { get; set; }
    public Guid? PresenceSessionId { get; set; }
    public Guid? AttendanceEventId { get; set; }
    public Guid? BiometricEventId { get; set; }
    public Guid FileRecordId { get; set; }
    public string EvidenceType { get; set; } = string.Empty;
    public string TriggerType { get; set; } = string.Empty;
    public DateTimeOffset CapturedAt { get; set; }
    public Guid? AgentId { get; set; }
    public Guid? BiometricDeviceId { get; set; }
    public Guid? RetentionPolicyId { get; set; }
    public Guid? LegalHoldId { get; set; }
    public string Metadata { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class VerificationReferencePhoto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid PhotoFileId { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? CapturedDeviceId { get; set; }
    public DateTimeOffset CapturedAt { get; set; }
    public Guid? ReviewedById { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }
    public Guid LegalAcceptanceRecordId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class BiometricDevice
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid LegalEntityId { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string ConnectionMethod { get; set; } = string.Empty;
    public string? WebhookUrl { get; set; }
    public string? VendorMiddlewareUrl { get; set; }
    public string ExternalDeviceRef { get; set; } = string.Empty;
    public byte[] ApiKeyEncrypted { get; set; } = Array.Empty<byte>();
    public string SupportedAuthMethods { get; set; } = string.Empty;
    public string EnabledAuthMethods { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset LastHeartbeatAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class BiometricEnrollment
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid BiometricDeviceId { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
    public bool ConsentGiven { get; set; }
    public string Modality { get; set; } = string.Empty;
    public string TemplateHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class BiometricEvent
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid BiometricDeviceId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string AuthMethod { get; set; } = string.Empty;
    public string? Modality { get; set; }
    public DateTimeOffset CapturedAt { get; set; }
    public bool Verified { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class BiometricAuditLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid BiometricDeviceId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string DetailsJson { get; set; } = string.Empty;
    public DateTimeOffset RecordedAt { get; set; }
}

public class DailyEmployeeReport
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public decimal TotalHours { get; set; }
    public decimal ActiveHours { get; set; }
    public decimal IdleHours { get; set; }
    public decimal MeetingHours { get; set; }
    public decimal ActivePercentage { get; set; }
    public decimal ProductiveAppHours { get; set; }
    public decimal FocusHours { get; set; }
    public decimal ActivityScore { get; set; }
    public decimal? WorkOutputScore { get; set; }
    public decimal ProductivityScore { get; set; }
    public string ProductivityScoreBasis { get; set; } = string.Empty;
    public decimal DataCoveragePercentage { get; set; }
    public string TopAppsJson { get; set; } = string.Empty;
    public decimal IntensityScore { get; set; }
    public string DeviceSplitJson { get; set; } = string.Empty;
    public int ExceptionsCount { get; set; }
    public string AnomalyFlagsJson { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class WeeklyEmployeeReport
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly WeekStart { get; set; }
    public decimal TotalHours { get; set; }
    public decimal ActiveHours { get; set; }
    public decimal IdleHours { get; set; }
    public decimal MeetingHours { get; set; }
    public decimal ActivePercentage { get; set; }
    public decimal ProductiveAppHours { get; set; }
    public decimal FocusHours { get; set; }
    public decimal ActivityScoreAvg { get; set; }
    public decimal? WorkOutputScoreAvg { get; set; }
    public decimal ProductivityScore { get; set; }
    public string ProductivityScoreBasis { get; set; } = string.Empty;
    public decimal DataCoveragePercentage { get; set; }
    public decimal IntensityAvg { get; set; }
    public int ExceptionsCount { get; set; }
    public string TrendVsPreviousWeekJson { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class MonthlyEmployeeReport
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalHours { get; set; }
    public decimal ActiveHours { get; set; }
    public decimal IdleHours { get; set; }
    public decimal MeetingHours { get; set; }
    public decimal ActivePercentage { get; set; }
    public decimal ProductiveAppHours { get; set; }
    public decimal FocusHours { get; set; }
    public decimal ActivityScoreAvg { get; set; }
    public decimal? WorkOutputScoreAvg { get; set; }
    public decimal ProductivityScore { get; set; }
    public string ProductivityScoreBasis { get; set; } = string.Empty;
    public decimal DataCoveragePercentage { get; set; }
    public decimal IntensityAvg { get; set; }
    public int ExceptionsCount { get; set; }
    public string PerformancePatternJson { get; set; } = string.Empty;
    public int ComparativeRankInDepartment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class MonitoringSnapshot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public DateOnly Date { get; set; }
    public int TotalEmployees { get; set; }
    public int ActiveCount { get; set; }
    public decimal AvgActivePercentage { get; set; }
    public decimal AvgActivityScore { get; set; }
    public decimal? AvgWorkOutputScore { get; set; }
    public decimal AvgProductivityScore { get; set; }
    public decimal AvgDataCoveragePercentage { get; set; }
    public decimal AvgMeetingPercentage { get; set; }
    public int TotalExceptions { get; set; }
    public string TopExceptionTypesJson { get; set; } = string.Empty;
    public string DepartmentBreakdownJson { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class WmsProductivitySnapshot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public string PeriodType { get; set; } = string.Empty;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksOnTime { get; set; }
    public decimal OnTimeDeliveryRate { get; set; }
    public decimal WorkOutputScore { get; set; }
    public decimal ProductivityScore { get; set; }
    public int ActiveProjectsCount { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class Workspace
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class WorkspaceRole
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
}

public class WorkspaceMember
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid UserId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid WorkspaceRoleId { get; set; }
    public Guid? InvitedById { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? RemovedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class Project
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid OwningLegalEntityId { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? LeadId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? TargetDate { get; set; }
    public string? IconUrl { get; set; }
    public string? Color { get; set; }
    public bool IsPrivate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string MembershipSource { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? RemovedAt { get; set; }
}

public class ProjectMemberInvitation
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TenantId { get; set; }
    public Guid InvitedUserId { get; set; }
    public Guid InvitedEmployeeId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid InvitedById { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}

public class ProjectLinkInvitation
{
    public Guid Id { get; set; }
    public Guid SourceProjectId { get; set; }
    public Guid TargetProjectId { get; set; }
    public Guid TenantId { get; set; }
    public decimal AllocatedHours { get; set; }
    public Guid InvitedProjectAdminId { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid InvitedById { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}

public class ProjectLink
{
    public Guid Id { get; set; }
    public Guid SourceProjectId { get; set; }
    public Guid TargetProjectId { get; set; }
    public Guid TenantId { get; set; }
    public decimal AllocatedHours { get; set; }
    public string CreatedVia { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class ProjectVersion
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class ReleaseCalendar
{
    public Guid Id { get; set; }
    public Guid VersionId { get; set; }
    public Guid WorkspaceId { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public string? Notes { get; set; }
}

public class Label
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class ProjectTask
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid TenantId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public Guid? ObjectiveId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int? StoryPoints { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class TaskAssignment
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid AssignedById { get; set; }
    public DateTimeOffset AssignedAt { get; set; }
    public string AvailabilityStatus { get; set; } = string.Empty;
    public DateTimeOffset? AvailabilityCheckedAt { get; set; }
    public string? AvailabilityWarning { get; set; }
}

public class TaskChecklist
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class TaskChecklistItem
{
    public Guid Id { get; set; }
    public Guid ChecklistId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsChecked { get; set; }
    public int Position { get; set; }
    public Guid? CheckedById { get; set; }
    public DateTimeOffset? CheckedAt { get; set; }
}

public class TaskTag
{
    public Guid TaskId { get; set; }
    public Guid LabelId { get; set; }
}

public class TaskApproval
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid RequestedById { get; set; }
    public Guid ApproverId { get; set; }
    public Guid ApproverEmployeeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
}

public class TaskWatcher
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public Guid EmployeeId { get; set; }
}

public class TaskLink
{
    public Guid Id { get; set; }
    public Guid SourceTaskId { get; set; }
    public Guid TargetTaskId { get; set; }
    public string LinkType { get; set; } = string.Empty;
}

public class CustomField
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string? OptionsJson { get; set; }
    public int Position { get; set; }
    public bool IsRequired { get; set; }
}

public class CustomFieldValue
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid FieldId { get; set; }
    public string? ValueText { get; set; }
    public decimal? ValueNumber { get; set; }
    public DateOnly? ValueDate { get; set; }
    public string? ValueJson { get; set; }
}

public class Document
{
    public Guid? WorkspaceId { get; set; }
    public Guid? ProjectId { get; set; }
    public string DocumentScope { get; set; } = string.Empty;
    public DateTimeOffset? LockedAt { get; set; }
    public Guid? LockedBy { get; set; }
    public Guid? ApprovedVersionId { get; set; }
}

public class DocumentVersion
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public int VersionNumber { get; set; }
    public string ContentSnapshot { get; set; } = string.Empty;
    public string? ChangeSummary { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class DocumentApproval
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid RequestedById { get; set; }
    public Guid ApproverId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
}

public class WikiPage
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentPageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid? LastEditedBy { get; set; }
    public int VersionNumber { get; set; }
    public bool IsPublished { get; set; }
    public int Position { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class TaskDocument
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid DocumentId { get; set; }
    public Guid LinkedById { get; set; }
    public DateTimeOffset LinkedAt { get; set; }
}

public class RegisteredAgent
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string OsVersion { get; set; } = string.Empty;
    public string AgentVersion { get; set; } = string.Empty;
    public DateTimeOffset RegisteredAt { get; set; }
    public DateTimeOffset LastHeartbeatAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // Added for the tray app device pairing flow (2026-07-09-device-pairing-flow-design.md).
    // Hash of the opaque bearer token issued to the tray app at enrollment; null for agents
    // registered through any other future path.
    public string? DeviceTokenHash { get; set; }
}

public class AgentSession
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid TenantId { get; set; }
    public Guid EmployeeId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
}

public class AgentCommand
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public Guid TenantId { get; set; }
    public string CommandType { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset DeliveredAt { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
    public string ResultJson { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}

public class AgentHealthLog
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public Guid TenantId { get; set; }
    public DateTimeOffset ReportedAt { get; set; }
    public decimal CpuUsage { get; set; }
    public int MemoryMb { get; set; }
    public string ErrorsJson { get; set; } = string.Empty;
    public bool TamperDetected { get; set; }
}

public class AgentPolicy
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public Guid TenantId { get; set; }
    public string PolicyJson { get; set; } = string.Empty;
    public DateTimeOffset LastSyncedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AgentWorkLocationEvidence
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid AgentId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? PresenceSessionId { get; set; }
    public DateTimeOffset CapturedAt { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public string PublicIp { get; set; } = string.Empty;
    public string? LocalIp { get; set; }
    public string? WifiSsid { get; set; }
    public string? WifiBssidHash { get; set; }
    public string? GatewayMacHash { get; set; }
    public bool VpnDetected { get; set; }
    public string? CoarseLocationJson { get; set; }
    public string MatchStatus { get; set; } = string.Empty;
    public string Confidence { get; set; } = string.Empty;
    public string? MatchedLocationSource { get; set; }
    public Guid? MatchedLocationSourceId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class ApiKey
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;
    public DateTimeOffset? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
}

public class ComplianceExport
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid RequestedById { get; set; }
    public string ExportType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public Guid TargetUserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}

public class EscalationRule
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public string TriggerCondition { get; set; } = string.Empty;
    public int SlaHours { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public Guid? EscalateToRoleId { get; set; }
    public Guid NotificationTemplateId { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class GlobalAppCatalog
{
    public Guid Id { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool IsProductiveDefault { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class FeatureFlagOverride
{
    public Guid Id { get; set; }
    public string FlagKey { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public bool Value { get; set; }
    public Guid GrantedById { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
    public string? Reason { get; set; }
}

public class LegalHold
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid PlacedById { get; set; }
    public DateTimeOffset PlacedAt { get; set; }
    public Guid? ReleasedById { get; set; }
    public DateTimeOffset? ReleasedAt { get; set; }
}

public class NotificationChannel
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ChannelType { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    /// <summary>Non-secret provider metadata (fromEmail, fromName, replyToEmail).</summary>
    public string ConfigJson { get; set; } = "{}";
    /// <summary>Data Protection-encrypted provider secret (e.g. the raw SendGrid API key). Never logged, never returned by any API.</summary>
    public string CredentialsEncrypted { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid ConfiguredById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class NotificationTemplate
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class EmailDeliveryLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? NotificationTemplateId { get; set; }
    public Guid? NotificationChannelId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string SubjectSnapshot { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string? ProviderMessageId { get; set; }
    public string? ProviderEventId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public string? LastError { get; set; }
    public string? BodyHtmlSnapshot { get; set; }
    public string? BodyTextSnapshot { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? BouncedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class SupportTicket
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public Guid? AssignedToId { get; set; }
    public DateTimeOffset? LastCustomerReplyAt { get; set; }
    public DateTimeOffset? LastPlatformReplyAt { get; set; }
    public DateTimeOffset LastActivityAt { get; set; }
    public Guid? ResolvedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
}

public class SupportTicketMessage
{
    public Guid Id { get; set; }
    public Guid SupportTicketId { get; set; }
    public Guid TenantId { get; set; }
    public string SenderType { get; set; } = string.Empty;
    public Guid? SenderUserId { get; set; }
    public Guid? SenderPlatformUserId { get; set; }
    public string MessageBody { get; set; } = string.Empty;
    public string MessageFormat { get; set; } = string.Empty;
    public bool IsCustomerVisible { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? EditedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public class SupportTicketInternalNote
{
    public Guid Id { get; set; }
    public Guid SupportTicketId { get; set; }
    public Guid TenantId { get; set; }
    public Guid AuthorPlatformUserId { get; set; }
    public string NoteBody { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? EditedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public class SupportTicketEvent
{
    public Guid Id { get; set; }
    public Guid SupportTicketId { get; set; }
    public Guid TenantId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string ActorType { get; set; } = string.Empty;
    public Guid? ActorUserId { get; set; }
    public Guid? ActorPlatformUserId { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? MetadataJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class PaymentMethod
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string LastFour { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
    public string PaymentProviderRef { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class PlanFeature
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public string FeatureKey { get; set; } = string.Empty;
    public int? LimitValue { get; set; }
    public bool IsIncluded { get; set; }
}

public class SubscriptionPlanPriceHistory
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public decimal? OldMonthlyPrice { get; set; }
    public decimal? NewMonthlyPrice { get; set; }
    public decimal? OldAnnualPrice { get; set; }
    public decimal? NewAnnualPrice { get; set; }
    public string? OldCurrency { get; set; }
    public string NewCurrency { get; set; } = string.Empty;
    public string? OldPricingUnit { get; set; }
    public string NewPricingUnit { get; set; } = string.Empty;
    public Guid ChangedById { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTimeOffset ChangedAt { get; set; }
}

public class ModuleCatalogPriceHistory
{
    public Guid Id { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string? OldPricingReference { get; set; }
    public string? NewPricingReference { get; set; }
    public string? OldStorageReference { get; set; }
    public string? NewStorageReference { get; set; }
    public string? OldAiTokenReference { get; set; }
    public string? NewAiTokenReference { get; set; }
    public string? OldPricingUnit { get; set; }
    public string NewPricingUnit { get; set; } = string.Empty;
    public Guid ChangedById { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTimeOffset ChangedAt { get; set; }
}

public class TenantSubscriptionEvent
{
    public Guid Id { get; set; }
    public Guid TenantSubscriptionId { get; set; }
    public Guid TenantId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Guid? ActorUserId { get; set; }
    public Guid? ActorPlatformUserId { get; set; }
    public string? OldValuesJson { get; set; }
    public string NewValuesJson { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class BillingSnapshot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public DateOnly SnapshotDate { get; set; }
    public int ActiveEmployeeCount { get; set; }
    public int EnrolledDeviceCount { get; set; }
    public string EmployeeBreakdown { get; set; } = string.Empty;
    public string DeviceBreakdown { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class TenantProvisioningValidationResult
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Section { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class SetupService
{
    public Guid Id { get; set; }
    public string ServiceKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ModuleKeysJson { get; set; } = string.Empty;
    public bool AppliesToAllEntitledModules { get; set; }
    public bool IsFree { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class TenantSetupService
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SetupServiceId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsBillable { get; set; }
    public decimal? Price { get; set; }
    public Guid SelectedById { get; set; }
    public Guid? ConfiguredById { get; set; }
    public DateTimeOffset? ConfiguredAt { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class RateLimitRule
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string EndpointPattern { get; set; } = string.Empty;
    public int MaxRequests { get; set; }
    public int WindowSeconds { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class RetentionPolicy
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public int RetentionDays { get; set; }
    public string ActionOnExpiry { get; set; } = string.Empty;
    public string ComplianceFramework { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class ScheduledTask
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset? LastRunAt { get; set; }
    public DateTimeOffset NextRunAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class SignalrConnection
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public DateTimeOffset ConnectedAt { get; set; }
    public DateTimeOffset LastPingAt { get; set; }
    public bool IsActive { get; set; }
}

public class SsoProvider
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ProviderType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public byte[] ClientIdEncrypted { get; set; } = Array.Empty<byte>();
    public byte[] ClientSecretEncrypted { get; set; } = Array.Empty<byte>();
    public string MetadataUrl { get; set; } = string.Empty;
    public string DomainHint { get; set; } = string.Empty;
    public bool AutoProvisionUsers { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class SystemSetting
{
    public Guid Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid UpdatedById { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class TenantBranding
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? LogoFileId { get; set; }
    public string PrimaryColor { get; set; } = string.Empty;
    public string AccentColor { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
    public Guid UpdatedById { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class UserPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string PreferenceKey { get; set; } = string.Empty;
    public string PreferenceValue { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}

public class WebhookEndpoint
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string SecretHash { get; set; } = string.Empty;
    public string Events { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class WebhookDelivery
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid WebhookEndpointId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public int ResponseStatus { get; set; }
    public string ResponseBody { get; set; } = string.Empty;
    public int AttemptNumber { get; set; }
    public DateTimeOffset DeliveredAt { get; set; }
}

public class PlatformUserInvite
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string InviteTokenHash { get; set; } = string.Empty;
    public Guid InvitedById { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

