using GoTorz.Shared.Models;
using Microsoft.JSInterop;                                          // Required to call JavaScript from C#

/// <summary>
/// C# wrapper for browser localStorage
/// </summary>
public class LocalStorage
{
    private readonly IJSRuntime _js;                                // JSInterop bridge (lets C# call JS functions)

    public LocalStorage(IJSRuntime js)                              
    {
        _js = js;                                                   
    }

    public ValueTask SetItemAsync(string key, string value) =>      // Writes value to LocalStorage with given key
        _js.InvokeVoidAsync("localStorage.setItem", key, value);

    public ValueTask<string> GetItemAsync(string key) =>            // Reads value from LocalStorage by key
        _js.InvokeAsync<string>("localStorage.getItem", key);

    public ValueTask RemoveItemAsync(string key) =>                 // Removes key + value from LocalStorage
        _js.InvokeVoidAsync("localStorage.removeItem", key);


    /* 
    LocalStorage is plain-text storage.

    If you inspect the browser in DevTools > Application > Local Storage, you will see:
      jwt = eyJhbGciOiJIUzI1NiIsIn...
      email = user @example.com

    Anyone with browser access can see these values, but it is okay because the JWT is signed, not encrypted. 
    */

}
