using GoTorz.Shared.DTOs.Auth;
using Moq;
using RichardSzalay.MockHttp;
using System.Net.Http.Json;

namespace GoTorz.Client.Tests.Auth;

public class ClientAuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ReturnsTrue_WhenResponseIsSuccessful()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "user@example.com", Password = "pass" };
        var responseDto = new LoginResponseDTO { Email = loginDto.Email, Token = "mock-token" };

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://localhost/api/auth/login")
            .Respond(JsonContent.Create(responseDto));

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://localhost");

        var localStorage = new Mock<ILocalStorage>();
        var authProvider = new Mock<ICustomAuthStateProvider>();


        var service = new ClientAuthService(httpClient, localStorage.Object, authProvider.Object);

        // Act
        var result = await service.LoginAsync(loginDto);

        // Assert
        Assert.True(result);
        Assert.Equal("mock-token", service.JwtToken);
        Assert.Equal("user@example.com", service.Email);
        localStorage.Verify(x => x.SetItemAsync("jwt", "mock-token"), Times.Once);
        localStorage.Verify(x => x.SetItemAsync("email", "user@example.com"), Times.Once);
        authProvider.Verify(x => x.NotifyUserAuthentication(), Times.Once);
    }
}
