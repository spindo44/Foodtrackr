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
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patients = await _context.Patients
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return Ok(patients);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patient patient)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            patient.UserId = userId!;
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return Ok(patient);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Patient updated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (patient == null) return NotFound();

            patient.Name = updated.Name;
            patient.Age = updated.Age;
            patient.Gender = updated.Gender;
            patient.Ethnicity = updated.Ethnicity;
            patient.WeightKg = updated.WeightKg;
            patient.HeightCm = updated.HeightCm;
            patient.IsMetric = updated.IsMetric;
            patient.ActivityLevel = updated.ActivityLevel;
            patient.MedicalConditions = updated.MedicalConditions;
            patient.DietaryRestrictions = updated.DietaryRestrictions;

            await _context.SaveChangesAsync();
            return Ok(patient);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (patient == null) return NotFound();
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}