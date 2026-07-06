using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostgresRlsPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CsrfTokenHash",
                table: "PlatformUserSessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            string[] tenantOwnedTables = new[]
            {
                "PositionRoleAssignments",
                "Roles",
                "Users",
                "UserSessions",
                "CalendarEvents",
                "Holidays",
                "Employees",
                "EmployeeAssignmentHistories",
                "LeaveBalances",
                "LeavePolicies",
                "LeavePolicyAssignments",
                "LeaveRequests",
                "LeaveTypes",
                "Notifications",
                "Departments",
                "EmployeeHierarchyClosures",
                "LegalEntities",
                "Positions",
                "PositionAssignments",
                "PositionReportingHistories",
                "SubscriptionInvoices",
                "TenantFeatureEntitlements",
                "TenantModuleEntitlements",
                "TenantResourceLimits",
                "TenantSubscriptions",
                "TenantConfigurationTemplateApplications",
                "TenantActivationChecklists",
                "TenantProvisioningStates",
                "ApprovalTasks",
                "WorkflowDefinitions",
                "WorkflowRuns"
            };

            foreach (var table in tenantOwnedTables)
            {
                migrationBuilder.Sql($@"
                    ALTER TABLE ""{table}"" ENABLE ROW LEVEL SECURITY;
                    ALTER TABLE ""{table}"" FORCE ROW LEVEL SECURITY;
                    DROP POLICY IF EXISTS tenant_isolation ON ""{table}"";
                    CREATE POLICY tenant_isolation ON ""{table}""
                        USING (
                            (current_setting('app.bypass_rls', true) = 'true')
                            OR (
                                current_setting('app.current_tenant_id', true) IS NOT NULL
                                AND current_setting('app.current_tenant_id', true) <> ''
                                AND current_setting('app.current_tenant_id', true) <> '00000000-0000-0000-0000-000000000000'
                                AND ""TenantId"" = current_setting('app.current_tenant_id')::uuid
                            )
                        );
                ");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CsrfTokenHash",
                table: "PlatformUserSessions");

            string[] tenantOwnedTables = new[]
            {
                "PositionRoleAssignments",
                "Roles",
                "Users",
                "UserSessions",
                "CalendarEvents",
                "Holidays",
                "Employees",
                "EmployeeAssignmentHistories",
                "LeaveBalances",
                "LeavePolicies",
                "LeavePolicyAssignments",
                "LeaveRequests",
                "LeaveTypes",
                "Notifications",
                "Departments",
                "EmployeeHierarchyClosures",
                "LegalEntities",
                "Positions",
                "PositionAssignments",
                "PositionReportingHistories",
                "SubscriptionInvoices",
                "TenantFeatureEntitlements",
                "TenantModuleEntitlements",
                "TenantResourceLimits",
                "TenantSubscriptions",
                "TenantConfigurationTemplateApplications",
                "TenantActivationChecklists",
                "TenantProvisioningStates",
                "ApprovalTasks",
                "WorkflowDefinitions",
                "WorkflowRuns"
            };

            foreach (var table in tenantOwnedTables)
            {
                migrationBuilder.Sql($@"
                    DROP POLICY IF EXISTS tenant_isolation ON ""{table}"";
                    ALTER TABLE ""{table}"" NO FORCE ROW LEVEL SECURITY;
                    ALTER TABLE ""{table}"" DISABLE ROW LEVEL SECURITY;
                ");
            }
        }
    }
}
