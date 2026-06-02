using Foodtrackr.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Foodtrackr.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NutritionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NutritionController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/nutrition/summary/{patientId}?date=2025-06-01
        [HttpGet("summary/{patientId}")]
        public async Task<IActionResult> GetDailySummary(int patientId, [FromQuery] DateTime? date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId && p.UserId == userId);

            if (patient == null)
                return NotFound("Patient not found.");

            var day = (date ?? DateTime.UtcNow).Date;

            var logs = await _context.FoodLogEntries
                .Where(e => e.PatientId == patientId && e.LoggedAt.Date == day && !e.IsCustom)
                .Join(_context.FoodItems,
                    log => log.FoodId,
                    food => food.FoodId,
                    (log, food) => new { log, food })
                .ToListAsync();

            // All FOODfiles values are per 100g — scale by portion weight
            double Scale(double? per100g, double portionG) =>
                per100g.HasValue ? Math.Round(per100g.Value * portionG / 100.0, 2) : 0;

            var totals = new NutrientTotalsDto();
            foreach (var item in logs)
            {
                var g = item.log.PortionWeightGrams;
                totals.EnergyKj += Scale(item.food.EnergyKj, g);
                totals.EnergyKcal += Scale(item.food.EnergyKcal, g);
                totals.ProteinG += Scale(item.food.ProteinG, g);
                totals.FatG += Scale(item.food.FatG, g);
                totals.CarbohydrateG += Scale(item.food.CarbohydrateG, g);
                totals.FibreG += Scale(item.food.FibreTotalG, g);
                totals.SodiumMg += Scale(item.food.SodiumMg, g);
                totals.CalciumMg += Scale(item.food.CalciumMg, g);
                totals.IronMg += Scale(item.food.IronMg, g);
                totals.VitaminCMg += Scale(item.food.VitaminCMg, g);
                totals.VitaminD += Scale(item.food.VitaminD, g);
                totals.FolateUg += Scale(item.food.FolateUg, g);
                totals.VitaminB12Ug += Scale(item.food.VitaminB12Ug, g);
                totals.ZincMg += Scale(item.food.ZincMg, g);
                totals.MagnesiumMg += Scale(item.food.MagnesiumMg, g);
                totals.PotassiumMg += Scale(item.food.PotassiumMg, g);
            }

            var rdi = RdiCalculator.GetRdi(patient);
            var comparison = RdiCalculator.Compare(totals, rdi);

            return Ok(new NutritionSummaryDto
            {
                PatientId = patientId,
                Date = day,
                Totals = totals,
                Rdi = rdi,
                Comparison = comparison,
                LogCount = logs.Count
            });
        }
    }

    // --- DTOs ---

    public class NutrientTotalsDto
    {
        public double EnergyKj { get; set; }
        public double EnergyKcal { get; set; }
        public double ProteinG { get; set; }
        public double FatG { get; set; }
        public double CarbohydrateG { get; set; }
        public double FibreG { get; set; }
        public double SodiumMg { get; set; }
        public double CalciumMg { get; set; }
        public double IronMg { get; set; }
        public double VitaminCMg { get; set; }
        public double VitaminD { get; set; }
        public double FolateUg { get; set; }
        public double VitaminB12Ug { get; set; }
        public double ZincMg { get; set; }
        public double MagnesiumMg { get; set; }
        public double PotassiumMg { get; set; }
    }

    public class RdiValuesDto
    {
        public double EnergyKj { get; set; }
        public double ProteinG { get; set; }
        public double FibreG { get; set; }
        public double CalciumMg { get; set; }
        public double IronMg { get; set; }
        public double VitaminCMg { get; set; }
        public double VitaminDUg { get; set; }
        public double FolateUg { get; set; }
        public double VitaminB12Ug { get; set; }
        public double ZincMg { get; set; }
        public double SodiumMg { get; set; }
        public double MagnesiumMg { get; set; }
        public double PotassiumMg { get; set; }
    }

    public class NutrientComparisonDto
    {
        public string Nutrient { get; set; } = string.Empty;
        public double Actual { get; set; }
        public double Rdi { get; set; }
        public double PercentOfRdi { get; set; }
        public string Status { get; set; } = string.Empty; // "OK", "Deficient", "Excess"
    }

    public class NutritionSummaryDto
    {
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public int LogCount { get; set; }
        public NutrientTotalsDto Totals { get; set; } = new();
        public RdiValuesDto Rdi { get; set; } = new();
        public List<NutrientComparisonDto> Comparison { get; set; } = new();
    }

    // --- RDI Calculator ---
    // Based on NZ Ministry of Health Nutrient Reference Values
    public static class RdiCalculator
    {
        public static RdiValuesDto GetRdi(Patient patient)
        {
            bool isMale = patient.Gender.ToLower() is "male" or "m";
            int age = patient.Age;

            // Activity multiplier for energy
            double activityMultiplier = patient.ActivityLevel?.ToLower() switch
            {
                "sedentary" => 1.2,
                "light" => 1.375,
                "moderate" => 1.55,
                "active" or "very active" => 1.725,
                _ => 1.4
            };

            // Base energy (Mifflin-St Jeor → kJ)
            double bmrKcal = isMale
                ? (10 * patient.WeightKg) + (6.25 * patient.HeightCm) - (5 * age) + 5
                : (10 * patient.WeightKg) + (6.25 * patient.HeightCm) - (5 * age) - 161;
            double energyKj = Math.Round(bmrKcal * activityMultiplier * 4.184, 0);

            return new RdiValuesDto
            {
                EnergyKj = energyKj,
                ProteinG = isMale ? 64 : 46,
                FibreG = isMale ? 30 : 25,
                CalciumMg = age < 50 ? 1000 : 1300,
                IronMg = isMale ? 8 : (age < 50 ? 18 : 8),
                VitaminCMg = isMale ? 45 : 45,
                VitaminDUg = age < 70 ? 15 : 20,
                FolateUg = 400,
                VitaminB12Ug = 2.4,
                ZincMg = isMale ? 14 : 8,
                SodiumMg = 2000, // Upper limit rather than RDI
                MagnesiumMg = isMale ? (age < 31 ? 400 : 420) : (age < 31 ? 310 : 320),
                PotassiumMg = isMale ? 3800 : 2800
            };
        }

        public static List<NutrientComparisonDto> Compare(NutrientTotalsDto totals, RdiValuesDto rdi)
        {
            var results = new List<NutrientComparisonDto>();

            void Add(string name, double actual, double rdiVal, bool lowerIsBetter = false)
            {
                double pct = rdiVal > 0 ? Math.Round(actual / rdiVal * 100, 1) : 0;
                string status = lowerIsBetter
                    ? (actual > rdiVal ? "Excess" : "OK")
                    : (pct < 70 ? "Deficient" : pct > 150 ? "Excess" : "OK");

                results.Add(new NutrientComparisonDto
                {
                    Nutrient = name,
                    Actual = Math.Round(actual, 2),
                    Rdi = rdiVal,
                    PercentOfRdi = pct,
                    Status = status
                });
            }

            Add("Energy (kJ)", totals.EnergyKj, rdi.EnergyKj);
            Add("Protein (g)", totals.ProteinG, rdi.ProteinG);
            Add("Fibre (g)", totals.FibreG, rdi.FibreG);
            Add("Calcium (mg)", totals.CalciumMg, rdi.CalciumMg);
            Add("Iron (mg)", totals.IronMg, rdi.IronMg);
            Add("Vitamin C (mg)", totals.VitaminCMg, rdi.VitaminCMg);
            Add("Vitamin D (µg)", totals.VitaminD, rdi.VitaminDUg);
            Add("Folate (µg)", totals.FolateUg, rdi.FolateUg);
            Add("Vitamin B12 (µg)", totals.VitaminB12Ug, rdi.VitaminB12Ug);
            Add("Zinc (mg)", totals.ZincMg, rdi.ZincMg);
            Add("Sodium (mg)", totals.SodiumMg, rdi.SodiumMg, lowerIsBetter: true);
            Add("Magnesium (mg)", totals.MagnesiumMg, rdi.MagnesiumMg);
            Add("Potassium (mg)", totals.PotassiumMg, rdi.PotassiumMg);

            return results;
        }
    }
}