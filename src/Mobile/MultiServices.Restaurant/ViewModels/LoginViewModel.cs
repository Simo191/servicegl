using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input; using MultiServices.Restaurant.Services.Auth;
namespace MultiServices.Restaurant.ViewModels;
public partial class LoginViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    public LoginViewModel(AuthService auth) { _auth = auth; Title = "Connexion"; }
    [RelayCommand] private async Task LoginAsync() { await ExecuteAsync(async () => { var (s, e) = await _auth.LoginAsync(Email, Password); if (s) await Shell.Current.GoToAsync("//main/dashboard"); else { HasError = true; ErrorMessage = e; } }); }
}