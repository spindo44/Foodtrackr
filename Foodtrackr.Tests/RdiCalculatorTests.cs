using Foodtrackr.Api.Controllers;
using Foodtrackr.Api.Models;

namespace Foodtrackr.Tests;

public class RdiCalculatorTests
{
    private static Patient Male30() => new()
    {
        Gender = "Male",
        Age = 30,
        WeightKg = 80,
        HeightCm = 180,
        ActivityLevel = "moderate"
    };

    [Fact]
    public void GetRdi_Male_UsesMaleProteinTarget()
    {
        var rdi = RdiCalculator.GetRdi(Male30());
        Assert.Equal(64, rdi.ProteinG);
        Assert.Equal(30, rdi.FibreG);
    }

    [Fact]
    public void GetRdi_Female_UsesFemaleProteinTarget()
    {
        var patient = Male30();
        patient.Gender = "Female";

        var rdi = RdiCalculator.GetRdi(patient);
        Assert.Equal(46, rdi.ProteinG);
        Assert.Equal(25, rdi.FibreG);
    }

    [Theory]
    [InlineData(30, 1000)]
    [InlineData(49, 1000)]
    [InlineData(50, 1300)]
    [InlineData(65, 1300)]
    public void GetRdi_Calcium_DependsOnAge(int age, double expectedCalcium)
    {
        var patient = Male30();
        patient.Age = age;

        Assert.Equal(expectedCalcium, RdiCalculator.GetRdi(patient).CalciumMg);
    }

    [Fact]
    public void GetRdi_Iron_HigherForPremenopausalFemale()
    {
        var female = Male30();
        female.Gender = "Female";
        female.Age = 30;

        Assert.Equal(18, RdiCalculator.GetRdi(female).IronMg);
        Assert.Equal(8, RdiCalculator.GetRdi(Male30()).IronMg);
    }

    [Fact]
    public void GetRdi_Energy_MatchesMifflinStJeorInKilojoules()
    {
        var rdi = RdiCalculator.GetRdi(Male30());
        Assert.Equal(11544, rdi.EnergyKj);
    }

    [Fact]
    public void Compare_BelowSeventyPercent_IsDeficient()
    {
        var rdi = RdiCalculator.GetRdi(Male30());
        var totals = new NutrientTotalsDto { ProteinG = 32 };

        var protein = Find(RdiCalculator.Compare(totals, rdi), "Protein (g)");
        Assert.Equal(50, protein.PercentOfRdi);
        Assert.Equal("Deficient", protein.Status);
    }

    [Fact]
    public void Compare_WithinRange_IsOk()
    {
        var rdi = RdiCalculator.GetRdi(Male30());
        var totals = new NutrientTotalsDto { ProteinG = 64 };

        var protein = Find(RdiCalculator.Compare(totals, rdi), "Protein (g)");
        Assert.Equal("OK", protein.Status);
    }

    [Fact]
    public void Compare_Sodium_OverLimitIsExcess_LowerIsBetter()
    {
        var rdi = RdiCalculator.GetRdi(Male30());
        var over = Find(RdiCalculator.Compare(new NutrientTotalsDto { SodiumMg = 2500 }, rdi), "Sodium (mg)");
        var under = Find(RdiCalculator.Compare(new NutrientTotalsDto { SodiumMg = 1500 }, rdi), "Sodium (mg)");

        Assert.Equal("Excess", over.Status);
        Assert.Equal("OK", under.Status);
    }

    private static NutrientComparisonDto Find(List<NutrientComparisonDto> all, string nutrient) =>
        all.Single(c => c.Nutrient == nutrient);
}
