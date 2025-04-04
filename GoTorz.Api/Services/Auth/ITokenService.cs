using Microsoft.AspNetCore.Identity;

namespace GoTorz.Api.Services.Auth
{
    public interface ITokenService
    {
        string GenerateToken(IdentityUser user, IList<string> roles);
    }
}