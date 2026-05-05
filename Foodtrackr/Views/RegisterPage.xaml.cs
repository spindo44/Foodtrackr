namespace Foodtrackr.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public RegisterPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                ErrorLabel.Text = "Passwords do not match.";
                ErrorLabel.IsVisible = true;
                return;
            }

            var payload = new
            {
                Email = EmailEntry.Text,
                Password = PasswordEntry.Text
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/api/auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Success", "Account created! Please log in.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    ErrorLabel.Text = "Registration failed. Try a stronger password.";
                    ErrorLabel.IsVisible = true;
                }
            }
            catch
            {
                ErrorLabel.Text = "Could not connect to server.";
                ErrorLabel.IsVisible = true;
            }
        }
    }
}