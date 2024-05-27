using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Cart_model_nullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_CustomerId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Employees_EmployeeId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Markets_MarketId",
                table: "Carts");

            migrationBuilder.AlterColumn<long>(
                name: "MarketId",
                table: "Carts",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "EmployeeId",
                table: "Carts",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Carts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_CustomerId",
                table: "Carts",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Employees_EmployeeId",
                table: "Carts",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Markets_MarketId",
                table: "Carts",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_CustomerId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Employees_EmployeeId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Markets_MarketId",
                table: "Carts");

            migrationBuilder.AlterColumn<long>(
                name: "MarketId",
                table: "Carts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EmployeeId",
                table: "Carts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Carts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_CustomerId",
                table: "Carts",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Employees_EmployeeId",
                table: "Carts",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Markets_MarketId",
                table: "Carts",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
