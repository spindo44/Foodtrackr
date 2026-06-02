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
    public class FoodLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FoodLogController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpPost]
        public async Task<IActionResult> LogFood([FromBody] LogFoodDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

           
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == dto.PatientId && p.UserId == userId);

            if (patient == null)
                return NotFound("Patient not found.");

            FoodLogEntry entry;

            if (dto.IsCustom)
            {
                entry = new FoodLogEntry
                {
                    PatientId = dto.PatientId,
                    FoodId = $"CUSTOM_{Guid.NewGuid():N}",
                    FoodName = dto.CustomFoodName ?? "Custom Food",
                    PortionWeightGrams = dto.PortionWeightGrams,
                    MealType = dto.MealType,
                    LoggedAt = dto.LoggedAt ?? DateTime.UtcNow,
                    IsCustom = true,
                    UserId = userId!
                };
            }
            else
            {
                var food = await _context.FoodItems
                    .FirstOrDefaultAsync(f => f.FoodId == dto.FoodId);

                if (food == null)
                    return NotFound("Food item not found.");

                entry = new FoodLogEntry
                {
                    PatientId = dto.PatientId,
                    FoodId = food.FoodId,
                    FoodName = food.FoodName,
                    PortionWeightGrams = dto.PortionWeightGrams,
                    MealType = dto.MealType,
                    LoggedAt = dto.LoggedAt ?? DateTime.UtcNow,
                    IsCustom = false,
                    UserId = userId!,
                    EnergyKcalPer100g = dto.EnergyKcalPer100g,
                    ProteinPer100g = dto.ProteinPer100g,
                    CarbsPer100g = dto.CarbsPer100g,
                    FatPer100g = dto.FatPer100g
                };
            }

            _context.FoodLogEntries.Add(entry);
            await _context.SaveChangesAsync();

            return Ok(entry);
        }


        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId, [FromQuery] string? date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId && p.UserId == userId);
            if (patient == null) return NotFound();

            var entries = await _context.FoodLogEntries
                .Where(e => e.PatientId == patientId && e.UserId == userId)
                .OrderByDescending(e => e.LoggedAt)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date.Trim(), out var parsedDate))
                entries = entries.Where(e => e.LoggedAt.Date == parsedDate.Date).ToList();

            return Ok(entries);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entry = await _context.FoodLogEntries
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (entry == null)
                return NotFound();

            _context.FoodLogEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class LogFoodDto
    {
        public int PatientId { get; set; }
        public string? FoodId { get; set; }
        public double PortionWeightGrams { get; set; }
        public string MealType { get; set; } = string.Empty; // Breakfast/Lunch/Dinner/Snack
        public DateTime? LoggedAt { get; set; }
        public bool IsCustom { get; set; } = false;
        public string? CustomFoodName { get; set; }

        public double? EnergyKcalPer100g { get; set; }
        public double? ProteinPer100g { get; set; }
        public double? CarbsPer100g { get; set; }
        public double? FatPer100g { get; set; }
    }
}