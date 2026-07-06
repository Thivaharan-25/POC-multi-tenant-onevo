using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    StartAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AudienceType = table.Column<string>(type: "text", nullable: false),
                    AudienceId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateKey = table.Column<string>(type: "text", nullable: false),
                    TemplateType = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    EmployeeRangeMin = table.Column<int>(type: "integer", nullable: true),
                    EmployeeRangeMax = table.Column<int>(type: "integer", nullable: true),
                    IndustryProfileTag = table.Column<string>(type: "text", nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DemoProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TrialDurationDays = table.Column<int>(type: "integer", nullable: false),
                    AutoExpire = table.Column<bool>(type: "boolean", nullable: false),
                    MaxEmployees = table.Column<int>(type: "integer", nullable: false),
                    DemoStorageLimitGb = table.Column<int>(type: "integer", nullable: false),
                    DemoAiTokenLimit = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DemoRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    BusinessEmail = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    RequestedCompanySizeRange = table.Column<string>(type: "text", nullable: false),
                    RequestedDemoProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByPlatformUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    ConvertedTenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeHierarchyClosures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagerEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Depth = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeHierarchyClosures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CountryCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleCatalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsFoundation = table.Column<bool>(type: "boolean", nullable: false),
                    IsSellable = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsStorage = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsAi = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionCatalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    IsTenantPermission = table.Column<bool>(type: "boolean", nullable: false),
                    IsPlatformPermission = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformAuthEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformAuthEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsSystemRole = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MfaEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsSystemRole = table.Column<bool>(type: "boolean", nullable: false),
                    SourceTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ModuleKeysJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuntimeFeatureFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    FeatureKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuntimeFeatureFlags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    BillingCycle = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SharedBaseStorageGb = table.Column<int>(type: "integer", nullable: false),
                    SharedBaseAiTokenAllowance = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantResourceLimits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageLimitGb = table.Column<int>(type: "integer", nullable: false),
                    AiTokenLimit = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeLimit = table.Column<int>(type: "integer", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantResourceLimits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    SourceDemoRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    DemoProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    EstimatedEmployeeCount = table.Column<int>(type: "integer", nullable: true),
                    ConfirmedEmployeeCount = table.Column<int>(type: "integer", nullable: true),
                    TrialStartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TrialEndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActivatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TriggerKey = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantConfigurationTemplateApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigurationTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateType = table.Column<string>(type: "text", nullable: false),
                    AppliedVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AppliedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppliedByPlatformUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomPayloadJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantConfigurationTemplateApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantConfigurationTemplateApplications_ConfigurationTempla~",
                        column: x => x.ConfigurationTemplateId,
                        principalTable: "ConfigurationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntitledDays = table.Column<decimal>(type: "numeric", nullable: false),
                    UsedDays = table.Column<decimal>(type: "numeric", nullable: false),
                    RemainingDays = table.Column<decimal>(type: "numeric", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveBalances_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeavePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntitlementDays = table.Column<decimal>(type: "numeric", nullable: false),
                    CarryForwardAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    CarryForwardLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePolicies_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ConflictSnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DemoProfileModuleAccess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DemoProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleCatalogId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessLevel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoProfileModuleAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemoProfileModuleAccess_DemoProfiles_DemoProfileId",
                        column: x => x.DemoProfileId,
                        principalTable: "DemoProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DemoProfileModuleAccess_ModuleCatalogs_ModuleCatalogId",
                        column: x => x.ModuleCatalogId,
                        principalTable: "ModuleCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModuleFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleCatalogId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleFeatures_ModuleCatalogs_ModuleCatalogId",
                        column: x => x.ModuleCatalogId,
                        principalTable: "ModuleCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionAddOns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AddOnKey = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AddOnType = table.Column<string>(type: "text", nullable: false),
                    ModuleCatalogId = table.Column<Guid>(type: "uuid", nullable: true),
                    StorageContributionGb = table.Column<int>(type: "integer", nullable: true),
                    AiTokenContribution = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionAddOns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionAddOns_ModuleCatalogs_ModuleCatalogId",
                        column: x => x.ModuleCatalogId,
                        principalTable: "ModuleCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantModuleEntitlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleCatalogId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    RuntimeOverride = table.Column<string>(type: "text", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantModuleEntitlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantModuleEntitlements_ModuleCatalogs_ModuleCatalogId",
                        column: x => x.ModuleCatalogId,
                        principalTable: "ModuleCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModulePermissionOwnerships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleCatalogId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionCatalogId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulePermissionOwnerships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModulePermissionOwnerships_ModuleCatalogs_ModuleCatalogId",
                        column: x => x.ModuleCatalogId,
                        principalTable: "ModuleCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModulePermissionOwnerships_PermissionCatalogs_PermissionCat~",
                        column: x => x.PermissionCatalogId,
                        principalTable: "PermissionCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlatformRolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformPermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformRolePermissions_PlatformPermissions_PlatformPermiss~",
                        column: x => x.PlatformPermissionId,
                        principalTable: "PlatformPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformRolePermissions_PlatformRoles_PlatformRoleId",
                        column: x => x.PlatformRoleId,
                        principalTable: "PlatformRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlatformUserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformRoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformUserRoles_PlatformRoles_PlatformRoleId",
                        column: x => x.PlatformRoleId,
                        principalTable: "PlatformRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformUserRoles_PlatformUsers_PlatformUserId",
                        column: x => x.PlatformUserId,
                        principalTable: "PlatformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlatformUserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionTokenHash = table.Column<string>(type: "text", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformUserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformUserSessions_PlatformUsers_PlatformUserId",
                        column: x => x.PlatformUserId,
                        principalTable: "PlatformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionCatalogId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_PermissionCatalogs_PermissionCatalogId",
                        column: x => x.PermissionCatalogId,
                        principalTable: "PermissionCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleTemplatePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionCatalogId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTemplatePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleTemplatePermissions_PermissionCatalogs_PermissionCatalo~",
                        column: x => x.PermissionCatalogId,
                        principalTable: "PermissionCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleTemplatePermissions_RoleTemplates_RoleTemplateId",
                        column: x => x.RoleTemplateId,
                        principalTable: "RoleTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleTemplateVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTemplateVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleTemplateVersions_RoleTemplates_RoleTemplateId",
                        column: x => x.RoleTemplateId,
                        principalTable: "RoleTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DemoProfileUpgradeOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DemoProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAllowed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoProfileUpgradeOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemoProfileUpgradeOptions_DemoProfiles_DemoProfileId",
                        column: x => x.DemoProfileId,
                        principalTable: "DemoProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DemoProfileUpgradeOptions_SubscriptionPlans_SubscriptionPla~",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlanPriceBrackets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeRangeMin = table.Column<int>(type: "integer", nullable: false),
                    EmployeeRangeMax = table.Column<int>(type: "integer", nullable: true),
                    MonthlyPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AnnualPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlanPriceBrackets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanPriceBrackets_SubscriptionPlans_Subscriptio~",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LegalEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalEntities_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantActivationChecklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequiredForActivation = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantActivationChecklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantActivationChecklists_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantDomains",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Domain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantDomains_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantProvisioningStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStep = table.Column<string>(type: "text", nullable: false),
                    OrganizationInfoCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminAccountCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SubscriptionCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModuleSelectionCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TemplateApplicationCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivationReady = table.Column<bool>(type: "boolean", nullable: false),
                    ActivationBlockedReason = table.Column<string>(type: "text", nullable: true),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProvisioningStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantProvisioningStates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    BillingCycle = table.Column<string>(type: "text", nullable: true),
                    ConfirmedEmployeeCount = table.Column<int>(type: "integer", nullable: true),
                    SelectedFeatureKeysJson = table.Column<string>(type: "jsonb", nullable: false),
                    SelectedAddOnsJson = table.Column<string>(type: "jsonb", nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantSubscriptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    StepType = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ConfigJson = table.Column<string>(type: "jsonb", nullable: false)
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
                name: "LeavePolicyAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentScope = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicyAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePolicyAssignments_LeavePolicies_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DemoProfileFeatureAccess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DemoProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoProfileFeatureAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemoProfileFeatureAccess_DemoProfiles_DemoProfileId",
                        column: x => x.DemoProfileId,
                        principalTable: "DemoProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DemoProfileFeatureAccess_ModuleFeatures_ModuleFeatureId",
                        column: x => x.ModuleFeatureId,
                        principalTable: "ModuleFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeaturePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionCatalogId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturePermissions_ModuleFeatures_ModuleFeatureId",
                        column: x => x.ModuleFeatureId,
                        principalTable: "ModuleFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeaturePermissions_PermissionCatalogs_PermissionCatalogId",
                        column: x => x.PermissionCatalogId,
                        principalTable: "PermissionCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlanFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleFeatureId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlanFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanFeatures_ModuleFeatures_ModuleFeatureId",
                        column: x => x.ModuleFeatureId,
                        principalTable: "ModuleFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanFeatures_SubscriptionPlans_SubscriptionPlan~",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantFeatureEntitlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantFeatureEntitlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantFeatureEntitlements_ModuleFeatures_ModuleFeatureId",
                        column: x => x.ModuleFeatureId,
                        principalTable: "ModuleFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DemoProfileAllowedAddOns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DemoProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionAddOnId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAllowed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoProfileAllowedAddOns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemoProfileAllowedAddOns_DemoProfiles_DemoProfileId",
                        column: x => x.DemoProfileId,
                        principalTable: "DemoProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DemoProfileAllowedAddOns_SubscriptionAddOns_SubscriptionAdd~",
                        column: x => x.SubscriptionAddOnId,
                        principalTable: "SubscriptionAddOns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeNumber = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    WorkEmail = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrentPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_LegalEntities_LegalEntityId",
                        column: x => x.LegalEntityId,
                        principalTable: "LegalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantSubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionInvoices_TenantSubscriptions_TenantSubscription~",
                        column: x => x.TenantSubscriptionId,
                        principalTable: "TenantSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeType = table.Column<string>(type: "text", nullable: true),
                    ScopeTargetId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionTokenHash = table.Column<string>(type: "text", nullable: false),
                    CsrfTokenHash = table.Column<string>(type: "text", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowRunId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedToEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResolverType = table.Column<string>(type: "text", nullable: false),
                    ResolverConfigJson = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "EmployeeAssignmentHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EffectiveFromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAssignmentHistories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobTitle = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProfiles_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ParentDepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    HeadPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Departments_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departments_LegalEntities_LegalEntityId",
                        column: x => x.LegalEntityId,
                        principalTable: "LegalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ReportsToPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    PositionType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SuggestedRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_LegalEntities_LegalEntityId",
                        column: x => x.LegalEntityId,
                        principalTable: "LegalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_Positions_ReportsToPositionId",
                        column: x => x.ReportsToPositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PositionAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PositionAssignments_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PositionReportingHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportsToPositionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EffectiveFromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionReportingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionReportingHistories_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PositionRoleAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionRoleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionRoleAssignments_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PositionRoleAssignments_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
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
                name: "IX_CalendarEvents_TenantId",
                table: "CalendarEvents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationTemplates_TemplateKey",
                table: "ConfigurationTemplates",
                column: "TemplateKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfileAllowedAddOns_DemoProfileId_SubscriptionAddOnId",
                table: "DemoProfileAllowedAddOns",
                columns: new[] { "DemoProfileId", "SubscriptionAddOnId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfileAllowedAddOns_SubscriptionAddOnId",
                table: "DemoProfileAllowedAddOns",
                column: "SubscriptionAddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfileFeatureAccess_DemoProfileId_ModuleFeatureId",
                table: "DemoProfileFeatureAccess",
                columns: new[] { "DemoProfileId", "ModuleFeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfileFeatureAccess_ModuleFeatureId",
                table: "DemoProfileFeatureAccess",
                column: "ModuleFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfileModuleAccess_DemoProfileId_ModuleCatalogId",
                table: "DemoProfileModuleAccess",
                columns: new[] { "DemoProfileId", "ModuleCatalogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfileModuleAccess_ModuleCatalogId",
                table: "DemoProfileModuleAccess",
                column: "ModuleCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfiles_Name",
                table: "DemoProfiles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfileUpgradeOptions_DemoProfileId_SubscriptionPlanId",
                table: "DemoProfileUpgradeOptions",
                columns: new[] { "DemoProfileId", "SubscriptionPlanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemoProfileUpgradeOptions_SubscriptionPlanId",
                table: "DemoProfileUpgradeOptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DemoRequests_BusinessEmail",
                table: "DemoRequests",
                column: "BusinessEmail");

            migrationBuilder.CreateIndex(
                name: "IX_DemoRequests_Status",
                table: "DemoRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadPositionId",
                table: "Departments",
                column: "HeadPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_LegalEntityId",
                table: "Departments",
                column: "LegalEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_TenantId",
                table: "Departments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_TenantId_LegalEntityId_Code",
                table: "Departments",
                columns: new[] { "TenantId", "LegalEntityId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssignmentHistories_EmployeeId",
                table: "EmployeeAssignmentHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssignmentHistories_TenantId_EmployeeId",
                table: "EmployeeAssignmentHistories",
                columns: new[] { "TenantId", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHierarchyClosures_TenantId_ManagerEmployeeId_Report~",
                table: "EmployeeHierarchyClosures",
                columns: new[] { "TenantId", "ManagerEmployeeId", "ReportEmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_EmployeeId",
                table: "EmployeeProfiles",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LegalEntityId",
                table: "Employees",
                column: "LegalEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Status",
                table: "Employees",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TenantId",
                table: "Employees",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TenantId_EmployeeNumber",
                table: "Employees",
                columns: new[] { "TenantId", "EmployeeNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_WorkEmail",
                table: "Employees",
                column: "WorkEmail");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturePermissions_ModuleFeatureId_PermissionCatalogId",
                table: "FeaturePermissions",
                columns: new[] { "ModuleFeatureId", "PermissionCatalogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeaturePermissions_PermissionCatalogId",
                table: "FeaturePermissions",
                column: "PermissionCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_TenantId_Date",
                table: "Holidays",
                columns: new[] { "TenantId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_LeaveTypeId",
                table: "LeaveBalances",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_TenantId_EmployeeId_LeaveTypeId_Year",
                table: "LeaveBalances",
                columns: new[] { "TenantId", "EmployeeId", "LeaveTypeId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicies_LeaveTypeId",
                table: "LeavePolicies",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicies_TenantId",
                table: "LeavePolicies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyAssignments_LeavePolicyId",
                table: "LeavePolicyAssignments",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyAssignments_TenantId",
                table: "LeavePolicyAssignments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_LeaveTypeId",
                table: "LeaveRequests",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_Status",
                table: "LeaveRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_TenantId_EmployeeId",
                table: "LeaveRequests",
                columns: new[] { "TenantId", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_TenantId_Code",
                table: "LeaveTypes",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegalEntities_TenantId",
                table: "LegalEntities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEntities_TenantId_Code",
                table: "LegalEntities",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleCatalogs_ModuleKey",
                table: "ModuleCatalogs",
                column: "ModuleKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleFeatures_FeatureKey",
                table: "ModuleFeatures",
                column: "FeatureKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleFeatures_ModuleCatalogId",
                table: "ModuleFeatures",
                column: "ModuleCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_ModulePermissionOwnerships_ModuleCatalogId_PermissionCatalo~",
                table: "ModulePermissionOwnerships",
                columns: new[] { "ModuleCatalogId", "PermissionCatalogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModulePermissionOwnerships_PermissionCatalogId",
                table: "ModulePermissionOwnerships",
                column: "PermissionCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantId_UserId",
                table: "Notifications",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionCatalogs_PermissionKey",
                table: "PermissionCatalogs",
                column: "PermissionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAuthEvents_Email",
                table: "PlatformAuthEvents",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformPermissions_PermissionKey",
                table: "PlatformPermissions",
                column: "PermissionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRolePermissions_PlatformPermissionId",
                table: "PlatformRolePermissions",
                column: "PlatformPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRolePermissions_PlatformRoleId_PlatformPermissionId",
                table: "PlatformRolePermissions",
                columns: new[] { "PlatformRoleId", "PlatformPermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformUserRoles_PlatformRoleId",
                table: "PlatformUserRoles",
                column: "PlatformRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformUserRoles_PlatformUserId_PlatformRoleId",
                table: "PlatformUserRoles",
                columns: new[] { "PlatformUserId", "PlatformRoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformUsers_Email",
                table: "PlatformUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformUserSessions_PlatformUserId",
                table: "PlatformUserSessions",
                column: "PlatformUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformUserSessions_SessionTokenHash",
                table: "PlatformUserSessions",
                column: "SessionTokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_PositionAssignments_EmployeeId",
                table: "PositionAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionAssignments_PositionId",
                table: "PositionAssignments",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionAssignments_TenantId_EmployeeId",
                table: "PositionAssignments",
                columns: new[] { "TenantId", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_PositionAssignments_TenantId_PositionId",
                table: "PositionAssignments",
                columns: new[] { "TenantId", "PositionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PositionReportingHistories_PositionId",
                table: "PositionReportingHistories",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionReportingHistories_TenantId_PositionId",
                table: "PositionReportingHistories",
                columns: new[] { "TenantId", "PositionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PositionRoleAssignments_PositionId",
                table: "PositionRoleAssignments",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionRoleAssignments_RoleId",
                table: "PositionRoleAssignments",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionRoleAssignments_TenantId_PositionId_RoleId",
                table: "PositionRoleAssignments",
                columns: new[] { "TenantId", "PositionId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_DepartmentId",
                table: "Positions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_LegalEntityId",
                table: "Positions",
                column: "LegalEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ReportsToPositionId",
                table: "Positions",
                column: "ReportsToPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_Status",
                table: "Positions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_TenantId",
                table: "Positions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_TenantId_LegalEntityId_Code",
                table: "Positions",
                columns: new[] { "TenantId", "LegalEntityId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionCatalogId",
                table: "RolePermissions",
                column: "PermissionCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionCatalogId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionCatalogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId_Name",
                table: "Roles",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleTemplatePermissions_PermissionCatalogId",
                table: "RoleTemplatePermissions",
                column: "PermissionCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleTemplatePermissions_RoleTemplateId_PermissionCatalogId",
                table: "RoleTemplatePermissions",
                columns: new[] { "RoleTemplateId", "PermissionCatalogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleTemplates_Name",
                table: "RoleTemplates",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleTemplateVersions_RoleTemplateId_Version",
                table: "RoleTemplateVersions",
                columns: new[] { "RoleTemplateId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RuntimeFeatureFlags_TenantId_FeatureKey",
                table: "RuntimeFeatureFlags",
                columns: new[] { "TenantId", "FeatureKey" });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddOns_AddOnKey",
                table: "SubscriptionAddOns",
                column: "AddOnKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddOns_ModuleCatalogId",
                table: "SubscriptionAddOns",
                column: "ModuleCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionInvoices_InvoiceNumber",
                table: "SubscriptionInvoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionInvoices_Status",
                table: "SubscriptionInvoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionInvoices_TenantId",
                table: "SubscriptionInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionInvoices_TenantSubscriptionId",
                table: "SubscriptionInvoices",
                column: "TenantSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanFeatures_ModuleFeatureId",
                table: "SubscriptionPlanFeatures",
                column: "ModuleFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanFeatures_SubscriptionPlanId_ModuleFeatureId",
                table: "SubscriptionPlanFeatures",
                columns: new[] { "SubscriptionPlanId", "ModuleFeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanPriceBrackets_SubscriptionPlanId",
                table: "SubscriptionPlanPriceBrackets",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_Code",
                table: "SubscriptionPlans",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantActivationChecklists_TenantId_Key",
                table: "TenantActivationChecklists",
                columns: new[] { "TenantId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantConfigurationTemplateApplications_ConfigurationTempla~",
                table: "TenantConfigurationTemplateApplications",
                column: "ConfigurationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantConfigurationTemplateApplications_TenantId",
                table: "TenantConfigurationTemplateApplications",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantDomains_Domain",
                table: "TenantDomains",
                column: "Domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantDomains_TenantId",
                table: "TenantDomains",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatureEntitlements_ModuleFeatureId",
                table: "TenantFeatureEntitlements",
                column: "ModuleFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatureEntitlements_TenantId_ModuleFeatureId",
                table: "TenantFeatureEntitlements",
                columns: new[] { "TenantId", "ModuleFeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantModuleEntitlements_ModuleCatalogId",
                table: "TenantModuleEntitlements",
                column: "ModuleCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantModuleEntitlements_TenantId_ModuleCatalogId",
                table: "TenantModuleEntitlements",
                columns: new[] { "TenantId", "ModuleCatalogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantProvisioningStates_TenantId",
                table: "TenantProvisioningStates",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantResourceLimits_TenantId",
                table: "TenantResourceLimits",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Status",
                table: "Tenants",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSubscriptions_Status",
                table: "TenantSubscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSubscriptions_SubscriptionPlanId",
                table: "TenantSubscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSubscriptions_TenantId",
                table: "TenantSubscriptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Email",
                table: "Users",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_SessionTokenHash",
                table: "UserSessions",
                column: "SessionTokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_TenantId",
                table: "UserSessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Positions_HeadPositionId",
                table: "Departments",
                column: "HeadPositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_LegalEntities_LegalEntityId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_LegalEntities_LegalEntityId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Positions_HeadPositionId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "ApprovalTasks");

            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "DemoProfileAllowedAddOns");

            migrationBuilder.DropTable(
                name: "DemoProfileFeatureAccess");

            migrationBuilder.DropTable(
                name: "DemoProfileModuleAccess");

            migrationBuilder.DropTable(
                name: "DemoProfileUpgradeOptions");

            migrationBuilder.DropTable(
                name: "DemoRequests");

            migrationBuilder.DropTable(
                name: "EmployeeAssignmentHistories");

            migrationBuilder.DropTable(
                name: "EmployeeHierarchyClosures");

            migrationBuilder.DropTable(
                name: "EmployeeProfiles");

            migrationBuilder.DropTable(
                name: "FeaturePermissions");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "LeaveBalances");

            migrationBuilder.DropTable(
                name: "LeavePolicyAssignments");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "ModulePermissionOwnerships");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "PlatformAuthEvents");

            migrationBuilder.DropTable(
                name: "PlatformRolePermissions");

            migrationBuilder.DropTable(
                name: "PlatformUserRoles");

            migrationBuilder.DropTable(
                name: "PlatformUserSessions");

            migrationBuilder.DropTable(
                name: "PositionAssignments");

            migrationBuilder.DropTable(
                name: "PositionReportingHistories");

            migrationBuilder.DropTable(
                name: "PositionRoleAssignments");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "RoleTemplatePermissions");

            migrationBuilder.DropTable(
                name: "RoleTemplateVersions");

            migrationBuilder.DropTable(
                name: "RuntimeFeatureFlags");

            migrationBuilder.DropTable(
                name: "SubscriptionInvoices");

            migrationBuilder.DropTable(
                name: "SubscriptionPlanFeatures");

            migrationBuilder.DropTable(
                name: "SubscriptionPlanPriceBrackets");

            migrationBuilder.DropTable(
                name: "TenantActivationChecklists");

            migrationBuilder.DropTable(
                name: "TenantConfigurationTemplateApplications");

            migrationBuilder.DropTable(
                name: "TenantDomains");

            migrationBuilder.DropTable(
                name: "TenantFeatureEntitlements");

            migrationBuilder.DropTable(
                name: "TenantModuleEntitlements");

            migrationBuilder.DropTable(
                name: "TenantProvisioningStates");

            migrationBuilder.DropTable(
                name: "TenantResourceLimits");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "WorkflowRuns");

            migrationBuilder.DropTable(
                name: "SubscriptionAddOns");

            migrationBuilder.DropTable(
                name: "DemoProfiles");

            migrationBuilder.DropTable(
                name: "LeavePolicies");

            migrationBuilder.DropTable(
                name: "PlatformPermissions");

            migrationBuilder.DropTable(
                name: "PlatformRoles");

            migrationBuilder.DropTable(
                name: "PlatformUsers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "PermissionCatalogs");

            migrationBuilder.DropTable(
                name: "RoleTemplates");

            migrationBuilder.DropTable(
                name: "TenantSubscriptions");

            migrationBuilder.DropTable(
                name: "ConfigurationTemplates");

            migrationBuilder.DropTable(
                name: "ModuleFeatures");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropTable(
                name: "LeaveTypes");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "ModuleCatalogs");

            migrationBuilder.DropTable(
                name: "LegalEntities");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
