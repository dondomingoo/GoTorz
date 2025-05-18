using GoTorz.Api.Services;
using GoTorz.Api.Services.Auth;
using GoTorz.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IBookingService bookingService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _bookingService = bookingService;
        _logger = logger;   
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var result = await _authService.RegisterUserAsync(dto);
        if (!result.Succeeded)
        { 
            _logger.LogWarning("User registration failed at {Timestamp}. Errors: {Errors}", string.Join(", ", result.Errors));  
            return BadRequest(result.Errors);
        }
        _logger.LogInformation("New user registered successfully.");
        return Ok(new { Message = "User registered successfully." });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO dto)
    {
        var loginResult = await _authService.LoginUserAsync(dto);
        if (loginResult == null)
        {
            _logger.LogWarning("Login failed for user with identifier provided.");
            return Unauthorized();
        }
        _logger.LogInformation("User logged in successfully.");
        return Ok(loginResult);
    }


    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var hasUpcomingBookings = await _bookingService.HasUpcomingBookingsAsync(userId);
        if (hasUpcomingBookings)
        {
            _logger.LogWarning("User deletion blocked due to existing upcoming bookings.");
            return BadRequest("You have upcoming bookings and cannot delete your profile.");
        }

        var result = await _authService.DeleteUserAsync(userId);
        if (result)
        {
            _logger.LogInformation("User deleted successfully");
            return Ok("User deleted.");
        }
        else
        {
            _logger.LogError("User deletion failed due to an internal error.");
            return StatusCode(500, "Failed to delete user.");
        }
    }


}

