// Auto-generated EF Core configurations for Phase 1 Entities
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("countries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Code).HasMaxLength(3);
        builder.Property(x => x.PhoneCode).HasMaxLength(10);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3);
    }
}

public class FileRecordConfiguration : IEntityTypeConfiguration<FileRecord>
{
    public void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.ToTable("file_records");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.StorageKey).HasMaxLength(700);
        builder.Property(x => x.OriginalFileName).HasMaxLength(255);
        builder.Property(x => x.SafeFileName).HasMaxLength(255);
        builder.Property(x => x.ContentType).HasMaxLength(150);
        builder.Property(x => x.Status).HasMaxLength(30);
    }
}

public class FileUploadReservationConfiguration : IEntityTypeConfiguration<FileUploadReservation>
{
    public void Configure(EntityTypeBuilder<FileUploadReservation> builder)
    {
        builder.ToTable("file_upload_reservations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Status).HasMaxLength(30);
    }
}

public class TenantStorageStatConfiguration : IEntityTypeConfiguration<TenantStorageStat>
{
    public void Configure(EntityTypeBuilder<TenantStorageStat> builder)
    {
        builder.ToTable("tenant_storage_stats");
        builder.HasKey(x => x.TenantId);
        builder.HasIndex(x => x.TenantId);
    }
}

public class EntityAssetConfiguration : IEntityTypeConfiguration<EntityAsset>
{
    public void Configure(EntityTypeBuilder<EntityAsset> builder)
    {
        builder.ToTable("entity_assets");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.OwnerType).HasMaxLength(50);
        builder.Property(x => x.AssetPurpose).HasMaxLength(50);
        builder.Property(x => x.Metadata).HasColumnType("jsonb");
        builder.Property(x => x.CreatedByType).HasMaxLength(30);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Action).HasMaxLength(100);
        builder.Property(x => x.ResourceType).HasMaxLength(50);
        builder.Property(x => x.OldValuesJson).HasColumnType("jsonb");
        builder.Property(x => x.NewValuesJson).HasColumnType("jsonb");
        builder.Property(x => x.IpAddress).HasMaxLength(45);
    }
}

public class FeatureAccessGrantConfiguration : IEntityTypeConfiguration<FeatureAccessGrant>
{
    public void Configure(EntityTypeBuilder<FeatureAccessGrant> builder)
    {
        builder.ToTable("feature_access_grants");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.GranteeType).HasMaxLength(10);
        builder.Property(x => x.Module).HasMaxLength(80);
        builder.Property(x => x.FeatureKey).HasMaxLength(120);
    }
}

public class LegalAcceptanceRecordConfiguration : IEntityTypeConfiguration<LegalAcceptanceRecord>
{
    public void Configure(EntityTypeBuilder<LegalAcceptanceRecord> builder)
    {
        builder.ToTable("legal_acceptance_records");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.DocumentType).HasMaxLength(80);
        builder.Property(x => x.DocumentVersion).HasMaxLength(50);
        builder.Property(x => x.Decision).HasMaxLength(20);
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.Property(x => x.UserAgent).HasMaxLength(500);
        builder.Property(x => x.Source).HasMaxLength(30);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(120);
        builder.Property(x => x.Description).HasMaxLength(255);
        builder.Property(x => x.Module).HasMaxLength(80);
        builder.Property(x => x.FeatureKey).HasMaxLength(120);
    }
}

public class InvitationTokenConfiguration : IEntityTypeConfiguration<InvitationToken>
{
    public void Configure(EntityTypeBuilder<InvitationToken> builder)
    {
        builder.ToTable("invitation_tokens");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.InvitedEmail).HasMaxLength(255);
        builder.Property(x => x.InvitedFullName).HasMaxLength(255);
        builder.Property(x => x.TokenHash).HasMaxLength(128);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.Property(x => x.CompletionMethodsJson).HasColumnType("jsonb");
        builder.Property(x => x.CompletedWith).HasMaxLength(20);
        builder.Property(x => x.AllowedEmailDomainsJson).HasColumnType("jsonb");
    }
}

public class TenantAuthPolicyConfiguration : IEntityTypeConfiguration<TenantAuthPolicy>
{
    public void Configure(EntityTypeBuilder<TenantAuthPolicy> builder)
    {
        builder.ToTable("tenant_auth_policies");
        builder.HasKey(x => x.TenantId);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.AllowedLoginDomainsJson).HasColumnType("jsonb");
    }
}

public class UserExternalIdentityConfiguration : IEntityTypeConfiguration<UserExternalIdentity>
{
    public void Configure(EntityTypeBuilder<UserExternalIdentity> builder)
    {
        builder.ToTable("user_external_identities");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Provider).HasMaxLength(30);
        builder.Property(x => x.ProviderSubject).HasMaxLength(255);
        builder.Property(x => x.ProviderEmail).HasMaxLength(255);
    }
}

public class UserPermissionOverrideConfiguration : IEntityTypeConfiguration<UserPermissionOverride>
{
    public void Configure(EntityTypeBuilder<UserPermissionOverride> builder)
    {
        builder.ToTable("user_permission_overrides");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.GrantType).HasMaxLength(10);
        builder.Property(x => x.Reason).HasMaxLength(255);
    }
}

public class AccessGrantRequestConfiguration : IEntityTypeConfiguration<AccessGrantRequest>
{
    public void Configure(EntityTypeBuilder<AccessGrantRequest> builder)
    {
        builder.ToTable("access_grant_requests");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ActionType).HasMaxLength(30);
        builder.Property(x => x.ApprovalStatus).HasMaxLength(20);
        builder.Property(x => x.DecisionComment).HasMaxLength(500);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TokenHash).HasMaxLength(128);
    }
}

public class UserMfaConfiguration : IEntityTypeConfiguration<UserMfa>
{
    public void Configure(EntityTypeBuilder<UserMfa> builder)
    {
        builder.ToTable("user_mfa");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Method).HasMaxLength(20);
        builder.Property(x => x.SecretEncrypted).HasMaxLength(500);
    }
}

public class MfaRecoveryCodeConfiguration : IEntityTypeConfiguration<MfaRecoveryCode>
{
    public void Configure(EntityTypeBuilder<MfaRecoveryCode> builder)
    {
        builder.ToTable("mfa_recovery_codes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodeHash).HasMaxLength(255);
    }
}

public class PositionAccessTemplateConfiguration : IEntityTypeConfiguration<PositionAccessTemplate>
{
    public void Configure(EntityTypeBuilder<PositionAccessTemplate> builder)
    {
        builder.ToTable("position_access_templates");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EffectiveFromRule).HasMaxLength(30);
        builder.Property(x => x.EffectiveToRule).HasMaxLength(30);
    }
}

public class ManagementCoverageRecordConfiguration : IEntityTypeConfiguration<ManagementCoverageRecord>
{
    public void Configure(EntityTypeBuilder<ManagementCoverageRecord> builder)
    {
        builder.ToTable("management_coverage_records");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.CoveredTargetType).HasMaxLength(20);
        builder.Property(x => x.Source).HasMaxLength(30);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class EmployeeAddresseConfiguration : IEntityTypeConfiguration<EmployeeAddresse>
{
    public void Configure(EntityTypeBuilder<EmployeeAddresse> builder)
    {
        builder.ToTable("employee_addresses");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.AddressType).HasMaxLength(20);
        builder.Property(x => x.AddressJson).HasColumnType("jsonb");
    }
}

