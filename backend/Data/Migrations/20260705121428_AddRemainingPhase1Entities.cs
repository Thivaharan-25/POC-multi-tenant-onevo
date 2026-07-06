using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingPhase1Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalTasks");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "WorkflowRuns");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.CreateTable(
                name: "access_grant_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TargetPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetDepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionAccessTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EffectiveFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DecisionComment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_grant_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activity_daily_summary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalActiveMinutes = table.Column<int>(type: "integer", nullable: false),
                    TotalIdleMinutes = table.Column<int>(type: "integer", nullable: false),
                    TotalMeetingMinutes = table.Column<int>(type: "integer", nullable: false),
                    ActivePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProductiveAppMinutes = table.Column<int>(type: "integer", nullable: false),
                    PersonalAppMinutes = table.Column<int>(type: "integer", nullable: false),
                    UnknownAppMinutes = table.Column<int>(type: "integer", nullable: false),
                    FocusMinutes = table.Column<int>(type: "integer", nullable: false),
                    ActivityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    DataCoveragePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TopAppsJson = table.Column<string>(type: "jsonb", nullable: false),
                    IntensityAvg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    KeyboardTotal = table.Column<int>(type: "integer", nullable: false),
                    MouseTotal = table.Column<int>(type: "integer", nullable: false),
                    BrowserActiveMinutes = table.Column<int>(type: "integer", nullable: true),
                    WorkBrowserMinutes = table.Column<int>(type: "integer", nullable: false),
                    PersonalBrowserMinutes = table.Column<int>(type: "integer", nullable: false),
                    DocumentTimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    DeepFocusSessionsCount = table.Column<int>(type: "integer", nullable: false),
                    DataSource = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_daily_summary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activity_raw_buffer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_raw_buffer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activity_snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    KeyboardEventsCount = table.Column<int>(type: "integer", nullable: false),
                    MouseEventsCount = table.Column<int>(type: "integer", nullable: false),
                    ActiveSeconds = table.Column<int>(type: "integer", nullable: false),
                    IdleSeconds = table.Column<int>(type: "integer", nullable: false),
                    IntensityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ForegroundProcessName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "agent_commands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommandType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RequestedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResultJson = table.Column<string>(type: "jsonb", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_commands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "agent_health_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CpuUsage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MemoryMb = table.Column<int>(type: "integer", nullable: false),
                    ErrorsJson = table.Column<string>(type: "jsonb", nullable: false),
                    TamperDetected = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_health_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "agent_policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyJson = table.Column<string>(type: "jsonb", nullable: false),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "agent_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "agent_work_location_evidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PresenceSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PublicIp = table.Column<string>(type: "text", nullable: false),
                    LocalIp = table.Column<string>(type: "text", nullable: true),
                    WifiSsid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    WifiBssidHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GatewayMacHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VpnDetected = table.Column<bool>(type: "boolean", nullable: false),
                    CoarseLocationJson = table.Column<string>(type: "jsonb", nullable: true),
                    MatchStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Confidence = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MatchedLocationSource = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    MatchedLocationSourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_work_location_evidence", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "api_keys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    KeyHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    KeyPrefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Scopes = table.Column<string>(type: "jsonb", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_keys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "app_allowlist_audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowlistId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ChangedById = table.Column<Guid>(type: "uuid", nullable: false),
                    OldValueJson = table.Column<string>(type: "jsonb", nullable: false),
                    NewValueJson = table.Column<string>(type: "jsonb", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_allowlist_audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "app_allowlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ScopeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProcessName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GlobalCatalogId = table.Column<Guid>(type: "uuid", nullable: true),
                    SetById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_allowlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "application_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationNamePattern = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsProductive = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "application_usage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProcessName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApplicationCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WindowTitleHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TotalSeconds = table.Column<int>(type: "integer", nullable: false),
                    IsProductive = table.Column<bool>(type: "boolean", nullable: true),
                    IsAllowed = table.Column<bool>(type: "boolean", nullable: true),
                    AppCategoryType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BrowserDomain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_usage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "attendance_corrections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    PresenceSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttendanceRecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    CorrectionType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OriginalClockInAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OriginalClockOutAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequestedClockInAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequestedClockOutAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OriginalBreakJson = table.Column<string>(type: "jsonb", nullable: true),
                    RequestedBreakJson = table.Column<string>(type: "jsonb", nullable: true),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewComment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_corrections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "attendance_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    WorkScheduleId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpectedWorkingDay = table.Column<bool>(type: "boolean", nullable: false),
                    WorkTimeType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ScheduledStart = table.Column<string>(type: "text", nullable: true),
                    ScheduledEnd = table.Column<string>(type: "text", nullable: true),
                    RequiredWorkMinutes = table.Column<int>(type: "integer", nullable: true),
                    ExpectedWorkArea = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ScheduleTimezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsHoliday = table.Column<bool>(type: "boolean", nullable: false),
                    HolidayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ActualStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    WorkedMinutes = table.Column<int>(type: "integer", nullable: false),
                    BreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    LateMinutes = table.Column<int>(type: "integer", nullable: true),
                    ShortMinutes = table.Column<int>(type: "integer", nullable: true),
                    DetectedWorkArea = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AttendanceSource = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldValuesJson = table.Column<string>(type: "jsonb", nullable: false),
                    NewValuesJson = table.Column<string>(type: "jsonb", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "billing_snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SnapshotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ActiveEmployeeCount = table.Column<int>(type: "integer", nullable: false),
                    EnrolledDeviceCount = table.Column<int>(type: "integer", nullable: false),
                    EmployeeBreakdown = table.Column<string>(type: "jsonb", nullable: false),
                    DeviceBreakdown = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "biometric_audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BiometricDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DetailsJson = table.Column<string>(type: "jsonb", nullable: false),
                    RecordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_biometric_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "biometric_devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Vendor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ConnectionMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    WebhookUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VendorMiddlewareUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExternalDeviceRef = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApiKeyEncrypted = table.Column<byte[]>(type: "bytea", nullable: false),
                    SupportedAuthMethods = table.Column<string>(type: "jsonb", nullable: false),
                    EnabledAuthMethods = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LastHeartbeatAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_biometric_devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "biometric_enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BiometricDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrolledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConsentGiven = table.Column<bool>(type: "boolean", nullable: false),
                    Modality = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TemplateHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_biometric_enrollments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "biometric_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BiometricDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AuthMethod = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Modality = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_biometric_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "break_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BreakStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BreakEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BreakType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AutoDetected = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_break_records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "browser_activity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Domain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DomainClassification = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TotalSeconds = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_browser_activity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "calendar_event_participants",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ResponseReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar_event_participants", x => new { x.EventId, x.EmployeeId });
                });

            migrationBuilder.CreateTable(
                name: "checklist_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TemplateType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TasksJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clock_in_late_deduction_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClockInPolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LateArrivalMinute = table.Column<int>(type: "integer", nullable: false),
                    Multiplier = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TimeOffTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clock_in_late_deduction_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clock_in_policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    ScopeType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DepartmentIds = table.Column<Guid>(type: "uuid", nullable: true),
                    PositionIds = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeIds = table.Column<Guid>(type: "uuid", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    ClockInRequired = table.Column<bool>(type: "boolean", nullable: false),
                    LocationVerificationRequired = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedRadiusMeters = table.Column<int>(type: "integer", nullable: true),
                    OnsiteBiometricEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    OnsiteWebEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    OnsiteTrayEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    OnsitePhotoRequired = table.Column<bool>(type: "boolean", nullable: false),
                    RemoteBiometricEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    RemoteWebEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    RemoteTrayEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    RemotePhotoRequired = table.Column<bool>(type: "boolean", nullable: false),
                    EitherBiometricEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    EitherWebEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    EitherTrayEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    EitherPhotoRequired = table.Column<bool>(type: "boolean", nullable: false),
                    EitherLocationCheckRequired = table.Column<bool>(type: "boolean", nullable: false),
                    EitherSourceRule = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FieldBiometricEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    FieldWebEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    FieldTrayEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    FieldPhotoRequirement = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AllowUnscheduledWorkClockIn = table.Column<bool>(type: "boolean", nullable: false),
                    AllowHolidayWorkClockIn = table.Column<bool>(type: "boolean", nullable: false),
                    CorrectionRequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    OutageFallbackEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationRecipientResolver = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clock_in_policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "compliance_exports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ExportType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Scope = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compliance_exports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "custom_field_values",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueText = table.Column<string>(type: "text", nullable: true),
                    ValueNumber = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    ValueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ValueJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_field_values", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "custom_fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FieldType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OptionsJson = table.Column<string>(type: "jsonb", nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_fields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "daily_employee_report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalHours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ActiveHours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    IdleHours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MeetingHours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ActivePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProductiveAppHours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    FocusHours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ActivityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    WorkOutputScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ProductivityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProductivityScoreBasis = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataCoveragePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TopAppsJson = table.Column<string>(type: "jsonb", nullable: false),
                    IntensityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    DeviceSplitJson = table.Column<string>(type: "jsonb", nullable: false),
                    ExceptionsCount = table.Column<int>(type: "integer", nullable: false),
                    AnomalyFlagsJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_employee_report", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "device_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SessionEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActiveMinutes = table.Column<int>(type: "integer", nullable: false),
                    IdleMinutes = table.Column<int>(type: "integer", nullable: false),
                    ActivePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "device_tracking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    LaptopActiveMinutes = table.Column<int>(type: "integer", nullable: false),
                    EstimatedMobileMinutes = table.Column<int>(type: "integer", nullable: false),
                    LaptopPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    DetectionMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_tracking", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "discrepancy_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    HrActiveMinutes = table.Column<int>(type: "integer", nullable: false),
                    WorkManagementLoggedMinutes = table.Column<int>(type: "integer", nullable: false),
                    CalendarMinutes = table.Column<int>(type: "integer", nullable: false),
                    UnaccountedMinutes = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ThresholdMinutes = table.Column<int>(type: "integer", nullable: false),
                    NotifiedOwner = table.Column<bool>(type: "boolean", nullable: false),
                    NotifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ZScore = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    BaselineAvgMinutes = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    BaselineStddevMinutes = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    SeverityMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discrepancy_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "document_approvals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ApproverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_approvals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "document_versions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    ContentSnapshot = table.Column<string>(type: "text", nullable: false),
                    ChangeSummary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_versions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentScope = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LockedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => new { x.WorkspaceId, x.ProjectId, x.ApprovedVersionId });
                });

            migrationBuilder.CreateTable(
                name: "email_delivery_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    NotificationChannelId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipientEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SubjectSnapshot = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderMessageId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProviderEventId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BouncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_delivery_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AddressJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_bank_details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BranchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AccountNumberEncrypted = table.Column<byte[]>(type: "bytea", nullable: false),
                    RoutingNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_bank_details", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_checklist_tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    LifecycleType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TaskTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: true),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_checklist_tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_custom_fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FieldValue = table.Column<string>(type: "text", nullable: false),
                    FieldType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_custom_fields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_dependents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Relationship = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    IsEmergencyContact = table.Column<bool>(type: "boolean", nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_dependents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_discrepancy_baselines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComputedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    WindowDays = table.Column<int>(type: "integer", nullable: false),
                    AvgUnaccountedMinutes = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    StddevUnaccountedMinutes = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    SampleCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_discrepancy_baselines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_emergency_contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Relationship = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_emergency_contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_lifecycle_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DetailsJson = table.Column<string>(type: "jsonb", nullable: false),
                    PerformedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_lifecycle_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_monitoring_overrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityMonitoring = table.Column<bool>(type: "boolean", nullable: true),
                    ApplicationTracking = table.Column<bool>(type: "boolean", nullable: true),
                    DocumentTracking = table.Column<bool>(type: "boolean", nullable: true),
                    CommunicationTracking = table.Column<bool>(type: "boolean", nullable: true),
                    ScreenshotCapture = table.Column<bool>(type: "boolean", nullable: true),
                    AutoScreenshotCapture = table.Column<bool>(type: "boolean", nullable: true),
                    MeetingDetection = table.Column<bool>(type: "boolean", nullable: true),
                    DeviceTracking = table.Column<bool>(type: "boolean", nullable: true),
                    WorkLocationVerification = table.Column<bool>(type: "boolean", nullable: true),
                    IdentityVerification = table.Column<bool>(type: "boolean", nullable: true),
                    Biometric = table.Column<bool>(type: "boolean", nullable: true),
                    OverrideReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SetById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_monitoring_overrides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_qualifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    QualificationType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Institution = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    YearObtained = table.Column<int>(type: "integer", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DocumentFileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_qualifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_remote_work_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PublicIp = table.Column<string>(type: "text", nullable: true),
                    WifiSsid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    WifiBssidHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GatewayMacHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VpnDetected = table.Column<bool>(type: "boolean", nullable: false),
                    CoarseLocationJson = table.Column<string>(type: "jsonb", nullable: true),
                    VerificationRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_remote_work_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_salary_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ChangeReason = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_salary_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_transfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromDepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToDepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FromPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_transfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_work_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReasonForLeaving = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_work_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_work_location_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkMode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    WorkLocationVerificationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    GracePeriodMinutes = table.Column<int>(type: "integer", nullable: true),
                    PhotoChallengeOnMismatch = table.Column<bool>(type: "boolean", nullable: false),
                    SetById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_work_location_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "entity_assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetPurpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedByType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "escalation_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TriggerCondition = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SlaHours = table.Column<int>(type: "integer", nullable: false),
                    ActionType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EscalateToRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    NotificationTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escalation_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "feature_access_grants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    GranteeType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GranteeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Module = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    FeatureKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    GrantedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feature_access_grants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "feature_flag_overrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FlagKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<bool>(type: "boolean", nullable: false),
                    GrantedById = table.Column<Guid>(type: "uuid", nullable: false),
                    GrantedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feature_flag_overrides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "file_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(700)", maxLength: 700, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SafeFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "file_upload_reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservedBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ReservedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedFileRecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_upload_reservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "global_app_catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProcessName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Publisher = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsProductiveDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_global_app_catalog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "invitation_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvitedEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    InvitedFullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompletionMethodsJson = table.Column<string>(type: "jsonb", nullable: false),
                    CompletedWith = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AllowGoogleEmailMismatch = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedEmailDomainsJson = table.Column<string>(type: "jsonb", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RevokedByPlatformUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByPlatformUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invitation_tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "labels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "legal_acceptance_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DocumentVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Decision = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_legal_acceptance_records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "legal_holds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    PlacedById = table.Column<Guid>(type: "uuid", nullable: false),
                    PlacedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReleasedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReleasedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_legal_holds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "management_coverage_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoveredTargetType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CoveredPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoveredDepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerOrder = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_management_coverage_records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "meeting_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    MeetingEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Platform = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    HadCameraOn = table.Column<bool>(type: "boolean", nullable: false),
                    HadMicActivity = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meeting_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mfa_recovery_codes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CodeHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mfa_recovery_codes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "module_catalog_price_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OldPricingReference = table.Column<string>(type: "jsonb", nullable: true),
                    NewPricingReference = table.Column<string>(type: "jsonb", nullable: true),
                    OldStorageReference = table.Column<string>(type: "jsonb", nullable: true),
                    NewStorageReference = table.Column<string>(type: "jsonb", nullable: true),
                    OldAiTokenReference = table.Column<string>(type: "jsonb", nullable: true),
                    NewAiTokenReference = table.Column<string>(type: "jsonb", nullable: true),
                    OldPricingUnit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NewPricingUnit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ChangedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_module_catalog_price_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_alert_policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringAlertRecipientResolver = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MonitoringAlertWaitForScheduledRecipientGraceMinutes = table.Column<int>(type: "integer", nullable: false),
                    MonitoringAlertFallbackToManagementCoverageChain = table.Column<bool>(type: "boolean", nullable: false),
                    MonitoringAlertUnresolvedRoutingAction = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_alert_policy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_evidence_assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentDeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivitySnapshotId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivityEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FileRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvidenceType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TriggerType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RetentionPolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    LegalHoldId = table.Column<Guid>(type: "uuid", nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_evidence_assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_feature_toggles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityMonitoring = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationTracking = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentTracking = table.Column<bool>(type: "boolean", nullable: false),
                    CommunicationTracking = table.Column<bool>(type: "boolean", nullable: false),
                    ScreenshotCapture = table.Column<bool>(type: "boolean", nullable: false),
                    AutoScreenshotCapture = table.Column<bool>(type: "boolean", nullable: false),
                    MeetingDetection = table.Column<bool>(type: "boolean", nullable: false),
                    DeviceTracking = table.Column<bool>(type: "boolean", nullable: false),
                    WorkLocationVerification = table.Column<bool>(type: "boolean", nullable: false),
                    IdentityVerification = table.Column<bool>(type: "boolean", nullable: false),
                    Biometric = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_feature_toggles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_policy_overrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ScopeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityMonitoring = table.Column<bool>(type: "boolean", nullable: true),
                    ApplicationTracking = table.Column<bool>(type: "boolean", nullable: true),
                    DocumentTracking = table.Column<bool>(type: "boolean", nullable: true),
                    CommunicationTracking = table.Column<bool>(type: "boolean", nullable: true),
                    ScreenshotCapture = table.Column<bool>(type: "boolean", nullable: true),
                    AutoScreenshotCapture = table.Column<bool>(type: "boolean", nullable: true),
                    MeetingDetection = table.Column<bool>(type: "boolean", nullable: true),
                    DeviceTracking = table.Column<bool>(type: "boolean", nullable: true),
                    WorkLocationVerification = table.Column<bool>(type: "boolean", nullable: true),
                    IdentityVerification = table.Column<bool>(type: "boolean", nullable: true),
                    Biometric = table.Column<bool>(type: "boolean", nullable: true),
                    OverrideReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SetById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_policy_overrides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_snapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalEmployees = table.Column<int>(type: "integer", nullable: false),
                    ActiveCount = table.Column<int>(type: "integer", nullable: false),
                    AvgActivePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AvgActivityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AvgWorkOutputScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    AvgProductivityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AvgDataCoveragePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AvgMeetingPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TotalExceptions = table.Column<int>(type: "integer", nullable: false),
                    TopExceptionTypesJson = table.Column<string>(type: "jsonb", nullable: false),
                    DepartmentBreakdownJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_snapshot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "monthly_employee_report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    TotalHours = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    ActiveHours = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    IdleHours = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    MeetingHours = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    ActivePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProductiveAppHours = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    FocusHours = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    ActivityScoreAvg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    WorkOutputScoreAvg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ProductivityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProductivityScoreBasis = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataCoveragePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    IntensityAvg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ExceptionsCount = table.Column<int>(type: "integer", nullable: false),
                    PerformancePatternJson = table.Column<string>(type: "jsonb", nullable: false),
                    ComparativeRankInDepartment = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monthly_employee_report", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notification_channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CredentialsEncrypted = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ConfiguredById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notification_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SubjectTemplate = table.Column<string>(type: "text", nullable: false),
                    BodyTemplate = table.Column<string>(type: "text", nullable: false),
                    Locale = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "observed_applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProcessName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GlobalCatalogId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EmployeeCount = table.Column<int>(type: "integer", nullable: false),
                    TotalSecondsObserved = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_observed_applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "offboarding_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LastWorkingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    KnowledgeRiskLevel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ExitInterviewNotes = table.Column<string>(type: "text", nullable: false),
                    PenaltiesJson = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offboarding_records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LastFour = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Brand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ExpiryMonth = table.Column<int>(type: "integer", nullable: false),
                    ExpiryYear = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentProviderRef = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_methods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Module = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    FeatureKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "plan_features",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LimitValue = table.Column<int>(type: "integer", nullable: true),
                    IsIncluded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platform_user_invites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    InviteTokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    InvitedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AcceptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_user_invites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "position_access_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    IsSensitive = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveFromRule = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EffectiveToRule = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position_access_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "presence_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    FirstSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalPresentMinutes = table.Column<int>(type: "integer", nullable: false),
                    TotalBreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_presence_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project_link_invitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedHours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    InvitedProjectAdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InvitedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_link_invitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project_links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedHours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedVia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_links", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project_member_invitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InvitedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_member_invitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MembershipSource = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RemovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwningLegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LeadId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "public_holidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_public_holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rate_limit_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    EndpointPattern = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaxRequests = table.Column<int>(type: "integer", nullable: false),
                    WindowSeconds = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rate_limit_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReplacedById = table.Column<Guid>(type: "uuid", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "registered_agents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OsVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AgentVersion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RegisteredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastHeartbeatAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registered_agents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "release_calendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_release_calendar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "remote_work_location_change_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewComment = table.Column<string>(type: "text", nullable: true),
                    NewProfileId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_remote_work_location_change_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "retention_policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RetentionDays = table.Column<int>(type: "integer", nullable: false),
                    ActionOnExpiry = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ComplianceFramework = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_retention_policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roster_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RosterPeriodId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedWorkArea = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roster_entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roster_periods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roster_periods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "schedule_assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDefaultForNewEmployee = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule_assignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "scheduled_tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CronExpression = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastRunAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    NextRunAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduled_tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "setup_services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ModuleKeysJson = table.Column<string>(type: "jsonb", nullable: false),
                    AppliesToAllEntitledModules = table.Column<bool>(type: "boolean", nullable: false),
                    IsFree = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_setup_services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shift_assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedWorkArea = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IsOverride = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shift_assignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<string>(type: "text", nullable: false),
                    EndTime = table.Column<string>(type: "text", nullable: false),
                    BreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsOvernight = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "signalr_connections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConnectionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Channel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ConnectedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastPingAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signalr_connections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sso_providers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ClientIdEncrypted = table.Column<byte[]>(type: "bytea", nullable: false),
                    ClientSecretEncrypted = table.Column<byte[]>(type: "bytea", nullable: false),
                    MetadataUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DomainHint = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AutoProvisionUsers = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sso_providers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plan_price_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldMonthlyPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    NewMonthlyPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    OldAnnualPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    NewAnnualPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    OldCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    NewCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    OldPricingUnit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NewPricingUnit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ChangedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plan_price_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "support_ticket_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupportTicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ActorType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActorPlatformUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OldValuesJson = table.Column<string>(type: "jsonb", nullable: true),
                    NewValuesJson = table.Column<string>(type: "jsonb", nullable: true),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_ticket_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "support_ticket_internal_notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupportTicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorPlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NoteBody = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EditedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_ticket_internal_notes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "support_ticket_messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupportTicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SenderUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SenderPlatformUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageBody = table.Column<string>(type: "text", nullable: false),
                    MessageFormat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsCustomerVisible = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EditedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_ticket_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "support_tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastCustomerReplyAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastPlatformReplyAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastActivityAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResolvedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "system_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SettingKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "jsonb", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_approvals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ApproverId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApproverEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_approvals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedById = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AvailabilityStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AvailabilityCheckedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AvailabilityWarning = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_assignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_checklist_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChecklistId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CheckedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CheckedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_checklist_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_checklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_checklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_links", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_tags",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_tags", x => new { x.TaskId, x.LabelId });
                });

            migrationBuilder.CreateTable(
                name: "task_watchers",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_watchers", x => new { x.TaskId, x.UserId, x.EmployeeId });
                });

            migrationBuilder.CreateTable(
                name: "tenant_auth_policies",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordLoginEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    GoogleLoginEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    InviteGoogleEmailMismatchAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedLoginDomainsJson = table.Column<string>(type: "jsonb", nullable: false),
                    MfaRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_auth_policies", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_branding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogoFileId = table.Column<Guid>(type: "uuid", nullable: true),
                    PrimaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    AccentColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_branding", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_provisioning_validation_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Section = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_provisioning_validation_results", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DateFormat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    WorkWeekDaysJson = table.Column<string>(type: "jsonb", nullable: false),
                    WorkHoursStart = table.Column<string>(type: "text", nullable: false),
                    WorkHoursEnd = table.Column<string>(type: "text", nullable: false),
                    PrivacyMode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DataRetentionDaysJson = table.Column<string>(type: "jsonb", nullable: false),
                    SettingsJson = table.Column<string>(type: "jsonb", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_setup_services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetupServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsBillable = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    SelectedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfiguredById = table.Column<Guid>(type: "uuid", nullable: true),
                    ConfiguredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_setup_services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_storage_stats",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsedR2Bytes = table.Column<long>(type: "bigint", nullable: false),
                    UsedDbBytes = table.Column<long>(type: "bigint", nullable: false),
                    ReservedR2Bytes = table.Column<long>(type: "bigint", nullable: false),
                    LastCalculatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_storage_stats", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_subscription_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantSubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActorPlatformUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OldValuesJson = table.Column<string>(type: "jsonb", nullable: true),
                    NewValuesJson = table.Column<string>(type: "jsonb", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_subscription_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "time_off_balances_audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeOffTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntitlementId = table.Column<Guid>(type: "uuid", nullable: true),
                    TimeOffRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttendanceRecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChangeType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MinutesChanged = table.Column<int>(type: "integer", nullable: false),
                    BalanceAfterMinutes = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CalculationSnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_off_balances_audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "time_off_policy_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeOffTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntitlementMinutes = table.Column<int>(type: "integer", nullable: false),
                    AccrualMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProrationMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CarryForwardAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    CarryForwardLimitMinutes = table.Column<int>(type: "integer", nullable: true),
                    CarryForwardExpiry = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RolloverPeriod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MinimumRequestMinutes = table.Column<int>(type: "integer", nullable: true),
                    MaxConsecutiveMinutes = table.Column<int>(type: "integer", nullable: true),
                    NoticePeriodDays = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_off_policy_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_external_identities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ProviderSubject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProviderEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    LinkedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_external_identities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_mfa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SecretEncrypted = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_mfa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_permission_overrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrantType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    GrantedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_permission_overrides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreferenceKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PreferenceValue = table.Column<string>(type: "jsonb", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "verification_evidence_assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationRecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    PresenceSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttendanceEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    BiometricEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    FileRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvidenceType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    TriggerType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BiometricDeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RetentionPolicyId = table.Column<Guid>(type: "uuid", nullable: true),
                    LegalHoldId = table.Column<Guid>(type: "uuid", nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verification_evidence_assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "verification_policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirePhotoClockIn = table.Column<bool>(type: "boolean", nullable: false),
                    RequirePhotoClockOut = table.Column<bool>(type: "boolean", nullable: false),
                    CameraPhotoVerificationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AbsencePhotoCaptureEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PhotoCaptureContextScope = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MatchThreshold = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ReferenceEnrollmentMode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    BlockMonitoringUntilReferenceApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verification_policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "verification_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    VerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MatchConfidence = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BiometricDeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Trigger = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: true),
                    AlertId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResponseDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verification_records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "verification_reference_photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhotoFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CapturedDeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CapturedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewComment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LegalAcceptanceRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verification_reference_photos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "webhook_deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    WebhookEndpointId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    ResponseStatus = table.Column<int>(type: "integer", nullable: false),
                    ResponseBody = table.Column<string>(type: "text", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhook_deliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "webhook_endpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SecretHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Events = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhook_endpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "weekly_employee_report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekStart = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    ActiveHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    IdleHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    MeetingHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    ActivePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProductiveAppHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    FocusHours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    ActivityScoreAvg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    WorkOutputScoreAvg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ProductivityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProductivityScoreBasis = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataCoveragePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    IntensityAvg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ExceptionsCount = table.Column<int>(type: "integer", nullable: false),
                    TrendVsPreviousWeekJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weekly_employee_report", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wiki_pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentPageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastEditedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wiki_pages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wms_productivity_snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    TasksCompleted = table.Column<int>(type: "integer", nullable: false),
                    TasksOnTime = table.Column<int>(type: "integer", nullable: false),
                    OnTimeDeliveryRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    WorkOutputScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProductivityScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ActiveProjectsCount = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wms_productivity_snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_area_change_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ShiftAssignmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrentExpectedWorkArea = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RequestedWorkArea = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewComment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_area_change_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_management_daily_time_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalLoggedMinutes = table.Column<int>(type: "integer", nullable: false),
                    ActiveTaskAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_management_daily_time_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_schedule_days",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    IsWorkingDay = table.Column<bool>(type: "boolean", nullable: false),
                    WorkTimeType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<string>(type: "text", nullable: true),
                    EndTime = table.Column<string>(type: "text", nullable: true),
                    RequiredWorkMinutes = table.Column<int>(type: "integer", nullable: true),
                    BreakType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BreakStartTime = table.Column<string>(type: "text", nullable: true),
                    BreakEndTime = table.Column<string>(type: "text", nullable: true),
                    BreakDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    ExpectedWorkArea = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IsOvernight = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_schedule_days", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_schedule_holidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicHolidayId = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_schedule_holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    PullPublicHolidays = table.Column<bool>(type: "boolean", nullable: false),
                    Timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DefaultForNewEmployee = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workspace_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RemovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workspace_roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspaces", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_access_grant_requests_TenantId",
                table: "access_grant_requests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_daily_summary_TenantId",
                table: "activity_daily_summary",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_raw_buffer_TenantId",
                table: "activity_raw_buffer",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_snapshots_TenantId",
                table: "activity_snapshots",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_agent_commands_TenantId",
                table: "agent_commands",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_agent_health_logs_TenantId",
                table: "agent_health_logs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_agent_policies_TenantId",
                table: "agent_policies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_agent_sessions_TenantId",
                table: "agent_sessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_agent_work_location_evidence_TenantId",
                table: "agent_work_location_evidence",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_api_keys_TenantId",
                table: "api_keys",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_app_allowlist_audit_TenantId",
                table: "app_allowlist_audit",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_app_allowlists_TenantId",
                table: "app_allowlists",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_application_categories_TenantId",
                table: "application_categories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_application_usage_TenantId",
                table: "application_usage",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_corrections_TenantId",
                table: "attendance_corrections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_records_TenantId",
                table: "attendance_records",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_TenantId",
                table: "audit_logs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_billing_snapshots_TenantId",
                table: "billing_snapshots",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_biometric_audit_logs_TenantId",
                table: "biometric_audit_logs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_biometric_devices_TenantId",
                table: "biometric_devices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_biometric_enrollments_TenantId",
                table: "biometric_enrollments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_biometric_events_TenantId",
                table: "biometric_events",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_break_records_TenantId",
                table: "break_records",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_browser_activity_TenantId",
                table: "browser_activity",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_checklist_templates_TenantId",
                table: "checklist_templates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_clock_in_late_deduction_rules_TenantId",
                table: "clock_in_late_deduction_rules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_clock_in_policies_TenantId",
                table: "clock_in_policies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_compliance_exports_TenantId",
                table: "compliance_exports",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_employee_report_TenantId",
                table: "daily_employee_report",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_device_sessions_TenantId",
                table: "device_sessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_device_tracking_TenantId",
                table: "device_tracking",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_discrepancy_events_TenantId",
                table: "discrepancy_events",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_email_delivery_logs_TenantId",
                table: "email_delivery_logs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_addresses_TenantId",
                table: "employee_addresses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_bank_details_TenantId",
                table: "employee_bank_details",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_checklist_tasks_TenantId",
                table: "employee_checklist_tasks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_custom_fields_TenantId",
                table: "employee_custom_fields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_dependents_TenantId",
                table: "employee_dependents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_discrepancy_baselines_TenantId",
                table: "employee_discrepancy_baselines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_emergency_contacts_TenantId",
                table: "employee_emergency_contacts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_lifecycle_events_TenantId",
                table: "employee_lifecycle_events",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_monitoring_overrides_TenantId",
                table: "employee_monitoring_overrides",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_qualifications_TenantId",
                table: "employee_qualifications",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_remote_work_profiles_TenantId",
                table: "employee_remote_work_profiles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_salary_history_TenantId",
                table: "employee_salary_history",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_transfers_TenantId",
                table: "employee_transfers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_work_history_TenantId",
                table: "employee_work_history",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_work_location_settings_TenantId",
                table: "employee_work_location_settings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_entity_assets_TenantId",
                table: "entity_assets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_escalation_rules_TenantId",
                table: "escalation_rules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_feature_access_grants_TenantId",
                table: "feature_access_grants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_feature_flag_overrides_TenantId",
                table: "feature_flag_overrides",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_file_records_TenantId",
                table: "file_records",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_file_upload_reservations_TenantId",
                table: "file_upload_reservations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_tokens_TenantId",
                table: "invitation_tokens",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_legal_acceptance_records_TenantId",
                table: "legal_acceptance_records",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_legal_holds_TenantId",
                table: "legal_holds",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_management_coverage_records_TenantId",
                table: "management_coverage_records",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_meeting_sessions_TenantId",
                table: "meeting_sessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_alert_policy_TenantId",
                table: "monitoring_alert_policy",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_evidence_assets_TenantId",
                table: "monitoring_evidence_assets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_feature_toggles_TenantId",
                table: "monitoring_feature_toggles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_policy_overrides_TenantId",
                table: "monitoring_policy_overrides",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_snapshot_TenantId",
                table: "monitoring_snapshot",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_monthly_employee_report_TenantId",
                table: "monthly_employee_report",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_channels_TenantId",
                table: "notification_channels",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_templates_TenantId",
                table: "notification_templates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_observed_applications_TenantId",
                table: "observed_applications",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_offboarding_records_TenantId",
                table: "offboarding_records",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_methods_TenantId",
                table: "payment_methods",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_position_access_templates_TenantId",
                table: "position_access_templates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_presence_sessions_TenantId",
                table: "presence_sessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_project_link_invitations_TenantId",
                table: "project_link_invitations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_project_links_TenantId",
                table: "project_links",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_project_member_invitations_TenantId",
                table: "project_member_invitations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_projects_TenantId",
                table: "projects",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_public_holidays_TenantId",
                table: "public_holidays",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_rate_limit_rules_TenantId",
                table: "rate_limit_rules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_registered_agents_TenantId",
                table: "registered_agents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_remote_work_location_change_requests_TenantId",
                table: "remote_work_location_change_requests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_retention_policies_TenantId",
                table: "retention_policies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_roster_entries_TenantId",
                table: "roster_entries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_roster_periods_TenantId",
                table: "roster_periods",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_assignments_TenantId",
                table: "schedule_assignments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_scheduled_tasks_TenantId",
                table: "scheduled_tasks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_shift_assignments_TenantId",
                table: "shift_assignments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_TenantId",
                table: "shifts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_signalr_connections_TenantId",
                table: "signalr_connections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sso_providers_TenantId",
                table: "sso_providers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_events_TenantId",
                table: "support_ticket_events",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_internal_notes_TenantId",
                table: "support_ticket_internal_notes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_messages_TenantId",
                table: "support_ticket_messages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_support_tickets_TenantId",
                table: "support_tickets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_auth_policies_TenantId",
                table: "tenant_auth_policies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_branding_TenantId",
                table: "tenant_branding",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_provisioning_validation_results_TenantId",
                table: "tenant_provisioning_validation_results",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_settings_TenantId",
                table: "tenant_settings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_setup_services_TenantId",
                table: "tenant_setup_services",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_storage_stats_TenantId",
                table: "tenant_storage_stats",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_subscription_events_TenantId",
                table: "tenant_subscription_events",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_time_off_balances_audit_TenantId",
                table: "time_off_balances_audit",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_time_off_policy_rules_TenantId",
                table: "time_off_policy_rules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_user_external_identities_TenantId",
                table: "user_external_identities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_user_mfa_TenantId",
                table: "user_mfa",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_user_permission_overrides_TenantId",
                table: "user_permission_overrides",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_TenantId",
                table: "user_preferences",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_verification_evidence_assets_TenantId",
                table: "verification_evidence_assets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_verification_policies_TenantId",
                table: "verification_policies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_verification_records_TenantId",
                table: "verification_records",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_verification_reference_photos_TenantId",
                table: "verification_reference_photos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_webhook_deliveries_TenantId",
                table: "webhook_deliveries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_webhook_endpoints_TenantId",
                table: "webhook_endpoints",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_weekly_employee_report_TenantId",
                table: "weekly_employee_report",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_wms_productivity_snapshots_TenantId",
                table: "wms_productivity_snapshots",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_work_area_change_requests_TenantId",
                table: "work_area_change_requests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_work_management_daily_time_logs_TenantId",
                table: "work_management_daily_time_logs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_work_schedule_days_TenantId",
                table: "work_schedule_days",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_work_schedule_holidays_TenantId",
                table: "work_schedule_holidays",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_work_schedules_TenantId",
                table: "work_schedules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_workspaces_TenantId",
                table: "workspaces",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_grant_requests");

            migrationBuilder.DropTable(
                name: "activity_daily_summary");

            migrationBuilder.DropTable(
                name: "activity_raw_buffer");

            migrationBuilder.DropTable(
                name: "activity_snapshots");

            migrationBuilder.DropTable(
                name: "agent_commands");

            migrationBuilder.DropTable(
                name: "agent_health_logs");

            migrationBuilder.DropTable(
                name: "agent_policies");

            migrationBuilder.DropTable(
                name: "agent_sessions");

            migrationBuilder.DropTable(
                name: "agent_work_location_evidence");

            migrationBuilder.DropTable(
                name: "api_keys");

            migrationBuilder.DropTable(
                name: "app_allowlist_audit");

            migrationBuilder.DropTable(
                name: "app_allowlists");

            migrationBuilder.DropTable(
                name: "application_categories");

            migrationBuilder.DropTable(
                name: "application_usage");

            migrationBuilder.DropTable(
                name: "attendance_corrections");

            migrationBuilder.DropTable(
                name: "attendance_records");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "billing_snapshots");

            migrationBuilder.DropTable(
                name: "biometric_audit_logs");

            migrationBuilder.DropTable(
                name: "biometric_devices");

            migrationBuilder.DropTable(
                name: "biometric_enrollments");

            migrationBuilder.DropTable(
                name: "biometric_events");

            migrationBuilder.DropTable(
                name: "break_records");

            migrationBuilder.DropTable(
                name: "browser_activity");

            migrationBuilder.DropTable(
                name: "calendar_event_participants");

            migrationBuilder.DropTable(
                name: "checklist_templates");

            migrationBuilder.DropTable(
                name: "clock_in_late_deduction_rules");

            migrationBuilder.DropTable(
                name: "clock_in_policies");

            migrationBuilder.DropTable(
                name: "compliance_exports");

            migrationBuilder.DropTable(
                name: "custom_field_values");

            migrationBuilder.DropTable(
                name: "custom_fields");

            migrationBuilder.DropTable(
                name: "daily_employee_report");

            migrationBuilder.DropTable(
                name: "device_sessions");

            migrationBuilder.DropTable(
                name: "device_tracking");

            migrationBuilder.DropTable(
                name: "discrepancy_events");

            migrationBuilder.DropTable(
                name: "document_approvals");

            migrationBuilder.DropTable(
                name: "document_versions");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "email_delivery_logs");

            migrationBuilder.DropTable(
                name: "employee_addresses");

            migrationBuilder.DropTable(
                name: "employee_bank_details");

            migrationBuilder.DropTable(
                name: "employee_checklist_tasks");

            migrationBuilder.DropTable(
                name: "employee_custom_fields");

            migrationBuilder.DropTable(
                name: "employee_dependents");

            migrationBuilder.DropTable(
                name: "employee_discrepancy_baselines");

            migrationBuilder.DropTable(
                name: "employee_emergency_contacts");

            migrationBuilder.DropTable(
                name: "employee_lifecycle_events");

            migrationBuilder.DropTable(
                name: "employee_monitoring_overrides");

            migrationBuilder.DropTable(
                name: "employee_qualifications");

            migrationBuilder.DropTable(
                name: "employee_remote_work_profiles");

            migrationBuilder.DropTable(
                name: "employee_salary_history");

            migrationBuilder.DropTable(
                name: "employee_transfers");

            migrationBuilder.DropTable(
                name: "employee_work_history");

            migrationBuilder.DropTable(
                name: "employee_work_location_settings");

            migrationBuilder.DropTable(
                name: "entity_assets");

            migrationBuilder.DropTable(
                name: "escalation_rules");

            migrationBuilder.DropTable(
                name: "feature_access_grants");

            migrationBuilder.DropTable(
                name: "feature_flag_overrides");

            migrationBuilder.DropTable(
                name: "file_records");

            migrationBuilder.DropTable(
                name: "file_upload_reservations");

            migrationBuilder.DropTable(
                name: "global_app_catalog");

            migrationBuilder.DropTable(
                name: "invitation_tokens");

            migrationBuilder.DropTable(
                name: "labels");

            migrationBuilder.DropTable(
                name: "legal_acceptance_records");

            migrationBuilder.DropTable(
                name: "legal_holds");

            migrationBuilder.DropTable(
                name: "management_coverage_records");

            migrationBuilder.DropTable(
                name: "meeting_sessions");

            migrationBuilder.DropTable(
                name: "mfa_recovery_codes");

            migrationBuilder.DropTable(
                name: "module_catalog_price_history");

            migrationBuilder.DropTable(
                name: "monitoring_alert_policy");

            migrationBuilder.DropTable(
                name: "monitoring_evidence_assets");

            migrationBuilder.DropTable(
                name: "monitoring_feature_toggles");

            migrationBuilder.DropTable(
                name: "monitoring_policy_overrides");

            migrationBuilder.DropTable(
                name: "monitoring_snapshot");

            migrationBuilder.DropTable(
                name: "monthly_employee_report");

            migrationBuilder.DropTable(
                name: "notification_channels");

            migrationBuilder.DropTable(
                name: "notification_templates");

            migrationBuilder.DropTable(
                name: "observed_applications");

            migrationBuilder.DropTable(
                name: "offboarding_records");

            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "plan_features");

            migrationBuilder.DropTable(
                name: "platform_user_invites");

            migrationBuilder.DropTable(
                name: "position_access_templates");

            migrationBuilder.DropTable(
                name: "presence_sessions");

            migrationBuilder.DropTable(
                name: "project_link_invitations");

            migrationBuilder.DropTable(
                name: "project_links");

            migrationBuilder.DropTable(
                name: "project_member_invitations");

            migrationBuilder.DropTable(
                name: "project_members");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "public_holidays");

            migrationBuilder.DropTable(
                name: "rate_limit_rules");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "registered_agents");

            migrationBuilder.DropTable(
                name: "release_calendar");

            migrationBuilder.DropTable(
                name: "remote_work_location_change_requests");

            migrationBuilder.DropTable(
                name: "retention_policies");

            migrationBuilder.DropTable(
                name: "roster_entries");

            migrationBuilder.DropTable(
                name: "roster_periods");

            migrationBuilder.DropTable(
                name: "schedule_assignments");

            migrationBuilder.DropTable(
                name: "scheduled_tasks");

            migrationBuilder.DropTable(
                name: "setup_services");

            migrationBuilder.DropTable(
                name: "shift_assignments");

            migrationBuilder.DropTable(
                name: "shifts");

            migrationBuilder.DropTable(
                name: "signalr_connections");

            migrationBuilder.DropTable(
                name: "sso_providers");

            migrationBuilder.DropTable(
                name: "subscription_plan_price_history");

            migrationBuilder.DropTable(
                name: "support_ticket_events");

            migrationBuilder.DropTable(
                name: "support_ticket_internal_notes");

            migrationBuilder.DropTable(
                name: "support_ticket_messages");

            migrationBuilder.DropTable(
                name: "support_tickets");

            migrationBuilder.DropTable(
                name: "system_settings");

            migrationBuilder.DropTable(
                name: "task_approvals");

            migrationBuilder.DropTable(
                name: "task_assignments");

            migrationBuilder.DropTable(
                name: "task_checklist_items");

            migrationBuilder.DropTable(
                name: "task_checklists");

            migrationBuilder.DropTable(
                name: "task_documents");

            migrationBuilder.DropTable(
                name: "task_links");

            migrationBuilder.DropTable(
                name: "task_tags");

            migrationBuilder.DropTable(
                name: "task_watchers");

            migrationBuilder.DropTable(
                name: "tenant_auth_policies");

            migrationBuilder.DropTable(
                name: "tenant_branding");

            migrationBuilder.DropTable(
                name: "tenant_provisioning_validation_results");

            migrationBuilder.DropTable(
                name: "tenant_settings");

            migrationBuilder.DropTable(
                name: "tenant_setup_services");

            migrationBuilder.DropTable(
                name: "tenant_storage_stats");

            migrationBuilder.DropTable(
                name: "tenant_subscription_events");

            migrationBuilder.DropTable(
                name: "time_off_balances_audit");

            migrationBuilder.DropTable(
                name: "time_off_policy_rules");

            migrationBuilder.DropTable(
                name: "user_external_identities");

            migrationBuilder.DropTable(
                name: "user_mfa");

            migrationBuilder.DropTable(
                name: "user_permission_overrides");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "verification_evidence_assets");

            migrationBuilder.DropTable(
                name: "verification_policies");

            migrationBuilder.DropTable(
                name: "verification_records");

            migrationBuilder.DropTable(
                name: "verification_reference_photos");

            migrationBuilder.DropTable(
                name: "webhook_deliveries");

            migrationBuilder.DropTable(
                name: "webhook_endpoints");

            migrationBuilder.DropTable(
                name: "weekly_employee_report");

            migrationBuilder.DropTable(
                name: "wiki_pages");

            migrationBuilder.DropTable(
                name: "wms_productivity_snapshots");

            migrationBuilder.DropTable(
                name: "work_area_change_requests");

            migrationBuilder.DropTable(
                name: "work_management_daily_time_logs");

            migrationBuilder.DropTable(
                name: "work_schedule_days");

            migrationBuilder.DropTable(
                name: "work_schedule_holidays");

            migrationBuilder.DropTable(
                name: "work_schedules");

            migrationBuilder.DropTable(
                name: "workspace_members");

            migrationBuilder.DropTable(
                name: "workspace_roles");

            migrationBuilder.DropTable(
                name: "workspaces");

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TriggerKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowRuns_WorkflowDefinitions_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigJson = table.Column<string>(type: "jsonb", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    StepType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_WorkflowDefinitions_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowRunId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedToEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolverConfigJson = table.Column<string>(type: "jsonb", nullable: false),
                    ResolverType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTasks_WorkflowRuns_WorkflowRunId",
                        column: x => x.WorkflowRunId,
                        principalTable: "WorkflowRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_Status",
                table: "ApprovalTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_TenantId",
                table: "ApprovalTasks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTasks_WorkflowRunId",
                table: "ApprovalTasks",
                column: "WorkflowRunId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_TenantId",
                table: "WorkflowDefinitions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRuns_Status",
                table: "WorkflowRuns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRuns_TenantId",
                table: "WorkflowRuns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRuns_WorkflowDefinitionId",
                table: "WorkflowRuns",
                column: "WorkflowDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_WorkflowDefinitionId",
                table: "WorkflowSteps",
                column: "WorkflowDefinitionId");
        }
    }
}
