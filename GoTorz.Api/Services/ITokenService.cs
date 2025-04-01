using Microsoft.AspNetCore.Identity;

namespace GoTorz.Api.Services
{
    public interface ITokenService
    {
        string GenerateToken(IdentityUser user, IList<string> roles);
    }
}