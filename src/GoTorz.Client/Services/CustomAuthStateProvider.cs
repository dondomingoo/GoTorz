using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/// <summary>
/// This class connects Blazor’s built-in authorization system to our JWT-based authentication.
/// It reads the token from local storage, checks if it’s valid, and builds a ClaimsPrincipal to represent the logged-in user.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider, ICustomAuthStateProvider
{
    private readonly ILocalStorage _localStorage;

    public CustomAuthStateProvider(ILocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var jwt = await _localStorage.GetItemAsync("jwt");

        if (string.IsNullOrWhiteSpace(jwt) || IsTokenExpired(jwt))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        return new AuthenticationState(BuildClaimsPrincipal(jwt));
    }

    public async Task NotifyUserAuthentication()
    {
        var jwt = await _localStorage.GetItemAsync("jwt");
        if (string.IsNullOrWhiteSpace(jwt)) return;

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(BuildClaimsPrincipal(jwt))));
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }

    public bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.ValidTo < DateTime.UtcNow;
    }

    /// <summary>
    /// Helper that parses the JWT and creates a ClaimsPrincipal from the token.
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns>ClaimPrincipal</returns>
    private ClaimsPrincipal BuildClaimsPrincipal(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        var identity = new ClaimsIdentity(token.Claims, "jwt");
        return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Retrieves the JWT token from local storage.
    /// This method safely exposes the token without exposing the local storage directly.
    /// </summary>
    public async Task<string?> GetJwtAsync()
    {
        return await _localStorage.GetItemAsync("jwt");
    }

}
