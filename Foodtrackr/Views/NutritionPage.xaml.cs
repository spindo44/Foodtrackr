using Foodtrackr.Models;
using Foodtrackr.Services;
using Microsoft.Maui.Controls;

namespace Foodtrackr.Views
{
    public partial class NutritionPage : ContentPage
    {
        private readonly ApiService _api = new();
        private readonly int _patientId;
        private readonly string _patientName;
        private readonly DateTime _date;

        // NZ RDI targets (adult average — can be personalised later)
        private const double RdiCalories = 2000;
        private const double RdiProtein = 50;
        private const double RdiCarbs = 275;
        private const double RdiFat = 70;

        public NutritionPage(int patientId, string patientName, DateTime date)
        {
            InitializeComponent();
            _patientId = patientId;
            _patientName = patientName;
            _date = date;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            PatientNameLabel.Text = _patientName;
            DateLabel.Text = _date.ToString("dddd, dd MMMM yyyy");
            await LoadNutritionAsync();
        }

        private async Task LoadNutritionAsync()
        {
            try
            {
                var entries = await _api.GetLogEntriesAsync(_patientId, _date);
                RenderNutrition(entries);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not load nutrition data: {ex.Message}", "OK");
            }
        }

        private void RenderNutrition(List<FoodLogEntry> entries)
        {
            double totalCalories = entries.Sum(e => e.Calories);
            double totalProtein = entries.Sum(e => e.Protein);
            double totalCarbs = entries.Sum(e => e.Carbs);
            double totalFat = entries.Sum(e => e.Fat);

            // Energy card
            CaloriesActualLabel.Text = $"{totalCalories:0} kcal";
            CaloriesTargetLabel.Text = $"/ {RdiCalories:0} kcal";
            CaloriesBar.Progress = Math.Min(totalCalories / RdiCalories, 1.0);
            SetStatus(CaloriesStatusLabel, CaloriesBar, totalCalories, RdiCalories, "kcal");

            // Macros
            ProteinActualLabel.Text = $"{totalProtein:0.#}g";
            ProteinTargetLabel.Text = $"RDI {RdiProtein}g";
            ProteinBar.Progress = Math.Min(totalProtein / RdiProtein, 1.0);
            SetStatus(ProteinStatusLabel, ProteinBar, totalProtein, RdiProtein, "g");

            CarbsActualLabel.Text = $"{totalCarbs:0.#}g";
            CarbsTargetLabel.Text = $"RDI {RdiCarbs}g";
            CarbsBar.Progress = Math.Min(totalCarbs / RdiCarbs, 1.0);
            SetStatus(CarbsStatusLabel, CarbsBar, totalCarbs, RdiCarbs, "g");

            FatActualLabel.Text = $"{totalFat:0.#}g";
            FatTargetLabel.Text = $"RDI {RdiFat}g";
            FatBar.Progress = Math.Min(totalFat / RdiFat, 1.0);
            SetStatus(FatStatusLabel, FatBar, totalFat, RdiFat, "g");

            // Meal breakdown
            double breakfastKcal = entries.Where(e => e.MealType == "Breakfast").Sum(e => e.Calories);
            double lunchKcal = entries.Where(e => e.MealType == "Lunch").Sum(e => e.Calories);
            double dinnerKcal = entries.Where(e => e.MealType == "Dinner").Sum(e => e.Calories);
            double snackKcal = entries.Where(e => e.MealType == "Snack").Sum(e => e.Calories);
            double maxMeal = Math.Max(totalCalories, 1);

            BreakfastBar.Progress = breakfastKcal / maxMeal;
            BreakfastLabel.Text = $"{breakfastKcal:0} kcal";
            LunchBar.Progress = lunchKcal / maxMeal;
            LunchLabel.Text = $"{lunchKcal:0} kcal";
            DinnerBar.Progress = dinnerKcal / maxMeal;
            DinnerLabel.Text = $"{dinnerKcal:0} kcal";
            SnackBar.Progress = snackKcal / maxMeal;
            SnackLabel.Text = $"{snackKcal:0} kcal";

            // Flags
            FlagsContainer.Children.Clear();
            AddFlag("Calories", totalCalories, RdiCalories, "kcal");
            AddFlag("Protein", totalProtein, RdiProtein, "g");
            AddFlag("Carbs", totalCarbs, RdiCarbs, "g");
            AddFlag("Fat", totalFat, RdiFat, "g");

            if (FlagsContainer.Children.Count == 0)
            {
                FlagsContainer.Children.Add(new Label
                {
                    Text = "✓ All nutrients within target range",
                    TextColor = Color.FromArgb("#3D8B5E"),
                    FontSize = 13
                });
            }
        }

        private void SetStatus(Label label, ProgressBar bar, double actual, double target, string unit)
        {
            double pct = actual / target * 100;
            if (pct < 75)
            {
                label.Text = $"{pct:0}% of RDI";
                label.TextColor = Color.FromArgb("#E6A817");
                bar.ProgressColor = Color.FromArgb("#E6A817");
            }
            else if (pct <= 110)
            {
                label.Text = $"{pct:0}% of RDI";
                label.TextColor = Color.FromArgb("#3D8B5E");
                bar.ProgressColor = Color.FromArgb("#3D8B5E");
            }
            else
            {
                label.Text = $"{pct:0}% — over target";
                label.TextColor = Color.FromArgb("#D9534F");
                bar.ProgressColor = Color.FromArgb("#D9534F");
            }
        }

        private void AddFlag(string nutrient, double actual, double target, string unit)
        {
            double pct = actual / target * 100;
            string text;
            Color color;

            if (pct < 75)
            {
                text = $"⚠ Low {nutrient} — {actual:0.#}{unit} ({pct:0}% of RDI)";
                color = Color.FromArgb("#E6A817");
            }
            else if (pct > 110)
            {
                text = $"✕ Excess {nutrient} — {actual:0.#}{unit} ({pct:0}% of RDI)";
                color = Color.FromArgb("#D9534F");
            }
            else return;

            FlagsContainer.Children.Add(new Label
            {
                Text = text,
                TextColor = color,
                FontSize = 13
            });
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}