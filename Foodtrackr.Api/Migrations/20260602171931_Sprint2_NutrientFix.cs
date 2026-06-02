using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foodtrackr.Api.Migrations
{
    /// <inheritdoc />
    public partial class Sprint2_NutrientFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CarbsPer100g",
                table: "FoodLogEntries",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EnergyKcalPer100g",
                table: "FoodLogEntries",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FatPer100g",
                table: "FoodLogEntries",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ProteinPer100g",
                table: "FoodLogEntries",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarbsPer100g",
                table: "FoodLogEntries");

            migrationBuilder.DropColumn(
                name: "EnergyKcalPer100g",
                table: "FoodLogEntries");

            migrationBuilder.DropColumn(
                name: "FatPer100g",
                table: "FoodLogEntries");

            migrationBuilder.DropColumn(
                name: "ProteinPer100g",
                table: "FoodLogEntries");
        }
    }
}
