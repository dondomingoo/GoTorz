using GoTorz.Shared.Auth;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;

/// <summary>
/// Service responsible for handling Authentication-related operations in the Blazor Client.
/// Communicates with the backend AuthController and manages JWT tokens in LocalStorage.
/// </summary>
public class ClientAuthService
{
    private readonly HttpClient _http;                              // Used to send HTTP requests to the API (login, register, etc.)
    private readonly LocalStorage _localStorage;                    // Used to store/retrieve JWT + Email in/from the browser's localStorage
    private readonly CustomAuthStateProvider _authStateProvider;    // Updates Blazor's built-in AuthenticationStateProvider after login/logout

    public string JwtToken { get; private set; } = string.Empty;    // Holds the JWT token after login
    public string Email { get; private set; } = string.Empty;       // Holds the user's email after login

    public ClientAuthService(HttpClient http, LocalStorage localStorage, CustomAuthStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Sends login request to API. Stores token + email if successful. Notifies Blazor.
    /// </summary>
    public async Task<bool> LoginAsync(LoginDTO dto)                                    
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", dto);              // Send POST to /api/auth/login with credentials
        if (!response.IsSuccessStatusCode) return false;                                // If login failed, return false

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();      // Read JWT + email from API response
        JwtToken = result?.Token ?? string.Empty;                                       // Store JWT
        Email = result?.Email ?? string.Empty;                                          // Store Email

        await _localStorage.SetItemAsync("jwt", JwtToken);                              // Save JWT in browser
        await _localStorage.SetItemAsync("email", Email);                               // Save Email in browser

        await _authStateProvider.NotifyUserAuthentication();                            // Notify Blazor that user is now authenticated
        return true;                                                                    // Return success
    }

    /// <summary>
    /// Sends register request to API.
    /// </summary>
    public async Task<bool> RegisterAsync(RegisterDTO dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", dto);           // POST to /api/auth/register with email + password
        return response.IsSuccessStatusCode;                                            // Return success if registration worked
    }

    /// <summary>
    /// Creates a prepared HttpRequestMessage with JWT attached.
    /// Returns null if no JWT is present (unauthenticated).
    /// </summary>
    public async Task<HttpRequestMessage?> CreateAuthorizedRequest(HttpMethod method, string url)   
    {
        var jwt = await _localStorage.GetItemAsync("jwt");                              // Get JWT from localStorage
        if (string.IsNullOrWhiteSpace(jwt)) return null;                                // If no JWT, return null (user is probably not logged in)

        var request = new HttpRequestMessage(method, url);                              // Create the request
        request.Headers.Authorization = new 
            System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);           // Add JWT in Authorization header
        return request;                                                                 // Return the prepared request
    }

    /// <summary>
    /// Checks if the user is logged in by validating the JWT.
    /// </summary>
    public async Task<bool> IsLoggedInAsync()
    {
        var jwt = await _localStorage.GetItemAsync("jwt");                              // Get JWT from localStorage
        return !string.IsNullOrWhiteSpace(jwt) &&       
            !_authStateProvider.IsTokenExpired(jwt);                                    // Return True if JWT exists AND is not expired
    }

    /// <summary>
    /// Logs out the user by clearing stored JWT + Email. Notifies Blazor.
    /// </summary>
    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("jwt");                                     // Remove JWT from browser
        await _localStorage.RemoveItemAsync("email");                                   // Remove Email from browser
        JwtToken = string.Empty;
        Email = string.Empty;                                                           // Clear memory values

        _authStateProvider.NotifyUserLogout();                                          // Notify Blazor that user is now anonymous
    }
}
