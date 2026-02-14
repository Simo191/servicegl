namespace MultiServices.Deliverer.Services.Storage;
public interface ISecureStorageService { Task<string?> GetAsync(string key); Task SetAsync(string key, string value); Task RemoveAsync(string key); Task ClearAllAsync(); }
public class SecureStorageService : ISecureStorageService
{
    public async Task<string?> GetAsync(string key) { try { return await SecureStorage.Default.GetAsync(key); } catch { return Preferences.Default.Get(key, (string?)null); } }
    public async Task SetAsync(string key, string value) { try { await SecureStorage.Default.SetAsync(key, value); } catch { Preferences.Default.Set(key, value); } }
    public Task RemoveAsync(string key) { try { SecureStorage.Default.Remove(key); } catch { Preferences.Default.Remove(key); } return Task.CompletedTask; }
    public Task ClearAllAsync() { try { SecureStorage.Default.RemoveAll(); } catch { Preferences.Default.Clear(); } return Task.CompletedTask; }
}
