using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Foodtrackr.Api.Models;

namespace Foodtrackr.Tests;

public class ApiIntegrationTests
{
    private static readonly string Email = "it-user@test.com";
    private const string Password = "Test123!pass";

    private static async Task<HttpClient> AuthenticatedClientAsync(TestApiFactory factory)
    {
        var client = factory.CreateClient();

        var register = await client.PostAsJsonAsync("/api/auth/register",
            new { Email, Password });
        register.EnsureSuccessStatusCode();

        var token = await LoginAndGetTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static async Task<string> LoginAndGetTokenAsync(HttpClient client)
    {
        var login = await client.PostAsJsonAsync("/api/auth/login", new { Email, Password });
        login.EnsureSuccessStatusCode();
        var body = await login.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task Register_ThenLogin_ReturnsToken()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var register = await client.PostAsJsonAsync("/api/auth/register", new { Email, Password });
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);

        var token = await LoginAndGetTokenAsync(client);
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/auth/register", new { Email, Password });

        var login = await client.PostAsJsonAsync("/api/auth/login",
            new { Email, Password = "Nope123!pass" });

        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }

    [Fact]
    public async Task GetPatients_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/patient");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePatient_ThenList_ReturnsCreatedPatient()
    {
        await using var factory = new TestApiFactory();
        var client = await AuthenticatedClientAsync(factory);

        var create = await client.PostAsJsonAsync("/api/patient",
            new { Name = "Integration Alice", Age = 30, Gender = "Female", WeightKg = 65, HeightCm = 165 });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);

        var list = await client.GetFromJsonAsync<List<Patient>>("/api/patient");
        Assert.NotNull(list);
        Assert.Contains(list!, p => p.Name == "Integration Alice");
    }

    [Fact]
    public async Task LogFood_ThenRetrieve_RoundTripsOverHttp()
    {
        await using var factory = new TestApiFactory();
        factory.Seed(db => db.FoodItems.Add(new FoodItem
        {
            FoodId = "A1011",
            FoodName = "Chicken stuffing",
            EnergyKcal = 139
        }));

        var client = await AuthenticatedClientAsync(factory);

        var created = await client.PostAsJsonAsync("/api/patient",
            new { Name = "Bob", Age = 40, Gender = "Male", WeightKg = 80, HeightCm = 180 });
        var patient = await created.Content.ReadFromJsonAsync<Patient>();

        var logged = await client.PostAsJsonAsync("/api/FoodLog", new
        {
            PatientId = patient!.Id,
            FoodId = "A1011",
            PortionWeightGrams = 150,
            MealType = "Breakfast",
            IsCustom = false,
            EnergyKcalPer100g = 139
        });
        Assert.Equal(HttpStatusCode.OK, logged.StatusCode);

        var entries = await client.GetFromJsonAsync<List<FoodLogEntry>>($"/api/FoodLog/patient/{patient.Id}");
        Assert.NotNull(entries);
        var entry = Assert.Single(entries!);
        Assert.Equal("Chicken stuffing", entry.FoodName);
        Assert.Equal(150, entry.PortionWeightGrams);
    }
}
