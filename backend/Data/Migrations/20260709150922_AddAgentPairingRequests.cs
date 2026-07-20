using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentPairingRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceTokenHash",
                table: "registered_agents",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "agent_pairing_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeviceName = table.Column<string>(type: "text", nullable: false),
                    UserCodeHash = table.Column<string>(type: "text", nullable: false),
                    DeviceCodeHash = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ConsentAcceptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RegisteredAgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FailedConfirmAttempts = table.Column<int>(type: "integer", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_pairing_requests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agent_pairing_requests_DeviceCodeHash",
                table: "agent_pairing_requests",
                column: "DeviceCodeHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_agent_pairing_requests_UserCodeHash",
                table: "agent_pairing_requests",
                column: "UserCodeHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_pairing_requests");

            migrationBuilder.DropColumn(
                name: "DeviceTokenHash",
                table: "registered_agents");
        }
    }
}