public class EmployeeBankDetailConfiguration : IEntityTypeConfiguration<EmployeeBankDetail>
{
    public void Configure(EntityTypeBuilder<EmployeeBankDetail> builder)
    {
        builder.ToTable("employee_bank_details");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.BankName).HasMaxLength(100);
        builder.Property(x => x.BranchName).HasMaxLength(100);
        builder.Property(x => x.RoutingNumber).HasMaxLength(20);
    }
}

public class EmployeeCustomFieldConfiguration : IEntityTypeConfiguration<EmployeeCustomField>
{
    public void Configure(EntityTypeBuilder<EmployeeCustomField> builder)
    {
        builder.ToTable("employee_custom_fields");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.FieldName).HasMaxLength(100);
        builder.Property(x => x.FieldType).HasMaxLength(20);
    }
}

public class EmployeeDependentConfiguration : IEntityTypeConfiguration<EmployeeDependent>
{
    public void Configure(EntityTypeBuilder<EmployeeDependent> builder)
    {
        builder.ToTable("employee_dependents");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Relationship).HasMaxLength(20);
        builder.Property(x => x.Phone).HasMaxLength(20);
    }
}

public class EmployeeEmergencyContactConfiguration : IEntityTypeConfiguration<EmployeeEmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmployeeEmergencyContact> builder)
    {
        builder.ToTable("employee_emergency_contacts");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Relationship).HasMaxLength(30);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(255);
    }
}

public class EmployeeLifecycleEventConfiguration : IEntityTypeConfiguration<EmployeeLifecycleEvent>
{
    public void Configure(EntityTypeBuilder<EmployeeLifecycleEvent> builder)
    {
        builder.ToTable("employee_lifecycle_events");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EventType).HasMaxLength(30);
        builder.Property(x => x.DetailsJson).HasColumnType("jsonb");
    }
}

public class EmployeeQualificationConfiguration : IEntityTypeConfiguration<EmployeeQualification>
{
    public void Configure(EntityTypeBuilder<EmployeeQualification> builder)
    {
        builder.ToTable("employee_qualifications");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.QualificationType).HasMaxLength(20);
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.Institution).HasMaxLength(200);
    }
}

public class EmployeeSalaryHistoryConfiguration : IEntityTypeConfiguration<EmployeeSalaryHistory>
{
    public void Configure(EntityTypeBuilder<EmployeeSalaryHistory> builder)
    {
        builder.ToTable("employee_salary_history");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.BaseSalary).HasPrecision(15, 2);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3);
        builder.Property(x => x.ChangeReason).HasMaxLength(100);
    }
}

public class EmployeeWorkHistoryConfiguration : IEntityTypeConfiguration<EmployeeWorkHistory>
{
    public void Configure(EntityTypeBuilder<EmployeeWorkHistory> builder)
    {
        builder.ToTable("employee_work_history");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.CompanyName).HasMaxLength(200);
        builder.Property(x => x.ReasonForLeaving).HasMaxLength(255);
    }
}

public class EmployeeTransferConfiguration : IEntityTypeConfiguration<EmployeeTransfer>
{
    public void Configure(EntityTypeBuilder<EmployeeTransfer> builder)
    {
        builder.ToTable("employee_transfers");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.Property(x => x.Reason).HasMaxLength(500);
    }
}

public class OffboardingRecordConfiguration : IEntityTypeConfiguration<OffboardingRecord>
{
    public void Configure(EntityTypeBuilder<OffboardingRecord> builder)
    {
        builder.ToTable("offboarding_records");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Reason).HasMaxLength(30);
        builder.Property(x => x.KnowledgeRiskLevel).HasMaxLength(10);
        builder.Property(x => x.PenaltiesJson).HasColumnType("jsonb");
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class EmployeeChecklistTaskConfiguration : IEntityTypeConfiguration<EmployeeChecklistTask>
{
    public void Configure(EntityTypeBuilder<EmployeeChecklistTask> builder)
    {
        builder.ToTable("employee_checklist_tasks");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.LifecycleType).HasMaxLength(20);
        builder.Property(x => x.TaskTitle).HasMaxLength(200);
        builder.Property(x => x.OwnerType).HasMaxLength(30);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class ChecklistTemplateConfiguration : IEntityTypeConfiguration<ChecklistTemplate>
{
    public void Configure(EntityTypeBuilder<ChecklistTemplate> builder)
    {
        builder.ToTable("checklist_templates");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.TemplateType).HasMaxLength(20);
        builder.Property(x => x.TasksJson).HasColumnType("jsonb");
    }
}

public class TimeOffPolicyRuleConfiguration : IEntityTypeConfiguration<TimeOffPolicyRule>
{
    public void Configure(EntityTypeBuilder<TimeOffPolicyRule> builder)
    {
        builder.ToTable("time_off_policy_rules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.AccrualMethod).HasMaxLength(20);
        builder.Property(x => x.ProrationMethod).HasMaxLength(20);
        builder.Property(x => x.CarryForwardExpiry).HasMaxLength(50);
        builder.Property(x => x.RolloverPeriod).HasMaxLength(20);
    }
}

public class TimeOffBalancesAuditConfiguration : IEntityTypeConfiguration<TimeOffBalancesAudit>
{
    public void Configure(EntityTypeBuilder<TimeOffBalancesAudit> builder)
    {
        builder.ToTable("time_off_balances_audit");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ChangeType).HasMaxLength(20);
        builder.Property(x => x.Source).HasMaxLength(30);
        builder.Property(x => x.CalculationSnapshotJson).HasColumnType("jsonb");
        builder.Property(x => x.Reason).HasMaxLength(255);
    }
}

public class CalendarEventParticipantConfiguration : IEntityTypeConfiguration<CalendarEventParticipant>
{
    public void Configure(EntityTypeBuilder<CalendarEventParticipant> builder)
    {
        builder.ToTable("calendar_event_participants");
        builder.HasKey(x => new { x.EventId, x.EmployeeId });
        builder.Property(x => x.ResponseStatus).HasMaxLength(30);
    }
}

public class HolidayCalendarSettingConfiguration : IEntityTypeConfiguration<HolidayCalendarSetting>
{
    public void Configure(EntityTypeBuilder<HolidayCalendarSetting> builder)
    {
        builder.ToTable("holiday_calendar_settings");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.DefaultCountryCode).HasMaxLength(2);
        builder.Property(x => x.OverrideCountryCode).HasMaxLength(2);
        builder.Property(x => x.EffectiveCountryCode).HasMaxLength(2);
        builder.Property(x => x.Provider).HasMaxLength(30);
    }
}

public class TenantSettingConfiguration : IEntityTypeConfiguration<TenantSetting>
{
    public void Configure(EntityTypeBuilder<TenantSetting> builder)
    {
        builder.ToTable("tenant_settings");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Timezone).HasMaxLength(50);
        builder.Property(x => x.DateFormat).HasMaxLength(20);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3);
        builder.Property(x => x.WorkWeekDaysJson).HasColumnType("jsonb");
        builder.Property(x => x.PrivacyMode).HasMaxLength(20);
        builder.Property(x => x.DataRetentionDaysJson).HasColumnType("jsonb");
        builder.Property(x => x.SettingsJson).HasColumnType("jsonb");
    }
}

public class MonitoringFeatureToggleConfiguration : IEntityTypeConfiguration<MonitoringFeatureToggle>
{
    public void Configure(EntityTypeBuilder<MonitoringFeatureToggle> builder)
    {
        builder.ToTable("monitoring_feature_toggles");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
    }
}

