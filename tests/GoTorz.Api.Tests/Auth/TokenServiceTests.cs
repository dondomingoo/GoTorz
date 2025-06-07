using Xunit;
using GoTorz.Api.Services.Auth;
using GoTorz.Shared;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GoTorz.Api.Tests.Auth;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "super_secret_test_key_12345_long!!_256bits_minimum_is_required", // Should be 32+ chars for HMAC
            Issuer = "GoTorzTestIssuer",
            Audience = "GoTorzTestAudience",
            ExpiryMinutes = 30
        };

        var options = Options.Create(_jwtSettings);
        _tokenService = new TokenService(options);
    }

    [Fact]
    public void GenerateToken_ReturnsValidJwt_WithExpectedClaims()
    {
        // Arrange
        var user = new IdentityUser
        {
            UserName = "testuser",
            Id = "user123"
        };

        var roles = new List<string> { "Admin", "SalesRep", "User" };

        // Act
        var token = _tokenService.GenerateToken(user, roles);

        // Assert basic
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));

        var jwt = handler.ReadJwtToken(token);

        // Assert claims
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id);
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "SalesRep");
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");


        // Assert issuer, audience, expiry
        Assert.Equal(_jwtSettings.Issuer, jwt.Issuer);
        Assert.Equal(_jwtSettings.Audience, jwt.Audiences.Single());
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }
}
