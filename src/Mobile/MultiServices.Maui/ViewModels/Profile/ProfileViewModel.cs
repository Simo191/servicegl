using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Services.Auth;
using System.Collections.ObjectModel;

namespace MultiServices.Maui.ViewModels.Profile;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly AuthService _authService;

    [ObservableProperty] private UserDto? _user;
    [ObservableProperty] private ObservableCollection<AddressDto> _addresses = new();
    [ObservableProperty] private WalletDto? _wallet;

    public ProfileViewModel(ApiService api, AuthService authService)
    {
        _api = api;
        _authService = authService;
        Title = "Mon profil";
        User = _authService.CurrentUser;
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        await ExecuteAsync(async () =>
        {
            var profileTask = _api.GetAsync<UserDto>("/profile");
            var addressesTask = _api.GetAsync<List<AddressDto>>("/profile/addresses");
            var walletTask = _api.GetAsync<WalletDto>("/profile/wallet");

            await Task.WhenAll(profileTask, addressesTask, walletTask);

            var profile = await profileTask;
            if (profile.Success && profile.Data != null)
                User = profile.Data;

            var addresses = await addressesTask;
            if (addresses.Success && addresses.Data != null)
                Addresses = new ObservableCollection<AddressDto>(addresses.Data);

            var wallet = await walletTask;
            if (wallet.Success && wallet.Data != null)
                Wallet = wallet.Data;
        });
    }

    [RelayCommand]
    private async Task EditProfileAsync() => await Shell.Current.GoToAsync("editprofile");

    [RelayCommand]
    private async Task ManageAddressesAsync() => await Shell.Current.GoToAsync("addresses");

    [RelayCommand]
    private async Task ViewWalletAsync() => await Shell.Current.GoToAsync("wallet");

    [RelayCommand]
    private async Task ViewFavoritesAsync() => await Shell.Current.GoToAsync("favorites");

    [RelayCommand]
    private async Task ViewLoyaltyAsync() => await Shell.Current.GoToAsync("loyalty");

    [RelayCommand]
    private async Task ViewNotificationsSettingsAsync() => await Shell.Current.GoToAsync("notificationsettings");

    [RelayCommand]
    private async Task ContactSupportAsync() => await Shell.Current.GoToAsync("support");

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Déconnexion", "Voulez-vous vous déconnecter ?", "Oui", "Non");
        if (confirm)
            await _authService.LogoutAsync();
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Supprimer le compte",
            "Cette action est irréversible. Voulez-vous continuer ?", "Supprimer", "Annuler");
        if (confirm)
        {
            await _api.DeleteAsync<object>("/profile");
            await _authService.LogoutAsync();
        }
    }
}