public class MonitoringAlertPolicyConfiguration : IEntityTypeConfiguration<MonitoringAlertPolicy>
{
    public void Configure(EntityTypeBuilder<MonitoringAlertPolicy> builder)
    {
        builder.ToTable("monitoring_alert_policy");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.MonitoringAlertRecipientResolver).HasMaxLength(50);
        builder.Property(x => x.MonitoringAlertUnresolvedRoutingAction).HasMaxLength(30);
    }
}

public class AppAllowlistConfiguration : IEntityTypeConfiguration<AppAllowlist>
{
    public void Configure(EntityTypeBuilder<AppAllowlist> builder)
    {
        builder.ToTable("app_allowlists");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ScopeType).HasMaxLength(20);
        builder.Property(x => x.ApplicationName).HasMaxLength(200);
        builder.Property(x => x.ProcessName).HasMaxLength(100);
        builder.Property(x => x.Category).HasMaxLength(50);
        builder.Property(x => x.Source).HasMaxLength(20);
    }
}

public class AppAllowlistAuditConfiguration : IEntityTypeConfiguration<AppAllowlistAudit>
{
    public void Configure(EntityTypeBuilder<AppAllowlistAudit> builder)
    {
        builder.ToTable("app_allowlist_audit");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Action).HasMaxLength(20);
        builder.Property(x => x.OldValueJson).HasColumnType("jsonb");
        builder.Property(x => x.NewValueJson).HasColumnType("jsonb");
    }
}

public class ObservedApplicationConfiguration : IEntityTypeConfiguration<ObservedApplication>
{
    public void Configure(EntityTypeBuilder<ObservedApplication> builder)
    {
        builder.ToTable("observed_applications");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ApplicationName).HasMaxLength(200);
        builder.Property(x => x.ProcessName).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class EmployeeMonitoringOverrideConfiguration : IEntityTypeConfiguration<EmployeeMonitoringOverride>
{
    public void Configure(EntityTypeBuilder<EmployeeMonitoringOverride> builder)
    {
        builder.ToTable("employee_monitoring_overrides");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.OverrideReason).HasMaxLength(255);
    }
}

public class MonitoringPolicyOverrideConfiguration : IEntityTypeConfiguration<MonitoringPolicyOverride>
{
    public void Configure(EntityTypeBuilder<MonitoringPolicyOverride> builder)
    {
        builder.ToTable("monitoring_policy_overrides");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ScopeType).HasMaxLength(30);
        builder.Property(x => x.OverrideReason).HasMaxLength(255);
    }
}

public class EmployeeWorkLocationSettingConfiguration : IEntityTypeConfiguration<EmployeeWorkLocationSetting>
{
    public void Configure(EntityTypeBuilder<EmployeeWorkLocationSetting> builder)
    {
        builder.ToTable("employee_work_location_settings");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.WorkMode).HasMaxLength(30);
    }
}

public class EmployeeRemoteWorkProfileConfiguration : IEntityTypeConfiguration<EmployeeRemoteWorkProfile>
{
    public void Configure(EntityTypeBuilder<EmployeeRemoteWorkProfile> builder)
    {
        builder.ToTable("employee_remote_work_profiles");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.Property(x => x.WifiSsid).HasMaxLength(255);
        builder.Property(x => x.WifiBssidHash).HasMaxLength(100);
        builder.Property(x => x.GatewayMacHash).HasMaxLength(100);
        builder.Property(x => x.CoarseLocationJson).HasColumnType("jsonb");
    }
}

public class RemoteWorkLocationChangeRequestConfiguration : IEntityTypeConfiguration<RemoteWorkLocationChangeRequest>
{
    public void Configure(EntityTypeBuilder<RemoteWorkLocationChangeRequest> builder)
    {
        builder.ToTable("remote_work_location_change_requests");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class ActivitySnapshotConfiguration : IEntityTypeConfiguration<ActivitySnapshot>
{
    public void Configure(EntityTypeBuilder<ActivitySnapshot> builder)
    {
        builder.ToTable("activity_snapshots");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.IntensityScore).HasPrecision(5, 2);
        builder.Property(x => x.ForegroundProcessName).HasMaxLength(100);
    }
}

public class ActivityRawBufferConfiguration : IEntityTypeConfiguration<ActivityRawBuffer>
{
    public void Configure(EntityTypeBuilder<ActivityRawBuffer> builder)
    {
        builder.ToTable("activity_raw_buffer");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");
    }
}

public class ActivityDailySummaryConfiguration : IEntityTypeConfiguration<ActivityDailySummary>
{
    public void Configure(EntityTypeBuilder<ActivityDailySummary> builder)
    {
        builder.ToTable("activity_daily_summary");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ActivePercentage).HasPrecision(5, 2);
        builder.Property(x => x.ActivityScore).HasPrecision(5, 2);
        builder.Property(x => x.DataCoveragePercentage).HasPrecision(5, 2);
        builder.Property(x => x.TopAppsJson).HasColumnType("jsonb");
        builder.Property(x => x.IntensityAvg).HasPrecision(5, 2);
        builder.Property(x => x.DataSource).HasMaxLength(20);
    }
}

public class ApplicationCategoryConfiguration : IEntityTypeConfiguration<ApplicationCategory>
{
    public void Configure(EntityTypeBuilder<ApplicationCategory> builder)
    {
        builder.ToTable("application_categories");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ApplicationNamePattern).HasMaxLength(255);
        builder.Property(x => x.Category).HasMaxLength(100);
    }
}

public class ApplicationUsageConfiguration : IEntityTypeConfiguration<ApplicationUsage>
{
    public void Configure(EntityTypeBuilder<ApplicationUsage> builder)
    {
        builder.ToTable("application_usage");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ApplicationName).HasMaxLength(255);
        builder.Property(x => x.ProcessName).HasMaxLength(100);
        builder.Property(x => x.ApplicationCategory).HasMaxLength(100);
        builder.Property(x => x.WindowTitleHash).HasMaxLength(64);
        builder.Property(x => x.AppCategoryType).HasMaxLength(20);
        builder.Property(x => x.BrowserDomain).HasMaxLength(255);
    }
}

public class DeviceTrackingConfiguration : IEntityTypeConfiguration<DeviceTracking>
{
    public void Configure(EntityTypeBuilder<DeviceTracking> builder)
    {
        builder.ToTable("device_tracking");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.LaptopPercentage).HasPrecision(5, 2);
        builder.Property(x => x.DetectionMethod).HasMaxLength(30);
    }
}

public class MeetingSessionConfiguration : IEntityTypeConfiguration<MeetingSession>
{
    public void Configure(EntityTypeBuilder<MeetingSession> builder)
    {
        builder.ToTable("meeting_sessions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Platform).HasMaxLength(20);
    }
}

public class MonitoringEvidenceAssetConfiguration : IEntityTypeConfiguration<MonitoringEvidenceAsset>
{
    public void Configure(EntityTypeBuilder<MonitoringEvidenceAsset> builder)
    {
        builder.ToTable("monitoring_evidence_assets");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EvidenceType).HasMaxLength(40);
        builder.Property(x => x.Source).HasMaxLength(30);
        builder.Property(x => x.TriggerType).HasMaxLength(20);
        builder.Property(x => x.Metadata).HasColumnType("jsonb");
    }
}

public class BrowserActivityConfiguration : IEntityTypeConfiguration<BrowserActivity>
{
    public void Configure(EntityTypeBuilder<BrowserActivity> builder)
    {
        builder.ToTable("browser_activity");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Domain).HasMaxLength(255);
        builder.Property(x => x.DomainClassification).HasMaxLength(20);
        builder.Property(x => x.Source).HasMaxLength(20);
    }
}

