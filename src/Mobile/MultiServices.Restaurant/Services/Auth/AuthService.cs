using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api; using MultiServices.Restaurant.Services.Storage;
namespace MultiServices.Restaurant.Services.Auth;
public class AuthService
{
    private readonly ApiService _api; private readonly ISecureStorageService _storage; private UserDto? _currentUser;
    public AuthService(ApiService api, ISecureStorageService storage) { _api = api; _storage = storage; }
    public UserDto? CurrentUser => _currentUser; public bool IsAuthenticated => _currentUser != null;
    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    { var r = await _api.PostAsync<AuthResponse>("/auth/login", new { email, password }); if (r.Success && r.Data != null) { await _storage.SetAsync("auth_token", r.Data.Token); await _storage.SetAsync("refresh_token", r.Data.RefreshToken); _currentUser = r.Data.User; return (true, null); } return (false, r.Message ?? "Identifiants incorrects"); }
    public async Task<(bool Success, string? Error)> RegisterAsync(object registration) { var r = await _api.PostAsync<AuthResponse>("/auth/register", registration); if (r.Success && r.Data != null) { await _storage.SetAsync("auth_token", r.Data.Token); await _storage.SetAsync("refresh_token", r.Data.RefreshToken); _currentUser = r.Data.User; return (true, null); } return (false, r.Message); }
    public async Task<bool> CheckAuthAsync() { var token = await _storage.GetAsync("auth_token"); if (string.IsNullOrEmpty(token)) return false; var r = await _api.GetAsync<UserDto>("/auth/me"); if (r.Success && r.Data != null) { _currentUser = r.Data; return true; } return false; }
    public async Task LogoutAsync() { _storage.Remove("auth_token"); _storage.Remove("refresh_token"); _currentUser = null; await Shell.Current.GoToAsync("//login"); }
}