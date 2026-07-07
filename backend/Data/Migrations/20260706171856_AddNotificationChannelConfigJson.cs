using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnevoHr.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationChannelConfigJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // jsonb -> text has no automatic cast in PostgreSQL, so a USING
            // expression is required. Data Protection payloads are opaque
            // base64 strings, not JSON, hence the type change.
            migrationBuilder.Sql(
                "ALTER TABLE notification_channels ALTER COLUMN \"CredentialsEncrypted\" TYPE text USING \"CredentialsEncrypted\"::text;");

            migrationBuilder.AddColumn<string>(
                name: "ConfigJson",
                table: "notification_channels",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigJson",
                table: "notification_channels");

            // Encrypted credential blobs are not valid JSON, so reverting
            // wipes them; the channel must be reconfigured after a rollback.
            migrationBuilder.Sql(
                "ALTER TABLE notification_channels ALTER COLUMN \"CredentialsEncrypted\" TYPE jsonb USING '\"\"'::jsonb;");
        }
    }
}