public class DiscrepancyEventConfiguration : IEntityTypeConfiguration<DiscrepancyEvent>
{
    public void Configure(EntityTypeBuilder<DiscrepancyEvent> builder)
    {
        builder.ToTable("discrepancy_events");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Severity).HasMaxLength(20);
        builder.Property(x => x.ZScore).HasPrecision(8, 2);
        builder.Property(x => x.BaselineAvgMinutes).HasPrecision(8, 2);
        builder.Property(x => x.BaselineStddevMinutes).HasPrecision(8, 2);
        builder.Property(x => x.SeverityMethod).HasMaxLength(20);
    }
}

public class WorkManagementDailyTimeLogConfiguration : IEntityTypeConfiguration<WorkManagementDailyTimeLog>
{
    public void Configure(EntityTypeBuilder<WorkManagementDailyTimeLog> builder)
    {
        builder.ToTable("work_management_daily_time_logs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
    }
}

public class EmployeeDiscrepancyBaselineConfiguration : IEntityTypeConfiguration<EmployeeDiscrepancyBaseline>
{
    public void Configure(EntityTypeBuilder<EmployeeDiscrepancyBaseline> builder)
    {
        builder.ToTable("employee_discrepancy_baselines");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.AvgUnaccountedMinutes).HasPrecision(8, 2);
        builder.Property(x => x.StddevUnaccountedMinutes).HasPrecision(8, 2);
    }
}

public class PresenceSessionConfiguration : IEntityTypeConfiguration<PresenceSession>
{
    public void Configure(EntityTypeBuilder<PresenceSession> builder)
    {
        builder.ToTable("presence_sessions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Source).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("attendance_records");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.WorkTimeType).HasMaxLength(20);
        builder.Property(x => x.ExpectedWorkArea).HasMaxLength(10);
        builder.Property(x => x.ScheduleTimezone).HasMaxLength(50);
        builder.Property(x => x.HolidayName).HasMaxLength(100);
        builder.Property(x => x.DetectedWorkArea).HasMaxLength(10);
        builder.Property(x => x.AttendanceSource).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(30);
    }
}

public class AttendanceCorrectionConfiguration : IEntityTypeConfiguration<AttendanceCorrection>
{
    public void Configure(EntityTypeBuilder<AttendanceCorrection> builder)
    {
        builder.ToTable("attendance_corrections");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.CorrectionType).HasMaxLength(30);
        builder.Property(x => x.OriginalBreakJson).HasColumnType("jsonb");
        builder.Property(x => x.RequestedBreakJson).HasColumnType("jsonb");
        builder.Property(x => x.Reason).HasMaxLength(255);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class BreakRecordConfiguration : IEntityTypeConfiguration<BreakRecord>
{
    public void Configure(EntityTypeBuilder<BreakRecord> builder)
    {
        builder.ToTable("break_records");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.BreakType).HasMaxLength(30);
    }
}

public class DeviceSessionConfiguration : IEntityTypeConfiguration<DeviceSession>
{
    public void Configure(EntityTypeBuilder<DeviceSession> builder)
    {
        builder.ToTable("device_sessions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ActivePercentage).HasPrecision(5, 2);
    }
}

public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.ToTable("work_schedules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.CountryCode).HasMaxLength(2);
        builder.Property(x => x.Timezone).HasMaxLength(50);
    }
}

public class WorkScheduleDayConfiguration : IEntityTypeConfiguration<WorkScheduleDay>
{
    public void Configure(EntityTypeBuilder<WorkScheduleDay> builder)
    {
        builder.ToTable("work_schedule_days");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.WorkTimeType).HasMaxLength(20);
        builder.Property(x => x.BreakType).HasMaxLength(20);
        builder.Property(x => x.ExpectedWorkArea).HasMaxLength(10);
    }
}

public class WorkScheduleHolidayConfiguration : IEntityTypeConfiguration<WorkScheduleHoliday>
{
    public void Configure(EntityTypeBuilder<WorkScheduleHoliday> builder)
    {
        builder.ToTable("work_schedule_holidays");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Source).HasMaxLength(30);
    }
}

public class ScheduleAssignmentConfiguration : IEntityTypeConfiguration<ScheduleAssignment>
{
    public void Configure(EntityTypeBuilder<ScheduleAssignment> builder)
    {
        builder.ToTable("schedule_assignments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.AssignmentType).HasMaxLength(30);
    }
}

public class PublicHolidayConfiguration : IEntityTypeConfiguration<PublicHoliday>
{
    public void Configure(EntityTypeBuilder<PublicHoliday> builder)
    {
        builder.ToTable("public_holidays");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
    }
}

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("shifts");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
    }
}

public class ShiftAssignmentConfiguration : IEntityTypeConfiguration<ShiftAssignment>
{
    public void Configure(EntityTypeBuilder<ShiftAssignment> builder)
    {
        builder.ToTable("shift_assignments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ExpectedWorkArea).HasMaxLength(10);
    }
}

public class RosterPeriodConfiguration : IEntityTypeConfiguration<RosterPeriod>
{
    public void Configure(EntityTypeBuilder<RosterPeriod> builder)
    {
        builder.ToTable("roster_periods");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class RosterEntryConfiguration : IEntityTypeConfiguration<RosterEntry>
{
    public void Configure(EntityTypeBuilder<RosterEntry> builder)
    {
        builder.ToTable("roster_entries");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ExpectedWorkArea).HasMaxLength(10);
    }
}

public class WorkAreaChangeRequestConfiguration : IEntityTypeConfiguration<WorkAreaChangeRequest>
{
    public void Configure(EntityTypeBuilder<WorkAreaChangeRequest> builder)
    {
        builder.ToTable("work_area_change_requests");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.CurrentExpectedWorkArea).HasMaxLength(10);
        builder.Property(x => x.RequestedWorkArea).HasMaxLength(10);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class ClockInPolicyConfiguration : IEntityTypeConfiguration<ClockInPolicy>
{
    public void Configure(EntityTypeBuilder<ClockInPolicy> builder)
    {
        builder.ToTable("clock_in_policies");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(120);
        builder.Property(x => x.ScopeType).HasMaxLength(30);
        builder.Property(x => x.EitherSourceRule).HasMaxLength(30);
        builder.Property(x => x.FieldPhotoRequirement).HasMaxLength(20);
        builder.Property(x => x.NotificationRecipientResolver).HasMaxLength(50);
    }
}

public class ClockInLateDeductionRuleConfiguration : IEntityTypeConfiguration<ClockInLateDeductionRule>
{
    public void Configure(EntityTypeBuilder<ClockInLateDeductionRule> builder)
    {
        builder.ToTable("clock_in_late_deduction_rules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Multiplier).HasPrecision(5, 2);
    }
}

public class VerificationPolicyConfiguration : IEntityTypeConfiguration<VerificationPolicy>
{
    public void Configure(EntityTypeBuilder<VerificationPolicy> builder)
    {
        builder.ToTable("verification_policies");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.PhotoCaptureContextScope).HasMaxLength(20);
        builder.Property(x => x.MatchThreshold).HasPrecision(5, 2);
        builder.Property(x => x.ReferenceEnrollmentMode).HasMaxLength(30);
    }
}

public class VerificationRecordConfiguration : IEntityTypeConfiguration<VerificationRecord>
{
    public void Configure(EntityTypeBuilder<VerificationRecord> builder)
    {
        builder.ToTable("verification_records");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Method).HasMaxLength(20);
        builder.Property(x => x.MatchConfidence).HasPrecision(5, 2);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.Property(x => x.FailureReason).HasMaxLength(255);
        builder.Property(x => x.Trigger).HasMaxLength(20);
        builder.Property(x => x.ReviewStatus).HasMaxLength(20);
    }
}

public class VerificationEvidenceAssetConfiguration : IEntityTypeConfiguration<VerificationEvidenceAsset>
{
    public void Configure(EntityTypeBuilder<VerificationEvidenceAsset> builder)
    {
        builder.ToTable("verification_evidence_assets");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EvidenceType).HasMaxLength(40);
        builder.Property(x => x.TriggerType).HasMaxLength(20);
        builder.Property(x => x.Metadata).HasColumnType("jsonb");
    }
}

public class VerificationReferencePhotoConfiguration : IEntityTypeConfiguration<VerificationReferencePhoto>
{
    public void Configure(EntityTypeBuilder<VerificationReferencePhoto> builder)
    {
        builder.ToTable("verification_reference_photos");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Source).HasMaxLength(30);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.Property(x => x.ReviewComment).HasMaxLength(255);
    }
}

