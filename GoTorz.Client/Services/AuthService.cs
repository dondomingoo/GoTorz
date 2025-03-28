using GoTorz.Shared.Auth;
using System.Net.Http.Json;

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

    public async Task<bool> RegisterAsync(RegisterDTO dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", dto);
        return response.IsSuccessStatusCode;
    }

}
