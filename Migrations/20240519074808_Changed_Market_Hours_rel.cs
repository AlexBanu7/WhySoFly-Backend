using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Changed_Market_Hours_rel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreHours_Markets_MarketId",
                table: "StoreHours");

            migrationBuilder.DropIndex(
                name: "IX_StoreHours_MarketId",
                table: "StoreHours");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "StoreHours");

            migrationBuilder.AddColumn<long>(
                name: "StoreHoursId",
                table: "Markets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Markets_StoreHoursId",
                table: "Markets",
                column: "StoreHoursId");

            migrationBuilder.AddForeignKey(
                name: "FK_Markets_StoreHours_StoreHoursId",
                table: "Markets",
                column: "StoreHoursId",
                principalTable: "StoreHours",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Markets_StoreHours_StoreHoursId",
                table: "Markets");

            migrationBuilder.DropIndex(
                name: "IX_Markets_StoreHoursId",
                table: "Markets");

            migrationBuilder.DropColumn(
                name: "StoreHoursId",
                table: "Markets");

            migrationBuilder.AddColumn<long>(
                name: "MarketId",
                table: "StoreHours",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_StoreHours_MarketId",
                table: "StoreHours",
                column: "MarketId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreHours_Markets_MarketId",
                table: "StoreHours",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