public class BiometricDeviceConfiguration : IEntityTypeConfiguration<BiometricDevice>
{
    public void Configure(EntityTypeBuilder<BiometricDevice> builder)
    {
        builder.ToTable("biometric_devices");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.DeviceCode).HasMaxLength(50);
        builder.Property(x => x.DeviceName).HasMaxLength(100);
        builder.Property(x => x.Vendor).HasMaxLength(100);
        builder.Property(x => x.Model).HasMaxLength(100);
        builder.Property(x => x.ConnectionMethod).HasMaxLength(30);
        builder.Property(x => x.WebhookUrl).HasMaxLength(500);
        builder.Property(x => x.VendorMiddlewareUrl).HasMaxLength(500);
        builder.Property(x => x.ExternalDeviceRef).HasMaxLength(100);
        builder.Property(x => x.SupportedAuthMethods).HasColumnType("jsonb");
        builder.Property(x => x.EnabledAuthMethods).HasColumnType("jsonb");
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class BiometricEnrollmentConfiguration : IEntityTypeConfiguration<BiometricEnrollment>
{
    public void Configure(EntityTypeBuilder<BiometricEnrollment> builder)
    {
        builder.ToTable("biometric_enrollments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Modality).HasMaxLength(30);
        builder.Property(x => x.TemplateHash).HasMaxLength(128);
    }
}

public class BiometricEventConfiguration : IEntityTypeConfiguration<BiometricEvent>
{
    public void Configure(EntityTypeBuilder<BiometricEvent> builder)
    {
        builder.ToTable("biometric_events");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EventType).HasMaxLength(20);
        builder.Property(x => x.AuthMethod).HasMaxLength(40);
        builder.Property(x => x.Modality).HasMaxLength(30);
    }
}

public class BiometricAuditLogConfiguration : IEntityTypeConfiguration<BiometricAuditLog>
{
    public void Configure(EntityTypeBuilder<BiometricAuditLog> builder)
    {
        builder.ToTable("biometric_audit_logs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EventType).HasMaxLength(50);
        builder.Property(x => x.DetailsJson).HasColumnType("jsonb");
    }
}

public class DailyEmployeeReportConfiguration : IEntityTypeConfiguration<DailyEmployeeReport>
{
    public void Configure(EntityTypeBuilder<DailyEmployeeReport> builder)
    {
        builder.ToTable("daily_employee_report");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.TotalHours).HasPrecision(5, 2);
        builder.Property(x => x.ActiveHours).HasPrecision(5, 2);
        builder.Property(x => x.IdleHours).HasPrecision(5, 2);
        builder.Property(x => x.MeetingHours).HasPrecision(5, 2);
        builder.Property(x => x.ActivePercentage).HasPrecision(5, 2);
        builder.Property(x => x.ProductiveAppHours).HasPrecision(5, 2);
        builder.Property(x => x.FocusHours).HasPrecision(5, 2);
        builder.Property(x => x.ActivityScore).HasPrecision(5, 2);
        builder.Property(x => x.WorkOutputScore).HasPrecision(5, 2);
        builder.Property(x => x.ProductivityScore).HasPrecision(5, 2);
        builder.Property(x => x.ProductivityScoreBasis).HasMaxLength(30);
        builder.Property(x => x.DataCoveragePercentage).HasPrecision(5, 2);
        builder.Property(x => x.TopAppsJson).HasColumnType("jsonb");
        builder.Property(x => x.IntensityScore).HasPrecision(5, 2);
        builder.Property(x => x.DeviceSplitJson).HasColumnType("jsonb");
        builder.Property(x => x.AnomalyFlagsJson).HasColumnType("jsonb");
    }
}

public class WeeklyEmployeeReportConfiguration : IEntityTypeConfiguration<WeeklyEmployeeReport>
{
    public void Configure(EntityTypeBuilder<WeeklyEmployeeReport> builder)
    {
        builder.ToTable("weekly_employee_report");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.TotalHours).HasPrecision(6, 2);
        builder.Property(x => x.ActiveHours).HasPrecision(6, 2);
        builder.Property(x => x.IdleHours).HasPrecision(6, 2);
        builder.Property(x => x.MeetingHours).HasPrecision(6, 2);
        builder.Property(x => x.ActivePercentage).HasPrecision(5, 2);
        builder.Property(x => x.ProductiveAppHours).HasPrecision(6, 2);
        builder.Property(x => x.FocusHours).HasPrecision(6, 2);
        builder.Property(x => x.ActivityScoreAvg).HasPrecision(5, 2);
        builder.Property(x => x.WorkOutputScoreAvg).HasPrecision(5, 2);
        builder.Property(x => x.ProductivityScore).HasPrecision(5, 2);
        builder.Property(x => x.ProductivityScoreBasis).HasMaxLength(30);
        builder.Property(x => x.DataCoveragePercentage).HasPrecision(5, 2);
        builder.Property(x => x.IntensityAvg).HasPrecision(5, 2);
        builder.Property(x => x.TrendVsPreviousWeekJson).HasColumnType("jsonb");
    }
}

public class MonthlyEmployeeReportConfiguration : IEntityTypeConfiguration<MonthlyEmployeeReport>
{
    public void Configure(EntityTypeBuilder<MonthlyEmployeeReport> builder)
    {
        builder.ToTable("monthly_employee_report");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.TotalHours).HasPrecision(7, 2);
        builder.Property(x => x.ActiveHours).HasPrecision(7, 2);
        builder.Property(x => x.IdleHours).HasPrecision(7, 2);
        builder.Property(x => x.MeetingHours).HasPrecision(7, 2);
        builder.Property(x => x.ActivePercentage).HasPrecision(5, 2);
        builder.Property(x => x.ProductiveAppHours).HasPrecision(7, 2);
        builder.Property(x => x.FocusHours).HasPrecision(7, 2);
        builder.Property(x => x.ActivityScoreAvg).HasPrecision(5, 2);
        builder.Property(x => x.WorkOutputScoreAvg).HasPrecision(5, 2);
        builder.Property(x => x.ProductivityScore).HasPrecision(5, 2);
        builder.Property(x => x.ProductivityScoreBasis).HasMaxLength(30);
        builder.Property(x => x.DataCoveragePercentage).HasPrecision(5, 2);
        builder.Property(x => x.IntensityAvg).HasPrecision(5, 2);
        builder.Property(x => x.PerformancePatternJson).HasColumnType("jsonb");
    }
}

