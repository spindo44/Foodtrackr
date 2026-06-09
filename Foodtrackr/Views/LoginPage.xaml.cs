using Foodtrackr.Helpers;


namespace Foodtrackr.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ThemeToggleBtn.Text = ThemeHelper.IsDarkMode() ? "☀" : "🌙";
        }

        private void OnThemeToggleClicked(object sender, EventArgs e)
        {
            bool isDark = !ThemeHelper.IsDarkMode();
            ThemeHelper.SetTheme(isDark);
            ThemeToggleBtn.Text = isDark ? "☀" : "🌙";
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ErrorLabel.Text = "Please enter your email and password.";
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
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
                };
                var http = new HttpClient(handler) { BaseAddress = new Uri("https://foodtrackr.onrender.com") };
                var response = await http.PostAsync("/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var doc = System.Text.Json.JsonDocument.Parse(body);
                    var token = doc.RootElement.GetProperty("token").GetString()!;
                    Foodtrackr.Services.ApiService.SetToken(token);
                    await DisplayAlert("Debug", $"Token: {token.Substring(0, 20)}...", "OK");
                    await Shell.Current.GoToAsync("//PatientListPage");
                }
                else
                {
                    ErrorLabel.Text = "Invalid email or password.";
                    ErrorLabel.IsVisible = true;
                }
            }
            catch
            {
                ErrorLabel.Text = "Could not connect to server.";
                ErrorLabel.IsVisible = true;
            }
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}