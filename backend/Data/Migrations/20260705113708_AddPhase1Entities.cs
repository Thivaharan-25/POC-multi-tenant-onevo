using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase1Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OfficeAddressLabel",
                table: "LegalEntities",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OfficeAllowedRadiusMeters",
                table: "LegalEntities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OfficeLatitude",
                table: "LegalEntities",
                type: "numeric(10,7)",
                precision: 10,
                scale: 7,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OfficeLongitude",
                table: "LegalEntities",
                type: "numeric(10,7)",
                precision: 10,
                scale: 7,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExternalCalendarConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    ExternalAccountEmail = table.Column<string>(type: "text", nullable: false),
                    ExternalCalendarId = table.Column<string>(type: "text", nullable: true),
                    ExternalCalendarName = table.Column<string>(type: "text", nullable: true),
                    AccessTokenEncrypted = table.Column<byte[]>(type: "bytea", nullable: true),
                    RefreshTokenEncrypted = table.Column<byte[]>(type: "bytea", nullable: false),
                    ScopesJson = table.Column<string>(type: "jsonb", nullable: false),
                    SyncDirection = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SyncTokenEncrypted = table.Column<byte[]>(type: "bytea", nullable: true),
                    DeltaLinkEncrypted = table.Column<byte[]>(type: "bytea", nullable: true),
                    FailureCount = table.Column<int>(type: "integer", nullable: false),
                    LastSyncedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSuccessfulSyncAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalCalendarConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalCalendarConnections_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalCalendarConnections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGatewayConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GatewayKey = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    Environment = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    PublicKey = table.Column<string>(type: "text", nullable: true),
                    MerchantId = table.Column<string>(type: "text", nullable: true),
                    WebhookUrl = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalCalendarEventLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CalendarEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalCalendarConnectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    ExternalCalendarId = table.Column<string>(type: "text", nullable: false),
                    ExternalEventId = table.Column<string>(type: "text", nullable: false),
                    ExternalEtag = table.Column<string>(type: "text", nullable: false),
                    SyncDirection = table.Column<string>(type: "text", nullable: false),
                    SyncStatus = table.Column<string>(type: "text", nullable: false),
                    LastSyncedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalCalendarEventLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalCalendarEventLinks_CalendarEvents_CalendarEventId",
                        column: x => x.CalendarEventId,
                        principalTable: "CalendarEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalCalendarEventLinks_ExternalCalendarConnections_Exte~",
                        column: x => x.ExternalCalendarConnectionId,
                        principalTable: "ExternalCalendarConnections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalCalendarEventLinks_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGatewayCountryRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CountryNameSnapshot = table.Column<string>(type: "text", nullable: false),
                    GatewayConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    Environment = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayCountryRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGatewayCountryRoutes_PaymentGatewayConfigs_GatewayCo~",
                        column: x => x.GatewayConfigId,
                        principalTable: "PaymentGatewayConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGatewayCredentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentGatewayConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    SecretEncrypted = table.Column<byte[]>(type: "bytea", nullable: false),
                    WebhookSecretEncrypted = table.Column<byte[]>(type: "bytea", nullable: true),
                    EncryptionKeyVersion = table.Column<string>(type: "text", nullable: false),
                    CredentialVersion = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RotatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    RotatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeactivatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    DeactivatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGatewayCredentials_PaymentGatewayConfigs_PaymentGate~",
                        column: x => x.PaymentGatewayConfigId,
                        principalTable: "PaymentGatewayConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCalendarConnections_TenantId_UserId_Provider_Extern~",
                table: "ExternalCalendarConnections",
                columns: new[] { "TenantId", "UserId", "Provider", "ExternalCalendarId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCalendarConnections_UserId",
                table: "ExternalCalendarConnections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCalendarEventLinks_CalendarEventId",
                table: "ExternalCalendarEventLinks",
                column: "CalendarEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCalendarEventLinks_ExternalCalendarConnectionId",
                table: "ExternalCalendarEventLinks",
                column: "ExternalCalendarConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCalendarEventLinks_TenantId_Provider_ExternalCalend~",
                table: "ExternalCalendarEventLinks",
                columns: new[] { "TenantId", "Provider", "ExternalCalendarId", "ExternalEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayConfigs_GatewayKey",
                table: "PaymentGatewayConfigs",
                column: "GatewayKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayCountryRoutes_CountryCode_Environment",
                table: "PaymentGatewayCountryRoutes",
                columns: new[] { "CountryCode", "Environment" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayCountryRoutes_GatewayConfigId",
                table: "PaymentGatewayCountryRoutes",
                column: "GatewayConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayCredentials_PaymentGatewayConfigId",
                table: "PaymentGatewayCredentials",
                column: "PaymentGatewayConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalCalendarEventLinks");

            migrationBuilder.DropTable(
                name: "PaymentGatewayCountryRoutes");

            migrationBuilder.DropTable(
                name: "PaymentGatewayCredentials");

            migrationBuilder.DropTable(
                name: "ExternalCalendarConnections");

            migrationBuilder.DropTable(
                name: "PaymentGatewayConfigs");

            migrationBuilder.DropColumn(
                name: "OfficeAddressLabel",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "OfficeAllowedRadiusMeters",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "OfficeLatitude",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "OfficeLongitude",
                table: "LegalEntities");
        }
    }
}
