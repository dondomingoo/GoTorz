using GoTorz.Api.Services;
using GoTorz.Api.Services.Auth;
using GoTorz.Shared.Auth;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IBookingService _bookingService;

    public AuthController(IAuthService authService, IBookingService bookingService)
    {
        _authService = authService;
        _bookingService = bookingService;

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var result = await _authService.RegisterUserAsync(dto);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO dto)
    {
        var loginResult = await _authService.LoginUserAsync(dto);
        if (loginResult == null)
            return Unauthorized();

        return Ok(loginResult);
    }

    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var hasUpcomingBookings = await _bookingService.HasUpcomingBookingsAsync(userId);
        if (hasUpcomingBookings)
            return BadRequest("You have upcoming bookings and cannot delete your profile.");

        var result = await _authService.DeleteUserAsync(userId);
        return result ? Ok("User deleted.") : StatusCode(500, "Failed to delete user.");
    }


}

