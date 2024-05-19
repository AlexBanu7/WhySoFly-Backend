using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class LatLng_Market : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Markets",
                newName: "Longitute");

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Markets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Markets");

            migrationBuilder.RenameColumn(
                name: "Longitute",
                table: "Markets",
                newName: "Location");
        }
    }
}
