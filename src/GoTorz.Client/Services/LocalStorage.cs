using Microsoft.JSInterop;

/// <summary>
/// C# wrapper for browser localStorage.
/// </summary>
public class LocalStorage : ILocalStorage
{
    private readonly IJSRuntime _js;

    public LocalStorage(IJSRuntime js)
    {
        _js = js;
    }

    public ValueTask SetItemAsync(string key, string value) =>
        _js.InvokeVoidAsync("localStorage.setItem", key, value);

    public ValueTask<string> GetItemAsync(string key) =>
        _js.InvokeAsync<string>("localStorage.getItem", key);

    public ValueTask RemoveItemAsync(string key) =>
        _js.InvokeVoidAsync("localStorage.removeItem", key);
}


    // LocalStorage is plain-text storage.
    // If you inspect the browser in DevTools > Application > Local Storage, you will see:
    //      jwt = eyJhbGciOiJIUzI1NiIsIn...
    //      email = user@example.com
    // Anyone with browser access can see these values, but it is okay because the JWT is signed, not encrypted. 