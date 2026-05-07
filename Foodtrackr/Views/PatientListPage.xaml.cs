using Foodtrackr.Helpers;
using Foodtrackr.Models;

namespace Foodtrackr.Views
{
    public partial class PatientListPage : ContentPage
    {
        public PatientListPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ThemeToggleBtn.Text = ThemeHelper.IsDarkMode() ? "☀️" : "🌙";
            LoadPatients();
        }

        private void OnThemeToggleClicked(object sender, EventArgs e)
        {
            bool isDark = !ThemeHelper.IsDarkMode();
            ThemeHelper.SetTheme(isDark);
            ThemeToggleBtn.Text = isDark ? "☀️" : "🌙";
        }

        private void LoadPatients()
        {
            var patients = new List<Patient>
            {
                new Patient { Name = "John Smith", Age = 45, Gender = "Male" },
                new Patient { Name = "Sarah Johnson", Age = 32, Gender = "Female" },
                new Patient { Name = "Mike Wilson", Age = 67, Gender = "Male" }
            };
            PatientCollection.ItemsSource = patients;
        }

        private async void OnAddPatientClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatePatientPage");
        }

        private async void OnPatientTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FoodLogPage");
        }
    }
}