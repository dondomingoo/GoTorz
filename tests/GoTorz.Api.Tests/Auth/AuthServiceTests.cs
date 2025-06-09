using Moq;
using GoTorz.Api.Services.Auth;
using Microsoft.AspNetCore.Identity;
using GoTorz.Shared.DTOs.Auth;


namespace GoTorz.Api.Tests.Auth;

public class AuthServiceTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // Setup mocks with dummy dependencies
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

        _tokenServiceMock = new Mock<ITokenService>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _tokenServiceMock.Object);
    }


    // -------- LoginUserAsync --------

    [Fact]
    public async Task LoginUserAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var dto = new LoginDTO { Email = "missing@example.com", Password = "irrelevant" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((IdentityUser)null);

        // Act
        var result = await _authService.LoginUserAsync(dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginUserAsync_ReturnsNull_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new IdentityUser { Email = "user@example.com" };
        var dto = new LoginDTO { Email = user.Email, Password = "wrongpass" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

        // Act
        var result = await _authService.LoginUserAsync(dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginUserAsync_ReturnsLoginResponse_WhenCredentialsAreCorrect()
    {
        // Arrange
        var user = new IdentityUser { Email = "valid@example.com", UserName = "valid@example.com", Id = "abc123" };
        var roles = new List<string> { "User", "Customer" };
        var dto = new LoginDTO { Email = user.Email, Password = "correctpass" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
        _tokenServiceMock.Setup(x => x.GenerateToken(user, roles)).Returns("mocked-jwt");

        // Act
        var result = await _authService.LoginUserAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal("mocked-jwt", result.Token);
    }


    // -------- RegisterUserAsync --------

    [Fact]
    public async Task RegisterUserAsync_ReturnsFailure_IfUserCreationFails()
    {
        // Arrange
        var dto = new RegisterDTO { Email = "fail@example.com", Password = "bad" };
        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Invalid password" });

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), dto.Password))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _authService.RegisterUserAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Description == "Invalid password");
    }

    [Fact]
    public async Task RegisterUserAsync_CreatesRole_IfNotExists()
    {
        // Arrange
        var dto = new RegisterDTO { Email = "newuser@example.com", Password = "ValidPass123!" };
        var identityResult = IdentityResult.Success;

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), dto.Password))
            .ReturnsAsync(identityResult);

        _roleManagerMock.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(false);
        _roleManagerMock.Setup(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == "User")))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterUserAsync(dto);

        // Assert
        Assert.True(result.Succeeded);
        _roleManagerMock.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == "User")), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_AssignsDefaultRole_WhenUserCreated()
    {
        // Arrange
        var dto = new RegisterDTO { Email = "assignrole@example.com", Password = "Secure123!" };
        var identityResult = IdentityResult.Success;

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), dto.Password))
            .ReturnsAsync(identityResult);

        _roleManagerMock.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(true);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterUserAsync(dto);

        // Assert
        Assert.True(result.Succeeded);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"), Times.Once);
    }




}
