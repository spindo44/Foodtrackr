using Foodtrackr.Models;
using Foodtrackr.Services;

namespace Foodtrackr.Views
{
    public partial class ReportPage : ContentPage
    {
        private readonly ApiService _api = new();
        private readonly int _patientId;
        private readonly string _patientName;
        private int _rangeDays = 1;

        private const double RdiCalories = 2000;
        private const double RdiProtein = 50;
        private const double RdiCarbs = 275;
        private const double RdiFat = 70;

        public ReportPage(int patientId, string patientName)
        {
            InitializeComponent();
            _patientId = patientId;
            _patientName = patientName;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            PatientNameLabel.Text = _patientName;
            await LoadReportAsync();
        }

        private async void OnRangeSelected(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            foreach (var b in new[] { BtnToday, BtnWeek, BtnMonth, BtnCustom })
            {
                b.BackgroundColor = Color.FromArgb("#E8F4E8");
                b.TextColor = Color.FromArgb("#3D8B5E");
            }
            btn.BackgroundColor = Color.FromArgb("#3D8B5E");
            btn.TextColor = Colors.White;

            if (btn == BtnCustom)
            {
                var fromStr = await DisplayPromptAsync("Custom Range", "Start date (yyyy-MM-dd):", "OK", "Cancel");
                var toStr = await DisplayPromptAsync("Custom Range", "End date (yyyy-MM-dd):", "OK", "Cancel");
                if (DateTime.TryParse(fromStr, out var from) && DateTime.TryParse(toStr, out var to))
                    _rangeDays = (int)(to - from).TotalDays + 1;
                else return;
            }
            else
            {
                _rangeDays = btn == BtnToday ? 1 : btn == BtnWeek ? 7 : 30;
            }

            await LoadReportAsync();
        }