public class MonitoringSnapshotConfiguration : IEntityTypeConfiguration<MonitoringSnapshot>
{
    public void Configure(EntityTypeBuilder<MonitoringSnapshot> builder)
    {
        builder.ToTable("monitoring_snapshot");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.AvgActivePercentage).HasPrecision(5, 2);
        builder.Property(x => x.AvgActivityScore).HasPrecision(5, 2);
        builder.Property(x => x.AvgWorkOutputScore).HasPrecision(5, 2);
        builder.Property(x => x.AvgProductivityScore).HasPrecision(5, 2);
        builder.Property(x => x.AvgDataCoveragePercentage).HasPrecision(5, 2);
        builder.Property(x => x.AvgMeetingPercentage).HasPrecision(5, 2);
        builder.Property(x => x.TopExceptionTypesJson).HasColumnType("jsonb");
        builder.Property(x => x.DepartmentBreakdownJson).HasColumnType("jsonb");
    }
}

public class WmsProductivitySnapshotConfiguration : IEntityTypeConfiguration<WmsProductivitySnapshot>
{
    public void Configure(EntityTypeBuilder<WmsProductivitySnapshot> builder)
    {
        builder.ToTable("wms_productivity_snapshots");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.PeriodType).HasMaxLength(10);
        builder.Property(x => x.OnTimeDeliveryRate).HasPrecision(5, 2);
        builder.Property(x => x.WorkOutputScore).HasPrecision(5, 2);
        builder.Property(x => x.ProductivityScore).HasPrecision(5, 2);
    }
}

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("workspaces");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Slug).HasMaxLength(100);
        builder.Property(x => x.IconUrl).HasMaxLength(500);
        builder.Property(x => x.Timezone).HasMaxLength(50);
    }
}

public class WorkspaceRoleConfiguration : IEntityTypeConfiguration<WorkspaceRole>
{
    public void Configure(EntityTypeBuilder<WorkspaceRole> builder)
    {
        builder.ToTable("workspace_roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50);
    }
}

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("workspace_members");
        builder.HasKey(x => x.Id);
    }
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.Property(x => x.IconUrl).HasMaxLength(500);
        builder.Property(x => x.Color).HasMaxLength(20);
    }
}

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("project_members");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Role).HasMaxLength(20);
        builder.Property(x => x.MembershipSource).HasMaxLength(20);
    }
}

public class ProjectMemberInvitationConfiguration : IEntityTypeConfiguration<ProjectMemberInvitation>
{
    public void Configure(EntityTypeBuilder<ProjectMemberInvitation> builder)
    {
        builder.ToTable("project_member_invitations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Role).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class ProjectLinkInvitationConfiguration : IEntityTypeConfiguration<ProjectLinkInvitation>
{
    public void Configure(EntityTypeBuilder<ProjectLinkInvitation> builder)
    {
        builder.ToTable("project_link_invitations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.AllocatedHours).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class ProjectLinkConfiguration : IEntityTypeConfiguration<ProjectLink>
{
    public void Configure(EntityTypeBuilder<ProjectLink> builder)
    {
        builder.ToTable("project_links");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.AllocatedHours).HasPrecision(18, 2);
        builder.Property(x => x.CreatedVia).HasMaxLength(20);
    }
}

public class ProjectVersionConfiguration : IEntityTypeConfiguration<ProjectVersion>
{
    public void Configure(EntityTypeBuilder<ProjectVersion> builder)
    {
        builder.ToTable("versions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class ReleaseCalendarConfiguration : IEntityTypeConfiguration<ReleaseCalendar>
{
    public void Configure(EntityTypeBuilder<ReleaseCalendar> builder)
    {
        builder.ToTable("release_calendar");
        builder.HasKey(x => x.Id);
    }
}

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("labels");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Color).HasMaxLength(20);
    }
}

public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("tasks");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Title).HasMaxLength(500);
        builder.Property(x => x.TaskType).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.Property(x => x.Priority).HasMaxLength(20);
    }
}

public class TaskAssignmentConfiguration : IEntityTypeConfiguration<TaskAssignment>
{
    public void Configure(EntityTypeBuilder<TaskAssignment> builder)
    {
        builder.ToTable("task_assignments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AvailabilityStatus).HasMaxLength(20);
    }
}

public class TaskChecklistConfiguration : IEntityTypeConfiguration<TaskChecklist>
{
    public void Configure(EntityTypeBuilder<TaskChecklist> builder)
    {
        builder.ToTable("task_checklists");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(255);
    }
}

public class TaskChecklistItemConfiguration : IEntityTypeConfiguration<TaskChecklistItem>
{
    public void Configure(EntityTypeBuilder<TaskChecklistItem> builder)
    {
        builder.ToTable("task_checklist_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Text).HasMaxLength(500);
    }
}

public class TaskTagConfiguration : IEntityTypeConfiguration<TaskTag>
{
    public void Configure(EntityTypeBuilder<TaskTag> builder)
    {
        builder.ToTable("task_tags");
        builder.HasKey(x => new { x.TaskId, x.LabelId });
    }
}

public class TaskApprovalConfiguration : IEntityTypeConfiguration<TaskApproval>
{
    public void Configure(EntityTypeBuilder<TaskApproval> builder)
    {
        builder.ToTable("task_approvals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class TaskWatcherConfiguration : IEntityTypeConfiguration<TaskWatcher>
{
    public void Configure(EntityTypeBuilder<TaskWatcher> builder)
    {
        builder.ToTable("task_watchers");
        builder.HasKey(x => new { x.TaskId, x.UserId, x.EmployeeId });
    }
}

public class TaskLinkConfiguration : IEntityTypeConfiguration<TaskLink>
{
    public void Configure(EntityTypeBuilder<TaskLink> builder)
    {
        builder.ToTable("task_links");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LinkType).HasMaxLength(30);
    }
}

public class CustomFieldConfiguration : IEntityTypeConfiguration<CustomField>
{
    public void Configure(EntityTypeBuilder<CustomField> builder)
    {
        builder.ToTable("custom_fields");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.FieldType).HasMaxLength(20);
        builder.Property(x => x.OptionsJson).HasColumnType("jsonb");
    }
}

public class CustomFieldValueConfiguration : IEntityTypeConfiguration<CustomFieldValue>
{
    public void Configure(EntityTypeBuilder<CustomFieldValue> builder)
    {
        builder.ToTable("custom_field_values");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ValueNumber).HasPrecision(18, 4);
        builder.Property(x => x.ValueJson).HasColumnType("jsonb");
    }
}

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");
        builder.HasKey(x => new { x.WorkspaceId, x.ProjectId, x.ApprovedVersionId });
        builder.Property(x => x.DocumentScope).HasMaxLength(30);
    }
}

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("document_versions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChangeSummary).HasMaxLength(500);
    }
}

public class DocumentApprovalConfiguration : IEntityTypeConfiguration<DocumentApproval>
{
    public void Configure(EntityTypeBuilder<DocumentApproval> builder)
    {
        builder.ToTable("document_approvals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class WikiPageConfiguration : IEntityTypeConfiguration<WikiPage>
{
    public void Configure(EntityTypeBuilder<WikiPage> builder)
    {
        builder.ToTable("wiki_pages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(255);
    }
}

public class TaskDocumentConfiguration : IEntityTypeConfiguration<TaskDocument>
{
    public void Configure(EntityTypeBuilder<TaskDocument> builder)
    {
        builder.ToTable("task_documents");
        builder.HasKey(x => x.Id);
    }
}

public class RegisteredAgentConfiguration : IEntityTypeConfiguration<RegisteredAgent>
{
    public void Configure(EntityTypeBuilder<RegisteredAgent> builder)
    {
        builder.ToTable("registered_agents");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.DeviceName).HasMaxLength(100);
        builder.Property(x => x.OsVersion).HasMaxLength(50);
        builder.Property(x => x.AgentVersion).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(20);
    }
}

public class AgentSessionConfiguration : IEntityTypeConfiguration<AgentSession>
{
    public void Configure(EntityTypeBuilder<AgentSession> builder)
    {
        builder.ToTable("agent_sessions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
    }
}

public class AgentCommandConfiguration : IEntityTypeConfiguration<AgentCommand>
{
    public void Configure(EntityTypeBuilder<AgentCommand> builder)
    {
        builder.ToTable("agent_commands");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.CommandType).HasMaxLength(50);
        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.Property(x => x.ResultJson).HasColumnType("jsonb");
    }
}

public class AgentHealthLogConfiguration : IEntityTypeConfiguration<AgentHealthLog>
{
    public void Configure(EntityTypeBuilder<AgentHealthLog> builder)
    {
        builder.ToTable("agent_health_logs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.CpuUsage).HasPrecision(5, 2);
        builder.Property(x => x.ErrorsJson).HasColumnType("jsonb");
    }
}

public class AgentPolicyConfiguration : IEntityTypeConfiguration<AgentPolicy>
{
    public void Configure(EntityTypeBuilder<AgentPolicy> builder)
    {
        builder.ToTable("agent_policies");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.PolicyJson).HasColumnType("jsonb");
    }
}

public class AgentWorkLocationEvidenceConfiguration : IEntityTypeConfiguration<AgentWorkLocationEvidence>
{
    public void Configure(EntityTypeBuilder<AgentWorkLocationEvidence> builder)
    {
        builder.ToTable("agent_work_location_evidence");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.WifiSsid).HasMaxLength(255);
        builder.Property(x => x.WifiBssidHash).HasMaxLength(100);
        builder.Property(x => x.GatewayMacHash).HasMaxLength(100);
        builder.Property(x => x.CoarseLocationJson).HasColumnType("jsonb");
        builder.Property(x => x.MatchStatus).HasMaxLength(20);
        builder.Property(x => x.Confidence).HasMaxLength(20);
        builder.Property(x => x.MatchedLocationSource).HasMaxLength(30);
    }
}

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("api_keys");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.KeyHash).HasMaxLength(255);
        builder.Property(x => x.KeyPrefix).HasMaxLength(10);
        builder.Property(x => x.Scopes).HasColumnType("jsonb");
    }
}

