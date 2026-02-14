using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels.Onboarding;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _rememberMe;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Connexion Livreur";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez remplir tous les champs";
            HasError = true;
            return;
        }

        await ExecuteAsync(async () =>
        {
            var success = await _authService.LoginAsync(Email, Password);
            if (success)
            {
                var profile = await _authService.GetDelivererProfileAsync();
                if (profile == null)
                {
                    ErrorMessage = "Profil livreur introuvable";
                    HasError = true;
                    return;
                }

                if (!profile.DocumentsUploaded)
                    await Shell.Current.GoToAsync("//documentUpload");
                else if (!profile.HasTrainingCompleted)
                    await Shell.Current.GoToAsync("//training");
                else if (profile.KycStatus == "Pending")
                    await Shell.Current.GoToAsync("//pendingVerification");
                else if (profile.KycStatus == "Rejected")
                    await Shell.Current.GoToAsync("//documentUpload");
                else
                    await Shell.Current.GoToAsync("//main/dashboard");
            }
            else
            {
                ErrorMessage = "Email ou mot de passe incorrect";
                HasError = true;
            }
        }, "Erreur de connexion");
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("register");
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            await Shell.Current.DisplayAlert("Info", "Entrez votre email d'abord", "OK");
            return;
        }
        await Shell.Current.DisplayAlert("Envoyé", "Un lien de réinitialisation a été envoyé à votre email", "OK");
    }
}