using Foodtrackr.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodtrackr.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FoodController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/food/search?q=chicken&limit=20
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return BadRequest("Search query must be at least 2 characters.");

            var results = await _context.FoodItems
                .Where(f => f.FoodName.ToLower().Contains(q.ToLower()))
                .Select(f => new FoodSearchResultDto
                {
                    FoodId = f.FoodId,
                    FoodName = f.FoodName,
                    EnergyKj = f.EnergyKj,
                    EnergyKcal = f.EnergyKcal,
                    ProteinG = f.ProteinG,
                    FatG = f.FatG,
                    CarbohydrateG = f.CarbohydrateG,
                    FibreTotalG = f.FibreTotalG,
                    Portions = f.Portions.Select(p => new PortionDto
                    {
                        Id = p.Id,
                        MeasureDescription = p.MeasureDescription,
                        WeightGrams = p.WeightGrams
                    }).ToList()
                })
                .Take(limit)
                .ToListAsync();

            return Ok(results);
        }

        // GET api/food/{foodId}
        [HttpGet("{foodId}")]
        public async Task<IActionResult> GetById(string foodId)
        {
            var food = await _context.FoodItems
                .Include(f => f.Portions)
                .FirstOrDefaultAsync(f => f.FoodId == foodId);

            if (food == null)
                return NotFound();

            return Ok(food);
        }
    }

    public class FoodSearchResultDto
    {
        public string FoodId { get; set; } = string.Empty;
        public string FoodName { get; set; } = string.Empty;
        public double? EnergyKj { get; set; }
        public double? EnergyKcal { get; set; }
        public double? ProteinG { get; set; }
        public double? FatG { get; set; }
        public double? CarbohydrateG { get; set; }
        public double? FibreTotalG { get; set; }
        public List<PortionDto> Portions { get; set; } = new();
    }

    public class PortionDto
    {
        public int Id { get; set; }
        public string MeasureDescription { get; set; } = string.Empty;
        public double WeightGrams { get; set; }
    }
}