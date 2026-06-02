namespace Foodtrackr.Models
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

        
        public double PortionGrams => PortionWeightGrams;
        public double Calories => Math.Round((EnergyKcalPer100g ?? 0) * PortionWeightGrams / 100, 0);
        public double Protein => Math.Round((ProteinPer100g ?? 0) * PortionWeightGrams / 100, 1);
        public double Carbs => Math.Round((CarbsPer100g ?? 0) * PortionWeightGrams / 100, 1);
        public double Fat => Math.Round((FatPer100g ?? 0) * PortionWeightGrams / 100, 1);

        public double? EnergyKcalPer100g { get; set; }
        public double? ProteinPer100g { get; set; }
        public double? CarbsPer100g { get; set; }
        public double? FatPer100g { get; set; }
    }

    public class FoodSearchResult
    {
        public string FoodId { get; set; } = string.Empty;
        public string FoodName { get; set; } = string.Empty;
        public double? EnergyKj { get; set; }
        public double? EnergyKcal { get; set; }
        public double? ProteinG { get; set; }
        public double? FatG { get; set; }
        public double? CarbohydrateG { get; set; }
        public double? FibreTotalG { get; set; }
        public List<PortionOption> Portions { get; set; } = new();
    }

    public class PortionOption
    {
        public int Id { get; set; }
        public string MeasureDescription { get; set; } = string.Empty;
        public double WeightGrams { get; set; }
    }
}