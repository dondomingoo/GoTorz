using GoTorz.Shared.Auth;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;


public class AuthService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly LocalStorage _localStorage;

    public string JwtToken { get; private set; }
    public string Email { get; private set; }

    public AuthService(HttpClient http, IConfiguration config, LocalStorage localStorage)
    {
        _http = http;
        _config = config;
        _localStorage = localStorage;
    }

    // Login and store JWT + Email
    public async Task<bool> LoginAsync(LoginDTO dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

        JwtToken = result.Token;
        Email = result.Email;

        await _localStorage.SetItemAsync("jwt", JwtToken);
        await _localStorage.SetItemAsync("email", Email);

        return true;
    }

    // Register user
    public async Task<bool> RegisterAsync(RegisterDTO dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", dto);
        return response.IsSuccessStatusCode;
    }

    // Create HTTP request with Authorization header
    public async Task<HttpRequestMessage?> CreateAuthorizedRequest(HttpMethod method, string url)
    {
        var jwt = await _localStorage.GetItemAsync("jwt");

        // For testing: throw if no JWT is present to catch unauthorized access early
        // if (string.IsNullOrWhiteSpace(jwt))
        //     throw new InvalidOperationException("No JWT token found. User might not be logged in.");
        if (string.IsNullOrWhiteSpace(jwt)) return null;


        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        return request;
    }

    // Checks if JWT is expired
    public bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return jwt.ValidTo < DateTime.UtcNow;
    }

    // Returns if user is currently logged in
    public async Task<bool> IsLoggedInAsync()
    {
        var jwt = await _localStorage.GetItemAsync("jwt");
        if (string.IsNullOrWhiteSpace(jwt)) return false;

        return !IsTokenExpired(jwt);
    }

    // Logout user (clear token + email)
    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("jwt");
        await _localStorage.RemoveItemAsync("email");
        JwtToken = string.Empty;
        Email = string.Empty;
    }


}
