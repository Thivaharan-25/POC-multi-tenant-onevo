using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLegalEntityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "legal_entities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "legal_entities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "legal_entities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "legal_entities",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "legal_entities");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "legal_entities");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "legal_entities");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "legal_entities");
        }
    }
}
