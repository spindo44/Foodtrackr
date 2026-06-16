using Foodtrackr.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Foodtrackr.Services
{
    public class ApiService
    {
        private const string BaseUrl = "https://foodtrackr.onrender.com";
        private static string? _token;
        private static readonly HttpClient _http;

        static ApiService()
        {
            _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public static void SetToken(string token)
        {
            _token = token;
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        private static void AttachToken()
        {
            if (_token != null)
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        }

        public async Task<List<Patient>> GetPatientsAsync()
        {
            AttachToken();
            var response = await _http.GetAsync("/api/patient");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status: {response.StatusCode} — {error}");
            }
            return await response.Content.ReadFromJsonAsync<List<Patient>>() ?? new();
        }

        public async Task<Patient?> CreatePatientAsync(Patient patient)
        {
            AttachToken();
            var response = await _http.PostAsJsonAsync("/api/patient", patient);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<Patient>();
        }

        public async Task<List<FoodSearchResult>> SearchFoodAsync(string query)
        {
            AttachToken();
            var response = await _http.GetAsync($"/api/Food/search?q={Uri.EscapeDataString(query)}&limit=20");
            if (!response.IsSuccessStatusCode) return new();
            return await response.Content.ReadFromJsonAsync<List<FoodSearchResult>>() ?? new();
        }

        public async Task<FoodLogEntry?> LogFoodAsync(int patientId, string foodId, string foodName,
            double portionGrams, string mealType,
            double? kcalPer100g, double? proteinPer100g, double? carbsPer100g, double? fatPer100g)
        {
            AttachToken();
            var payload = new
            {
                PatientId = patientId,
                FoodId = foodId,
                PortionWeightGrams = portionGrams,
                MealType = mealType,
                LoggedAt = DateTime.UtcNow,
                IsCustom = false,
                EnergyKcalPer100g = kcalPer100g,
                ProteinPer100g = proteinPer100g,
                CarbsPer100g = carbsPer100g,
                FatPer100g = fatPer100g
            };
            System.Diagnostics.Debug.WriteLine($"[ApiService] POST {BaseUrl}/api/FoodLog token={(_token != null)}");
            var response = await _http.PostAsJsonAsync("/api/FoodLog", payload);
            System.Diagnostics.Debug.WriteLine($"[ApiService] /api/FoodLog -> {(int)response.StatusCode} {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status {response.StatusCode}: {err}");
            }

            var raw = await response.Content.ReadFromJsonAsync<JsonElement>();
            return new FoodLogEntry
            {
                Id = raw.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0,
                PatientId = patientId,
                FoodId = foodId,
                FoodName = foodName,
                PortionWeightGrams = portionGrams,
                MealType = mealType,
                LoggedAt = DateTime.Now,
                EnergyKcalPer100g = kcalPer100g,
                ProteinPer100g = proteinPer100g,
                CarbsPer100g = carbsPer100g,
                FatPer100g = fatPer100g
            };
        }

        public async Task<FoodLogEntry?> LogCustomFoodAsync(int patientId, string foodName,
            double portionGrams, string mealType,
            double? kcalPer100g, double? proteinPer100g, double? carbsPer100g, double? fatPer100g)
        {
            AttachToken();
            var payload = new
            {
                PatientId = patientId,
                FoodId = "CUSTOM",
                CustomFoodName = foodName,
                PortionWeightGrams = portionGrams,
                MealType = mealType,
                LoggedAt = DateTime.UtcNow,
                IsCustom = true,
                EnergyKcalPer100g = kcalPer100g,
                ProteinPer100g = proteinPer100g,
                CarbsPer100g = carbsPer100g,
                FatPer100g = fatPer100g
            };
            var response = await _http.PostAsJsonAsync("/api/FoodLog", payload);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status {response.StatusCode}: {err}");
            }

            var raw = await response.Content.ReadFromJsonAsync<JsonElement>();
            return new FoodLogEntry
            {
                Id = raw.GetProperty("id").GetInt32(),
                PatientId = patientId,
                FoodId = "CUSTOM",
                FoodName = foodName,
                PortionWeightGrams = portionGrams,
                MealType = mealType,
                LoggedAt = DateTime.Now,
                EnergyKcalPer100g = kcalPer100g,
                ProteinPer100g = proteinPer100g,
                CarbsPer100g = carbsPer100g,
                FatPer100g = fatPer100g
            };
        }

        public async Task<List<FoodLogEntry>> GetLogEntriesAsync(int patientId, DateTime date)
        {
            AttachToken();

            var response = await _http.GetAsync($"/api/FoodLog/patient/{patientId}");
            if (!response.IsSuccessStatusCode) return new();

            var all = await response.Content.ReadFromJsonAsync<List<FoodLogEntry>>() ?? new();
            var localDay = date.Date;
            return all
                .Where(e => e.LoggedAt.ToLocalTime().Date == localDay)
                .ToList();
        }

        public async Task DeleteLogEntryAsync(int entryId)
        {
            AttachToken();
            await _http.DeleteAsync($"/api/FoodLog/{entryId}");
        }
    }
}