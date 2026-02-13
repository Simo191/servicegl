namespace MultiServices.Maui.Services.Storage;

public interface ISecureStorageService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    void Remove(string key);
    void RemoveAll();
}

public class SecureStorageService : ISecureStorageService
{
    public async Task<string?> GetAsync(string key)
    {
        try { return await SecureStorage.Default.GetAsync(key); }
        catch { return null; }
    }

    public async Task SetAsync(string key, string value)
    {
        try { await Task.Run(() => SecureStorage.Default.SetAsync(key, value)); }
        catch { Preferences.Default.Set(key, value); }
    }

    public void Remove(string key)
    {
        try { SecureStorage.Default.Remove(key); }
        catch { Preferences.Default.Remove(key); }
    }

    public void RemoveAll()
    {
        try { SecureStorage.Default.RemoveAll(); }
        catch { Preferences.Default.Clear(); }
    }
}
