using Foodtrackr.Api.Controllers;
using Foodtrackr.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Foodtrackr.Tests;

public class NutritionCalculationTests
{
    private const string UserA = "user-a";
    private static readonly DateTime Day = new(2026, 6, 12, 8, 0, 0, DateTimeKind.Utc);

    private static async Task<(AppDbContext db, int patientId)> SeedAsync()
    {
        var db = TestHelpers.NewDb();
        var patient = new Patient
        {
            Name = "Alice", UserId = UserA, Gender = "Female",
            Age = 30, WeightKg = 65, HeightCm = 165, ActivityLevel = "moderate"
        };
        db.Patients.Add(patient);
        db.FoodItems.Add(new FoodItem
        {
            FoodId = "A1011",
            FoodName = "Chicken stuffing",
            EnergyKcal = 139,
            ProteinG = 4.56,
            FatG = 2,
            CarbohydrateG = 25.8
        });
        await db.SaveChangesAsync();
        return (db, patient.Id);
    }

    private static void Log(AppDbContext db, int patientId, string foodId, double grams, bool custom = false) =>
        db.FoodLogEntries.Add(new FoodLogEntry
        {
            PatientId = patientId,
            UserId = UserA,
            FoodId = foodId,
            FoodName = "x",
            PortionWeightGrams = grams,
            IsCustom = custom,
            LoggedAt = Day
        });

    private static async Task<NutritionSummaryDto> SummaryAsync(AppDbContext db, int patientId)
    {
        var result = await new NutritionController(db).AsUser(UserA).GetDailySummary(patientId, Day);
        return Assert.IsType<NutritionSummaryDto>(Assert.IsType<OkObjectResult>(result).Value);
    }

    [Fact]
    public async Task Totals_ScalePer100gByPortionWeight()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            Log(db, patientId, "A1011", grams: 200);
            await db.SaveChangesAsync();

            var totals = (await SummaryAsync(db, patientId)).Totals;

            Assert.Equal(278, totals.EnergyKcal);
            Assert.Equal(9.12, totals.ProteinG);
            Assert.Equal(4, totals.FatG);
            Assert.Equal(51.6, totals.CarbohydrateG);
        }
    }

    [Fact]
    public async Task Totals_SumAcrossMultipleEntries()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            Log(db, patientId, "A1011", grams: 100);
            Log(db, patientId, "A1011", grams: 50);
            await db.SaveChangesAsync();

            var summary = await SummaryAsync(db, patientId);

            Assert.Equal(2, summary.LogCount);
            Assert.Equal(208.5, summary.Totals.EnergyKcal);
        }
    }

    [Fact]
    public async Task Totals_HalfPortion_HalvesValues()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            Log(db, patientId, "A1011", grams: 50);
            await db.SaveChangesAsync();

            Assert.Equal(69.5, (await SummaryAsync(db, patientId)).Totals.EnergyKcal);
        }
    }

    [Fact]
    public async Task Totals_ExcludeOtherDays()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            db.FoodLogEntries.Add(new FoodLogEntry
            {
                PatientId = patientId, UserId = UserA, FoodId = "A1011", FoodName = "x",
                PortionWeightGrams = 100, LoggedAt = Day.AddDays(-1)
            });
            await db.SaveChangesAsync();

            var summary = await SummaryAsync(db, patientId);
            Assert.Equal(0, summary.LogCount);
            Assert.Equal(0, summary.Totals.EnergyKcal);
        }
    }
}
