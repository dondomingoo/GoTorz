public interface ICustomAuthStateProvider
{
    /// <summary>
    /// Call this when a user logs in and you want to notify the UI that a user is now authenticated.
    /// </summary>
    Task NotifyUserAuthentication();

    /// <summary>
    /// Call this on logout to make Blazor treat the user as logged out.
    /// </summary>
    void NotifyUserLogout();

    /// <summary>
    /// Just checks if the token has expired using the built-in ValidTo field.
    /// </summary>
    bool IsTokenExpired(string token);
}
