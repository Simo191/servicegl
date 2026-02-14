using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Restaurant;

[QueryProperty(nameof(RestaurantId), "id")]
public partial class RestaurantDetailViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _restaurantId = string.Empty;
    [ObservableProperty] private RestaurantDto? _restaurant;
    [ObservableProperty] private ObservableCollection<MenuCategoryDto> _menuCategories = new();
    [ObservableProperty] private MenuCategoryDto? _selectedCategory;
    [ObservableProperty] private RestaurantCart _cart = new();
    [ObservableProperty] private bool _showCart;
    [ObservableProperty] private bool _isFavorite;

    public RestaurantDetailViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnRestaurantIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadRestaurantCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadRestaurantAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<RestaurantDto>($"/restaurants/{RestaurantId}");
            if (result.Success && result.Data != null)
            {
                Restaurant = result.Data;
                Title = result.Data.Name;

                // FIX: Backend uses "Menu" not "MenuCategories"
                MenuCategories = new ObservableCollection<MenuCategoryDto>(result.Data.Menu);
                if (MenuCategories.Any())
                    SelectedCategory = MenuCategories.First();

                Cart.RestaurantId = result.Data.Id;
                Cart.RestaurantName = result.Data.Name;
                Cart.RestaurantLogoUrl = result.Data.LogoUrl;
                Cart.DeliveryFee = result.Data.DeliveryFee;
                // FIX: Backend uses "MinOrderAmount" not "MinimumOrder"
                Cart.MinimumOrder = result.Data.MinOrderAmount;
            }
        });
    }

    [RelayCommand]
    private void AddToCart(MenuItemDto item)
    {
        var existingItem = Cart.Items.FirstOrDefault(i => i.MenuItemId == item.Id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            Cart.Items.Add(new RestaurantCartItem
            {
                MenuItemId = item.Id,
                Name = item.Name,
                ImageUrl = item.ImageUrl,
                // FIX: Backend uses "Price" not "BasePrice"
                BasePrice = item.Price,
                Quantity = 1
            });
        }
        OnPropertyChanged(nameof(Cart));
    }

    [RelayCommand]
    private void RemoveFromCart(RestaurantCartItem item)
    {
        if (item.Quantity > 1)
            item.Quantity--;
        else
            Cart.Items.Remove(item);
        OnPropertyChanged(nameof(Cart));
    }

    [RelayCommand]
    private void ToggleCart() => ShowCart = !ShowCart;

    [RelayCommand]
    private async Task ProceedToCheckoutAsync()
    {
        if (!Cart.MeetsMinimum)
        {
            await Shell.Current.DisplayAlert("Minimum non atteint",
                $"Le minimum de commande est {Cart.MinimumOrder:N2} DH", "OK");
            return;
        }
        await Shell.Current.GoToAsync("checkout");
    }

    /// <summary>
    /// FIX: Backend uses POST /restaurants/{id}/favorite as toggle (not separate POST/DELETE)
    /// </summary>
    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        var result = await _api.PostAsync<object>($"/restaurants/{RestaurantId}/favorite");
        if (result.Success)
            IsFavorite = !IsFavorite;
    }
}