        private async Task LoadReportAsync()
        {
            try
            {
                var today = DateTime.Today;
                var allEntries = new List<FoodLogEntry>();
                var dailyTotals = new List<(DateTime Date, double Kcal, double Protein, double Carbs, double Fat)>();

                for (int i = 0; i < _rangeDays; i++)
                {
                    var date = today.AddDays(-i);
                    var entries = await _api.GetLogEntriesAsync(_patientId, date);
                    allEntries.AddRange(entries);

                    if (entries.Any())
                    {
                        dailyTotals.Add((
                            date,
                            entries.Sum(e => e.Calories),
                            entries.Sum(e => e.Protein),
                            entries.Sum(e => e.Carbs),
                            entries.Sum(e => e.Fat)
                        ));
                    }
                }

                var rangeEnd = today;
                var rangeStart = today.AddDays(-(_rangeDays - 1));
                DateRangeLabel.Text = _rangeDays == 1
                    ? today.ToString("dddd, dd MMMM yyyy")
                    : $"{rangeStart:dd MMM} – {rangeEnd:dd MMM yyyy}";

                int days = Math.Max(dailyTotals.Count, 1);
                AvgCaloriesLabel.Text = $"{dailyTotals.Average(d => d.Kcal):0}";
                AvgProteinLabel.Text = $"{dailyTotals.Average(d => d.Protein):0.#}g";
                AvgCarbsLabel.Text = $"{dailyTotals.Average(d => d.Carbs):0.#}g";
                AvgFatLabel.Text = $"{dailyTotals.Average(d => d.Fat):0.#}g";

                // Daily breakdown
                DailyBreakdownContainer.Children.Clear();
                if (!dailyTotals.Any())
                {
                    DailyBreakdownContainer.Children.Add(new Label
                    {
                        Text = "No data for this period.",
                        TextColor = Color.FromArgb("#9AAA9A"),
                        FontSize = 13
                    });
                }
                else
                {
                    foreach (var day in dailyTotals.OrderByDescending(d => d.Date))
                    {
                        var grid = new Grid
                        {
                            ColumnDefinitions = new ColumnDefinitionCollection(
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto))
                        };

                        var dateLabel = new Label
                        {
                            Text = day.Date.ToString("ddd dd MMM"),
                            FontSize = 13,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Color.FromArgb("#FFFFFF")
                        };
                        var kcalLabel = new Label
                        {
                            Text = $"{day.Kcal:0} kcal",
                            FontSize = 13,
                            TextColor = Color.FromArgb("#3D8B5E"),
                            HorizontalOptions = LayoutOptions.End
                        };
                        var macroLabel = new Label
                        {
                            Text = $"P:{day.Protein:0.#}g  C:{day.Carbs:0.#}g  F:{day.Fat:0.#}g",
                            FontSize = 11,
                            TextColor = Color.FromArgb("#9AAA9A")
                        };

                        Grid.SetColumn(dateLabel, 0);
                        Grid.SetColumn(kcalLabel, 1);
                        grid.Children.Add(dateLabel);
                        grid.Children.Add(kcalLabel);

                        var stack = new VerticalStackLayout { Spacing = 2 };
                        stack.Children.Add(grid);
                        stack.Children.Add(macroLabel);

                        var border = new Border
                        {
                            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                            BackgroundColor = Color.FromArgb("#1A2A1A"),
                            StrokeThickness = 0,
                            Padding = new Thickness(12, 8),
                            Content = stack
                        };
                        DailyBreakdownContainer.Children.Add(border);
                    }
                }

                // RDI compliance
                RdiContainer.Children.Clear();
                if (dailyTotals.Any())
                {
                    double avgKcal = dailyTotals.Average(d => d.Kcal);
                    double avgProtein = dailyTotals.Average(d => d.Protein);
                    double avgCarbs = dailyTotals.Average(d => d.Carbs);
                    double avgFat = dailyTotals.Average(d => d.Fat);

                    AddRdiRow("Calories", avgKcal, RdiCalories, "kcal");
                    AddRdiRow("Protein", avgProtein, RdiProtein, "g");
                    AddRdiRow("Carbs", avgCarbs, RdiCarbs, "g");
                    AddRdiRow("Fat", avgFat, RdiFat, "g");
                }
                else
                {
                    RdiContainer.Children.Add(new Label
                    {
                        Text = "No data for this period.",
                        TextColor = Color.FromArgb("#9AAA9A"),
                        FontSize = 13
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not load report: {ex.Message}", "OK");
            }
        }

        private void AddRdiRow(string nutrient, double actual, double target, string unit)
        {
            double pct = actual / target * 100;
            Color color = pct < 75 ? Color.FromArgb("#E6A817") :
                          pct > 110 ? Color.FromArgb("#D9534F") :
                          Color.FromArgb("#3D8B5E");

            string status = pct < 75 ? $"⚠ Low ({pct:0}%)" :
                            pct > 110 ? $"✕ Excess ({pct:0}%)" :
                            $"✓ On target ({pct:0}%)";

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto))
            };

            var nameLabel = new Label
            {
                Text = nutrient,
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#FFFFFF")
            };
            var valueLabel = new Label
            {
                Text = $"{actual:0.#}{unit}",
                FontSize = 13,
                TextColor = color,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(8, 0)
            };
            var statusLabel = new Label
            {
                Text = status,
                FontSize = 12,
                TextColor = color,
                HorizontalOptions = LayoutOptions.End
            };

            Grid.SetColumn(nameLabel, 0);
            Grid.SetColumn(valueLabel, 1);
            Grid.SetColumn(statusLabel, 2);
            grid.Children.Add(nameLabel);
            grid.Children.Add(valueLabel);
            grid.Children.Add(statusLabel);

            var progressBar = new ProgressBar
            {
                Progress = Math.Min(actual / target, 1.0),
                ProgressColor = color,
                BackgroundColor = Color.FromArgb("#222222"),
                HeightRequest = 6,
                Margin = new Thickness(0, 4, 0, 0)
            };

            var stack = new VerticalStackLayout { Spacing = 2 };
            stack.Children.Add(grid);
            stack.Children.Add(progressBar);
            RdiContainer.Children.Add(stack);
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}