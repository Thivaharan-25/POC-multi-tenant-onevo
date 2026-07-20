using OnevoHr.Api.Models.Generated;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Models.Agents;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Calendar;
using OnevoHr.Api.Models.Catalog;
using OnevoHr.Api.Models.Demo;
using OnevoHr.Api.Models.DeveloperPlatform;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.Leave;
using OnevoHr.Api.Models.Notifications;
using OnevoHr.Api.Models.OrgStructure;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Models.Templates;
using TenantEntity = OnevoHr.Api.Models.Tenant.Tenant;
using OnevoHr.Api.Models.Tenant;

namespace OnevoHr.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Developer Platform
    public DbSet<PlatformUser> PlatformUsers => Set<PlatformUser>();
    public DbSet<PlatformRole> PlatformRoles => Set<PlatformRole>();
    public DbSet<PlatformPermission> PlatformPermissions => Set<PlatformPermission>();
    public DbSet<PlatformRolePermission> PlatformRolePermissions => Set<PlatformRolePermission>();
    public DbSet<PlatformUserRole> PlatformUserRoles => Set<PlatformUserRole>();
    public DbSet<PlatformUserSession> PlatformUserSessions => Set<PlatformUserSession>();
    public DbSet<PlatformAuthEvent> PlatformAuthEvents => Set<PlatformAuthEvent>();

    // Tenant / lifecycle
    public DbSet<TenantEntity> Tenants => Set<TenantEntity>();
    public DbSet<TenantProvisioningState> TenantProvisioningStates => Set<TenantProvisioningState>();

    // Catalog
    public DbSet<ModuleCatalog> ModuleCatalogs => Set<ModuleCatalog>();
    public DbSet<ModuleFeature> ModuleFeatures => Set<ModuleFeature>();
    public DbSet<ModulePermissionOwnership> ModulePermissionOwnerships => Set<ModulePermissionOwnership>();

    // Subscriptions / billing
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<TenantModuleEntitlement> TenantModuleEntitlements => Set<TenantModuleEntitlement>();
    public DbSet<TenantFeatureEntitlement> TenantFeatureEntitlements => Set<TenantFeatureEntitlement>();
    public DbSet<RuntimeFeatureFlag> RuntimeFeatureFlags => Set<RuntimeFeatureFlag>();
    public DbSet<SubscriptionInvoice> SubscriptionInvoices => Set<SubscriptionInvoice>();
    public DbSet<PaymentGatewayConfig> PaymentGatewayConfigs => Set<PaymentGatewayConfig>();
    public DbSet<PaymentGatewayCredential> PaymentGatewayCredentials => Set<PaymentGatewayCredential>();
    public DbSet<PaymentGatewayCountryRoute> PaymentGatewayCountryRoutes => Set<PaymentGatewayCountryRoute>();

    // Auth
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Org structure
    public DbSet<LegalEntity> LegalEntities => Set<LegalEntity>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<PositionReportingHistory> PositionReportingHistories => Set<PositionReportingHistory>();
    public DbSet<PositionAssignment> PositionAssignments => Set<PositionAssignment>();
    public DbSet<EmployeeHierarchyClosure> EmployeeHierarchyClosures => Set<EmployeeHierarchyClosure>();

    // Employees
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeAssignmentHistory> EmployeeAssignmentHistories => Set<EmployeeAssignmentHistory>();
    public DbSet<OnboardingDraft> OnboardingDrafts => Set<OnboardingDraft>();

    // Templates
    public DbSet<RoleTemplate> RoleTemplates => Set<RoleTemplate>();
    public DbSet<ConfigurationTemplate> ConfigurationTemplates => Set<ConfigurationTemplate>();
    public DbSet<TenantConfigurationTemplateApplication> TenantConfigurationTemplateApplications => Set<TenantConfigurationTemplateApplication>();

    // Leave
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeavePolicy> LeavePolicies => Set<LeavePolicy>();
    public DbSet<LeavePolicyAssignment> LeavePolicyAssignments => Set<LeavePolicyAssignment>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

    // Calendar
    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
    public DbSet<ExternalCalendarConnection> ExternalCalendarConnections => Set<ExternalCalendarConnection>();
    public DbSet<ExternalCalendarEventLink> ExternalCalendarEventLinks => Set<ExternalCalendarEventLink>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    // Generated Phase 1 DbSets
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<FileRecord> FileRecords => Set<FileRecord>();
    public DbSet<FileUploadReservation> FileUploadReservations => Set<FileUploadReservation>();
    public DbSet<TenantStorageStat> TenantStorageStats => Set<TenantStorageStat>();
    public DbSet<EntityAsset> EntityAssets => Set<EntityAsset>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<FeatureAccessGrant> FeatureAccessGrants => Set<FeatureAccessGrant>();
    public DbSet<LegalAcceptanceRecord> LegalAcceptanceRecords => Set<LegalAcceptanceRecord>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<InvitationToken> InvitationTokens => Set<InvitationToken>();
    public DbSet<TenantAuthPolicy> TenantAuthPolicies => Set<TenantAuthPolicy>();
    public DbSet<UserExternalIdentity> UserExternalIdentities => Set<UserExternalIdentity>();
    public DbSet<UserPermissionOverride> UserPermissionOverrides => Set<UserPermissionOverride>();
    public DbSet<AccessGrantRequest> AccessGrantRequests => Set<AccessGrantRequest>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserMfa> UserMfas => Set<UserMfa>();
    public DbSet<MfaRecoveryCode> MfaRecoveryCodes => Set<MfaRecoveryCode>();
    public DbSet<PositionAccessTemplate> PositionAccessTemplates => Set<PositionAccessTemplate>();
    public DbSet<ManagementCoverageRecord> ManagementCoverageRecords => Set<ManagementCoverageRecord>();
    public DbSet<EmployeeAddresse> EmployeeAddresses => Set<EmployeeAddresse>();
    public DbSet<EmployeeBankDetail> EmployeeBankDetails => Set<EmployeeBankDetail>();
    public DbSet<EmployeeCustomField> EmployeeCustomFields => Set<EmployeeCustomField>();
    public DbSet<EmployeeDependent> EmployeeDependents => Set<EmployeeDependent>();
    public DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts => Set<EmployeeEmergencyContact>();
    public DbSet<EmployeeLifecycleEvent> EmployeeLifecycleEvents => Set<EmployeeLifecycleEvent>();
    public DbSet<EmployeeQualification> EmployeeQualifications => Set<EmployeeQualification>();
    public DbSet<EmployeeSalaryHistory> EmployeeSalaryHistories => Set<EmployeeSalaryHistory>();
    public DbSet<EmployeeWorkHistory> EmployeeWorkHistories => Set<EmployeeWorkHistory>();
    public DbSet<EmployeeTransfer> EmployeeTransfers => Set<EmployeeTransfer>();
    public DbSet<OffboardingRecord> OffboardingRecords => Set<OffboardingRecord>();
    public DbSet<EmployeeChecklistTask> EmployeeChecklistTasks => Set<EmployeeChecklistTask>();
    public DbSet<ChecklistTemplate> ChecklistTemplates => Set<ChecklistTemplate>();
    public DbSet<TimeOffPolicyRule> TimeOffPolicyRules => Set<TimeOffPolicyRule>();
    public DbSet<TimeOffBalancesAudit> TimeOffBalancesAudits => Set<TimeOffBalancesAudit>();
    public DbSet<CalendarEventParticipant> CalendarEventParticipants => Set<CalendarEventParticipant>();
    public DbSet<HolidayCalendarSetting> HolidayCalendarSettings => Set<HolidayCalendarSetting>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
    public DbSet<MonitoringFeatureToggle> MonitoringFeatureToggles => Set<MonitoringFeatureToggle>();
    public DbSet<MonitoringAlertPolicy> MonitoringAlertPolicies => Set<MonitoringAlertPolicy>();
    public DbSet<AppAllowlist> AppAllowlists => Set<AppAllowlist>();
    public DbSet<AppAllowlistAudit> AppAllowlistAudits => Set<AppAllowlistAudit>();
    public DbSet<ObservedApplication> ObservedApplications => Set<ObservedApplication>();
    public DbSet<EmployeeMonitoringOverride> EmployeeMonitoringOverrides => Set<EmployeeMonitoringOverride>();
    public DbSet<MonitoringPolicyOverride> MonitoringPolicyOverrides => Set<MonitoringPolicyOverride>();
    public DbSet<EmployeeWorkLocationSetting> EmployeeWorkLocationSettings => Set<EmployeeWorkLocationSetting>();
    public DbSet<EmployeeRemoteWorkProfile> EmployeeRemoteWorkProfiles => Set<EmployeeRemoteWorkProfile>();
    public DbSet<RemoteWorkLocationChangeRequest> RemoteWorkLocationChangeRequests => Set<RemoteWorkLocationChangeRequest>();
    public DbSet<ActivitySnapshot> ActivitySnapshots => Set<ActivitySnapshot>();
    public DbSet<ActivityRawBuffer> ActivityRawBuffers => Set<ActivityRawBuffer>();
    public DbSet<ActivityDailySummary> ActivityDailySummaries => Set<ActivityDailySummary>();
    public DbSet<ApplicationCategory> ApplicationCategories => Set<ApplicationCategory>();
    public DbSet<ApplicationUsage> ApplicationUsages => Set<ApplicationUsage>();
    public DbSet<DeviceTracking> DeviceTrackings => Set<DeviceTracking>();
    public DbSet<MeetingSession> MeetingSessions => Set<MeetingSession>();
    public DbSet<MonitoringEvidenceAsset> MonitoringEvidenceAssets => Set<MonitoringEvidenceAsset>();
    public DbSet<BrowserActivity> BrowserActivities => Set<BrowserActivity>();
    public DbSet<DiscrepancyEvent> DiscrepancyEvents => Set<DiscrepancyEvent>();
    public DbSet<WorkManagementDailyTimeLog> WorkManagementDailyTimeLogs => Set<WorkManagementDailyTimeLog>();
    public DbSet<EmployeeDiscrepancyBaseline> EmployeeDiscrepancyBaselines => Set<EmployeeDiscrepancyBaseline>();
    public DbSet<PresenceSession> PresenceSessions => Set<PresenceSession>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<AttendanceCorrection> AttendanceCorrections => Set<AttendanceCorrection>();
    public DbSet<BreakRecord> BreakRecords => Set<BreakRecord>();
    public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();
    public DbSet<WorkScheduleDay> WorkScheduleDaies => Set<WorkScheduleDay>();
    public DbSet<WorkScheduleHoliday> WorkScheduleHolidaies => Set<WorkScheduleHoliday>();
    public DbSet<ScheduleAssignment> ScheduleAssignments => Set<ScheduleAssignment>();
    public DbSet<PublicHoliday> PublicHolidaies => Set<PublicHoliday>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<ShiftAssignment> ShiftAssignments => Set<ShiftAssignment>();
    public DbSet<RosterPeriod> RosterPeriods => Set<RosterPeriod>();
    public DbSet<RosterEntry> RosterEntries => Set<RosterEntry>();
    public DbSet<WorkAreaChangeRequest> WorkAreaChangeRequests => Set<WorkAreaChangeRequest>();
    public DbSet<ClockInPolicy> ClockInPolicies => Set<ClockInPolicy>();
    public DbSet<ClockInLateDeductionRule> ClockInLateDeductionRules => Set<ClockInLateDeductionRule>();
    public DbSet<VerificationPolicy> VerificationPolicies => Set<VerificationPolicy>();
    public DbSet<VerificationRecord> VerificationRecords => Set<VerificationRecord>();
    public DbSet<VerificationEvidenceAsset> VerificationEvidenceAssets => Set<VerificationEvidenceAsset>();
    public DbSet<VerificationReferencePhoto> VerificationReferencePhotos => Set<VerificationReferencePhoto>();
    public DbSet<BiometricDevice> BiometricDevices => Set<BiometricDevice>();
    public DbSet<BiometricEnrollment> BiometricEnrollments => Set<BiometricEnrollment>();
    public DbSet<BiometricEvent> BiometricEvents => Set<BiometricEvent>();
    public DbSet<BiometricAuditLog> BiometricAuditLogs => Set<BiometricAuditLog>();
    public DbSet<DailyEmployeeReport> DailyEmployeeReports => Set<DailyEmployeeReport>();
    public DbSet<WeeklyEmployeeReport> WeeklyEmployeeReports => Set<WeeklyEmployeeReport>();
    public DbSet<MonthlyEmployeeReport> MonthlyEmployeeReports => Set<MonthlyEmployeeReport>();
    public DbSet<MonitoringSnapshot> MonitoringSnapshots => Set<MonitoringSnapshot>();
    public DbSet<WmsProductivitySnapshot> WmsProductivitySnapshots => Set<WmsProductivitySnapshot>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceRole> WorkspaceRoles => Set<WorkspaceRole>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<ProjectMemberInvitation> ProjectMemberInvitations => Set<ProjectMemberInvitation>();
    public DbSet<ProjectLinkInvitation> ProjectLinkInvitations => Set<ProjectLinkInvitation>();
    public DbSet<ProjectLink> ProjectLinks => Set<ProjectLink>();
    public DbSet<ProjectVersion> ProjectVersions => Set<ProjectVersion>();
    public DbSet<ReleaseCalendar> ReleaseCalendars => Set<ReleaseCalendar>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();
    public DbSet<TaskChecklist> TaskChecklists => Set<TaskChecklist>();
    public DbSet<TaskChecklistItem> TaskChecklistItems => Set<TaskChecklistItem>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<TaskApproval> TaskApprovals => Set<TaskApproval>();
    public DbSet<TaskWatcher> TaskWatchers => Set<TaskWatcher>();
    public DbSet<TaskLink> TaskLinks => Set<TaskLink>();
    public DbSet<CustomField> CustomFields => Set<CustomField>();
    public DbSet<CustomFieldValue> CustomFieldValues => Set<CustomFieldValue>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentApproval> DocumentApprovals => Set<DocumentApproval>();
    public DbSet<WikiPage> WikiPages => Set<WikiPage>();
    public DbSet<TaskDocument> TaskDocuments => Set<TaskDocument>();
    public DbSet<RegisteredAgent> RegisteredAgents => Set<RegisteredAgent>();
    public DbSet<AgentSession> AgentSessions => Set<AgentSession>();
    public DbSet<AgentCommand> AgentCommands => Set<AgentCommand>();
    public DbSet<AgentHealthLog> AgentHealthLogs => Set<AgentHealthLog>();
    public DbSet<AgentPolicy> AgentPolicies => Set<AgentPolicy>();
    public DbSet<AgentWorkLocationEvidence> AgentWorkLocationEvidences => Set<AgentWorkLocationEvidence>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<ComplianceExport> ComplianceExports => Set<ComplianceExport>();
    public DbSet<EscalationRule> EscalationRules => Set<EscalationRule>();
    public DbSet<GlobalAppCatalog> GlobalAppCatalogs => Set<GlobalAppCatalog>();
    public DbSet<FeatureFlagOverride> FeatureFlagOverrides => Set<FeatureFlagOverride>();
    public DbSet<LegalHold> LegalHolds => Set<LegalHold>();
    public DbSet<NotificationChannel> NotificationChannels => Set<NotificationChannel>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<EmailDeliveryLog> EmailDeliveryLogs => Set<EmailDeliveryLog>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<SupportTicketMessage> SupportTicketMessages => Set<SupportTicketMessage>();
    public DbSet<SupportTicketInternalNote> SupportTicketInternalNotes => Set<SupportTicketInternalNote>();
    public DbSet<SupportTicketEvent> SupportTicketEvents => Set<SupportTicketEvent>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<SubscriptionPlanPriceHistory> SubscriptionPlanPriceHistories => Set<SubscriptionPlanPriceHistory>();
    public DbSet<ModuleCatalogPriceHistory> ModuleCatalogPriceHistories => Set<ModuleCatalogPriceHistory>();
    public DbSet<TenantSubscriptionEvent> TenantSubscriptionEvents => Set<TenantSubscriptionEvent>();
    public DbSet<BillingSnapshot> BillingSnapshots => Set<BillingSnapshot>();
    public DbSet<TenantProvisioningValidationResult> TenantProvisioningValidationResults => Set<TenantProvisioningValidationResult>();
    public DbSet<SetupService> SetupServices => Set<SetupService>();
    public DbSet<TenantSetupService> TenantSetupServices => Set<TenantSetupService>();
    public DbSet<RateLimitRule> RateLimitRules => Set<RateLimitRule>();
    public DbSet<RetentionPolicy> RetentionPolicies => Set<RetentionPolicy>();
    public DbSet<ScheduledTask> ScheduledTasks => Set<ScheduledTask>();
    public DbSet<SignalrConnection> SignalrConnections => Set<SignalrConnection>();
    public DbSet<SsoProvider> SsoProviders => Set<SsoProvider>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<TenantBranding> TenantBrandings => Set<TenantBranding>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<WebhookEndpoint> WebhookEndpoints => Set<WebhookEndpoint>();
    public DbSet<WebhookDelivery> WebhookDeliveries => Set<WebhookDelivery>();
    public DbSet<PlatformUserInvite> PlatformUserInvites => Set<PlatformUserInvite>();

    // Device pairing (see docs/superpowers/specs/2026-07-09-device-pairing-flow-design.md)
    public DbSet<AgentPairingRequest> AgentPairingRequests => Set<AgentPairingRequest>();

    // Clock in/out + app usage tracking (see docs/superpowers/specs/2026-07-09-clock-in-app-usage-tracking-design.md)
    public DbSet<AgentClockState> AgentClockStates => Set<AgentClockState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Remap legacy PascalCase tables to canonical snake_case
        modelBuilder.Entity<PlatformUser>().ToTable("platform_users");
        modelBuilder.Entity<PlatformRole>().ToTable("platform_roles");
        modelBuilder.Entity<PlatformPermission>().ToTable("platform_permissions");
        modelBuilder.Entity<PlatformRolePermission>().ToTable("platform_role_permissions");
        modelBuilder.Entity<PlatformUserRole>().ToTable("platform_user_roles");
        modelBuilder.Entity<PlatformUserSession>().ToTable("platform_user_sessions");
        modelBuilder.Entity<PlatformAuthEvent>().ToTable("platform_auth_events");

        modelBuilder.Entity<TenantEntity>().ToTable("tenants");
        modelBuilder.Entity<TenantProvisioningState>().ToTable("tenant_provisioning_states");

        modelBuilder.Entity<ModuleCatalog>().ToTable("module_catalog");
        modelBuilder.Entity<ModuleFeature>().ToTable("module_features");
        modelBuilder.Entity<ModulePermissionOwnership>().ToTable("module_permission_ownership");

        modelBuilder.Entity<SubscriptionPlan>().ToTable("subscription_plans");
        modelBuilder.Entity<TenantSubscription>().ToTable("tenant_subscriptions");
        modelBuilder.Entity<TenantModuleEntitlement>().ToTable("tenant_module_entitlements");
        modelBuilder.Entity<RuntimeFeatureFlag>().ToTable("feature_flags");
        modelBuilder.Entity<SubscriptionInvoice>().ToTable("subscription_invoices");
        modelBuilder.Entity<PaymentGatewayConfig>().ToTable("payment_gateway_configs");
        modelBuilder.Entity<PaymentGatewayCredential>().ToTable("payment_gateway_credentials");
        modelBuilder.Entity<PaymentGatewayCountryRoute>().ToTable("payment_gateway_country_routes");

        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<UserSession>().ToTable("sessions");
        modelBuilder.Entity<Role>().ToTable("roles");
        modelBuilder.Entity<UserRole>().ToTable("user_roles");
        modelBuilder.Entity<RolePermission>().ToTable("role_permissions");

        modelBuilder.Entity<LegalEntity>().ToTable("legal_entities");
        modelBuilder.Entity<Department>().ToTable("departments");
        modelBuilder.Entity<Position>().ToTable("positions");
        modelBuilder.Entity<PositionReportingHistory>().ToTable("position_reporting_history");
        modelBuilder.Entity<PositionAssignment>().ToTable("position_assignments");
        modelBuilder.Entity<EmployeeHierarchyClosure>().ToTable("employee_hierarchy_closure");

        modelBuilder.Entity<Employee>().ToTable("employees");
        modelBuilder.Entity<EmployeeAssignmentHistory>().ToTable("employee_assignment_history");
        
        modelBuilder.Entity<OnboardingDraft>().ToTable("onboarding_drafts");
        modelBuilder.Entity<OnboardingDraft>()
            .HasIndex(d => new { d.TenantId, d.Status });
        modelBuilder.Entity<OnboardingDraft>()
            .HasIndex(d => new { d.TenantId, d.WorkEmail });
        modelBuilder.Entity<OnboardingDraft>()
            .HasIndex(d => new { d.TenantId, d.StartedById });

        modelBuilder.Entity<RoleTemplate>().ToTable("role_templates");
        modelBuilder.Entity<RoleTemplate>().Property(x => x.PermissionCodesJson).HasColumnType("jsonb");
        modelBuilder.Entity<ConfigurationTemplate>().ToTable("configuration_templates");
        modelBuilder.Entity<TenantConfigurationTemplateApplication>().ToTable("tenant_configuration_template_applications");

        modelBuilder.Entity<LeaveType>().ToTable("time_off_types");
        modelBuilder.Entity<LeavePolicy>().ToTable("time_off_policies");
        modelBuilder.Entity<LeavePolicyAssignment>().ToTable("time_off_policy_assignments");
        modelBuilder.Entity<LeaveRequest>().ToTable("time_off_requests");
        modelBuilder.Entity<LeaveBalance>().ToTable("time_off_entitlements");

        modelBuilder.Entity<CalendarEvent>().ToTable("calendar_events");
        modelBuilder.Entity<ExternalCalendarConnection>().ToTable("external_calendar_connections");
        modelBuilder.Entity<ExternalCalendarEventLink>().ToTable("external_calendar_event_links");

        modelBuilder.Entity<Notification>().ToTable("notifications");

        modelBuilder.Entity<OutboxMessage>().ToTable("outbox_messages");
        modelBuilder.Entity<OutboxMessage>().Property(m => m.PayloadJson).HasColumnType("jsonb");
        modelBuilder.Entity<OutboxMessage>()
            .HasIndex(m => new { m.Status, m.CreatedAtUtc });

        modelBuilder.Entity<PlatformUserInvite>().ToTable("platform_user_invites");

        // Table name was previously implicit; making it explicit to match the
        // registered_agents table created by the AddRemainingPhase1Entities migration.
        modelBuilder.Entity<RegisteredAgent>().ToTable("registered_agents");
        modelBuilder.Entity<RegisteredAgent>().HasIndex(a => a.TenantId);

        modelBuilder.Entity<AgentPairingRequest>().ToTable("agent_pairing_requests");
        modelBuilder.Entity<AgentPairingRequest>().Property(p => p.Status).HasConversion<string>();
        modelBuilder.Entity<AgentPairingRequest>().HasIndex(p => p.UserCodeHash).IsUnique();
        modelBuilder.Entity<AgentPairingRequest>().HasIndex(p => p.DeviceCodeHash).IsUnique();

        modelBuilder.Entity<AgentClockState>().ToTable("agent_clock_states");
        modelBuilder.Entity<AgentClockState>().HasIndex(c => c.RegisteredAgentId).IsUnique();

        // Table name was previously implicit; making it explicit to match the
        // application_usage table created by the AddRemainingPhase1Entities migration.
        modelBuilder.Entity<Models.Generated.ApplicationUsage>().ToTable("application_usage");
        modelBuilder.Entity<Models.Generated.ApplicationUsage>().HasIndex(a => a.TenantId);
        modelBuilder.Entity<Models.Generated.ApplicationUsage>()
            .HasIndex(a => new { a.TenantId, a.EmployeeId, a.Date, a.ApplicationName });

        // Cross-module Developer Platform tables not enumerated in the 199-table
        // Phase 1 inventory (database/phase1-table-inventory.md), but documented in
        // developer-platform/database/schema.md and required by real Demo Profile /
        // subscription add-on functionality. See PHASE1_CANONICAL_TABLE_AUDIT.md.
        modelBuilder.Entity<Models.Subscriptions.SubscriptionPlanModule>().ToTable("subscription_plan_modules");
        modelBuilder.Entity<Models.Subscriptions.SubscriptionPlanResourceAddon>().ToTable("subscription_plan_resource_addons");
        modelBuilder.Entity<Models.Subscriptions.SubscriptionPlanPriceBracket>().ToTable("subscription_plan_price_brackets");
    }
}
