using System.ComponentModel.DataAnnotations;

namespace GoTorz.Shared.Auth;

public class LoginDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(1)] // Change later
    public string Password { get; set; }
}
