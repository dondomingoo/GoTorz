using Microsoft.AspNetCore.Identity;
using GoTorz.Api.Services.Auth;
using GoTorz.Shared.Auth;
using Moq;

namespace GoTorz.Api.Tests.Services.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);  // This is because UserManager<TUser> has a very long constructor, sets everything to null

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);

            _tokenServiceMock = new Mock<ITokenService>();

            _authService = new AuthService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenUserIsCreated()
        {
            // Arrange
            var dto = new RegisterDTO { Email = "test@example.com", Password = "Password123!" };
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), dto.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.RegisterUserAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var dto = new LoginDTO { Email = "missing@example.com", Password = "wrong" };
            _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _authService.LoginUserAsync(dto);

            // Assert
            Assert.Null(result);
        }
    }
}