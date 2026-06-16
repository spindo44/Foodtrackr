using System.IdentityModel.Tokens.Jwt;
using Foodtrackr.Api.Controllers;
using Foodtrackr.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Foodtrackr.Tests;

public class AuthControllerTests
{
    private const string Email = "person@test.com";
    private const string GoodPassword = "Test123!pass";

    private static UserManager<IdentityUser> NewUserManager()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<AppDbContext>();

        return services.BuildServiceProvider().GetRequiredService<UserManager<IdentityUser>>();
    }

    private static async Task<AuthController> RegisteredControllerAsync()
    {
        var um = NewUserManager();
        await um.CreateAsync(new IdentityUser { UserName = Email, Email = Email }, GoodPassword);
        return new AuthController(um);
    }

    [Fact]
    public async Task Register_ValidCredentials_ReturnsOk_AndCreatesUser()
    {
        var um = NewUserManager();
        var controller = new AuthController(um);

        var result = await controller.Register(new RegisterDto { Email = Email, Password = GoodPassword });

        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(await um.FindByEmailAsync(Email));
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsBadRequest()
    {
        var controller = new AuthController(NewUserManager());

        var result = await controller.Register(new RegisterDto { Email = Email, Password = "abc" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_CorrectCredentials_ReturnsJwtTokenCarryingUserId()
    {
        var um = NewUserManager();
        var user = new IdentityUser { UserName = Email, Email = Email };
        await um.CreateAsync(user, GoodPassword);
        var controller = new AuthController(um);

        var result = await controller.Login(new LoginDto { Email = Email, Password = GoodPassword });

        var ok = Assert.IsType<OkObjectResult>(result);
        var token = ok.Value!.GetType().GetProperty("token")!.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(token));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Contains(jwt.Claims, c => c.Value == user.Id);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var controller = await RegisteredControllerAsync();

        var result = await controller.Login(new LoginDto { Email = Email, Password = "Wrong123!pass" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsUnauthorized()
    {
        var controller = new AuthController(NewUserManager());

        var result = await controller.Login(new LoginDto { Email = "nobody@test.com", Password = GoodPassword });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
