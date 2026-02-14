using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private DelivererProfile? _profile;

    public ProfileViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Mon Profil";
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        await ExecuteAsync(async () =>
        {
            Profile = await _authService.GetDelivererProfileAsync();
        });
    }

    [RelayCommand]
    private async Task EditProfileAsync() => await Shell.Current.GoToAsync("editProfile");

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Déconnexion", "Voulez-vous vous déconnecter ?", "Oui", "Non");
        if (confirm) await _authService.LogoutAsync();
    }

    [RelayCommand]
    private async Task SOSAsync() => await Shell.Current.GoToAsync("sos");
}