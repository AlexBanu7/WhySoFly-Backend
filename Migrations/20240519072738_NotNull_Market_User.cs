using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class NotNull_Market_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Markets_AspNetUsers_UserAccountId",
                table: "Markets");

            migrationBuilder.AlterColumn<string>(
                name: "UserAccountId",
                table: "Markets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Markets_AspNetUsers_UserAccountId",
                table: "Markets",
                column: "UserAccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Markets_AspNetUsers_UserAccountId",
                table: "Markets");

            migrationBuilder.AlterColumn<string>(
                name: "UserAccountId",
                table: "Markets",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Markets_AspNetUsers_UserAccountId",
                table: "Markets",
                column: "UserAccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
