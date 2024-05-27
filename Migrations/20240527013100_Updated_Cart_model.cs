using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Cart_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MarketId",
                table: "Carts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_MarketId",
                table: "Carts",
                column: "MarketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Markets_MarketId",
                table: "Carts",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Markets_MarketId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_MarketId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "Carts");
        }
    }
}
