using GoTorz.Api.Services;
using GoTorz.Api.Services.Auth;
using GoTorz.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace GoTorz.Api.Tests.Auth;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly Mock<IBookingService> _bookingServiceMock = new();
    private readonly Mock<ILogger<AuthController>> _loggerMock = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(
            _authServiceMock.Object,
            _bookingServiceMock.Object,
            _loggerMock.Object);
    }

    // -------- Register --------

    [Fact]
    public async Task Register_ReturnsBadRequest_IfRegistrationFails()
    {
        var dto = new RegisterDTO { Email = "fail@example.com", Password = "bad" };
        var failedResult = IdentityResult.Failed(new IdentityError { Description = "Error" });
        _authServiceMock.Setup(x => x.RegisterUserAsync(dto)).ReturnsAsync(failedResult);

        var result = await _controller.Register(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsOk_IfRegistrationSucceeds()
    {
        var dto = new RegisterDTO { Email = "ok@example.com", Password = "test" };
        _authServiceMock.Setup(x => x.RegisterUserAsync(dto)).ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Register(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    // -------- Login --------

    [Fact]
    public async Task Login_ReturnsUnauthorized_IfLoginFails()
    {
        var dto = new LoginDTO { Email = "fail", Password = "wrong" };
        _authServiceMock.Setup(x => x.LoginUserAsync(dto)).ReturnsAsync((LoginResponseDTO?)null);

        var result = await _controller.Login(dto);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task Login_ReturnsOk_IfLoginSucceeds()
    {
        var dto = new LoginDTO { Email = "ok", Password = "correct" };
        var loginResponse = new LoginResponseDTO { Email = dto.Email, Token = "jwt" };
        _authServiceMock.Setup(x => x.LoginUserAsync(dto)).ReturnsAsync(loginResponse);

        var result = await _controller.Login(dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(loginResponse, ok.Value);
    }

    // -------- DeleteUser --------

    [Fact]
    public async Task DeleteUser_ReturnsUnauthorized_IfNoUserId()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // No user
        };

        var result = await _controller.DeleteUser();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsBadRequest_IfUserHasBookings()
    {
        var userId = "123";
        _controller.ControllerContext = WithUser(userId);
        _bookingServiceMock.Setup(x => x.HasUpcomingBookingsAsync(userId)).ReturnsAsync(true);

        var result = await _controller.DeleteUser();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsOk_IfDeletionSucceeds()
    {
        var userId = "123";
        _controller.ControllerContext = WithUser(userId);
        _bookingServiceMock.Setup(x => x.HasUpcomingBookingsAsync(userId)).ReturnsAsync(false);
        _authServiceMock.Setup(x => x.DeleteUserAsync(userId)).ReturnsAsync(true);

        var result = await _controller.DeleteUser();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsServerError_IfDeletionFails()
    {
        var userId = "123";
        _controller.ControllerContext = WithUser(userId);
        _bookingServiceMock.Setup(x => x.HasUpcomingBookingsAsync(userId)).ReturnsAsync(false);
        _authServiceMock.Setup(x => x.DeleteUserAsync(userId)).ReturnsAsync(false);

        var result = await _controller.DeleteUser();

        var error = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, error.StatusCode);
    }

    private ControllerContext WithUser(string userId)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, "mock");

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }
}
