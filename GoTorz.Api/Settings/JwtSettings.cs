namespace GoTorz.Shared;

/// <summary>
/// POCO = Plain Old CLR Object.
/// This class is bound to the "JwtSettings" section in appsettings.json
/// and gives strongly typed access to the JWT configuration.
/// </summary>
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;          // Who issues the token (the API)
    public string Audience { get; set; } = string.Empty;        // Who the token is intended for (the client)
    public string SecretKey { get; set; } = string.Empty;       // Secret used to sign the token (MUST be protected)
    public int ExpiryMinutes { get; set; }                      // How long the token is valid (in minutes)
}