public class ComplianceExportConfiguration : IEntityTypeConfiguration<ComplianceExport>
{
    public void Configure(EntityTypeBuilder<ComplianceExport> builder)
    {
        builder.ToTable("compliance_exports");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ExportType).HasMaxLength(30);
        builder.Property(x => x.Scope).HasMaxLength(30);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.Property(x => x.FileUrl).HasMaxLength(500);
    }
}

public class EscalationRuleConfiguration : IEntityTypeConfiguration<EscalationRule>
{
    public void Configure(EntityTypeBuilder<EscalationRule> builder)
    {
        builder.ToTable("escalation_rules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ResourceType).HasMaxLength(50);
        builder.Property(x => x.TriggerCondition).HasMaxLength(100);
        builder.Property(x => x.ActionType).HasMaxLength(30);
    }
}

public class GlobalAppCatalogConfiguration : IEntityTypeConfiguration<GlobalAppCatalog>
{
    public void Configure(EntityTypeBuilder<GlobalAppCatalog> builder)
    {
        builder.ToTable("global_app_catalog");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AppName).HasMaxLength(200);
        builder.Property(x => x.ProcessName).HasMaxLength(100);
        builder.Property(x => x.Category).HasMaxLength(50);
        builder.Property(x => x.Publisher).HasMaxLength(200);
        builder.Property(x => x.IconUrl).HasMaxLength(500);
    }
}

public class FeatureFlagOverrideConfiguration : IEntityTypeConfiguration<FeatureFlagOverride>
{
    public void Configure(EntityTypeBuilder<FeatureFlagOverride> builder)
    {
        builder.ToTable("feature_flag_overrides");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FlagKey).HasMaxLength(120);
        builder.HasIndex(x => x.TenantId);
    }
}

public class LegalHoldConfiguration : IEntityTypeConfiguration<LegalHold>
{
    public void Configure(EntityTypeBuilder<LegalHold> builder)
    {
        builder.ToTable("legal_holds");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ResourceType).HasMaxLength(50);
    }
}

public class NotificationChannelConfiguration : IEntityTypeConfiguration<NotificationChannel>
{
    public void Configure(EntityTypeBuilder<NotificationChannel> builder)
    {
        builder.ToTable("notification_channels");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ChannelType).HasMaxLength(30);
        builder.Property(x => x.Provider).HasMaxLength(50);
        builder.Property(x => x.ConfigJson).HasColumnType("jsonb").HasDefaultValueSql("'{}'");
        // Data Protection payloads are opaque base64 strings, not JSON.
        builder.Property(x => x.CredentialsEncrypted).HasColumnType("text");
    }
}

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("notification_templates");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.TemplateCode).HasMaxLength(50);
        builder.Property(x => x.Channel).HasMaxLength(20);
        builder.Property(x => x.Locale).HasMaxLength(10);
    }
}

public class EmailDeliveryLogConfiguration : IEntityTypeConfiguration<EmailDeliveryLog>
{
    public void Configure(EntityTypeBuilder<EmailDeliveryLog> builder)
    {
        builder.ToTable("email_delivery_logs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.RecipientEmail).HasMaxLength(255);
        builder.Property(x => x.SubjectSnapshot).HasMaxLength(500);
        builder.Property(x => x.Provider).HasMaxLength(50);
        builder.Property(x => x.ProviderMessageId).HasMaxLength(255);
        builder.Property(x => x.ProviderEventId).HasMaxLength(255);
        builder.Property(x => x.Status).HasMaxLength(30);
    }
}

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("support_tickets");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Subject).HasMaxLength(200);
        builder.Property(x => x.Category).HasMaxLength(80);
        builder.Property(x => x.Priority).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(30);
    }
}

public class SupportTicketMessageConfiguration : IEntityTypeConfiguration<SupportTicketMessage>
{
    public void Configure(EntityTypeBuilder<SupportTicketMessage> builder)
    {
        builder.ToTable("support_ticket_messages");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.SenderType).HasMaxLength(30);
        builder.Property(x => x.MessageFormat).HasMaxLength(20);
    }
}

public class SupportTicketInternalNoteConfiguration : IEntityTypeConfiguration<SupportTicketInternalNote>
{
    public void Configure(EntityTypeBuilder<SupportTicketInternalNote> builder)
    {
        builder.ToTable("support_ticket_internal_notes");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
    }
}

public class SupportTicketEventConfiguration : IEntityTypeConfiguration<SupportTicketEvent>
{
    public void Configure(EntityTypeBuilder<SupportTicketEvent> builder)
    {
        builder.ToTable("support_ticket_events");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EventType).HasMaxLength(80);
        builder.Property(x => x.ActorType).HasMaxLength(30);
        builder.Property(x => x.OldValuesJson).HasColumnType("jsonb");
        builder.Property(x => x.NewValuesJson).HasColumnType("jsonb");
        builder.Property(x => x.MetadataJson).HasColumnType("jsonb");
    }
}

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_methods");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Type).HasMaxLength(20);
        builder.Property(x => x.LastFour).HasMaxLength(4);
        builder.Property(x => x.Brand).HasMaxLength(20);
        builder.Property(x => x.PaymentProviderRef).HasMaxLength(100);
    }
}

public class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature>
{
    public void Configure(EntityTypeBuilder<PlanFeature> builder)
    {
        builder.ToTable("plan_features");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FeatureKey).HasMaxLength(100);
    }
}

