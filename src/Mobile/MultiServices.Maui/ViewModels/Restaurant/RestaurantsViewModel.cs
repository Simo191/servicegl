using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Restaurant;

public partial class RestaurantsViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<RestaurantListDto> _restaurants = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private string? _selectedCuisine;
    [ObservableProperty] private string? _selectedPriceRange;
    [ObservableProperty] private double? _minRating;
    [ObservableProperty] private bool _showFilters;
    [ObservableProperty] private ObservableCollection<string> _cuisineTypes = new(Helpers.AppConstants.CuisineTypes.All);

    public RestaurantsViewModel(ApiService api)
    {
        _api = api;
        Title = "Restaurants";
    }

    [RelayCommand]
    private async Task LoadRestaurantsAsync()
    {
        await ExecuteAsync(async () =>
        {
            // FIX: Backend param names: q, cuisine, price, minRating, hasPromo, sortBy
            var queryParams = new Dictionary<string, string>
            {
                ["q"] = SearchQuery,                          // Was: query
                ["cuisine"] = SelectedCuisine ?? "",          // Was: cuisineType
                ["price"] = SelectedPriceRange ?? "",         // Was: priceRange
                ["minRating"] = MinRating?.ToString() ?? "",
                ["page"] = "1",
                ["pageSize"] = "20"
            };

            var result = await _api.GetAsync<PaginatedResult<RestaurantListDto>>("/restaurants", queryParams);
            if (result.Success && result.Data != null)
            {
                Restaurants = new ObservableCollection<RestaurantListDto>(result.Data.Items);
                IsEmpty = !Restaurants.Any();
            }
        });
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadRestaurantsAsync();
    }

    [RelayCommand]
    private void ToggleFilters() => ShowFilters = !ShowFilters;

    [RelayCommand]
    private async Task SelectRestaurantAsync(RestaurantListDto restaurant)
    {
        await Shell.Current.GoToAsync($"restaurantdetail?id={restaurant.Id}");
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SelectedCuisine = null;
        SelectedPriceRange = null;
        MinRating = null;
    }
}
