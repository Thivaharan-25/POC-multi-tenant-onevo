using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase1CanonicalSchemaAlignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Pre-alignment seed rows lose their identity columns in this migration
            // (price brackets lose EmployeeRangeMin/Max + MonthlyPrice and would all get
            // CompanySizeRange = ""; upgrade options lose SubscriptionPlanId, leaving
            // several rows per profile), which violates the new unique indexes.
            migrationBuilder.Sql("DELETE FROM \"SubscriptionPlanPriceBrackets\";");
            migrationBuilder.Sql("DELETE FROM \"DemoProfileUpgradeOptions\";");

            migrationBuilder.DropForeignKey(
                name: "FK_DemoProfileModuleAccess_DemoProfiles_DemoProfileId",
                table: "DemoProfileModuleAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_DemoProfileModuleAccess_ModuleCatalogs_ModuleCatalogId",
                table: "DemoProfileModuleAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_DemoProfileUpgradeOptions_DemoProfiles_DemoProfileId",
                table: "DemoProfileUpgradeOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_DemoProfileUpgradeOptions_SubscriptionPlans_SubscriptionPla~",
                table: "DemoProfileUpgradeOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_LegalEntities_LegalEntityId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Positions_HeadPositionId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeAssignmentHistories_Employees_EmployeeId",
                table: "EmployeeAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_LegalEntities_LegalEntityId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Tenants_TenantId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalCalendarConnections_Tenants_TenantId",
                table: "ExternalCalendarConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalCalendarConnections_Users_UserId",
                table: "ExternalCalendarConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalCalendarEventLinks_CalendarEvents_CalendarEventId",
                table: "ExternalCalendarEventLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalCalendarEventLinks_ExternalCalendarConnections_Exte~",
                table: "ExternalCalendarEventLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalCalendarEventLinks_Tenants_TenantId",
                table: "ExternalCalendarEventLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveBalances_LeaveTypes_LeaveTypeId",
                table: "LeaveBalances");

            migrationBuilder.DropForeignKey(
                name: "FK_LeavePolicies_LeaveTypes_LeaveTypeId",
                table: "LeavePolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_LeavePolicyAssignments_LeavePolicies_LeavePolicyId",
                table: "LeavePolicyAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalEntities_Tenants_TenantId",
                table: "LegalEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleFeatures_ModuleCatalogs_ModuleCatalogId",
                table: "ModuleFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_ModulePermissionOwnerships_ModuleCatalogs_ModuleCatalogId",
                table: "ModulePermissionOwnerships");

            migrationBuilder.DropForeignKey(
                name: "FK_ModulePermissionOwnerships_PermissionCatalogs_PermissionCat~",
                table: "ModulePermissionOwnerships");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentGatewayCountryRoutes_PaymentGatewayConfigs_GatewayCo~",
                table: "PaymentGatewayCountryRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentGatewayCredentials_PaymentGatewayConfigs_PaymentGate~",
                table: "PaymentGatewayCredentials");

            migrationBuilder.DropForeignKey(
                name: "FK_PlatformRolePermissions_PlatformPermissions_PlatformPermiss~",
                table: "PlatformRolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlatformRolePermissions_PlatformRoles_PlatformRoleId",
                table: "PlatformRolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlatformUserRoles_PlatformRoles_PlatformRoleId",
                table: "PlatformUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_PlatformUserRoles_PlatformUsers_PlatformUserId",
                table: "PlatformUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_PlatformUserSessions_PlatformUsers_PlatformUserId",
                table: "PlatformUserSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionAssignments_Employees_EmployeeId",
                table: "PositionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionAssignments_Positions_PositionId",
                table: "PositionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionReportingHistories_Positions_PositionId",
                table: "PositionReportingHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Departments_DepartmentId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_LegalEntities_LegalEntityId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Positions_ReportsToPositionId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_PermissionCatalogs_PermissionCatalogId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionInvoices_TenantSubscriptions_TenantSubscription~",
                table: "SubscriptionInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionPlanPriceBrackets_SubscriptionPlans_Subscriptio~",
                table: "SubscriptionPlanPriceBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantConfigurationTemplateApplications_ConfigurationTempla~",
                table: "TenantConfigurationTemplateApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantModuleEntitlements_ModuleCatalogs_ModuleCatalogId",
                table: "TenantModuleEntitlements");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantProvisioningStates_Tenants_TenantId",
                table: "TenantProvisioningStates");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "TenantSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantSubscriptions_Tenants_TenantId",
                table: "TenantSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions");

            migrationBuilder.DropTable(
                name: "DemoProfileAllowedAddOns");

            migrationBuilder.DropTable(
                name: "DemoProfileFeatureAccess");

            migrationBuilder.DropTable(
                name: "EmployeeProfiles");

            migrationBuilder.DropTable(
                name: "FeaturePermissions");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "PositionRoleAssignments");

            migrationBuilder.DropTable(
                name: "RoleTemplatePermissions");

            migrationBuilder.DropTable(
                name: "RoleTemplateVersions");

            migrationBuilder.DropTable(
                name: "SubscriptionPlanFeatures");

            migrationBuilder.DropTable(
                name: "TenantActivationChecklists");

            migrationBuilder.DropTable(
                name: "TenantDomains");

            migrationBuilder.DropTable(
                name: "TenantFeatureEntitlements");

            migrationBuilder.DropTable(
                name: "TenantResourceLimits");

            migrationBuilder.DropTable(
                name: "SubscriptionAddOns");

            migrationBuilder.DropTable(
                name: "PermissionCatalogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Positions",
                table: "Positions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSessions",
                table: "UserSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantSubscriptions",
                table: "TenantSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantProvisioningStates",
                table: "TenantProvisioningStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantModuleEntitlements",
                table: "TenantModuleEntitlements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantConfigurationTemplateApplications",
                table: "TenantConfigurationTemplateApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPlans",
                table: "SubscriptionPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPlanPriceBrackets",
                table: "SubscriptionPlanPriceBrackets");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionPlanPriceBrackets_SubscriptionPlanId",
                table: "SubscriptionPlanPriceBrackets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionInvoices",
                table: "SubscriptionInvoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RuntimeFeatureFlags",
                table: "RuntimeFeatureFlags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleTemplates",
                table: "RoleTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PositionReportingHistories",
                table: "PositionReportingHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PositionAssignments",
                table: "PositionAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformUserSessions",
                table: "PlatformUserSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformUsers",
                table: "PlatformUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformUserRoles",
                table: "PlatformUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformRoles",
                table: "PlatformRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformRolePermissions",
                table: "PlatformRolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformPermissions",
                table: "PlatformPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformAuthEvents",
                table: "PlatformAuthEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentGatewayCredentials",
                table: "PaymentGatewayCredentials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentGatewayCountryRoutes",
                table: "PaymentGatewayCountryRoutes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentGatewayConfigs",
                table: "PaymentGatewayConfigs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModulePermissionOwnerships",
                table: "ModulePermissionOwnerships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuleFeatures",
                table: "ModuleFeatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuleCatalogs",
                table: "ModuleCatalogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LegalEntities",
                table: "LegalEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveTypes",
                table: "LeaveTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequests",
                table: "LeaveRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeavePolicyAssignments",
                table: "LeavePolicyAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeavePolicies",
                table: "LeavePolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveBalances",
                table: "LeaveBalances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalCalendarEventLinks",
                table: "ExternalCalendarEventLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalCalendarConnections",
                table: "ExternalCalendarConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeHierarchyClosures",
                table: "EmployeeHierarchyClosures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeAssignmentHistories",
                table: "EmployeeAssignmentHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DemoRequests",
                table: "DemoRequests");

            migrationBuilder.DropIndex(
                name: "IX_DemoRequests_BusinessEmail",
                table: "DemoRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DemoProfileUpgradeOptions",
                table: "DemoProfileUpgradeOptions");

            migrationBuilder.DropIndex(
                name: "IX_DemoProfileUpgradeOptions_DemoProfileId_SubscriptionPlanId",
                table: "DemoProfileUpgradeOptions");

            migrationBuilder.DropIndex(
                name: "IX_DemoProfileUpgradeOptions_SubscriptionPlanId",
                table: "DemoProfileUpgradeOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DemoProfiles",
                table: "DemoProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DemoProfileModuleAccess",
                table: "DemoProfileModuleAccess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConfigurationTemplates",
                table: "ConfigurationTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalendarEvents",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "EmployeeRangeMax",
                table: "SubscriptionPlanPriceBrackets");

            migrationBuilder.DropColumn(
                name: "EmployeeRangeMin",
                table: "SubscriptionPlanPriceBrackets");

            migrationBuilder.DropColumn(
                name: "MonthlyPrice",
                table: "SubscriptionPlanPriceBrackets");

            migrationBuilder.DropColumn(
                name: "IsAllowed",
                table: "DemoProfileUpgradeOptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "DemoProfileUpgradeOptions");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "tenants");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "roles");

            migrationBuilder.RenameTable(
                name: "Positions",
                newName: "positions");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "notifications");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "employees");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "departments");

            migrationBuilder.RenameTable(
                name: "UserSessions",
                newName: "sessions");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "user_roles");

            migrationBuilder.RenameTable(
                name: "TenantSubscriptions",
                newName: "tenant_subscriptions");

            migrationBuilder.RenameTable(
                name: "TenantProvisioningStates",
                newName: "tenant_provisioning_states");

            migrationBuilder.RenameTable(
                name: "TenantModuleEntitlements",
                newName: "tenant_module_entitlements");

            migrationBuilder.RenameTable(
                name: "TenantConfigurationTemplateApplications",
                newName: "tenant_configuration_template_applications");

            migrationBuilder.RenameTable(
                name: "SubscriptionPlans",
                newName: "subscription_plans");

            migrationBuilder.RenameTable(
                name: "SubscriptionPlanPriceBrackets",
                newName: "subscription_plan_price_brackets");

            migrationBuilder.RenameTable(
                name: "SubscriptionInvoices",
                newName: "subscription_invoices");

            migrationBuilder.RenameTable(
                name: "RuntimeFeatureFlags",
                newName: "feature_flags");

            migrationBuilder.RenameTable(
                name: "RoleTemplates",
                newName: "role_templates");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "role_permissions");

            migrationBuilder.RenameTable(
                name: "PositionReportingHistories",
                newName: "position_reporting_history");

            migrationBuilder.RenameTable(
                name: "PositionAssignments",
                newName: "position_assignments");

            migrationBuilder.RenameTable(
                name: "PlatformUserSessions",
                newName: "platform_user_sessions");

            migrationBuilder.RenameTable(
                name: "PlatformUsers",
                newName: "platform_users");

            migrationBuilder.RenameTable(
                name: "PlatformUserRoles",
                newName: "platform_user_roles");

            migrationBuilder.RenameTable(
                name: "PlatformRoles",
                newName: "platform_roles");

            migrationBuilder.RenameTable(
                name: "PlatformRolePermissions",
                newName: "platform_role_permissions");

            migrationBuilder.RenameTable(
                name: "PlatformPermissions",
                newName: "platform_permissions");

            migrationBuilder.RenameTable(
                name: "PlatformAuthEvents",
                newName: "platform_auth_events");

            migrationBuilder.RenameTable(
                name: "PaymentGatewayCredentials",
                newName: "payment_gateway_credentials");

            migrationBuilder.RenameTable(
                name: "PaymentGatewayCountryRoutes",
                newName: "payment_gateway_country_routes");

            migrationBuilder.RenameTable(
                name: "PaymentGatewayConfigs",
                newName: "payment_gateway_configs");

            migrationBuilder.RenameTable(
                name: "ModulePermissionOwnerships",
                newName: "module_permission_ownership");

            migrationBuilder.RenameTable(
                name: "ModuleFeatures",
                newName: "module_features");

            migrationBuilder.RenameTable(
                name: "ModuleCatalogs",
                newName: "module_catalog");

            migrationBuilder.RenameTable(
                name: "LegalEntities",
                newName: "legal_entities");

            migrationBuilder.RenameTable(
                name: "LeaveTypes",
                newName: "time_off_types");

            migrationBuilder.RenameTable(
                name: "LeaveRequests",
                newName: "time_off_requests");

            migrationBuilder.RenameTable(
                name: "LeavePolicyAssignments",
                newName: "time_off_policy_assignments");

            migrationBuilder.RenameTable(
                name: "LeavePolicies",
                newName: "time_off_policies");

            migrationBuilder.RenameTable(
                name: "LeaveBalances",
                newName: "time_off_entitlements");

            migrationBuilder.RenameTable(
                name: "ExternalCalendarEventLinks",
                newName: "external_calendar_event_links");

            migrationBuilder.RenameTable(
                name: "ExternalCalendarConnections",
                newName: "external_calendar_connections");

            migrationBuilder.RenameTable(
                name: "EmployeeHierarchyClosures",
                newName: "employee_hierarchy_closure");

            migrationBuilder.RenameTable(
                name: "EmployeeAssignmentHistories",
                newName: "employee_assignment_history");

            migrationBuilder.RenameTable(
                name: "DemoRequests",
                newName: "demo_access_requests");

            migrationBuilder.RenameTable(
                name: "DemoProfileUpgradeOptions",
                newName: "demo_profile_upgrade_options");

            migrationBuilder.RenameTable(
                name: "DemoProfiles",
                newName: "demo_profiles");

            migrationBuilder.RenameTable(
                name: "DemoProfileModuleAccess",
                newName: "demo_profile_modules");

            migrationBuilder.RenameTable(
                name: "ConfigurationTemplates",
                newName: "configuration_templates");

            migrationBuilder.RenameTable(
                name: "CalendarEvents",
                newName: "calendar_events");

            migrationBuilder.RenameIndex(
                name: "IX_Users_TenantId_Email",
                table: "users",
                newName: "IX_users_TenantId_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_Status",
                table: "tenants",
                newName: "IX_tenants_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_Slug",
                table: "tenants",
                newName: "IX_tenants_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_TenantId_Name",
                table: "roles",
                newName: "IX_roles_TenantId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_TenantId_LegalEntityId_Code",
                table: "positions",
                newName: "IX_positions_TenantId_LegalEntityId_Code");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_TenantId",
                table: "positions",
                newName: "IX_positions_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_Status",
                table: "positions",
                newName: "IX_positions_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_ReportsToPositionId",
                table: "positions",
                newName: "IX_positions_ReportsToPositionId");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_LegalEntityId",
                table: "positions",
                newName: "IX_positions_LegalEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_DepartmentId",
                table: "positions",
                newName: "IX_positions_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TenantId_UserId",
                table: "notifications",
                newName: "IX_notifications_TenantId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_WorkEmail",
                table: "employees",
                newName: "IX_employees_WorkEmail");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_TenantId_EmployeeNumber",
                table: "employees",
                newName: "IX_employees_TenantId_EmployeeNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_TenantId",
                table: "employees",
                newName: "IX_employees_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_Status",
                table: "employees",
                newName: "IX_employees_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_LegalEntityId",
                table: "employees",
                newName: "IX_employees_LegalEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_TenantId_LegalEntityId_Code",
                table: "departments",
                newName: "IX_departments_TenantId_LegalEntityId_Code");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_TenantId",
                table: "departments",
                newName: "IX_departments_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "departments",
                newName: "IX_departments_ParentDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_LegalEntityId",
                table: "departments",
                newName: "IX_departments_LegalEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_HeadPositionId",
                table: "departments",
                newName: "IX_departments_HeadPositionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_UserId",
                table: "sessions",
                newName: "IX_sessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_TenantId",
                table: "sessions",
                newName: "IX_sessions_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_SessionTokenHash",
                table: "sessions",
                newName: "IX_sessions_SessionTokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_UserId",
                table: "user_roles",
                newName: "IX_user_roles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "user_roles",
                newName: "IX_user_roles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantSubscriptions_TenantId",
                table: "tenant_subscriptions",
                newName: "IX_tenant_subscriptions_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantSubscriptions_SubscriptionPlanId",
                table: "tenant_subscriptions",
                newName: "IX_tenant_subscriptions_SubscriptionPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantSubscriptions_Status",
                table: "tenant_subscriptions",
                newName: "IX_tenant_subscriptions_Status");

            migrationBuilder.RenameIndex(
                name: "IX_TenantProvisioningStates_TenantId",
                table: "tenant_provisioning_states",
                newName: "IX_tenant_provisioning_states_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantModuleEntitlements_TenantId_ModuleCatalogId",
                table: "tenant_module_entitlements",
                newName: "IX_tenant_module_entitlements_TenantId_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantModuleEntitlements_ModuleCatalogId",
                table: "tenant_module_entitlements",
                newName: "IX_tenant_module_entitlements_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantConfigurationTemplateApplications_TenantId",
                table: "tenant_configuration_template_applications",
                newName: "IX_tenant_configuration_template_applications_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantConfigurationTemplateApplications_ConfigurationTempla~",
                table: "tenant_configuration_template_applications",
                newName: "IX_tenant_configuration_template_applications_ConfigurationTem~");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionPlans_Code",
                table: "subscription_plans",
                newName: "IX_subscription_plans_Code");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionInvoices_TenantSubscriptionId",
                table: "subscription_invoices",
                newName: "IX_subscription_invoices_TenantSubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionInvoices_TenantId",
                table: "subscription_invoices",
                newName: "IX_subscription_invoices_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionInvoices_Status",
                table: "subscription_invoices",
                newName: "IX_subscription_invoices_Status");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionInvoices_InvoiceNumber",
                table: "subscription_invoices",
                newName: "IX_subscription_invoices_InvoiceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_RuntimeFeatureFlags_TenantId_FeatureKey",
                table: "feature_flags",
                newName: "IX_feature_flags_TenantId_FeatureKey");

            migrationBuilder.RenameIndex(
                name: "IX_RoleTemplates_Name",
                table: "role_templates",
                newName: "IX_role_templates_Name");

            migrationBuilder.RenameColumn(
                name: "PermissionCatalogId",
                table: "role_permissions",
                newName: "PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_RoleId_PermissionCatalogId",
                table: "role_permissions",
                newName: "IX_role_permissions_RoleId_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_PermissionCatalogId",
                table: "role_permissions",
                newName: "IX_role_permissions_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PositionReportingHistories_TenantId_PositionId",
                table: "position_reporting_history",
                newName: "IX_position_reporting_history_TenantId_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_PositionReportingHistories_PositionId",
                table: "position_reporting_history",
                newName: "IX_position_reporting_history_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_PositionAssignments_TenantId_PositionId",
                table: "position_assignments",
                newName: "IX_position_assignments_TenantId_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_PositionAssignments_TenantId_EmployeeId",
                table: "position_assignments",
                newName: "IX_position_assignments_TenantId_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_PositionAssignments_PositionId",
                table: "position_assignments",
                newName: "IX_position_assignments_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_PositionAssignments_EmployeeId",
                table: "position_assignments",
                newName: "IX_position_assignments_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformUserSessions_SessionTokenHash",
                table: "platform_user_sessions",
                newName: "IX_platform_user_sessions_SessionTokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformUserSessions_PlatformUserId",
                table: "platform_user_sessions",
                newName: "IX_platform_user_sessions_PlatformUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformUsers_Email",
                table: "platform_users",
                newName: "IX_platform_users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformUserRoles_PlatformUserId_PlatformRoleId",
                table: "platform_user_roles",
                newName: "IX_platform_user_roles_PlatformUserId_PlatformRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformUserRoles_PlatformRoleId",
                table: "platform_user_roles",
                newName: "IX_platform_user_roles_PlatformRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformRolePermissions_PlatformRoleId_PlatformPermissionId",
                table: "platform_role_permissions",
                newName: "IX_platform_role_permissions_PlatformRoleId_PlatformPermission~");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformRolePermissions_PlatformPermissionId",
                table: "platform_role_permissions",
                newName: "IX_platform_role_permissions_PlatformPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformPermissions_PermissionKey",
                table: "platform_permissions",
                newName: "IX_platform_permissions_PermissionKey");

            migrationBuilder.RenameIndex(
                name: "IX_PlatformAuthEvents_Email",
                table: "platform_auth_events",
                newName: "IX_platform_auth_events_Email");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentGatewayCredentials_PaymentGatewayConfigId",
                table: "payment_gateway_credentials",
                newName: "IX_payment_gateway_credentials_PaymentGatewayConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentGatewayCountryRoutes_GatewayConfigId",
                table: "payment_gateway_country_routes",
                newName: "IX_payment_gateway_country_routes_GatewayConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentGatewayCountryRoutes_CountryCode_Environment",
                table: "payment_gateway_country_routes",
                newName: "IX_payment_gateway_country_routes_CountryCode_Environment");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentGatewayConfigs_GatewayKey",
                table: "payment_gateway_configs",
                newName: "IX_payment_gateway_configs_GatewayKey");

            migrationBuilder.RenameColumn(
                name: "PermissionCatalogId",
                table: "module_permission_ownership",
                newName: "PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_ModulePermissionOwnerships_PermissionCatalogId",
                table: "module_permission_ownership",
                newName: "IX_module_permission_ownership_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_ModulePermissionOwnerships_ModuleCatalogId_PermissionCatalo~",
                table: "module_permission_ownership",
                newName: "IX_module_permission_ownership_ModuleCatalogId_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_ModuleFeatures_ModuleCatalogId",
                table: "module_features",
                newName: "IX_module_features_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_ModuleFeatures_FeatureKey",
                table: "module_features",
                newName: "IX_module_features_FeatureKey");

            migrationBuilder.RenameIndex(
                name: "IX_ModuleCatalogs_ModuleKey",
                table: "module_catalog",
                newName: "IX_module_catalog_ModuleKey");

            migrationBuilder.RenameIndex(
                name: "IX_LegalEntities_TenantId_Code",
                table: "legal_entities",
                newName: "IX_legal_entities_TenantId_Code");

            migrationBuilder.RenameIndex(
                name: "IX_LegalEntities_TenantId",
                table: "legal_entities",
                newName: "IX_legal_entities_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveTypes_TenantId_Code",
                table: "time_off_types",
                newName: "IX_time_off_types_TenantId_Code");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequests_TenantId_EmployeeId",
                table: "time_off_requests",
                newName: "IX_time_off_requests_TenantId_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequests_Status",
                table: "time_off_requests",
                newName: "IX_time_off_requests_Status");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequests_LeaveTypeId",
                table: "time_off_requests",
                newName: "IX_time_off_requests_LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeavePolicyAssignments_TenantId",
                table: "time_off_policy_assignments",
                newName: "IX_time_off_policy_assignments_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_LeavePolicyAssignments_LeavePolicyId",
                table: "time_off_policy_assignments",
                newName: "IX_time_off_policy_assignments_LeavePolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_LeavePolicies_TenantId",
                table: "time_off_policies",
                newName: "IX_time_off_policies_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_LeavePolicies_LeaveTypeId",
                table: "time_off_policies",
                newName: "IX_time_off_policies_LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveBalances_TenantId_EmployeeId_LeaveTypeId_Year",
                table: "time_off_entitlements",
                newName: "IX_time_off_entitlements_TenantId_EmployeeId_LeaveTypeId_Year");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveBalances_LeaveTypeId",
                table: "time_off_entitlements",
                newName: "IX_time_off_entitlements_LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalCalendarEventLinks_TenantId_Provider_ExternalCalend~",
                table: "external_calendar_event_links",
                newName: "IX_external_calendar_event_links_TenantId_Provider_ExternalCal~");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalCalendarEventLinks_ExternalCalendarConnectionId",
                table: "external_calendar_event_links",
                newName: "IX_external_calendar_event_links_ExternalCalendarConnectionId");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalCalendarEventLinks_CalendarEventId",
                table: "external_calendar_event_links",
                newName: "IX_external_calendar_event_links_CalendarEventId");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalCalendarConnections_UserId",
                table: "external_calendar_connections",
                newName: "IX_external_calendar_connections_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalCalendarConnections_TenantId_UserId_Provider_Extern~",
                table: "external_calendar_connections",
                newName: "IX_external_calendar_connections_TenantId_UserId_Provider_Exte~");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeHierarchyClosures_TenantId_ManagerEmployeeId_Report~",
                table: "employee_hierarchy_closure",
                newName: "IX_employee_hierarchy_closure_TenantId_ManagerEmployeeId_Repor~");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeAssignmentHistories_TenantId_EmployeeId",
                table: "employee_assignment_history",
                newName: "IX_employee_assignment_history_TenantId_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeAssignmentHistories_EmployeeId",
                table: "employee_assignment_history",
                newName: "IX_employee_assignment_history_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "ReviewedByPlatformUserId",
                table: "demo_access_requests",
                newName: "ReviewedById");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "demo_access_requests",
                newName: "RequesterName");

            migrationBuilder.RenameColumn(
                name: "ConvertedTenantId",
                table: "demo_access_requests",
                newName: "CreatedTenantId");

            migrationBuilder.RenameColumn(
                name: "ContactName",
                table: "demo_access_requests",
                newName: "RequesterEmail");

            migrationBuilder.RenameColumn(
                name: "BusinessEmail",
                table: "demo_access_requests",
                newName: "CountryCode");

            migrationBuilder.RenameIndex(
                name: "IX_DemoRequests_Status",
                table: "demo_access_requests",
                newName: "IX_demo_access_requests_Status");

            migrationBuilder.RenameIndex(
                name: "IX_DemoProfiles_Name",
                table: "demo_profiles",
                newName: "IX_demo_profiles_Name");

            migrationBuilder.RenameIndex(
                name: "IX_DemoProfileModuleAccess_ModuleCatalogId",
                table: "demo_profile_modules",
                newName: "IX_demo_profile_modules_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_DemoProfileModuleAccess_DemoProfileId_ModuleCatalogId",
                table: "demo_profile_modules",
                newName: "IX_demo_profile_modules_DemoProfileId_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_ConfigurationTemplates_TemplateKey",
                table: "configuration_templates",
                newName: "IX_configuration_templates_TemplateKey");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvents_TenantId",
                table: "calendar_events",
                newName: "IX_calendar_events_TenantId");

            migrationBuilder.AlterColumn<decimal>(
                name: "AnnualPrice",
                table: "subscription_plan_price_brackets",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePlanMonthlyPrice",
                table: "subscription_plan_price_brackets",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CompanySizeRange",
                table: "subscription_plan_price_brackets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "subscription_plan_price_brackets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "subscription_plan_price_brackets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionalAddonPrices",
                table: "subscription_plan_price_brackets",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResourceAddonPrices",
                table: "subscription_plan_price_brackets",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PermissionCodesJson",
                table: "role_templates",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "demo_access_requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyWebsite",
                table: "demo_access_requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "demo_access_requests",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedAccessNotes",
                table: "demo_access_requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedModuleKeys",
                table: "demo_access_requests",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestedSubdomain",
                table: "demo_access_requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterPhone",
                table: "demo_access_requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantVisibleNote",
                table: "demo_access_requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddonDemoLimits",
                table: "demo_profile_upgrade_options",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddonVisibility",
                table: "demo_profile_upgrade_options",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AllowedAddonModuleKeys",
                table: "demo_profile_upgrade_options",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AllowedPlanIds",
                table: "demo_profile_upgrade_options",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HiddenAddonModuleKeys",
                table: "demo_profile_upgrade_options",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByPlatformUserId",
                table: "demo_profiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "demo_profiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturePermissions",
                table: "demo_profile_modules",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenants",
                table: "tenants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_positions",
                table: "positions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notifications",
                table: "notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employees",
                table: "employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_departments",
                table: "departments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sessions",
                table: "sessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_subscriptions",
                table: "tenant_subscriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_provisioning_states",
                table: "tenant_provisioning_states",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_module_entitlements",
                table: "tenant_module_entitlements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_configuration_template_applications",
                table: "tenant_configuration_template_applications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subscription_plans",
                table: "subscription_plans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subscription_plan_price_brackets",
                table: "subscription_plan_price_brackets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subscription_invoices",
                table: "subscription_invoices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_feature_flags",
                table: "feature_flags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role_templates",
                table: "role_templates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role_permissions",
                table: "role_permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_position_reporting_history",
                table: "position_reporting_history",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_position_assignments",
                table: "position_assignments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platform_user_sessions",
                table: "platform_user_sessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platform_users",
                table: "platform_users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platform_user_roles",
                table: "platform_user_roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platform_roles",
                table: "platform_roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platform_role_permissions",
                table: "platform_role_permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platform_permissions",
                table: "platform_permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platform_auth_events",
                table: "platform_auth_events",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payment_gateway_credentials",
                table: "payment_gateway_credentials",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payment_gateway_country_routes",
                table: "payment_gateway_country_routes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payment_gateway_configs",
                table: "payment_gateway_configs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_module_permission_ownership",
                table: "module_permission_ownership",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_module_features",
                table: "module_features",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_module_catalog",
                table: "module_catalog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_legal_entities",
                table: "legal_entities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_time_off_types",
                table: "time_off_types",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_time_off_requests",
                table: "time_off_requests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_time_off_policy_assignments",
                table: "time_off_policy_assignments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_time_off_policies",
                table: "time_off_policies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_time_off_entitlements",
                table: "time_off_entitlements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_external_calendar_event_links",
                table: "external_calendar_event_links",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_external_calendar_connections",
                table: "external_calendar_connections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employee_hierarchy_closure",
                table: "employee_hierarchy_closure",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employee_assignment_history",
                table: "employee_assignment_history",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_demo_access_requests",
                table: "demo_access_requests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_demo_profile_upgrade_options",
                table: "demo_profile_upgrade_options",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_demo_profiles",
                table: "demo_profiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_demo_profile_modules",
                table: "demo_profile_modules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_configuration_templates",
                table: "configuration_templates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_calendar_events",
                table: "calendar_events",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PhoneCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "holiday_calendar_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    OverrideCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    EffectiveCountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    HolidaySyncEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Provider = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LastSyncedYear = table.Column<int>(type: "integer", nullable: true),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_calendar_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plan_modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleKey = table.Column<string>(type: "text", nullable: false),
                    PackageType = table.Column<string>(type: "text", nullable: false),
                    StorageContributionGb = table.Column<int>(type: "integer", nullable: true),
                    AiTokenContribution = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plan_modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_plan_modules_subscription_plans_SubscriptionPl~",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plan_resource_addons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    StorageContributionGb = table.Column<int>(type: "integer", nullable: true),
                    AiTokenContribution = table.Column<long>(type: "bigint", nullable: true),
                    PriceByEmployeeTier = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plan_resource_addons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_plan_resource_addons_subscription_plans_Subscr~",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentTaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    ObjectiveId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TaskType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StoryPoints = table.Column<int>(type: "integer", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "versions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_versions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_price_brackets_SubscriptionPlanId_Company~",
                table: "subscription_plan_price_brackets",
                columns: new[] { "SubscriptionPlanId", "CompanySizeRange" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_demo_access_requests_RequesterEmail",
                table: "demo_access_requests",
                column: "RequesterEmail");

            migrationBuilder.CreateIndex(
                name: "IX_demo_profile_upgrade_options_DemoProfileId",
                table: "demo_profile_upgrade_options",
                column: "DemoProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_holiday_calendar_settings_TenantId",
                table: "holiday_calendar_settings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_modules_SubscriptionPlanId_ModuleKey",
                table: "subscription_plan_modules",
                columns: new[] { "SubscriptionPlanId", "ModuleKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_resource_addons_SubscriptionPlanId",
                table: "subscription_plan_resource_addons",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_TenantId",
                table: "tasks",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_demo_profile_modules_demo_profiles_DemoProfileId",
                table: "demo_profile_modules",
                column: "DemoProfileId",
                principalTable: "demo_profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_demo_profile_modules_module_catalog_ModuleCatalogId",
                table: "demo_profile_modules",
                column: "ModuleCatalogId",
                principalTable: "module_catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_demo_profile_upgrade_options_demo_profiles_DemoProfileId",
                table: "demo_profile_upgrade_options",
                column: "DemoProfileId",
                principalTable: "demo_profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_departments_ParentDepartmentId",
                table: "departments",
                column: "ParentDepartmentId",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_legal_entities_LegalEntityId",
                table: "departments",
                column: "LegalEntityId",
                principalTable: "legal_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_positions_HeadPositionId",
                table: "departments",
                column: "HeadPositionId",
                principalTable: "positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_assignment_history_employees_EmployeeId",
                table: "employee_assignment_history",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_legal_entities_LegalEntityId",
                table: "employees",
                column: "LegalEntityId",
                principalTable: "legal_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_tenants_TenantId",
                table: "employees",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_external_calendar_connections_tenants_TenantId",
                table: "external_calendar_connections",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_external_calendar_connections_users_UserId",
                table: "external_calendar_connections",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_external_calendar_event_links_calendar_events_CalendarEvent~",
                table: "external_calendar_event_links",
                column: "CalendarEventId",
                principalTable: "calendar_events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_external_calendar_event_links_external_calendar_connections~",
                table: "external_calendar_event_links",
                column: "ExternalCalendarConnectionId",
                principalTable: "external_calendar_connections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_external_calendar_event_links_tenants_TenantId",
                table: "external_calendar_event_links",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_legal_entities_tenants_TenantId",
                table: "legal_entities",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_module_features_module_catalog_ModuleCatalogId",
                table: "module_features",
                column: "ModuleCatalogId",
                principalTable: "module_catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_module_permission_ownership_module_catalog_ModuleCatalogId",
                table: "module_permission_ownership",
                column: "ModuleCatalogId",
                principalTable: "module_catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_module_permission_ownership_permissions_PermissionId",
                table: "module_permission_ownership",
                column: "PermissionId",
                principalTable: "permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_gateway_country_routes_payment_gateway_configs_Gate~",
                table: "payment_gateway_country_routes",
                column: "GatewayConfigId",
                principalTable: "payment_gateway_configs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_gateway_credentials_payment_gateway_configs_Payment~",
                table: "payment_gateway_credentials",
                column: "PaymentGatewayConfigId",
                principalTable: "payment_gateway_configs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_platform_role_permissions_platform_permissions_PlatformPerm~",
                table: "platform_role_permissions",
                column: "PlatformPermissionId",
                principalTable: "platform_permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_platform_role_permissions_platform_roles_PlatformRoleId",
                table: "platform_role_permissions",
                column: "PlatformRoleId",
                principalTable: "platform_roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_platform_user_roles_platform_roles_PlatformRoleId",
                table: "platform_user_roles",
                column: "PlatformRoleId",
                principalTable: "platform_roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_platform_user_roles_platform_users_PlatformUserId",
                table: "platform_user_roles",
                column: "PlatformUserId",
                principalTable: "platform_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_platform_user_sessions_platform_users_PlatformUserId",
                table: "platform_user_sessions",
                column: "PlatformUserId",
                principalTable: "platform_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_position_assignments_employees_EmployeeId",
                table: "position_assignments",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_position_assignments_positions_PositionId",
                table: "position_assignments",
                column: "PositionId",
                principalTable: "positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_position_reporting_history_positions_PositionId",
                table: "position_reporting_history",
                column: "PositionId",
                principalTable: "positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_positions_departments_DepartmentId",
                table: "positions",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_positions_legal_entities_LegalEntityId",
                table: "positions",
                column: "LegalEntityId",
                principalTable: "legal_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_positions_positions_ReportsToPositionId",
                table: "positions",
                column: "ReportsToPositionId",
                principalTable: "positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_permissions_PermissionId",
                table: "role_permissions",
                column: "PermissionId",
                principalTable: "permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_roles_RoleId",
                table: "role_permissions",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subscription_invoices_tenant_subscriptions_TenantSubscripti~",
                table: "subscription_invoices",
                column: "TenantSubscriptionId",
                principalTable: "tenant_subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subscription_plan_price_brackets_subscription_plans_Subscri~",
                table: "subscription_plan_price_brackets",
                column: "SubscriptionPlanId",
                principalTable: "subscription_plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_configuration_template_applications_configuration_te~",
                table: "tenant_configuration_template_applications",
                column: "ConfigurationTemplateId",
                principalTable: "configuration_templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_module_entitlements_module_catalog_ModuleCatalogId",
                table: "tenant_module_entitlements",
                column: "ModuleCatalogId",
                principalTable: "module_catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_provisioning_states_tenants_TenantId",
                table: "tenant_provisioning_states",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_subscriptions_subscription_plans_SubscriptionPlanId",
                table: "tenant_subscriptions",
                column: "SubscriptionPlanId",
                principalTable: "subscription_plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_subscriptions_tenants_TenantId",
                table: "tenant_subscriptions",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_time_off_entitlements_time_off_types_LeaveTypeId",
                table: "time_off_entitlements",
                column: "LeaveTypeId",
                principalTable: "time_off_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_time_off_policies_time_off_types_LeaveTypeId",
                table: "time_off_policies",
                column: "LeaveTypeId",
                principalTable: "time_off_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_time_off_policy_assignments_time_off_policies_LeavePolicyId",
                table: "time_off_policy_assignments",
                column: "LeavePolicyId",
                principalTable: "time_off_policies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_time_off_requests_time_off_types_LeaveTypeId",
                table: "time_off_requests",
                column: "LeaveTypeId",
                principalTable: "time_off_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_tenants_TenantId",
                table: "users",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_demo_profile_modules_demo_profiles_DemoProfileId",
                table: "demo_profile_modules");

            migrationBuilder.DropForeignKey(
                name: "FK_demo_profile_modules_module_catalog_ModuleCatalogId",
                table: "demo_profile_modules");

            migrationBuilder.DropForeignKey(
                name: "FK_demo_profile_upgrade_options_demo_profiles_DemoProfileId",
                table: "demo_profile_upgrade_options");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_departments_ParentDepartmentId",
                table: "departments");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_legal_entities_LegalEntityId",
                table: "departments");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_positions_HeadPositionId",
                table: "departments");

            migrationBuilder.DropForeignKey(
                name: "FK_employee_assignment_history_employees_EmployeeId",
                table: "employee_assignment_history");

            migrationBuilder.DropForeignKey(
                name: "FK_employees_legal_entities_LegalEntityId",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_employees_tenants_TenantId",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_external_calendar_connections_tenants_TenantId",
                table: "external_calendar_connections");

            migrationBuilder.DropForeignKey(
                name: "FK_external_calendar_connections_users_UserId",
                table: "external_calendar_connections");

            migrationBuilder.DropForeignKey(
                name: "FK_external_calendar_event_links_calendar_events_CalendarEvent~",
                table: "external_calendar_event_links");

            migrationBuilder.DropForeignKey(
                name: "FK_external_calendar_event_links_external_calendar_connections~",
                table: "external_calendar_event_links");

            migrationBuilder.DropForeignKey(
                name: "FK_external_calendar_event_links_tenants_TenantId",
                table: "external_calendar_event_links");

            migrationBuilder.DropForeignKey(
                name: "FK_legal_entities_tenants_TenantId",
                table: "legal_entities");

            migrationBuilder.DropForeignKey(
                name: "FK_module_features_module_catalog_ModuleCatalogId",
                table: "module_features");

            migrationBuilder.DropForeignKey(
                name: "FK_module_permission_ownership_module_catalog_ModuleCatalogId",
                table: "module_permission_ownership");

            migrationBuilder.DropForeignKey(
                name: "FK_module_permission_ownership_permissions_PermissionId",
                table: "module_permission_ownership");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_gateway_country_routes_payment_gateway_configs_Gate~",
                table: "payment_gateway_country_routes");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_gateway_credentials_payment_gateway_configs_Payment~",
                table: "payment_gateway_credentials");

            migrationBuilder.DropForeignKey(
                name: "FK_platform_role_permissions_platform_permissions_PlatformPerm~",
                table: "platform_role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_platform_role_permissions_platform_roles_PlatformRoleId",
                table: "platform_role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_platform_user_roles_platform_roles_PlatformRoleId",
                table: "platform_user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_platform_user_roles_platform_users_PlatformUserId",
                table: "platform_user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_platform_user_sessions_platform_users_PlatformUserId",
                table: "platform_user_sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_position_assignments_employees_EmployeeId",
                table: "position_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_position_assignments_positions_PositionId",
                table: "position_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_position_reporting_history_positions_PositionId",
                table: "position_reporting_history");

            migrationBuilder.DropForeignKey(
                name: "FK_positions_departments_DepartmentId",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "FK_positions_legal_entities_LegalEntityId",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "FK_positions_positions_ReportsToPositionId",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_permissions_PermissionId",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_roles_RoleId",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_subscription_invoices_tenant_subscriptions_TenantSubscripti~",
                table: "subscription_invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_subscription_plan_price_brackets_subscription_plans_Subscri~",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_configuration_template_applications_configuration_te~",
                table: "tenant_configuration_template_applications");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_module_entitlements_module_catalog_ModuleCatalogId",
                table: "tenant_module_entitlements");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_provisioning_states_tenants_TenantId",
                table: "tenant_provisioning_states");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_subscriptions_subscription_plans_SubscriptionPlanId",
                table: "tenant_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_subscriptions_tenants_TenantId",
                table: "tenant_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_time_off_entitlements_time_off_types_LeaveTypeId",
                table: "time_off_entitlements");

            migrationBuilder.DropForeignKey(
                name: "FK_time_off_policies_time_off_types_LeaveTypeId",
                table: "time_off_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_time_off_policy_assignments_time_off_policies_LeavePolicyId",
                table: "time_off_policy_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_time_off_requests_time_off_types_LeaveTypeId",
                table: "time_off_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_tenants_TenantId",
                table: "users");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "holiday_calendar_settings");

            migrationBuilder.DropTable(
                name: "subscription_plan_modules");

            migrationBuilder.DropTable(
                name: "subscription_plan_resource_addons");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "versions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenants",
                table: "tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_positions",
                table: "positions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notifications",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employees",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_departments",
                table: "departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_time_off_types",
                table: "time_off_types");

            migrationBuilder.DropPrimaryKey(
                name: "PK_time_off_requests",
                table: "time_off_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_time_off_policy_assignments",
                table: "time_off_policy_assignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_time_off_policies",
                table: "time_off_policies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_time_off_entitlements",
                table: "time_off_entitlements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_subscriptions",
                table: "tenant_subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_provisioning_states",
                table: "tenant_provisioning_states");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_module_entitlements",
                table: "tenant_module_entitlements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_configuration_template_applications",
                table: "tenant_configuration_template_applications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subscription_plans",
                table: "subscription_plans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subscription_plan_price_brackets",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropIndex(
                name: "IX_subscription_plan_price_brackets_SubscriptionPlanId_Company~",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subscription_invoices",
                table: "subscription_invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sessions",
                table: "sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_role_templates",
                table: "role_templates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_role_permissions",
                table: "role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_position_reporting_history",
                table: "position_reporting_history");

            migrationBuilder.DropPrimaryKey(
                name: "PK_position_assignments",
                table: "position_assignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_platform_users",
                table: "platform_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_platform_user_sessions",
                table: "platform_user_sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_platform_user_roles",
                table: "platform_user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_platform_roles",
                table: "platform_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_platform_role_permissions",
                table: "platform_role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_platform_permissions",
                table: "platform_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_platform_auth_events",
                table: "platform_auth_events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payment_gateway_credentials",
                table: "payment_gateway_credentials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payment_gateway_country_routes",
                table: "payment_gateway_country_routes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payment_gateway_configs",
                table: "payment_gateway_configs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_module_permission_ownership",
                table: "module_permission_ownership");

            migrationBuilder.DropPrimaryKey(
                name: "PK_module_features",
                table: "module_features");

            migrationBuilder.DropPrimaryKey(
                name: "PK_module_catalog",
                table: "module_catalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_legal_entities",
                table: "legal_entities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_feature_flags",
                table: "feature_flags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_external_calendar_event_links",
                table: "external_calendar_event_links");

            migrationBuilder.DropPrimaryKey(
                name: "PK_external_calendar_connections",
                table: "external_calendar_connections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employee_hierarchy_closure",
                table: "employee_hierarchy_closure");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employee_assignment_history",
                table: "employee_assignment_history");

            migrationBuilder.DropPrimaryKey(
                name: "PK_demo_profiles",
                table: "demo_profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_demo_profile_upgrade_options",
                table: "demo_profile_upgrade_options");

            migrationBuilder.DropIndex(
                name: "IX_demo_profile_upgrade_options_DemoProfileId",
                table: "demo_profile_upgrade_options");

            migrationBuilder.DropPrimaryKey(
                name: "PK_demo_profile_modules",
                table: "demo_profile_modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_demo_access_requests",
                table: "demo_access_requests");

            migrationBuilder.DropIndex(
                name: "IX_demo_access_requests_RequesterEmail",
                table: "demo_access_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_configuration_templates",
                table: "configuration_templates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_calendar_events",
                table: "calendar_events");

            migrationBuilder.DropColumn(
                name: "BasePlanMonthlyPrice",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropColumn(
                name: "CompanySizeRange",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropColumn(
                name: "OptionalAddonPrices",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropColumn(
                name: "ResourceAddonPrices",
                table: "subscription_plan_price_brackets");

            migrationBuilder.DropColumn(
                name: "PermissionCodesJson",
                table: "role_templates");

            migrationBuilder.DropColumn(
                name: "CreatedByPlatformUserId",
                table: "demo_profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "demo_profiles");

            migrationBuilder.DropColumn(
                name: "AddonDemoLimits",
                table: "demo_profile_upgrade_options");

            migrationBuilder.DropColumn(
                name: "AddonVisibility",
                table: "demo_profile_upgrade_options");

            migrationBuilder.DropColumn(
                name: "AllowedAddonModuleKeys",
                table: "demo_profile_upgrade_options");

            migrationBuilder.DropColumn(
                name: "AllowedPlanIds",
                table: "demo_profile_upgrade_options");

            migrationBuilder.DropColumn(
                name: "HiddenAddonModuleKeys",
                table: "demo_profile_upgrade_options");

            migrationBuilder.DropColumn(
                name: "FeaturePermissions",
                table: "demo_profile_modules");

            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "demo_access_requests");

            migrationBuilder.DropColumn(
                name: "CompanyWebsite",
                table: "demo_access_requests");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "demo_access_requests");

            migrationBuilder.DropColumn(
                name: "RequestedAccessNotes",
                table: "demo_access_requests");

            migrationBuilder.DropColumn(
                name: "RequestedModuleKeys",
                table: "demo_access_requests");

            migrationBuilder.DropColumn(
                name: "RequestedSubdomain",
                table: "demo_access_requests");

            migrationBuilder.DropColumn(
                name: "RequesterPhone",
                table: "demo_access_requests");

            migrationBuilder.DropColumn(
                name: "TenantVisibleNote",
                table: "demo_access_requests");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "tenants",
                newName: "Tenants");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "positions",
                newName: "Positions");

            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "departments",
                newName: "Departments");

            migrationBuilder.RenameTable(
                name: "user_roles",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "time_off_types",
                newName: "LeaveTypes");

            migrationBuilder.RenameTable(
                name: "time_off_requests",
                newName: "LeaveRequests");

            migrationBuilder.RenameTable(
                name: "time_off_policy_assignments",
                newName: "LeavePolicyAssignments");

            migrationBuilder.RenameTable(
                name: "time_off_policies",
                newName: "LeavePolicies");

            migrationBuilder.RenameTable(
                name: "time_off_entitlements",
                newName: "LeaveBalances");

            migrationBuilder.RenameTable(
                name: "tenant_subscriptions",
                newName: "TenantSubscriptions");

            migrationBuilder.RenameTable(
                name: "tenant_provisioning_states",
                newName: "TenantProvisioningStates");

            migrationBuilder.RenameTable(
                name: "tenant_module_entitlements",
                newName: "TenantModuleEntitlements");

            migrationBuilder.RenameTable(
                name: "tenant_configuration_template_applications",
                newName: "TenantConfigurationTemplateApplications");

            migrationBuilder.RenameTable(
                name: "subscription_plans",
                newName: "SubscriptionPlans");

            migrationBuilder.RenameTable(
                name: "subscription_plan_price_brackets",
                newName: "SubscriptionPlanPriceBrackets");

            migrationBuilder.RenameTable(
                name: "subscription_invoices",
                newName: "SubscriptionInvoices");

            migrationBuilder.RenameTable(
                name: "sessions",
                newName: "UserSessions");

            migrationBuilder.RenameTable(
                name: "role_templates",
                newName: "RoleTemplates");

            migrationBuilder.RenameTable(
                name: "role_permissions",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "position_reporting_history",
                newName: "PositionReportingHistories");

            migrationBuilder.RenameTable(
                name: "position_assignments",
                newName: "PositionAssignments");

            migrationBuilder.RenameTable(
                name: "platform_users",
                newName: "PlatformUsers");

            migrationBuilder.RenameTable(
                name: "platform_user_sessions",
                newName: "PlatformUserSessions");

            migrationBuilder.RenameTable(
                name: "platform_user_roles",
                newName: "PlatformUserRoles");

            migrationBuilder.RenameTable(
                name: "platform_roles",
                newName: "PlatformRoles");

            migrationBuilder.RenameTable(
                name: "platform_role_permissions",
                newName: "PlatformRolePermissions");

            migrationBuilder.RenameTable(
                name: "platform_permissions",
                newName: "PlatformPermissions");

            migrationBuilder.RenameTable(
                name: "platform_auth_events",
                newName: "PlatformAuthEvents");

            migrationBuilder.RenameTable(
                name: "payment_gateway_credentials",
                newName: "PaymentGatewayCredentials");

            migrationBuilder.RenameTable(
                name: "payment_gateway_country_routes",
                newName: "PaymentGatewayCountryRoutes");

            migrationBuilder.RenameTable(
                name: "payment_gateway_configs",
                newName: "PaymentGatewayConfigs");

            migrationBuilder.RenameTable(
                name: "module_permission_ownership",
                newName: "ModulePermissionOwnerships");

            migrationBuilder.RenameTable(
                name: "module_features",
                newName: "ModuleFeatures");

            migrationBuilder.RenameTable(
                name: "module_catalog",
                newName: "ModuleCatalogs");

            migrationBuilder.RenameTable(
                name: "legal_entities",
                newName: "LegalEntities");

            migrationBuilder.RenameTable(
                name: "feature_flags",
                newName: "RuntimeFeatureFlags");

            migrationBuilder.RenameTable(
                name: "external_calendar_event_links",
                newName: "ExternalCalendarEventLinks");

            migrationBuilder.RenameTable(
                name: "external_calendar_connections",
                newName: "ExternalCalendarConnections");

            migrationBuilder.RenameTable(
                name: "employee_hierarchy_closure",
                newName: "EmployeeHierarchyClosures");

            migrationBuilder.RenameTable(
                name: "employee_assignment_history",
                newName: "EmployeeAssignmentHistories");

            migrationBuilder.RenameTable(
                name: "demo_profiles",
                newName: "DemoProfiles");

            migrationBuilder.RenameTable(
                name: "demo_profile_upgrade_options",
                newName: "DemoProfileUpgradeOptions");

            migrationBuilder.RenameTable(
                name: "demo_profile_modules",
                newName: "DemoProfileModuleAccess");

            migrationBuilder.RenameTable(
                name: "demo_access_requests",
                newName: "DemoRequests");

            migrationBuilder.RenameTable(
                name: "configuration_templates",
                newName: "ConfigurationTemplates");

            migrationBuilder.RenameTable(
                name: "calendar_events",
                newName: "CalendarEvents");

            migrationBuilder.RenameIndex(
                name: "IX_users_TenantId_Email",
                table: "Users",
                newName: "IX_Users_TenantId_Email");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_Status",
                table: "Tenants",
                newName: "IX_Tenants_Status");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_Slug",
                table: "Tenants",
                newName: "IX_Tenants_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_roles_TenantId_Name",
                table: "Roles",
                newName: "IX_Roles_TenantId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_positions_TenantId_LegalEntityId_Code",
                table: "Positions",
                newName: "IX_Positions_TenantId_LegalEntityId_Code");

            migrationBuilder.RenameIndex(
                name: "IX_positions_TenantId",
                table: "Positions",
                newName: "IX_Positions_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_positions_Status",
                table: "Positions",
                newName: "IX_Positions_Status");

            migrationBuilder.RenameIndex(
                name: "IX_positions_ReportsToPositionId",
                table: "Positions",
                newName: "IX_Positions_ReportsToPositionId");

            migrationBuilder.RenameIndex(
                name: "IX_positions_LegalEntityId",
                table: "Positions",
                newName: "IX_Positions_LegalEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_positions_DepartmentId",
                table: "Positions",
                newName: "IX_Positions_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_TenantId_UserId",
                table: "Notifications",
                newName: "IX_Notifications_TenantId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_WorkEmail",
                table: "Employees",
                newName: "IX_Employees_WorkEmail");

            migrationBuilder.RenameIndex(
                name: "IX_employees_TenantId_EmployeeNumber",
                table: "Employees",
                newName: "IX_Employees_TenantId_EmployeeNumber");

            migrationBuilder.RenameIndex(
                name: "IX_employees_TenantId",
                table: "Employees",
                newName: "IX_Employees_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_Status",
                table: "Employees",
                newName: "IX_Employees_Status");

            migrationBuilder.RenameIndex(
                name: "IX_employees_LegalEntityId",
                table: "Employees",
                newName: "IX_Employees_LegalEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_departments_TenantId_LegalEntityId_Code",
                table: "Departments",
                newName: "IX_Departments_TenantId_LegalEntityId_Code");

            migrationBuilder.RenameIndex(
                name: "IX_departments_TenantId",
                table: "Departments",
                newName: "IX_Departments_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_departments_ParentDepartmentId",
                table: "Departments",
                newName: "IX_Departments_ParentDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_departments_LegalEntityId",
                table: "Departments",
                newName: "IX_Departments_LegalEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_departments_HeadPositionId",
                table: "Departments",
                newName: "IX_Departments_HeadPositionId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_UserId",
                table: "UserRoles",
                newName: "IX_UserRoles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_types_TenantId_Code",
                table: "LeaveTypes",
                newName: "IX_LeaveTypes_TenantId_Code");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_requests_TenantId_EmployeeId",
                table: "LeaveRequests",
                newName: "IX_LeaveRequests_TenantId_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_requests_Status",
                table: "LeaveRequests",
                newName: "IX_LeaveRequests_Status");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_requests_LeaveTypeId",
                table: "LeaveRequests",
                newName: "IX_LeaveRequests_LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_policy_assignments_TenantId",
                table: "LeavePolicyAssignments",
                newName: "IX_LeavePolicyAssignments_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_policy_assignments_LeavePolicyId",
                table: "LeavePolicyAssignments",
                newName: "IX_LeavePolicyAssignments_LeavePolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_policies_TenantId",
                table: "LeavePolicies",
                newName: "IX_LeavePolicies_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_policies_LeaveTypeId",
                table: "LeavePolicies",
                newName: "IX_LeavePolicies_LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_entitlements_TenantId_EmployeeId_LeaveTypeId_Year",
                table: "LeaveBalances",
                newName: "IX_LeaveBalances_TenantId_EmployeeId_LeaveTypeId_Year");

            migrationBuilder.RenameIndex(
                name: "IX_time_off_entitlements_LeaveTypeId",
                table: "LeaveBalances",
                newName: "IX_LeaveBalances_LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_subscriptions_TenantId",
                table: "TenantSubscriptions",
                newName: "IX_TenantSubscriptions_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_subscriptions_SubscriptionPlanId",
                table: "TenantSubscriptions",
                newName: "IX_TenantSubscriptions_SubscriptionPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_subscriptions_Status",
                table: "TenantSubscriptions",
                newName: "IX_TenantSubscriptions_Status");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_provisioning_states_TenantId",
                table: "TenantProvisioningStates",
                newName: "IX_TenantProvisioningStates_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_module_entitlements_TenantId_ModuleCatalogId",
                table: "TenantModuleEntitlements",
                newName: "IX_TenantModuleEntitlements_TenantId_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_module_entitlements_ModuleCatalogId",
                table: "TenantModuleEntitlements",
                newName: "IX_TenantModuleEntitlements_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_configuration_template_applications_TenantId",
                table: "TenantConfigurationTemplateApplications",
                newName: "IX_TenantConfigurationTemplateApplications_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_configuration_template_applications_ConfigurationTem~",
                table: "TenantConfigurationTemplateApplications",
                newName: "IX_TenantConfigurationTemplateApplications_ConfigurationTempla~");

            migrationBuilder.RenameIndex(
                name: "IX_subscription_plans_Code",
                table: "SubscriptionPlans",
                newName: "IX_SubscriptionPlans_Code");

            migrationBuilder.RenameIndex(
                name: "IX_subscription_invoices_TenantSubscriptionId",
                table: "SubscriptionInvoices",
                newName: "IX_SubscriptionInvoices_TenantSubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_subscription_invoices_TenantId",
                table: "SubscriptionInvoices",
                newName: "IX_SubscriptionInvoices_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_subscription_invoices_Status",
                table: "SubscriptionInvoices",
                newName: "IX_SubscriptionInvoices_Status");

            migrationBuilder.RenameIndex(
                name: "IX_subscription_invoices_InvoiceNumber",
                table: "SubscriptionInvoices",
                newName: "IX_SubscriptionInvoices_InvoiceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_sessions_UserId",
                table: "UserSessions",
                newName: "IX_UserSessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_sessions_TenantId",
                table: "UserSessions",
                newName: "IX_UserSessions_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_sessions_SessionTokenHash",
                table: "UserSessions",
                newName: "IX_UserSessions_SessionTokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_role_templates_Name",
                table: "RoleTemplates",
                newName: "IX_RoleTemplates_Name");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "RolePermissions",
                newName: "PermissionCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_role_permissions_RoleId_PermissionId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_RoleId_PermissionCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_role_permissions_PermissionId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_PermissionCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_position_reporting_history_TenantId_PositionId",
                table: "PositionReportingHistories",
                newName: "IX_PositionReportingHistories_TenantId_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_position_reporting_history_PositionId",
                table: "PositionReportingHistories",
                newName: "IX_PositionReportingHistories_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_position_assignments_TenantId_PositionId",
                table: "PositionAssignments",
                newName: "IX_PositionAssignments_TenantId_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_position_assignments_TenantId_EmployeeId",
                table: "PositionAssignments",
                newName: "IX_PositionAssignments_TenantId_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_position_assignments_PositionId",
                table: "PositionAssignments",
                newName: "IX_PositionAssignments_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_position_assignments_EmployeeId",
                table: "PositionAssignments",
                newName: "IX_PositionAssignments_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_platform_users_Email",
                table: "PlatformUsers",
                newName: "IX_PlatformUsers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_platform_user_sessions_SessionTokenHash",
                table: "PlatformUserSessions",
                newName: "IX_PlatformUserSessions_SessionTokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_platform_user_sessions_PlatformUserId",
                table: "PlatformUserSessions",
                newName: "IX_PlatformUserSessions_PlatformUserId");

            migrationBuilder.RenameIndex(
                name: "IX_platform_user_roles_PlatformUserId_PlatformRoleId",
                table: "PlatformUserRoles",
                newName: "IX_PlatformUserRoles_PlatformUserId_PlatformRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_platform_user_roles_PlatformRoleId",
                table: "PlatformUserRoles",
                newName: "IX_PlatformUserRoles_PlatformRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_platform_role_permissions_PlatformRoleId_PlatformPermission~",
                table: "PlatformRolePermissions",
                newName: "IX_PlatformRolePermissions_PlatformRoleId_PlatformPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_platform_role_permissions_PlatformPermissionId",
                table: "PlatformRolePermissions",
                newName: "IX_PlatformRolePermissions_PlatformPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_platform_permissions_PermissionKey",
                table: "PlatformPermissions",
                newName: "IX_PlatformPermissions_PermissionKey");

            migrationBuilder.RenameIndex(
                name: "IX_platform_auth_events_Email",
                table: "PlatformAuthEvents",
                newName: "IX_PlatformAuthEvents_Email");

            migrationBuilder.RenameIndex(
                name: "IX_payment_gateway_credentials_PaymentGatewayConfigId",
                table: "PaymentGatewayCredentials",
                newName: "IX_PaymentGatewayCredentials_PaymentGatewayConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_payment_gateway_country_routes_GatewayConfigId",
                table: "PaymentGatewayCountryRoutes",
                newName: "IX_PaymentGatewayCountryRoutes_GatewayConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_payment_gateway_country_routes_CountryCode_Environment",
                table: "PaymentGatewayCountryRoutes",
                newName: "IX_PaymentGatewayCountryRoutes_CountryCode_Environment");

            migrationBuilder.RenameIndex(
                name: "IX_payment_gateway_configs_GatewayKey",
                table: "PaymentGatewayConfigs",
                newName: "IX_PaymentGatewayConfigs_GatewayKey");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "ModulePermissionOwnerships",
                newName: "PermissionCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_module_permission_ownership_PermissionId",
                table: "ModulePermissionOwnerships",
                newName: "IX_ModulePermissionOwnerships_PermissionCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_module_permission_ownership_ModuleCatalogId_PermissionId",
                table: "ModulePermissionOwnerships",
                newName: "IX_ModulePermissionOwnerships_ModuleCatalogId_PermissionCatalo~");

            migrationBuilder.RenameIndex(
                name: "IX_module_features_ModuleCatalogId",
                table: "ModuleFeatures",
                newName: "IX_ModuleFeatures_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_module_features_FeatureKey",
                table: "ModuleFeatures",
                newName: "IX_ModuleFeatures_FeatureKey");

            migrationBuilder.RenameIndex(
                name: "IX_module_catalog_ModuleKey",
                table: "ModuleCatalogs",
                newName: "IX_ModuleCatalogs_ModuleKey");

            migrationBuilder.RenameIndex(
                name: "IX_legal_entities_TenantId_Code",
                table: "LegalEntities",
                newName: "IX_LegalEntities_TenantId_Code");

            migrationBuilder.RenameIndex(
                name: "IX_legal_entities_TenantId",
                table: "LegalEntities",
                newName: "IX_LegalEntities_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_feature_flags_TenantId_FeatureKey",
                table: "RuntimeFeatureFlags",
                newName: "IX_RuntimeFeatureFlags_TenantId_FeatureKey");

            migrationBuilder.RenameIndex(
                name: "IX_external_calendar_event_links_TenantId_Provider_ExternalCal~",
                table: "ExternalCalendarEventLinks",
                newName: "IX_ExternalCalendarEventLinks_TenantId_Provider_ExternalCalend~");

            migrationBuilder.RenameIndex(
                name: "IX_external_calendar_event_links_ExternalCalendarConnectionId",
                table: "ExternalCalendarEventLinks",
                newName: "IX_ExternalCalendarEventLinks_ExternalCalendarConnectionId");

            migrationBuilder.RenameIndex(
                name: "IX_external_calendar_event_links_CalendarEventId",
                table: "ExternalCalendarEventLinks",
                newName: "IX_ExternalCalendarEventLinks_CalendarEventId");

            migrationBuilder.RenameIndex(
                name: "IX_external_calendar_connections_UserId",
                table: "ExternalCalendarConnections",
                newName: "IX_ExternalCalendarConnections_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_external_calendar_connections_TenantId_UserId_Provider_Exte~",
                table: "ExternalCalendarConnections",
                newName: "IX_ExternalCalendarConnections_TenantId_UserId_Provider_Extern~");

            migrationBuilder.RenameIndex(
                name: "IX_employee_hierarchy_closure_TenantId_ManagerEmployeeId_Repor~",
                table: "EmployeeHierarchyClosures",
                newName: "IX_EmployeeHierarchyClosures_TenantId_ManagerEmployeeId_Report~");

            migrationBuilder.RenameIndex(
                name: "IX_employee_assignment_history_TenantId_EmployeeId",
                table: "EmployeeAssignmentHistories",
                newName: "IX_EmployeeAssignmentHistories_TenantId_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_employee_assignment_history_EmployeeId",
                table: "EmployeeAssignmentHistories",
                newName: "IX_EmployeeAssignmentHistories_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_demo_profiles_Name",
                table: "DemoProfiles",
                newName: "IX_DemoProfiles_Name");

            migrationBuilder.RenameIndex(
                name: "IX_demo_profile_modules_ModuleCatalogId",
                table: "DemoProfileModuleAccess",
                newName: "IX_DemoProfileModuleAccess_ModuleCatalogId");

            migrationBuilder.RenameIndex(
                name: "IX_demo_profile_modules_DemoProfileId_ModuleCatalogId",
                table: "DemoProfileModuleAccess",
                newName: "IX_DemoProfileModuleAccess_DemoProfileId_ModuleCatalogId");

            migrationBuilder.RenameColumn(
                name: "ReviewedById",
                table: "DemoRequests",
                newName: "ReviewedByPlatformUserId");

            migrationBuilder.RenameColumn(
                name: "RequesterName",
                table: "DemoRequests",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "RequesterEmail",
                table: "DemoRequests",
                newName: "ContactName");

            migrationBuilder.RenameColumn(
                name: "CreatedTenantId",
                table: "DemoRequests",
                newName: "ConvertedTenantId");

            migrationBuilder.RenameColumn(
                name: "CountryCode",
                table: "DemoRequests",
                newName: "BusinessEmail");

            migrationBuilder.RenameIndex(
                name: "IX_demo_access_requests_Status",
                table: "DemoRequests",
                newName: "IX_DemoRequests_Status");

            migrationBuilder.RenameIndex(
                name: "IX_configuration_templates_TemplateKey",
                table: "ConfigurationTemplates",
                newName: "IX_ConfigurationTemplates_TemplateKey");

            migrationBuilder.RenameIndex(
                name: "IX_calendar_events_TenantId",
                table: "CalendarEvents",
                newName: "IX_CalendarEvents_TenantId");

            migrationBuilder.AlterColumn<decimal>(
                name: "AnnualPrice",
                table: "SubscriptionPlanPriceBrackets",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeRangeMax",
                table: "SubscriptionPlanPriceBrackets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeRangeMin",
                table: "SubscriptionPlanPriceBrackets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPrice",
                table: "SubscriptionPlanPriceBrackets",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowed",
                table: "DemoProfileUpgradeOptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionPlanId",
                table: "DemoProfileUpgradeOptions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Positions",
                table: "Positions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveTypes",
                table: "LeaveTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequests",
                table: "LeaveRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeavePolicyAssignments",
                table: "LeavePolicyAssignments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeavePolicies",
                table: "LeavePolicies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveBalances",
                table: "LeaveBalances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantSubscriptions",
                table: "TenantSubscriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantProvisioningStates",
                table: "TenantProvisioningStates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantModuleEntitlements",
                table: "TenantModuleEntitlements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantConfigurationTemplateApplications",
                table: "TenantConfigurationTemplateApplications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPlans",
                table: "SubscriptionPlans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPlanPriceBrackets",
                table: "SubscriptionPlanPriceBrackets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionInvoices",
                table: "SubscriptionInvoices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSessions",
                table: "UserSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleTemplates",
                table: "RoleTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PositionReportingHistories",
                table: "PositionReportingHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PositionAssignments",
                table: "PositionAssignments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformUsers",
                table: "PlatformUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformUserSessions",
                table: "PlatformUserSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformUserRoles",
                table: "PlatformUserRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformRoles",
                table: "PlatformRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformRolePermissions",
                table: "PlatformRolePermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformPermissions",
                table: "PlatformPermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformAuthEvents",
                table: "PlatformAuthEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentGatewayCredentials",
                table: "PaymentGatewayCredentials",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentGatewayCountryRoutes",
                table: "PaymentGatewayCountryRoutes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentGatewayConfigs",
                table: "PaymentGatewayConfigs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModulePermissionOwnerships",
                table: "ModulePermissionOwnerships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuleFeatures",
                table: "ModuleFeatures",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuleCatalogs",
                table: "ModuleCatalogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LegalEntities",
                table: "LegalEntities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RuntimeFeatureFlags",
                table: "RuntimeFeatureFlags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalCalendarEventLinks",
                table: "ExternalCalendarEventLinks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalCalendarConnections",
                table: "ExternalCalendarConnections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeHierarchyClosures",
                table: "EmployeeHierarchyClosures",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeAssignmentHistories",
                table: "EmployeeAssignmentHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DemoProfiles",
                table: "DemoProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DemoProfileUpgradeOptions",
                table: "DemoProfileUpgradeOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DemoProfileModuleAccess",
                table: "DemoProfileModuleAccess",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DemoRequests",
                table: "DemoRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConfigurationTemplates",
                table: "ConfigurationTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalendarEvents",
                table: "CalendarEvents",
                column: "Id");

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
                name: "EmployeeProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobTitle = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true)
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
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryCode = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false)
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
                    Category = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPlatformPermission = table.Column<bool>(type: "boolean", nullable: false),
                    IsTenantPermission = table.Column<bool>(type: "boolean", nullable: false),
                    PermissionKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionRoleAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "RoleTemplateVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
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
                name: "SubscriptionAddOns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleCatalogId = table.Column<Guid>(type: "uuid", nullable: true),
                    AddOnKey = table.Column<string>(type: "text", nullable: false),
                    AddOnType = table.Column<string>(type: "text", nullable: false),
                    AiTokenContribution = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StorageContributionGb = table.Column<int>(type: "integer", nullable: true)
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
                name: "SubscriptionPlanFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "TenantActivationChecklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
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
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Domain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "TenantFeatureEntitlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "TenantResourceLimits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AiTokenLimit = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmployeeLimit = table.Column<int>(type: "integer", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: false),
                    StorageLimitGb = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantResourceLimits", x => x.Id);
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
                name: "RoleTemplatePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionCatalogId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleTemplateId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanPriceBrackets_SubscriptionPlanId",
                table: "SubscriptionPlanPriceBrackets",
                column: "SubscriptionPlanId");

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
                name: "IX_EmployeeProfiles_EmployeeId",
                table: "EmployeeProfiles",
                column: "EmployeeId",
                unique: true);

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
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionCatalogs_PermissionKey",
                table: "PermissionCatalogs",
                column: "PermissionKey",
                unique: true);

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
                name: "IX_RoleTemplatePermissions_PermissionCatalogId",
                table: "RoleTemplatePermissions",
                column: "PermissionCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleTemplatePermissions_RoleTemplateId_PermissionCatalogId",
                table: "RoleTemplatePermissions",
                columns: new[] { "RoleTemplateId", "PermissionCatalogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleTemplateVersions_RoleTemplateId_Version",
                table: "RoleTemplateVersions",
                columns: new[] { "RoleTemplateId", "Version" },
                unique: true);

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
                name: "IX_SubscriptionPlanFeatures_ModuleFeatureId",
                table: "SubscriptionPlanFeatures",
                column: "ModuleFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanFeatures_SubscriptionPlanId_ModuleFeatureId",
                table: "SubscriptionPlanFeatures",
                columns: new[] { "SubscriptionPlanId", "ModuleFeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantActivationChecklists_TenantId_Key",
                table: "TenantActivationChecklists",
                columns: new[] { "TenantId", "Key" },
                unique: true);

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
                name: "IX_TenantResourceLimits_TenantId",
                table: "TenantResourceLimits",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_DemoProfileModuleAccess_DemoProfiles_DemoProfileId",
                table: "DemoProfileModuleAccess",
                column: "DemoProfileId",
                principalTable: "DemoProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DemoProfileModuleAccess_ModuleCatalogs_ModuleCatalogId",
                table: "DemoProfileModuleAccess",
                column: "ModuleCatalogId",
                principalTable: "ModuleCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DemoProfileUpgradeOptions_DemoProfiles_DemoProfileId",
                table: "DemoProfileUpgradeOptions",
                column: "DemoProfileId",
                principalTable: "DemoProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DemoProfileUpgradeOptions_SubscriptionPlans_SubscriptionPla~",
                table: "DemoProfileUpgradeOptions",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_LegalEntities_LegalEntityId",
                table: "Departments",
                column: "LegalEntityId",
                principalTable: "LegalEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Positions_HeadPositionId",
                table: "Departments",
                column: "HeadPositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeAssignmentHistories_Employees_EmployeeId",
                table: "EmployeeAssignmentHistories",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_LegalEntities_LegalEntityId",
                table: "Employees",
                column: "LegalEntityId",
                principalTable: "LegalEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Tenants_TenantId",
                table: "Employees",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalCalendarConnections_Tenants_TenantId",
                table: "ExternalCalendarConnections",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalCalendarConnections_Users_UserId",
                table: "ExternalCalendarConnections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalCalendarEventLinks_CalendarEvents_CalendarEventId",
                table: "ExternalCalendarEventLinks",
                column: "CalendarEventId",
                principalTable: "CalendarEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalCalendarEventLinks_ExternalCalendarConnections_Exte~",
                table: "ExternalCalendarEventLinks",
                column: "ExternalCalendarConnectionId",
                principalTable: "ExternalCalendarConnections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalCalendarEventLinks_Tenants_TenantId",
                table: "ExternalCalendarEventLinks",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveBalances_LeaveTypes_LeaveTypeId",
                table: "LeaveBalances",
                column: "LeaveTypeId",
                principalTable: "LeaveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeavePolicies_LeaveTypes_LeaveTypeId",
                table: "LeavePolicies",
                column: "LeaveTypeId",
                principalTable: "LeaveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeavePolicyAssignments_LeavePolicies_LeavePolicyId",
                table: "LeavePolicyAssignments",
                column: "LeavePolicyId",
                principalTable: "LeavePolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                table: "LeaveRequests",
                column: "LeaveTypeId",
                principalTable: "LeaveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalEntities_Tenants_TenantId",
                table: "LegalEntities",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleFeatures_ModuleCatalogs_ModuleCatalogId",
                table: "ModuleFeatures",
                column: "ModuleCatalogId",
                principalTable: "ModuleCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModulePermissionOwnerships_ModuleCatalogs_ModuleCatalogId",
                table: "ModulePermissionOwnerships",
                column: "ModuleCatalogId",
                principalTable: "ModuleCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModulePermissionOwnerships_PermissionCatalogs_PermissionCat~",
                table: "ModulePermissionOwnerships",
                column: "PermissionCatalogId",
                principalTable: "PermissionCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentGatewayCountryRoutes_PaymentGatewayConfigs_GatewayCo~",
                table: "PaymentGatewayCountryRoutes",
                column: "GatewayConfigId",
                principalTable: "PaymentGatewayConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentGatewayCredentials_PaymentGatewayConfigs_PaymentGate~",
                table: "PaymentGatewayCredentials",
                column: "PaymentGatewayConfigId",
                principalTable: "PaymentGatewayConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlatformRolePermissions_PlatformPermissions_PlatformPermiss~",
                table: "PlatformRolePermissions",
                column: "PlatformPermissionId",
                principalTable: "PlatformPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlatformRolePermissions_PlatformRoles_PlatformRoleId",
                table: "PlatformRolePermissions",
                column: "PlatformRoleId",
                principalTable: "PlatformRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlatformUserRoles_PlatformRoles_PlatformRoleId",
                table: "PlatformUserRoles",
                column: "PlatformRoleId",
                principalTable: "PlatformRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlatformUserRoles_PlatformUsers_PlatformUserId",
                table: "PlatformUserRoles",
                column: "PlatformUserId",
                principalTable: "PlatformUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlatformUserSessions_PlatformUsers_PlatformUserId",
                table: "PlatformUserSessions",
                column: "PlatformUserId",
                principalTable: "PlatformUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAssignments_Employees_EmployeeId",
                table: "PositionAssignments",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAssignments_Positions_PositionId",
                table: "PositionAssignments",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionReportingHistories_Positions_PositionId",
                table: "PositionReportingHistories",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Departments_DepartmentId",
                table: "Positions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_LegalEntities_LegalEntityId",
                table: "Positions",
                column: "LegalEntityId",
                principalTable: "LegalEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Positions_ReportsToPositionId",
                table: "Positions",
                column: "ReportsToPositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_PermissionCatalogs_PermissionCatalogId",
                table: "RolePermissions",
                column: "PermissionCatalogId",
                principalTable: "PermissionCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionInvoices_TenantSubscriptions_TenantSubscription~",
                table: "SubscriptionInvoices",
                column: "TenantSubscriptionId",
                principalTable: "TenantSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionPlanPriceBrackets_SubscriptionPlans_Subscriptio~",
                table: "SubscriptionPlanPriceBrackets",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantConfigurationTemplateApplications_ConfigurationTempla~",
                table: "TenantConfigurationTemplateApplications",
                column: "ConfigurationTemplateId",
                principalTable: "ConfigurationTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantModuleEntitlements_ModuleCatalogs_ModuleCatalogId",
                table: "TenantModuleEntitlements",
                column: "ModuleCatalogId",
                principalTable: "ModuleCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantProvisioningStates_Tenants_TenantId",
                table: "TenantProvisioningStates",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "TenantSubscriptions",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSubscriptions_Tenants_TenantId",
                table: "TenantSubscriptions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
