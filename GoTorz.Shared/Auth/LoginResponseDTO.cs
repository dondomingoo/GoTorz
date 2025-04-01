using System.Diagnostics.CodeAnalysis;

namespace GoTorz.Shared.Auth;

public class LoginResponseDTO
{
    public string Token { get; set; }                   // The JWT token that the client stores
    public string Email { get; set; }                   // Sent back just for confirmation or UI use
}
