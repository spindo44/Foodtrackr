using Foodtrackr.Api.Controllers;
using Foodtrackr.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodtrackr.Tests;

public class PatientControllerTests
{
    private const string UserA = "user-a";
    private const string UserB = "user-b";

    private static Patient NewPatient(string name) => new()
    {
        Name = name,
        Age = 30,
        Gender = "Male",
        WeightKg = 80,
        HeightCm = 180
    };

    [Fact]
    public async Task Create_PersistsPatient_AndStampsCurrentUser()
    {
        using var db = TestHelpers.NewDb();
        var controller = new PatientController(db).AsUser(UserA);

        var result = await controller.Create(NewPatient("Alice"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<Patient>(ok.Value);
        Assert.Equal(UserA, returned.UserId);
        Assert.True(returned.Id > 0);

        Assert.Single(db.Patients);
        Assert.Equal(UserA, db.Patients.Single().UserId);
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyCurrentUsersPatients_OrderedByName()
    {
        using var db = TestHelpers.NewDb();
        db.Patients.AddRange(
            new Patient { Name = "Charlie", UserId = UserA },
            new Patient { Name = "Alice", UserId = UserA },
            new Patient { Name = "Bob", UserId = UserB });
        await db.SaveChangesAsync();

        var result = await new PatientController(db).AsUser(UserA).GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var patients = Assert.IsAssignableFrom<IEnumerable<Patient>>(ok.Value).ToList();

        Assert.Equal(2, patients.Count);
        Assert.All(patients, p => Assert.Equal(UserA, p.UserId));
        Assert.Equal(new[] { "Alice", "Charlie" }, patients.Select(p => p.Name));
    }

    [Fact]
    public async Task GetById_OtherUsersPatient_ReturnsNotFound()
    {
        using var db = TestHelpers.NewDb();
        var theirs = NewPatient("Bob");
        theirs.UserId = UserB;
        db.Patients.Add(theirs);
        await db.SaveChangesAsync();

        var result = await new PatientController(db).AsUser(UserA).GetById(theirs.Id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_ChangesEditableFields()
    {
        using var db = TestHelpers.NewDb();
        var patient = NewPatient("Alice");
        patient.UserId = UserA;
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var edit = NewPatient("Alice Smith");
        edit.WeightKg = 72;
        edit.ActivityLevel = "active";

        var result = await new PatientController(db).AsUser(UserA).Update(patient.Id, edit);

        Assert.IsType<OkObjectResult>(result);
        var saved = await db.Patients.FindAsync(patient.Id);
        Assert.Equal("Alice Smith", saved!.Name);
        Assert.Equal(72, saved.WeightKg);
        Assert.Equal("active", saved.ActivityLevel);
    }

    [Fact]
    public async Task Update_MissingPatient_ReturnsNotFound()
    {
        using var db = TestHelpers.NewDb();
        var result = await new PatientController(db).AsUser(UserA).Update(999, NewPatient("Ghost"));
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_RemovesPatient_ReturnsNoContent()
    {
        using var db = TestHelpers.NewDb();
        var patient = NewPatient("Alice");
        patient.UserId = UserA;
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var result = await new PatientController(db).AsUser(UserA).Delete(patient.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(db.Patients);
    }

    [Fact]
    public async Task Delete_OtherUsersPatient_ReturnsNotFound_AndKeepsRow()
    {
        using var db = TestHelpers.NewDb();
        var theirs = NewPatient("Bob");
        theirs.UserId = UserB;
        db.Patients.Add(theirs);
        await db.SaveChangesAsync();

        var result = await new PatientController(db).AsUser(UserA).Delete(theirs.Id);

        Assert.IsType<NotFoundResult>(result);
        Assert.Single(db.Patients);
    }
}