public class SubscriptionPlanPriceHistoryConfiguration : IEntityTypeConfiguration<SubscriptionPlanPriceHistory>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlanPriceHistory> builder)
    {
        builder.ToTable("subscription_plan_price_history");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OldMonthlyPrice).HasPrecision(10, 2);
        builder.Property(x => x.NewMonthlyPrice).HasPrecision(10, 2);
        builder.Property(x => x.OldAnnualPrice).HasPrecision(10, 2);
        builder.Property(x => x.NewAnnualPrice).HasPrecision(10, 2);
        builder.Property(x => x.OldCurrency).HasMaxLength(3);
        builder.Property(x => x.NewCurrency).HasMaxLength(3);
        builder.Property(x => x.OldPricingUnit).HasMaxLength(30);
        builder.Property(x => x.NewPricingUnit).HasMaxLength(30);
    }
}

public class ModuleCatalogPriceHistoryConfiguration : IEntityTypeConfiguration<ModuleCatalogPriceHistory>
{
    public void Configure(EntityTypeBuilder<ModuleCatalogPriceHistory> builder)
    {
        builder.ToTable("module_catalog_price_history");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ModuleKey).HasMaxLength(100);
        builder.Property(x => x.OldPricingReference).HasColumnType("jsonb");
        builder.Property(x => x.NewPricingReference).HasColumnType("jsonb");
        builder.Property(x => x.OldStorageReference).HasColumnType("jsonb");
        builder.Property(x => x.NewStorageReference).HasColumnType("jsonb");
        builder.Property(x => x.OldAiTokenReference).HasColumnType("jsonb");
        builder.Property(x => x.NewAiTokenReference).HasColumnType("jsonb");
        builder.Property(x => x.OldPricingUnit).HasMaxLength(30);
        builder.Property(x => x.NewPricingUnit).HasMaxLength(30);
    }
}

public class TenantSubscriptionEventConfiguration : IEntityTypeConfiguration<TenantSubscriptionEvent>
{
    public void Configure(EntityTypeBuilder<TenantSubscriptionEvent> builder)
    {
        builder.ToTable("tenant_subscription_events");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EventType).HasMaxLength(80);
        builder.Property(x => x.OldValuesJson).HasColumnType("jsonb");
        builder.Property(x => x.NewValuesJson).HasColumnType("jsonb");
    }
}

public class BillingSnapshotConfiguration : IEntityTypeConfiguration<BillingSnapshot>
{
    public void Configure(EntityTypeBuilder<BillingSnapshot> builder)
    {
        builder.ToTable("billing_snapshots");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EmployeeBreakdown).HasColumnType("jsonb");
        builder.Property(x => x.DeviceBreakdown).HasColumnType("jsonb");
    }
}

public class TenantProvisioningValidationResultConfiguration : IEntityTypeConfiguration<TenantProvisioningValidationResult>
{
    public void Configure(EntityTypeBuilder<TenantProvisioningValidationResult> builder)
    {
        builder.ToTable("tenant_provisioning_validation_results");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Section).HasMaxLength(50);
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Severity).HasMaxLength(20);
    }
}

public class SetupServiceConfiguration : IEntityTypeConfiguration<SetupService>
{
    public void Configure(EntityTypeBuilder<SetupService> builder)
    {
        builder.ToTable("setup_services");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ServiceKey).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(150);
        builder.Property(x => x.ModuleKeysJson).HasColumnType("jsonb");
        builder.Property(x => x.Price).HasPrecision(12, 2);
        builder.Property(x => x.Currency).HasMaxLength(3);
    }
}

public class TenantSetupServiceConfiguration : IEntityTypeConfiguration<TenantSetupService>
{
    public void Configure(EntityTypeBuilder<TenantSetupService> builder)
    {
        builder.ToTable("tenant_setup_services");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ModuleKey).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.Property(x => x.Price).HasPrecision(12, 2);
    }
}

public class RateLimitRuleConfiguration : IEntityTypeConfiguration<RateLimitRule>
{
    public void Configure(EntityTypeBuilder<RateLimitRule> builder)
    {
        builder.ToTable("rate_limit_rules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EndpointPattern).HasMaxLength(200);
    }
}

public class RetentionPolicyConfiguration : IEntityTypeConfiguration<RetentionPolicy>
{
    public void Configure(EntityTypeBuilder<RetentionPolicy> builder)
    {
        builder.ToTable("retention_policies");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ResourceType).HasMaxLength(50);
        builder.Property(x => x.ActionOnExpiry).HasMaxLength(30);
        builder.Property(x => x.ComplianceFramework).HasMaxLength(50);
    }
}

public class ScheduledTaskConfiguration : IEntityTypeConfiguration<ScheduledTask>
{
    public void Configure(EntityTypeBuilder<ScheduledTask> builder)
    {
        builder.ToTable("scheduled_tasks");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.TaskType).HasMaxLength(100);
        builder.Property(x => x.CronExpression).HasMaxLength(50);
    }
}

public class SignalrConnectionConfiguration : IEntityTypeConfiguration<SignalrConnection>
{
    public void Configure(EntityTypeBuilder<SignalrConnection> builder)
    {
        builder.ToTable("signalr_connections");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ConnectionId).HasMaxLength(100);
        builder.Property(x => x.Channel).HasMaxLength(30);
        builder.Property(x => x.DeviceType).HasMaxLength(30);
    }
}

public class SsoProviderConfiguration : IEntityTypeConfiguration<SsoProvider>
{
    public void Configure(EntityTypeBuilder<SsoProvider> builder)
    {
        builder.ToTable("sso_providers");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.ProviderType).HasMaxLength(30);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.MetadataUrl).HasMaxLength(500);
        builder.Property(x => x.DomainHint).HasMaxLength(100);
    }
}

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("system_settings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SettingKey).HasMaxLength(100);
        builder.Property(x => x.SettingValue).HasColumnType("jsonb");
    }
}

public class TenantBrandingConfiguration : IEntityTypeConfiguration<TenantBranding>
{
    public void Configure(EntityTypeBuilder<TenantBranding> builder)
    {
        builder.ToTable("tenant_branding");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.PrimaryColor).HasMaxLength(7);
        builder.Property(x => x.AccentColor).HasMaxLength(7);
        builder.Property(x => x.Metadata).HasColumnType("jsonb");
    }
}

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("user_preferences");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.PreferenceKey).HasMaxLength(100);
        builder.Property(x => x.PreferenceValue).HasColumnType("jsonb");
    }
}

public class WebhookEndpointConfiguration : IEntityTypeConfiguration<WebhookEndpoint>
{
    public void Configure(EntityTypeBuilder<WebhookEndpoint> builder)
    {
        builder.ToTable("webhook_endpoints");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.Url).HasMaxLength(500);
        builder.Property(x => x.SecretHash).HasMaxLength(255);
        builder.Property(x => x.Events).HasColumnType("jsonb");
    }
}

public class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
{
    public void Configure(EntityTypeBuilder<WebhookDelivery> builder)
    {
        builder.ToTable("webhook_deliveries");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.EventType).HasMaxLength(50);
        builder.Property(x => x.Payload).HasColumnType("jsonb");
    }
}

public class PlatformUserInviteConfiguration : IEntityTypeConfiguration<PlatformUserInvite>
{
    public void Configure(EntityTypeBuilder<PlatformUserInvite> builder)
    {
        builder.ToTable("platform_user_invites");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasMaxLength(255);
        builder.Property(x => x.FullName).HasMaxLength(255);
        builder.Property(x => x.InviteTokenHash).HasMaxLength(64);
    }
}

