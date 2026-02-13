using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Services.Auth;

namespace MultiServices.Maui.ViewModels.Common;

public partial class HomeViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly AuthService _authService;

    [ObservableProperty] private string _greeting = string.Empty;
    [ObservableProperty] private ObservableCollection<RestaurantListDto> _popularRestaurants = new();
    [ObservableProperty] private ObservableCollection<ServiceProviderListDto> _topProviders = new();
    [ObservableProperty] private ObservableCollection<GroceryStoreListDto> _nearbyStores = new();
    [ObservableProperty] private int _activeOrdersCount;

    public HomeViewModel(ApiService api, AuthService authService)
    {
        _api = api;
        _authService = authService;
        Title = "MultiServices";
        SetGreeting();
    }

    private void SetGreeting()
    {
        var hour = DateTime.Now.Hour;
        var name = _authService.CurrentUser?.FirstName ?? "";
        Greeting = hour switch
        {
            < 12 => $"Bonjour {name} 👋",
            < 18 => $"Bon après-midi {name} 👋",
            _ => $"Bonsoir {name} 👋"
        };
    }

    [RelayCommand]
    private async Task LoadHomeDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var restaurantsTask = _api.GetAsync<PaginatedResult<RestaurantListDto>>("/restaurants",
                new Dictionary<string, string> { ["pageSize"] = "5", ["sortBy"] = "popularity" });
            var providersTask = _api.GetAsync<PaginatedResult<ServiceProviderListDto>>("/services/providers",
                new Dictionary<string, string> { ["pageSize"] = "5", ["sortBy"] = "rating" });
            var storesTask = _api.GetAsync<PaginatedResult<GroceryStoreListDto>>("/grocery/stores",
                new Dictionary<string, string> { ["pageSize"] = "5" });

            await Task.WhenAll(restaurantsTask, providersTask, storesTask);

            var restaurants = await restaurantsTask;
            if (restaurants.Success && restaurants.Data != null)
                PopularRestaurants = new ObservableCollection<RestaurantListDto>(restaurants.Data.Items);

            var providers = await providersTask;
            if (providers.Success && providers.Data != null)
                TopProviders = new ObservableCollection<ServiceProviderListDto>(providers.Data.Items);

            var stores = await storesTask;
            if (stores.Success && stores.Data != null)
                NearbyStores = new ObservableCollection<GroceryStoreListDto>(stores.Data.Items);
        });
    }

    [RelayCommand]
    private async Task GoToRestaurantsAsync() => await Shell.Current.GoToAsync("//main/restaurants");

    [RelayCommand]
    private async Task GoToServicesAsync() => await Shell.Current.GoToAsync("//main/services");

    [RelayCommand]
    private async Task GoToGroceryAsync() => await Shell.Current.GoToAsync("//main/grocery");

    [RelayCommand]
    private async Task GoToOrdersAsync() => await Shell.Current.GoToAsync("//main/orders");

    [RelayCommand]
    private async Task GoToNotificationsAsync() => await Shell.Current.GoToAsync("notifications");
}
