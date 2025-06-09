using Xunit;
using Moq;
using GoTorz.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoTorz.Client.Services.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace GoTorz.Client.Tests.Auth;

public class CustomAuthStateProviderTests
{
    [Fact]
    public async Task GetAuthenticationStateAsync_ReturnsAuthenticatedPrincipal_IfTokenIsValid()
    {
        // Arrange
        var jwt = CreateValidJwt("testuser", "123", DateTime.UtcNow.AddMinutes(30));
        var localStorage = new Mock<ILocalStorage>();
        localStorage.Setup(x => x.GetItemAsync("jwt")).ReturnsAsync(jwt);

        var Provider = new CustomAuthStateProvider(localStorage.Object);

        // Act
        var authState = await Provider.GetAuthenticationStateAsync();

        // Assert
        var user = authState.User;
        Assert.True(user.Identity?.IsAuthenticated);
        Assert.Equal("testuser", user.Identity?.Name);
        Assert.Equal("123", user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }


    /// <summary>
    /// Helper method to create a valid JWT token for testing purposes. (normally happens in backend - not in client code)
    /// </summary>
    private string CreateValidJwt(string username, string userId, DateTime validTo)
    {
        var handler = new JwtSecurityTokenHandler();

        var token = new JwtSecurityToken(
            issuer: "TestIssuer",
            audience: "TestAudience",
            claims: new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId)
            },
            expires: validTo,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this_is_a_secure_32char_secret!!")),
                SecurityAlgorithms.HmacSha256)
        );

        return handler.WriteToken(token); // use this token for test as if it was received from server
    }
}
