using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentClockStateAndAppUsageMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agent_clock_states",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisteredAgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsClockedIn = table.Column<bool>(type: "boolean", nullable: false),
                    ClockedInAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ClockedOutAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_clock_states", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_usage_TenantId_EmployeeId_Date_ApplicationName",
                table: "application_usage",
                columns: new[] { "TenantId", "EmployeeId", "Date", "ApplicationName" });

            migrationBuilder.CreateIndex(
                name: "IX_agent_clock_states_RegisteredAgentId",
                table: "agent_clock_states",
                column: "RegisteredAgentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_clock_states");

            migrationBuilder.DropIndex(
                name: "IX_application_usage_TenantId_EmployeeId_Date_ApplicationName",
                table: "application_usage");
        }
    }
}
