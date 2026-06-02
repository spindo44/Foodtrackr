namespace Foodtrackr.Api.Models
{
    public class FoodItem
    {
        public string FoodId { get; set; } = string.Empty;
        public string FoodName { get; set; } = string.Empty;

        // Energy (col 27 = kJ, col 25 = kcal)
        public double? EnergyKj { get; set; }
        public double? EnergyKcal { get; set; }

        // Macros
        public double? ProteinG { get; set; }          // col 66
        public double? FatG { get; set; }              // col 29
        public double? CarbohydrateG { get; set; }     // col 7  (Available carbohydrate, FSANZ)
        public double? FibreTotalG { get; set; }       // col 43
        public double? SugarsG { get; set; }           // col 75
        public double? StarchG { get; set; }           // col 71
        public double? AlcoholG { get; set; }          // col 2
        public double? WaterG { get; set; }            // col 87

        // Fats breakdown
        public double? FatSaturatedG { get; set; }    // col 41
        public double? FatMonoG { get; set; }         // col 37
        public double? FatPolyG { get; set; }         // col 38
        public double? FatTransG { get; set; }        // col 42
        public double? Omega3G { get; set; }          // col 39
        public double? Omega6G { get; set; }          // col 40
        public double? CholesterolMg { get; set; }   // col 16

        // Minerals
        public double? CalciumMg { get; set; }        // col 14
        public double? IronMg { get; set; }           // col 55
        public double? SodiumMg { get; set; }         // col 70
        public double? PotassiumMg { get; set; }      // col 65
        public double? MagnesiumMg { get; set; }      // col 57
        public double? PhosphorusMg { get; set; }     // col 64
        public double? ZincMg { get; set; }           // col 88
        public double? SeleniumUg { get; set; }       // col 69
        public double? IodideUg { get; set; }         // col 54
        public double? CopperMg { get; set; }         // col 17
        public double? ManganeseUg { get; set; }      // col 59

        // Vitamins
        public double? VitaminCMg { get; set; }       // col 84
        public double? VitaminD { get; set; }         // col 85 (µg)
        public double? VitaminB12Ug { get; set; }     // col 82
        public double? VitaminB6Mg { get; set; }      // col 83
        public double? FolateUg { get; set; }         // col 48
        public double? DietaryFolateEqUg { get; set; }// col 19
        public double? ThiaminMg { get; set; }        // col 76
        public double? RiboflavinMg { get; set; }     // col 68
        public double? NiacinMg { get; set; }         // col 62
        public double? RetinolUg { get; set; }        // col 67
        public double? VitaminAUg { get; set; }       // col 80 (RAE)
        public double? VitaminEMg { get; set; }       // col 86
        public double? BetaCaroteneUg { get; set; }   // col 10
        public double? CaffeineM { get; set; }        // col 13

        public ICollection<FoodPortion> Portions { get; set; } = new List<FoodPortion>();
    }

    public class FoodPortion
    {
        public int Id { get; set; }
        public string FoodId { get; set; } = string.Empty;
        public string MeasureDescription { get; set; } = string.Empty;
        public double WeightGrams { get; set; }
        public double? Density { get; set; }
        public FoodItem? FoodItem { get; set; }
    }
}