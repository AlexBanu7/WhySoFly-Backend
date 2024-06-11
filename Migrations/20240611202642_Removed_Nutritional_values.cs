using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Removed_Nutritional_values : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_NutritionalValues_NutritionalValuesId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "NutritionalValues");

            migrationBuilder.DropIndex(
                name: "IX_Products_NutritionalValuesId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NutritionalValuesId",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "NutritionalValuesId",
                table: "Products",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NutritionalValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Energy = table.Column<float>(type: "real", nullable: false),
                    Fibers = table.Column<float>(type: "real", nullable: false),
                    Proteins = table.Column<float>(type: "real", nullable: false),
                    SaturatedFats = table.Column<float>(type: "real", nullable: false),
                    Sugars = table.Column<float>(type: "real", nullable: false),
                    TotalCarbohydrates = table.Column<float>(type: "real", nullable: false),
                    TotalFats = table.Column<float>(type: "real", nullable: false),
                    TransFats = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionalValues", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_NutritionalValuesId",
                table: "Products",
                column: "NutritionalValuesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_NutritionalValues_NutritionalValuesId",
                table: "Products",
                column: "NutritionalValuesId",
                principalTable: "NutritionalValues",
                principalColumn: "Id");
        }
    }
}
