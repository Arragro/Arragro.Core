using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arragro.Core.Openiddict.EntityFrameworkCore.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Upgrade50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "openiddict",
                table: "OpenIddictApplications",
                newName: "ClientType");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationType",
                schema: "openiddict",
                table: "OpenIddictApplications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JsonWebKeySet",
                schema: "openiddict",
                table: "OpenIddictApplications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                schema: "openiddict",
                table: "OpenIddictApplications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationType",
                schema: "openiddict",
                table: "OpenIddictApplications");

            migrationBuilder.DropColumn(
                name: "JsonWebKeySet",
                schema: "openiddict",
                table: "OpenIddictApplications");

            migrationBuilder.DropColumn(
                name: "Settings",
                schema: "openiddict",
                table: "OpenIddictApplications");

            migrationBuilder.RenameColumn(
                name: "ClientType",
                schema: "openiddict",
                table: "OpenIddictApplications",
                newName: "Type");
        }
    }
}
