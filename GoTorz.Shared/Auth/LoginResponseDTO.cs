using System.Diagnostics.CodeAnalysis;

namespace GoTorz.Shared.Auth;

public class LoginResponseDTO
{
    public string Token { get; set; }
    public string Email { get; set; }
}
