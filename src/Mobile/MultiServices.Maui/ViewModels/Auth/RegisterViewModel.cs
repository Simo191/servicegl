using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Services.Auth;

namespace MultiServices.Maui.ViewModels.Auth;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private bool _acceptTerms;

    public RegisterViewModel(AuthService authService)
    {
        _authService = authService;
        Title = "Inscription";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez remplir tous les champs obligatoires", "OK");
            return;
        }
        if (Password != ConfirmPassword)
        {
            await Shell.Current.DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "OK");
            return;
        }
        if (!AcceptTerms)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez accepter les conditions d'utilisation", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var (success, error) = await _authService.RegisterAsync(FirstName, LastName, Email, Password, Phone);
            if (success)
                await Shell.Current.GoToAsync("//main/home");
            else
                await Shell.Current.DisplayAlert("Erreur", error, "OK");
        });
    }

    [RelayCommand]
    private async Task GoToLoginAsync() => await Shell.Current.GoToAsync("..");
}
