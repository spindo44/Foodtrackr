using Foodtrackr.Helpers;
using Foodtrackr.Models;
using Foodtrackr.Services;

namespace Foodtrackr.Views
{
    public partial class PatientListPage : ContentPage
    {
        private readonly ApiService _api = new();

        public PatientListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ThemeToggleBtn.Text = ThemeHelper.IsDarkMode() ? "☀️" : "🌙";
            await LoadPatientsAsync();
        }

        private void OnThemeToggleClicked(object sender, EventArgs e)
        {
            bool isDark = !ThemeHelper.IsDarkMode();
            ThemeHelper.SetTheme(isDark);
            ThemeToggleBtn.Text = isDark ? "☀️" : "🌙";
        }

        private async Task LoadPatientsAsync()
        {
            try
            {
                var patients = await _api.GetPatientsAsync();
                PatientCollection.ItemsSource = patients;

                if (patients.Count == 0)
                    await DisplayAlert("Debug", "API returned 0 patients", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnAddPatientClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatePatientPage");
        }

        private async void OnPatientTapped(object sender, EventArgs e)
        {
            Patient? patient = null;

            if (sender is Border border && border.BindingContext is Patient p)
                patient = p;
            else if (sender is Grid grid && grid.BindingContext is Patient p2)
                patient = p2;

            if (patient == null) return;

            var page = new FoodLogPage();
            page.Init(patient.Id, patient.Name);
            await Navigation.PushAsync(page);
        }
    }
}