using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Services.Auth;

namespace MultiServices.Maui.ViewModels.Common;

/// <summary>
/// REMPLACE HomeViewModel.cs existant.
/// Changements: affiche l'adresse en haut, ne nécessite PAS d'authentification.
/// </summary>
public partial class HomeViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly AuthService _authService;
    private readonly AddressService _addressService;

    [ObservableProperty] private string _greeting = string.Empty;
    [ObservableProperty] private string _currentAddress = string.Empty;
    [ObservableProperty] private ObservableCollection<RestaurantListDto> _popularRestaurants = new();
    [ObservableProperty] private ObservableCollection<ServiceProviderListDto> _topProviders = new();
    [ObservableProperty] private ObservableCollection<GroceryStoreListDto> _nearbyStores = new();

    public HomeViewModel(ApiService api, AuthService authService, AddressService addressService)
    {
        _api = api; _authService = authService; _addressService = addressService;
        Title = "MultiServices";
        SetGreeting();
        CurrentAddress = _addressService.CurrentAddress?.FullAddress ?? "Choisir une adresse";
    }

    private void SetGreeting()
    {
        var hour = DateTime.Now.Hour;
        // Pas besoin d'être connecté pour avoir un greeting
        var name = _authService.CurrentUser?.FirstName ?? "";
        Greeting = hour switch { < 12 => $"Bonjour {name} 👋", < 18 => $"Bon après-midi {name} 👋", _ => $"Bonsoir {name} 👋" };
    }

    [RelayCommand]
    private async Task LoadHomeDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var addr = _addressService.CurrentAddress;
            var qp = new Dictionary<string, string> { ["pageSize"] = "5" };
            if (addr != null) { qp["latitude"] = addr.Latitude.ToString(); qp["longitude"] = addr.Longitude.ToString(); }

            var restaurants = await _api.GetAsync<PaginatedResult<RestaurantListDto>>("/restaurants", qp);
            if (restaurants.Success && restaurants.Data != null) PopularRestaurants = new(restaurants.Data.Items);

            var providers = await _api.GetAsync<PaginatedResult<ServiceProviderListDto>>("/services/providers", qp);
            if (providers.Success && providers.Data != null) TopProviders = new(providers.Data.Items);

            var stores = await _api.GetAsync<PaginatedResult<GroceryStoreListDto>>("/grocery/stores", qp);
            if (stores.Success && stores.Data != null) NearbyStores = new(stores.Data.Items);
        });
    }

    [RelayCommand]
    private async Task ChangeAddressAsync()
    {
        // Retourner à l'écran de saisie d'adresse
        await Shell.Current.GoToAsync("//address");
    }

    [RelayCommand] private async Task GoToRestaurantsAsync() => await Shell.Current.GoToAsync("//main/restaurants");
    [RelayCommand] private async Task GoToServicesAsync() => await Shell.Current.GoToAsync("//main/services");
    [RelayCommand] private async Task GoToGroceryAsync() => await Shell.Current.GoToAsync("//main/grocery");
    [RelayCommand] private async Task GoToNotificationsAsync() => await Shell.Current.GoToAsync("notifications");
}
