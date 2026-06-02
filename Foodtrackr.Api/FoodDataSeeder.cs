using Foodtrackr.Api.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; 

namespace Foodtrackr.Api
{
    public static class FoodDataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.FoodItems.AnyAsync())
                return; // Already seeded

            Console.WriteLine("Seeding FOODfiles data...");

            var foodItems = new List<FoodItem>();
            var portions = new List<FoodPortion>();

            // --- Seed FoodItems from Standard DATA.AP.xlsx ---
            var dataPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "Standard_DATA_AP.xlsx");
            if (!File.Exists(dataPath))
            {
                Console.WriteLine($"WARNING: Seed file not found at {dataPath}. Skipping food seeding.");
                return;
            }

            ExcelPackage.License.SetNonCommercialPersonal("Foodtrackr");

            using (var package = new ExcelPackage(new FileInfo(dataPath)))
            {
                var ws = package.Workbook.Worksheets[0];
                // Row 1 = copyright, Row 2 = headers, Row 3 = units, Row 4+ = data
                int totalRows = ws.Dimension.Rows;

                for (int row = 4; row <= totalRows; row++)
                {
                    var foodId = ws.Cells[row, 1].Text?.Trim();
                    if (string.IsNullOrEmpty(foodId)) continue;

                    double? GetVal(int col)
                    {
                        var text = ws.Cells[row, col].Text?.Trim();
                        return double.TryParse(text, out var v) ? v : null;
                    }

                    foodItems.Add(new FoodItem
                    {
                        FoodId = foodId,
                        FoodName = ws.Cells[row, 2].Text?.Trim() ?? string.Empty,
                        EnergyKj = GetVal(28),
                        EnergyKcal = GetVal(26),
                        ProteinG = GetVal(67),
                        FatG = GetVal(30),
                        CarbohydrateG = GetVal(8),
                        FibreTotalG = GetVal(44),
                        SugarsG = GetVal(76),
                        StarchG = GetVal(72),
                        AlcoholG = GetVal(3),
                        WaterG = GetVal(88),
                        FatSaturatedG = GetVal(42),
                        FatMonoG = GetVal(38),
                        FatPolyG = GetVal(39),
                        FatTransG = GetVal(43),
                        Omega3G = GetVal(40),
                        Omega6G = GetVal(41),
                        CholesterolMg = GetVal(17),
                        CalciumMg = GetVal(15),
                        IronMg = GetVal(56),
                        SodiumMg = GetVal(71),
                        PotassiumMg = GetVal(66),
                        MagnesiumMg = GetVal(58),
                        PhosphorusMg = GetVal(65),
                        ZincMg = GetVal(89),
                        SeleniumUg = GetVal(70),
                        IodideUg = GetVal(55),
                        CopperMg = GetVal(18),
                        ManganeseUg = GetVal(60),
                        VitaminCMg = GetVal(85),
                        VitaminD = GetVal(86),
                        VitaminB12Ug = GetVal(83),
                        VitaminB6Mg = GetVal(84),
                        FolateUg = GetVal(49),
                        DietaryFolateEqUg = GetVal(20),
                        ThiaminMg = GetVal(77),
                        RiboflavinMg = GetVal(69),
                        NiacinMg = GetVal(63),
                        RetinolUg = GetVal(68),
                        VitaminAUg = GetVal(81),
                        VitaminEMg = GetVal(87),
                        BetaCaroteneUg = GetVal(11),
                        CaffeineM = GetVal(14),
                    });
                }
            }

            // --- Seed Portions from CSM_FT.xlsx ---
            var csmPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "CSM_FT.xlsx");
            if (File.Exists(csmPath))
            {
                using var package = new ExcelPackage(new FileInfo(csmPath));
                var ws = package.Workbook.Worksheets[0];
                int totalRows = ws.Dimension.Rows;
                var foodIds = foodItems.Select(f => f.FoodId).ToHashSet();

                for (int row = 3; row <= totalRows; row++) // Row 1 = copyright, Row 2 = headers
                {
                    var foodId = ws.Cells[row, 1].Text?.Trim();
                    if (string.IsNullOrEmpty(foodId) || !foodIds.Contains(foodId)) continue;

                    var weightText = ws.Cells[row, 4].Text?.Trim();
                    if (!double.TryParse(weightText, out var weight)) continue;

                    double? density = double.TryParse(ws.Cells[row, 5].Text?.Trim(), out var d) ? d : null;

                    portions.Add(new FoodPortion
                    {
                        FoodId = foodId,
                        MeasureDescription = ws.Cells[row, 3].Text?.Trim() ?? string.Empty,
                        WeightGrams = weight,
                        Density = density
                    });
                }
            }

            // Bulk insert
            await context.FoodItems.AddRangeAsync(foodItems);
            await context.SaveChangesAsync();

            await context.FoodPortions.AddRangeAsync(portions);
            await context.SaveChangesAsync();

            Console.WriteLine($"Seeded {foodItems.Count} food items and {portions.Count} portions.");
        }
    }
}