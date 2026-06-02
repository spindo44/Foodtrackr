using Foodtrackr.Models;
using Foodtrackr.Services;

namespace Foodtrackr.Views
{
    public partial class FoodSearchPage : ContentPage
    {
        private readonly ApiService _api = new();
        private readonly int _patientId;
        private readonly string _patientName;
        private string _selectedMealType = "Breakfast";
        private CancellationTokenSource? _searchCts;

        public FoodSearchPage(int patientId, string patientName)
        {
            InitializeComponent();
            _patientId = patientId;
            _patientName = patientName;
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            await Task.Delay(400);
            if (token.IsCancellationRequested) return;

            var query = e.NewTextValue?.Trim();
            if (string.IsNullOrEmpty(query) || query.Length < 2)
            {
                ResultsCollection.ItemsSource = null;
                return;
            }

            try
            {
                var results = await _api.SearchFoodAsync(query);
                if (!token.IsCancellationRequested)
                    ResultsCollection.ItemsSource = results;
            }
            catch
            {
                // silently fail on search
            }
        }

        private void OnMealTypeSelected(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            _selectedMealType = btn.Text;

            foreach (var b in new[] { BtnBreakfast, BtnLunch, BtnDinner, BtnSnack })
            {
                b.BackgroundColor = Color.FromArgb("#E8F4E8");
                b.TextColor = Color.FromArgb("#3D8B5E");
            }
            btn.BackgroundColor = Color.FromArgb("#3D8B5E");
            btn.TextColor = Colors.White;
        }

        private async void OnAddFoodClicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.CommandParameter is not FoodSearchResult food) return;

            double portionGrams;

            if (food.Portions != null && food.Portions.Count > 0)
            {
                var options = food.Portions.Select(p => $"{p.MeasureDescription} ({p.WeightGrams}g)").ToArray();
                var customOption = "Enter custom amount (g)";
                var allOptions = options.Append(customOption).ToArray();

                var pick = await DisplayActionSheet("Select portion size", "Cancel", null, allOptions);
                if (pick == null || pick == "Cancel") return;

                if (pick == customOption)
                {
                    portionGrams = await PromptGrams();
                    if (portionGrams <= 0) return;
                }
                else
                {
                    var selected = food.Portions.First(p =>
                        pick.StartsWith(p.MeasureDescription));
                    portionGrams = selected.WeightGrams;
                }
            }
            else
            {
                portionGrams = await PromptGrams();
                if (portionGrams <= 0) return;
            }

            try
            {
                var entry = await _api.LogFoodAsync(
    _patientId, food.FoodId, food.FoodName,
    portionGrams, _selectedMealType,
    food.EnergyKcal, food.ProteinG, food.CarbohydrateG, food.FatG);

                if (entry != null)
                {
                    await DisplayAlert("Success", $"{food.FoodName} logged!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", $"Could not log food entry.\nFoodId: {food.FoodId}\nPatientId: {_patientId}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnAddCustomFoodClicked(object sender, EventArgs e)
        {
            var name = await DisplayPromptAsync("Custom Food", "Food name:", "OK", "Cancel");
            if (string.IsNullOrWhiteSpace(name)) return;

            var kcalStr = await DisplayPromptAsync("Custom Food", "Calories per 100g:", "OK", "Cancel", keyboard: Keyboard.Numeric);
            if (!double.TryParse(kcalStr, out double kcal)) return;

            var proteinStr = await DisplayPromptAsync("Custom Food", "Protein per 100g (g):", "OK", "Cancel", keyboard: Keyboard.Numeric);
            double.TryParse(proteinStr, out double protein);

            var carbsStr = await DisplayPromptAsync("Custom Food", "Carbs per 100g (g):", "OK", "Cancel", keyboard: Keyboard.Numeric);
            double.TryParse(carbsStr, out double carbs);

            var fatStr = await DisplayPromptAsync("Custom Food", "Fat per 100g (g):", "OK", "Cancel", keyboard: Keyboard.Numeric);
            double.TryParse(fatStr, out double fat);

            var portionGrams = await PromptGrams();
            if (portionGrams <= 0) return;

            try
            {
                var entry = await _api.LogCustomFoodAsync(
                    _patientId, name, portionGrams, _selectedMealType,
                    kcal, protein, carbs, fat);

                if (entry != null)
                {
                    await DisplayAlert("Success", $"{name} logged!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Could not log custom food.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async Task<double> PromptGrams()
        {
            var input = await DisplayPromptAsync("Portion Size",
                "Enter amount in grams:", "OK", "Cancel",
                placeholder: "e.g. 150", keyboard: Keyboard.Numeric);
            return double.TryParse(input, out var g) ? g : 0;
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}