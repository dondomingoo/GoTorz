using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoTorz.Shared;
using GoTorz.Shared.Auth;
using Microsoft.Extensions.Options;
using GoTorz.Api.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;    // Manages users (create, validate passwords, etc.)
    private readonly RoleManager<IdentityRole> _roleManager;    // Manages roles (Admin, User, SalesRep, etc.)
    private readonly ITokenService _tokenService;               // Creates the Json Web Token (JWT)

    public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };            // Create IdentityUser (Username is mandatory, we use Email)
        var result = await _userManager.CreateAsync(user, dto.Password);                    // Automatically hashes password + stores user in DB
        if (!result.Succeeded)
            return BadRequest(result.Errors);                                               // If creation failed, return error list

        // Add default role
        if (!await _roleManager.RoleExistsAsync("User"))                                   // Check if "Admin" role exists
            await _roleManager.CreateAsync(new IdentityRole("User"));                      // Create "Admin" role if missing

            await _userManager.AddToRoleAsync(user, "User");                               // Assign "Admin" role to the new user (TEMPORARY for testing) 

        return Ok();                                                                        // Success!
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO dto)                   
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);                          // Find user by email
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))     // Check password
            return Unauthorized();                                                          // Invalid login

        var userRoles = await _userManager.GetRolesAsync(user);                             // Get the user's roles

        var token = _tokenService.GenerateToken(user, userRoles);                           // Ask TokenService to create the JWT for this user

        return Ok(new LoginResponseDTO                                                      // Return DTO with token + email
        {
            Email = user.Email,
            Token = token
        });
    }

}

