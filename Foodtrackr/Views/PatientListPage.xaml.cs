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
            LoadPatients();
        }

        private void LoadPatients()
        {
            // Mock data for Sprint 1 - will connect to API in Sprint 2
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
            // Will navigate to patient detail in Sprint 2
            await DisplayAlert("Coming Soon", "Patient details coming in Sprint 2!", "OK");
        }
    }
}