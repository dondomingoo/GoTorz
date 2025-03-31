using GoTorz.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoTorz.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;                                      // Strongly typed JwtSettings object
        
        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(IdentityUser user, IList<string> roles)
        {
            var claims = new List<Claim>                                                // Prepare claims (small information pieces about User) - these will later be read directly from the token by the server or client
            {
                new Claim(ClaimTypes.Name, user.UserName),                              // Adds Username as claim
                new Claim(ClaimTypes.NameIdentifier, user.Id)                           // Adds User ID as claim
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));    // Adds all roles as Role claims

            var key = new SymmetricSecurityKey(                                         // Create the symmetric security key
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));                        // Encodes the SecretKey from settings

            var creds = new SigningCredentials(                                         // Creates credentials used to sign the JWT - so the server can later verify the token has not been tampered with
                key, SecurityAlgorithms.HmacSha256);                                    // Uses HMAC SHA-256 as the hashing algorithm                                  

            var token = new JwtSecurityToken(                                           // Creates the actual JWT token
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);                     // Converts token into a string for the client
        }
    }
}
