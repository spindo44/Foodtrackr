namespace Foodtrackr.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Ethnicity { get; set; } = string.Empty;
        public double WeightKg { get; set; }
        public double HeightCm { get; set; }
        public bool IsMetric { get; set; } = true;
        public string ActivityLevel { get; set; } = string.Empty;
        public string MedicalConditions { get; set; } = string.Empty;
        public string DietaryRestrictions { get; set; } = string.Empty;
    }
}