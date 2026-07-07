using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailBodySnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyHtmlSnapshot",
                table: "email_delivery_logs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyTextSnapshot",
                table: "email_delivery_logs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyHtmlSnapshot",
                table: "email_delivery_logs");

            migrationBuilder.DropColumn(
                name: "BodyTextSnapshot",
                table: "email_delivery_logs");
        }
    }
}
