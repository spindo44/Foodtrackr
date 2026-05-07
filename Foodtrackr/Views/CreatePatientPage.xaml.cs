using Foodtrackr.Models;

namespace Foodtrackr.Views
{
    public partial class CreatePatientPage : ContentPage
    {
        public CreatePatientPage()
        {
            InitializeComponent();
        }

        private void OnUnitSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                WeightEntry.Placeholder = "Weight (lbs)";
                HeightEntry.Placeholder = "Height (inches)";
            }
            else
            {
                WeightEntry.Placeholder = "Weight (kg)";
                HeightEntry.Placeholder = "Height (cm)";
            }
        }

        private async void OnSavePatientClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                ErrorLabel.Text = "Please enter the patient's name.";
                ErrorLabel.IsVisible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(AgeEntry.Text) ||
                !int.TryParse(AgeEntry.Text, out int age))
            {
                ErrorLabel.Text = "Please enter a valid age.";
                ErrorLabel.IsVisible = true;
                return;
            }

            bool isMetric = !UnitSwitch.IsToggled;
            double.TryParse(WeightEntry.Text, out double weight);
            double.TryParse(HeightEntry.Text, out double height);

            var patient = new Patient
            {
                Name = NameEntry.Text,
                Age = age,
                Gender = GenderPicker.SelectedItem?.ToString() ?? string.Empty,
                Ethnicity = EthnicityEntry.Text ?? string.Empty,
                WeightKg = isMetric ? weight : weight * 0.453592,
                HeightCm = isMetric ? height : height * 2.54,
                IsMetric = isMetric,
                MedicalConditions = MedicalConditionsEditor.Text ?? string.Empty,
                DietaryRestrictions = DietaryRestrictionsEditor.Text ?? string.Empty
            };

            await DisplayAlert("Success",
                $"Patient {patient.Name} saved successfully!", "OK");
            await Shell.Current.GoToAsync("//PatientListPage");
        }
    }
}