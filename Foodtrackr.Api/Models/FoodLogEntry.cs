namespace Foodtrackr.Api.Models
{
    public class FoodLogEntry
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string FoodId { get; set; } = string.Empty;
        public string FoodName { get; set; } = string.Empty;
        public double PortionWeightGrams { get; set; }
        public string MealType { get; set; } = string.Empty;
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
        public bool IsCustom { get; set; } = false;
        public string UserId { get; set; } = string.Empty;

        
        public double? EnergyKcalPer100g { get; set; }
        public double? ProteinPer100g { get; set; }
        public double? CarbsPer100g { get; set; }
        public double? FatPer100g { get; set; }

        public Patient? Patient { get; set; }
        public FoodItem? FoodItem { get; set; }
    }
}