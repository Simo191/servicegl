using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Services.Auth;

namespace MultiServices.Maui.ViewModels.Auth;

public partial class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _showPassword;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        Title = "Connexion";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var (success, error) = await _authService.LoginAsync(Email, Password);
            if (success)
                await Shell.Current.GoToAsync("//main/home");
            else
                await Shell.Current.DisplayAlert("Erreur", error ?? "Identifiants incorrects", "OK");
        });
    }

    [RelayCommand]
    private async Task SocialLoginAsync(string provider)
    {
        await Shell.Current.DisplayAlert("Info", $"Connexion {provider} - Bientôt disponible", "OK");
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        await Shell.Current.GoToAsync("forgotpassword");
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("register");
    }

    [RelayCommand]
    private void TogglePassword() => ShowPassword = !ShowPassword;
}
