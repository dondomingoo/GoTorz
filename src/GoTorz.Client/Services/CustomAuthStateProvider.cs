using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/// <summary>
/// This class connects Blazor’s built-in authorization system to our JWT-based authentication.
/// It reads the token from local storage, checks if it’s valid, and builds a ClaimsPrincipal to represent the logged-in user.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly LocalStorage _localStorage;                                              // Service to read/write browser local storage(your JWT is stored there)

    public CustomAuthStateProvider(LocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// This method runs automatically when Blazor needs to know the user's current login state.
    /// </summary>
    /// <returns>AuthenticationState</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var jwt = await _localStorage.GetItemAsync("jwt");                                    // Try to get the token from local storage
        
        if (string.IsNullOrWhiteSpace(jwt) || IsTokenExpired(jwt))                            // If no token or it's expired
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));        // Return anonymous user (not logged in)
        }

        return new AuthenticationState(BuildClaimsPrincipal(jwt));                            // Build a user from token and return as logged-in
    }

    /// <summary>
    /// Call this when a user logs in and you want to notify the UI that a user is now authenticated.
    /// </summary>
    public async Task NotifyUserAuthentication()
    {
        var jwt = await _localStorage.GetItemAsync("jwt");                                     // Get latest token from storage
        if (string.IsNullOrWhiteSpace(jwt)) return;                                                             

        NotifyAuthenticationStateChanged(                                                      // Notify Blazor that state changed    
            Task.FromResult(new AuthenticationState(BuildClaimsPrincipal(jwt))));              // Rebuild the user and notify
    }

    /// <summary>
    /// Call this on logout to make Blazor treat the user as logged out.
    /// </summary>
    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());                              // Empty identity = not logged in
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));  // Notify Blazor to re-render
    }

    /// <summary>
    /// Helper that parses the JWT and creates a ClaimsPrincipal from the token.
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns>ClaimPrincipal</returns>
    private ClaimsPrincipal BuildClaimsPrincipal(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();                                            // Built-in JWT parser
        var token = handler.ReadJwtToken(jwt);                                                  // Read + decode the token (does not validate)

        var identity = new ClaimsIdentity(token.Claims, "jwt");                                 // Build identity with claims and set auth type to "jwt"
        return new ClaimsPrincipal(identity);                                                   // Wrap in ClaimsPrincipal and return (Principal = User in Blazor) - So it reads the token and builds a user from it
    }

    /// <summary>
    /// Just checks if the token has expired using the built-in ValidTo field.
    /// </summary>
    public bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);                                                  // Parse token
        return jwt.ValidTo < DateTime.UtcNow;                                                   // Expired = true if expiry time has passed
    }
}
