using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foodtrackr.Api.Migrations
{
    /// <inheritdoc />
    public partial class Sprint2_FoodAndLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodItems",
                columns: table => new
                {
                    FoodId = table.Column<string>(type: "TEXT", nullable: false),
                    FoodName = table.Column<string>(type: "TEXT", nullable: false),
                    EnergyKj = table.Column<double>(type: "REAL", nullable: true),
                    EnergyKcal = table.Column<double>(type: "REAL", nullable: true),
                    ProteinG = table.Column<double>(type: "REAL", nullable: true),
                    FatG = table.Column<double>(type: "REAL", nullable: true),
                    CarbohydrateG = table.Column<double>(type: "REAL", nullable: true),
                    FibreTotalG = table.Column<double>(type: "REAL", nullable: true),
                    SugarsG = table.Column<double>(type: "REAL", nullable: true),
                    StarchG = table.Column<double>(type: "REAL", nullable: true),
                    AlcoholG = table.Column<double>(type: "REAL", nullable: true),
                    WaterG = table.Column<double>(type: "REAL", nullable: true),
                    FatSaturatedG = table.Column<double>(type: "REAL", nullable: true),
                    FatMonoG = table.Column<double>(type: "REAL", nullable: true),
                    FatPolyG = table.Column<double>(type: "REAL", nullable: true),
                    FatTransG = table.Column<double>(type: "REAL", nullable: true),
                    Omega3G = table.Column<double>(type: "REAL", nullable: true),
                    Omega6G = table.Column<double>(type: "REAL", nullable: true),
                    CholesterolMg = table.Column<double>(type: "REAL", nullable: true),
                    CalciumMg = table.Column<double>(type: "REAL", nullable: true),
                    IronMg = table.Column<double>(type: "REAL", nullable: true),
                    SodiumMg = table.Column<double>(type: "REAL", nullable: true),
                    PotassiumMg = table.Column<double>(type: "REAL", nullable: true),
                    MagnesiumMg = table.Column<double>(type: "REAL", nullable: true),
                    PhosphorusMg = table.Column<double>(type: "REAL", nullable: true),
                    ZincMg = table.Column<double>(type: "REAL", nullable: true),
                    SeleniumUg = table.Column<double>(type: "REAL", nullable: true),
                    IodideUg = table.Column<double>(type: "REAL", nullable: true),
                    CopperMg = table.Column<double>(type: "REAL", nullable: true),
                    ManganeseUg = table.Column<double>(type: "REAL", nullable: true),
                    VitaminCMg = table.Column<double>(type: "REAL", nullable: true),
                    VitaminD = table.Column<double>(type: "REAL", nullable: true),
                    VitaminB12Ug = table.Column<double>(type: "REAL", nullable: true),
                    VitaminB6Mg = table.Column<double>(type: "REAL", nullable: true),
                    FolateUg = table.Column<double>(type: "REAL", nullable: true),
                    DietaryFolateEqUg = table.Column<double>(type: "REAL", nullable: true),
                    ThiaminMg = table.Column<double>(type: "REAL", nullable: true),
                    RiboflavinMg = table.Column<double>(type: "REAL", nullable: true),
                    NiacinMg = table.Column<double>(type: "REAL", nullable: true),
                    RetinolUg = table.Column<double>(type: "REAL", nullable: true),
                    VitaminAUg = table.Column<double>(type: "REAL", nullable: true),
                    VitaminEMg = table.Column<double>(type: "REAL", nullable: true),
                    BetaCaroteneUg = table.Column<double>(type: "REAL", nullable: true),
                    CaffeineM = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItems", x => x.FoodId);
                });

            migrationBuilder.CreateTable(
                name: "FoodLogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PatientId = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodId = table.Column<string>(type: "TEXT", nullable: false),
                    FoodName = table.Column<string>(type: "TEXT", nullable: false),
                    PortionWeightGrams = table.Column<double>(type: "REAL", nullable: false),
                    MealType = table.Column<string>(type: "TEXT", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsCustom = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    FoodItemFoodId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodLogEntries_FoodItems_FoodItemFoodId",
                        column: x => x.FoodItemFoodId,
                        principalTable: "FoodItems",
                        principalColumn: "FoodId");
                    table.ForeignKey(
                        name: "FK_FoodLogEntries_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodPortions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoodId = table.Column<string>(type: "TEXT", nullable: false),
                    MeasureDescription = table.Column<string>(type: "TEXT", nullable: false),
                    WeightGrams = table.Column<double>(type: "REAL", nullable: false),
                    Density = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPortions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodPortions_FoodItems_FoodId",
                        column: x => x.FoodId,
                        principalTable: "FoodItems",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodLogEntries_FoodItemFoodId",
                table: "FoodLogEntries",
                column: "FoodItemFoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodLogEntries_PatientId",
                table: "FoodLogEntries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPortions_FoodId",
                table: "FoodPortions",
                column: "FoodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodLogEntries");

            migrationBuilder.DropTable(
                name: "FoodPortions");

            migrationBuilder.DropTable(
                name: "FoodItems");
        }
    }
}
