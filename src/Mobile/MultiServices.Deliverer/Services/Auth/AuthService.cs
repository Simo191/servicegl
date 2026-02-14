using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.Services.Storage;
namespace MultiServices.Deliverer.Services.Auth;
public class AuthService
{
    private readonly ApiService _api; private readonly ISecureStorageService _storage;
    public AuthService(ApiService api, ISecureStorageService storage) { _api = api; _storage = storage; }
    public async Task<bool> LoginAsync(string email, string password)
    {
        var r = await _api.PostAsync<LoginResponse>(AppConstants.LoginEndpoint, new LoginRequest { Email = email, Password = password });
        if (r == null) return false;
        await _storage.SetAsync(AppConstants.TokenKey, r.Token);
        await _storage.SetAsync(AppConstants.RefreshTokenKey, r.RefreshToken);
        await _storage.SetAsync(AppConstants.UserIdKey, r.UserId);
        return true;
    }
    public async Task<bool> RegisterAsync(RegisterRequest req) { var r = await _api.PostAsync<LoginResponse>(AppConstants.RegisterEndpoint, req); if (r == null) return false; await _storage.SetAsync(AppConstants.TokenKey, r.Token); await _storage.SetAsync(AppConstants.RefreshTokenKey, r.RefreshToken); return true; }
    public async Task<bool> CheckAuthAsync() => !string.IsNullOrEmpty(await _storage.GetAsync(AppConstants.TokenKey));
    public async Task<bool> IsOnboardingCompleteAsync() => (await _storage.GetAsync(AppConstants.OnboardingCompletedKey)) == "true";
    public async Task<bool> IsVerifiedAsync() => (await _storage.GetAsync(AppConstants.IsVerifiedKey)) == "true";
    public async Task LogoutAsync() => await _storage.ClearAllAsync();
}
