using GoTorz.Shared.Auth;
using Microsoft.AspNetCore.Identity;

namespace GoTorz.Api.Services.Auth
{
    /// <summary>
    /// Implements authentication logic using ASP.NET Core Identity.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO dto)
        {
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return result;

            var DefaultRole = "Admin";
            if (!await _roleManager.RoleExistsAsync(DefaultRole))
                await _roleManager.CreateAsync(new IdentityRole(DefaultRole));

            await _userManager.AddToRoleAsync(user, DefaultRole);

            return result;
        }
        
        public async Task<LoginResponseDTO>LoginUserAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new LoginResponseDTO
            {
                Email = user.Email,
                Token = token,
            };
        }
    }
}
