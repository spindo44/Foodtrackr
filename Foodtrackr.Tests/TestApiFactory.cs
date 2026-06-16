using Foodtrackr.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Foodtrackr.Tests;

public class TestApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "it-" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Jwt:Key", "FoodtrackrSuperSecretKey123456789!");

        builder.ConfigureTestServices(services =>
        {
            var toRemove = services
                .Where(d => d.ServiceType == typeof(AppDbContext)
                         || d.ServiceType == typeof(DbContextOptions)
                         || (d.ServiceType.IsGenericType
                             && d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))
                         || d.ServiceType.Name.Contains("DbContextOptionsConfiguration"))
                .ToList();
            foreach (var d in toRemove)
                services.Remove(d);

            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(_dbName));
        });
    }

    public void Seed(Action<AppDbContext> seed)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        seed(db);
        db.SaveChanges();
    }
}
