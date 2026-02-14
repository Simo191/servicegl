using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Services.Storage;

namespace MultiServices.Maui.Services.Auth;

public class AuthService
{
    private readonly ApiService _api;
    private readonly ISecureStorageService _secureStorage;
    private UserDto? _currentUser;

    public AuthService(ApiService api, ISecureStorageService secureStorage)
    {
        _api = api;
        _secureStorage = secureStorage;
    }

    public UserDto? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        var result = await _api.PostAsync<AuthResponse>("/auth/login", new { email, password });
        if (result.Success && result.Data != null)
        {
            await SaveTokensAsync(result.Data);
            _currentUser = result.Data.User;
            return (true, null);
        }
        return (false, result.Data?.Errors?.FirstOrDefault() ?? result.Message ?? "Identifiants incorrects");
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string firstName, string lastName, string email, string password, string phone)
    {
        var result = await _api.PostAsync<AuthResponse>("/auth/register", new
        {
            firstName,
            lastName,
            email,
            password,
            confirmPassword = password,
            phoneNumber = phone
        });
        if (result.Success && result.Data != null)
        {
            await SaveTokensAsync(result.Data);
            _currentUser = result.Data.User;
            return (true, null);
        }
        return (false, result.Data?.Errors?.FirstOrDefault() ?? result.Message ?? "Erreur lors de l'inscription");
    }

    /// <summary>
    /// FIX: Backend endpoint is /auth/external-login (not /auth/social-login)
    /// Backend expects: { provider, accessToken, deviceToken? }
    /// </summary>
    public async Task<(bool Success, string? Error)> SocialLoginAsync(string provider, string token)
    {
        var result = await _api.PostAsync<AuthResponse>("/auth/external-login", new
        {
            provider,
            accessToken = token
        });
        if (result.Success && result.Data != null)
        {
            await SaveTokensAsync(result.Data);
            _currentUser = result.Data.User;
            return (true, null);
        }
        return (false, result.Message ?? "Erreur de connexion sociale");
    }

    /// <summary>
    /// FIX: Backend endpoint is /auth/profile (not /auth/me)
    /// </summary>
    public async Task<bool> CheckAuthAsync()
    {
        var token = await _secureStorage.GetAsync("auth_token");
        if (string.IsNullOrEmpty(token)) return false;

        var result = await _api.GetAsync<UserDto>("/auth/profile");
        if (result.Success && result.Data != null)
        {
            _currentUser = result.Data;
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        await _api.PostAsync<object>("/auth/logout");
        _secureStorage.Remove("auth_token");
        _secureStorage.Remove("refresh_token");
        _secureStorage.Remove("user_data");
        _currentUser = null;
        await Shell.Current.GoToAsync("//login");
    }

    public async Task<(bool Success, string? Error)> ForgotPasswordAsync(string email)
    {
        var result = await _api.PostAsync<object>("/auth/forgot-password", new { email });
        return (result.Success, result.Message);
    }

    /// <summary>
    /// FIX: Backend endpoint is /auth/verify-otp (not /auth/verify-phone)
    /// Backend expects: { phoneNumber, otp }
    /// </summary>
    public async Task<(bool Success, string? Error)> VerifyPhoneAsync(string phoneNumber, string code)
    {
        var result = await _api.PostAsync<object>("/auth/verify-otp", new { phoneNumber, otp = code });
        return (result.Success, result.Message);
    }

    public async Task<(bool Success, string? Error)> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var result = await _api.PostAsync<object>("/auth/change-password", new
        {
            currentPassword,
            newPassword,
            confirmPassword = newPassword
        });
        return (result.Success, result.Message);
    }

    /// <summary>
    /// FIX: Use GetToken() to handle both Token and AccessToken from backend
    /// </summary>
    private async Task SaveTokensAsync(AuthResponse auth)
    {
        var token = auth.GetToken();
        if (!string.IsNullOrEmpty(token))
            await _secureStorage.SetAsync("auth_token", token);

        if (!string.IsNullOrEmpty(auth.RefreshToken))
            await _secureStorage.SetAsync("refresh_token", auth.RefreshToken);
    }
}
