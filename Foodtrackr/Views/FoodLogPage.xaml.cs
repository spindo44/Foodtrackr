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
            await DisplayAlert("Debug", $"Loading for patientId: {_patientId}, date: {_selectedDate:yyyy-MM-dd}", "OK");
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

            TotalCaloriesLabel.Text = $"{totalCalories:0} kcal";
            TotalProteinLabel.Text = $"{totalProtein:0.0}g protein";
            TotalCarbsLabel.Text = $"{totalCarbs:0.0}g carbs";
            TotalFatLabel.Text = $"{totalFat:0.0}g fat";
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
    }
}