using Foodtrackr.Helpers;
using Foodtrackr.Models;

namespace Foodtrackr.Views
{
    public partial class FoodLogPage : ContentPage
    {
        private List<FoodLogEntry> _entries = new();

        public FoodLogPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ThemeToggleBtn.Text = ThemeHelper.IsDarkMode() ? "☀️" : "🌙";
            LoadMockData();
        }

        private void OnThemeToggleClicked(object sender, EventArgs e)
        {
            bool isDark = !ThemeHelper.IsDarkMode();
            ThemeHelper.SetTheme(isDark);
            ThemeToggleBtn.Text = isDark ? "☀️" : "🌙";
        }

        private void LoadMockData()
        {
            // Mock data for Sprint 1 - replaced with real API in Sprint 2
            _entries = new List<FoodLogEntry>
            {
                new FoodLogEntry { FoodName = "Oats (cooked)", PortionGrams = 150, MealType = "Breakfast", Calories = 166, Protein = 6, Carbs = 28, Fat = 3 },
                new FoodLogEntry { FoodName = "Banana", PortionGrams = 120, MealType = "Breakfast", Calories = 107, Protein = 1, Carbs = 27, Fat = 0 },
                new FoodLogEntry { FoodName = "Chicken Breast", PortionGrams = 200, MealType = "Lunch", Calories = 330, Protein = 62, Carbs = 0, Fat = 7 },
                new FoodLogEntry { FoodName = "Brown Rice", PortionGrams = 180, MealType = "Lunch", Calories = 216, Protein = 5, Carbs = 45, Fat = 2 },
            };

            FoodLogCollection.ItemsSource = _entries;
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            TotalCaloriesLabel.Text = _entries.Sum(e => e.Calories).ToString();
            TotalProteinLabel.Text = _entries.Sum(e => e.Protein) + "g";
            TotalCarbsLabel.Text = _entries.Sum(e => e.Carbs) + "g";
            TotalFatLabel.Text = _entries.Sum(e => e.Fat) + "g";
        }

        private async void OnLogFoodClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Coming Soon",
                "Food search coming in Sprint 2 with NZ FOODfiles database!", "OK");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//PatientListPage");
        }
    }
}