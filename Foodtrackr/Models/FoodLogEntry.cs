using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodtrackr.Models
{
    public class FoodLogEntry
    {
        public string FoodName { get; set; } = string.Empty;
        public double PortionGrams { get; set; }
        public string MealType { get; set; } = string.Empty;
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Carbs { get; set; }
        public int Fat { get; set; }
    }
}