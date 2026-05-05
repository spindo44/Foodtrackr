namespace Foodtrackr.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public LoginPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
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
                var response = await _httpClient.PostAsync("/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
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