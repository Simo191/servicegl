using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Services.Auth;
using MultiServices.Maui.Services.Storage;

namespace MultiServices.Maui.ViewModels.Checkout;

/// <summary>
/// Inscription simplifiée au moment du checkout (style Glovo).
/// Étape 1: Prénom, Nom, Téléphone, Email
/// Étape 2: Validation SMS OTP
/// Puis retour au checkout pour le paiement.
/// </summary>
public partial class CheckoutAuthViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly AuthService _authService;
    private readonly ISecureStorageService _storage;

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _otpCode = string.Empty;
    [ObservableProperty] private bool _isStep1 = true;
    [ObservableProperty] private bool _isStep2;
    [ObservableProperty] private bool _canResend;
    [ObservableProperty] private bool _isTimerRunning;
    [ObservableProperty] private int _resendTimer = 60;

    public CheckoutAuthViewModel(ApiService api, AuthService authService, ISecureStorageService storage)
    {
        _api = api; _authService = authService; _storage = storage;
        Title = "Inscription rapide";
    }

    [RelayCommand]
    private async Task SendOtpAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName)
            || string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Email))
        {
            HasError = true; ErrorMessage = "Tous les champs sont obligatoires";
            return;
        }

        await ExecuteAsync(async () =>
        {
            var result = await _api.PostAsync<object>("/auth/send-otp", new OtpRequest { PhoneNumber = Phone });
            if (result.Success)
            {
                IsStep1 = false; IsStep2 = true;
                StartResendTimer();
            }
            else { HasError = true; ErrorMessage = result.Message; }
        });
    }

    [RelayCommand]
    private async Task VerifyOtpAsync()
    {
        if (string.IsNullOrWhiteSpace(OtpCode) || OtpCode.Length != 6)
        {
            HasError = true; ErrorMessage = "Entrez le code à 6 chiffres"; return;
        }

        await ExecuteAsync(async () =>
        {
            // Inscription rapide avec vérification OTP
            var result = await _api.PostAsync<AuthResponse>("/auth/quick-register", new QuickRegisterRequest
            {
                FirstName = FirstName, LastName = LastName,
                PhoneNumber = Phone, Email = Email, OtpCode = OtpCode
            });

            if (result.Success && result.Data != null)
            {
                await _storage.SetAsync("auth_token", result.Data.Token);
                await _storage.SetAsync("refresh_token", result.Data.RefreshToken);
                // Retour au checkout
                await Shell.Current.GoToAsync("..");
            }
            else { HasError = true; ErrorMessage = result.Message ?? "Code invalide"; }
        });
    }

    [RelayCommand]
    private async Task ResendOtpAsync()
    {
        await _api.PostAsync<object>("/auth/send-otp", new OtpRequest { PhoneNumber = Phone });
        StartResendTimer();
    }

    private async void StartResendTimer()
    {
        CanResend = false; IsTimerRunning = true; ResendTimer = 60;
        while (ResendTimer > 0)
        {
            await Task.Delay(1000);
            ResendTimer--;
        }
        CanResend = true; IsTimerRunning = false;
    }
}
