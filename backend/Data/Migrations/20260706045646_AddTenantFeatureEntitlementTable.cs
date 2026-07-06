using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantFeatureEntitlementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        name: "FK_TenantFeatureEntitlements_module_features_ModuleFeatureId",
                        column: x => x.ModuleFeatureId,
                        principalTable: "module_features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatureEntitlements_ModuleFeatureId",
                table: "TenantFeatureEntitlements",
                column: "ModuleFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatureEntitlements_TenantId_ModuleFeatureId",
                table: "TenantFeatureEntitlements",
                columns: new[] { "TenantId", "ModuleFeatureId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantFeatureEntitlements");
        }
    }
}
