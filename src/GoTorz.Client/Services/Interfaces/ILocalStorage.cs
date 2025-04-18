/// <summary>
/// Abstraction for interacting with browser localStorage from C# via JavaScript interop.
/// </summary>
public interface ILocalStorage
{
    /// <summary>
    /// Writes a value to localStorage under the specified key.
    /// </summary>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The string value to store.</param>
    /// <returns>A ValueTask representing the async operation.</returns>
    ValueTask SetItemAsync(string key, string value);

    /// <summary>
    /// Retrieves a string value from localStorage by key.
    /// </summary>
    /// <param name="key">The key of the stored value.</param>
    /// <returns>A ValueTask containing the retrieved string, or null if not found.</returns>
    ValueTask<string> GetItemAsync(string key);

    /// <summary>
    /// Removes an item from localStorage by key.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>A ValueTask representing the async operation.</returns>
    ValueTask RemoveItemAsync(string key);
}
