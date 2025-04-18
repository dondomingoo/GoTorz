using System.ComponentModel.DataAnnotations;

namespace GoTorz.Shared.Auth;

public class RegisterDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
