using Foodtrackr.Api.Controllers;
using Foodtrackr.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Foodtrackr.Tests;

public class FoodLogControllerTests
{
    private const string UserA = "user-a";

    private static async Task<(AppDbContext db, int patientId)> SeedAsync()
    {
        var db = TestHelpers.NewDb();
        var patient = new Patient { Name = "Alice", UserId = UserA };
        db.Patients.Add(patient);
        db.FoodItems.Add(new FoodItem
        {
            FoodId = "A1011",
            FoodName = "Chicken stuffing",
            EnergyKcal = 139,
            ProteinG = 4.56
        });
        await db.SaveChangesAsync();
        return (db, patient.Id);
    }

    [Fact]
    public async Task LogFood_KnownFood_PersistsEntry()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            var dto = new LogFoodDto
            {
                PatientId = patientId,
                FoodId = "A1011",
                PortionWeightGrams = 150,
                MealType = "Breakfast",
                EnergyKcalPer100g = 139
            };

            var result = await new FoodLogController(db).AsUser(UserA).LogFood(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var entry = Assert.IsType<FoodLogEntry>(ok.Value);
            Assert.Equal("Chicken stuffing", entry.FoodName);
            Assert.Equal(150, entry.PortionWeightGrams);
            Assert.Equal(UserA, entry.UserId);
            Assert.False(entry.IsCustom);
            Assert.Single(db.FoodLogEntries);
        }
    }

    [Fact]
    public async Task LogFood_UnknownPatient_ReturnsNotFound()
    {
        var (db, _) = await SeedAsync();
        using (db)
        {
            var dto = new LogFoodDto { PatientId = 9999, FoodId = "A1011", PortionWeightGrams = 100 };

            var result = await new FoodLogController(db).AsUser(UserA).LogFood(dto);

            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Empty(db.FoodLogEntries);
        }
    }

    [Fact]
    public async Task LogFood_UnknownFood_ReturnsNotFound()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            var dto = new LogFoodDto { PatientId = patientId, FoodId = "DOES_NOT_EXIST", PortionWeightGrams = 100 };

            var result = await new FoodLogController(db).AsUser(UserA).LogFood(dto);

            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Empty(db.FoodLogEntries);
        }
    }

    [Fact]
    public async Task LogFood_Custom_StoresCustomNameAndGeneratedFoodId()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            var dto = new LogFoodDto
            {
                PatientId = patientId,
                IsCustom = true,
                CustomFoodName = "Grandma's pie",
                PortionWeightGrams = 200,
                MealType = "Dinner"
            };

            var result = await new FoodLogController(db).AsUser(UserA).LogFood(dto);

            var entry = Assert.IsType<FoodLogEntry>(Assert.IsType<OkObjectResult>(result).Value);
            Assert.True(entry.IsCustom);
            Assert.Equal("Grandma's pie", entry.FoodName);
            Assert.StartsWith("CUSTOM_", entry.FoodId);
        }
    }

    [Fact]
    public async Task GetByPatient_ReturnsEntriesNewestFirst()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            db.FoodLogEntries.AddRange(
                new FoodLogEntry { PatientId = patientId, UserId = UserA, FoodId = "A1011", FoodName = "Older", LoggedAt = new DateTime(2026, 6, 10, 8, 0, 0, DateTimeKind.Utc) },
                new FoodLogEntry { PatientId = patientId, UserId = UserA, FoodId = "A1011", FoodName = "Newer", LoggedAt = new DateTime(2026, 6, 12, 8, 0, 0, DateTimeKind.Utc) });
            await db.SaveChangesAsync();

            var result = await new FoodLogController(db).AsUser(UserA).GetByPatient(patientId, date: null);

            var entries = Assert.IsAssignableFrom<IEnumerable<FoodLogEntry>>(Assert.IsType<OkObjectResult>(result).Value).ToList();
            Assert.Equal(new[] { "Newer", "Older" }, entries.Select(e => e.FoodName));
        }
    }

    [Fact]
    public async Task GetByPatient_FiltersByDate()
    {
        var (db, patientId) = await SeedAsync();
        using (db)
        {
            db.FoodLogEntries.AddRange(
                new FoodLogEntry { PatientId = patientId, UserId = UserA, FoodId = "A1011", FoodName = "On the 10th", LoggedAt = new DateTime(2026, 6, 10, 8, 0, 0, DateTimeKind.Utc) },
                new FoodLogEntry { PatientId = patientId, UserId = UserA, FoodId = "A1011", FoodName = "On the 12th", LoggedAt = new DateTime(2026, 6, 12, 8, 0, 0, DateTimeKind.Utc) });
            await db.SaveChangesAsync();

            var result = await new FoodLogController(db).AsUser(UserA).GetByPatient(patientId, date: "2026-06-12");

            var entries = Assert.IsAssignableFrom<IEnumerable<FoodLogEntry>>(Assert.IsType<OkObjectResult>(result).Value).ToList();
            Assert.Equal("On the 12th", Assert.Single(entries).FoodName);
        }
    }

    [Fact]
    public async Task GetByPatient_UnknownPatient_ReturnsNotFound()
    {
        var (db, _) = await SeedAsync();
        using (db)
        {
            var result = await new FoodLogController(db).AsUser(UserA).GetByPatient(9999, date: null);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
