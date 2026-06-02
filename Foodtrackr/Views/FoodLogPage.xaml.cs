using Foodtrackr.Models;
using Foodtrackr.Services;

namespace Foodtrackr.Views
{
    public partial class FoodLogPage : ContentPage
    {
        private readonly ApiService _api = new();
        private int _patientId;
        private string _patientName = string.Empty;
        private DateTime _selectedDate = DateTime.Today;

        public FoodLogPage()
        {
            InitializeComponent();
        }

        public void Init(int patientId, string patientName)
        {
            _patientId = patientId;
            _patientName = patientName;
            PatientNameLabel.Text = patientName;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadEntriesAsync();
        }

        private async Task LoadEntriesAsync()
        {
            
            try
            {
                var entries = await _api.GetLogEntriesAsync(_patientId, _selectedDate);
                FoodLogCollection.ItemsSource = entries;
                UpdateTotals(entries);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not load entries: {ex.Message}", "OK");
            }
        }

        private void UpdateTotals(List<FoodLogEntry> entries)
        {
            double totalCalories = entries.Sum(e => e.Calories);
            double totalProtein = entries.Sum(e => e.Protein);
            double totalCarbs = entries.Sum(e => e.Carbs);
            double totalFat = entries.Sum(e => e.Fat);

            TotalCaloriesLabel.Text = $"{totalCalories:0}";
            TotalProteinLabel.Text = $"{totalProtein:0.#}g";
            TotalCarbsLabel.Text = $"{totalCarbs:0.#}g";
            TotalFatLabel.Text = $"{totalFat:0.#}g";
        }

        private async void OnAddFoodClicked(object sender, EventArgs e)
        {
            var searchPage = new FoodSearchPage(_patientId, _patientName);
            await Navigation.PushAsync(searchPage);
        }

        private async void OnDateChanged(object sender, DateChangedEventArgs e)
        {
            _selectedDate = e.NewDate;
            await LoadEntriesAsync();
        }

        private async void OnDeleteEntryTapped(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int entryId)
            {
                bool confirm = await DisplayAlert("Delete", "Remove this entry?", "Yes", "No");
                if (!confirm) return;
                await _api.DeleteLogEntryAsync(entryId);
                await LoadEntriesAsync();
            }
        }

        private async void OnViewNutritionClicked(object sender, EventArgs e)
        {
            var page = new NutritionPage(_patientId, _patientName, _selectedDate);
            await Navigation.PushAsync(page);
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnThemeToggleClicked(object sender, EventArgs e)
        {
            bool isDark = !Foodtrackr.Helpers.ThemeHelper.IsDarkMode();
            Foodtrackr.Helpers.ThemeHelper.SetTheme(isDark);
            ThemeToggleBtn.Text = isDark ? "☀️" : "🌙";
        }

        private async void OnEntryTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not FoodLogEntry entry) return;

            await DisplayAlert(entry.FoodName,
                $"Portion: {entry.PortionGrams:0}g\n" +
                $"Meal: {entry.MealType}\n" +
                $"Time: {entry.LoggedAt:h:mm tt}\n\n" +
                $"Calories: {entry.Calories:0} kcal\n" +
                $"Protein: {entry.Protein:0.#}g\n" +
                $"Carbs: {entry.Carbs:0.#}g\n" +
                $"Fat: {entry.Fat:0.#}g",
                "OK");
        }

        private async void OnViewReportClicked(object sender, EventArgs e)
        {
            var page = new ReportPage(_patientId, _patientName);
            await Navigation.PushAsync(page);
        }
    }
}