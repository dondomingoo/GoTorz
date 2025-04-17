using GoTorz.Shared.Auth;

/// <summary>
/// Service responsible for handling Authentication-related operations in the Blazor Client.
/// Communicates with the backend AuthController and manages JWT tokens in LocalStorage.
/// </summary>
public interface IClientAuthService
{
    /// <summary>
    /// Gets the current JWT token after successful login.
    /// </summary>
    string JwtToken { get; }
    /// <summary>
    /// Gets the user's email associated with the current session.
    /// </summary>
    string Email { get; }

    /// <summary>
    /// Sends login request to API. Stores token + email if successful. Notifies Blazor.
    /// </summary>
    Task<bool> LoginAsync(LoginDTO dto);

    /// <summary>
    /// Sends register request to API.
    /// </summary>
    Task<bool> RegisterAsync(RegisterDTO dto);

    /// <summary>
    /// Creates a prepared HttpRequestMessage with JWT attached.
    /// Returns null if no JWT is present (unauthenticated).
    /// </summary>
    Task<HttpRequestMessage?> CreateAuthorizedRequest(HttpMethod method, string url);

    /// <summary>
    /// Checks if the user is logged in by validating the JWT.
    /// </summary>
    Task<bool> IsLoggedInAsync();

    /// <summary>
    /// Logs out the user by clearing stored JWT + Email. Notifies Blazor.
    /// </summary>
    Task LogoutAsync();
}
