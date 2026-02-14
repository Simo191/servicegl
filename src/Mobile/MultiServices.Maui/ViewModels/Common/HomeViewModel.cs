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
        var name = _authService.CurrentUser?.FirstName ?? "vous";
        Greeting = hour switch
        {
            < 12 => $"Bonjour {name} 👋",
            < 18 => $"Bon après-midi {name} 👋",
            _ => $"Bonsoir {name} 👋"
        };
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Restaurants: GET /restaurants/popular?count=6
            var restaurantsTask = _api.GetAsync<List<RestaurantListDto>>("/restaurants/popular",
                new Dictionary<string, string> { ["count"] = "6" });

            // Services: GET /services/providers?page=1&pageSize=6
            var providersTask = _api.GetAsync<PaginatedResult<ServiceProviderListDto>>("/services/providers",
                new Dictionary<string, string> { ["page"] = "1", ["pageSize"] = "6" });

            // Grocery: GET /grocery/stores?page=1&pageSize=6
            var storesTask = _api.GetAsync<PaginatedResult<GroceryStoreListDto>>("/grocery/stores",
                new Dictionary<string, string> { ["page"] = "1", ["pageSize"] = "6" });

            await Task.WhenAll(restaurantsTask, providersTask, storesTask);

            var restaurantsResult = await restaurantsTask;
            if (restaurantsResult.Success && restaurantsResult.Data != null)
                PopularRestaurants = new ObservableCollection<RestaurantListDto>(restaurantsResult.Data);

            var providersResult = await providersTask;
            if (providersResult.Success && providersResult.Data != null)
                TopProviders = new ObservableCollection<ServiceProviderListDto>(providersResult.Data.Items);

            var storesResult = await storesTask;
            if (storesResult.Success && storesResult.Data != null)
                NearbyStores = new ObservableCollection<GroceryStoreListDto>(storesResult.Data.Items);
        });
    }

    [RelayCommand]
    private async Task GoToRestaurantsAsync() => await Shell.Current.GoToAsync("//restaurants");

    [RelayCommand]
    private async Task GoToServicesAsync() => await Shell.Current.GoToAsync("//services");

    [RelayCommand]
    private async Task GoToGroceryAsync() => await Shell.Current.GoToAsync("//grocery");

    [RelayCommand]
    private async Task GoToOrdersAsync() => await Shell.Current.GoToAsync("//main/orders");

    [RelayCommand]
    private async Task GoToNotificationsAsync() => await Shell.Current.GoToAsync("notifications");

    [RelayCommand]
    private async Task SelectRestaurantAsync(RestaurantListDto restaurant)
    {
        await Shell.Current.GoToAsync($"restaurantdetail?id={restaurant.Id}");
    }

    [RelayCommand]
    private async Task SelectProviderAsync(ServiceProviderListDto provider)
    {
        await Shell.Current.GoToAsync($"servicedetail?id={provider.Id}");
    }

    [RelayCommand]
    private async Task SelectStoreAsync(GroceryStoreListDto store)
    {
        await Shell.Current.GoToAsync($"storedetail?id={store.Id}");
    }
}
