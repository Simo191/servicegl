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
                MenuCategories = new ObservableCollection<MenuCategoryDto>(result.Data.MenuCategories);
                if (MenuCategories.Any())
                    SelectedCategory = MenuCategories.First();
                
                Cart.RestaurantId = result.Data.Id;
                Cart.RestaurantName = result.Data.Name;
                Cart.RestaurantLogoUrl = result.Data.LogoUrl;
                Cart.DeliveryFee = result.Data.DeliveryFee;
                Cart.MinimumOrder = result.Data.MinimumOrder;
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
                BasePrice = item.BasePrice,
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

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        IsFavorite = !IsFavorite;
        if (IsFavorite)
            await _api.PostAsync<object>($"/favorites/restaurants/{RestaurantId}");
        else
            await _api.DeleteAsync<object>($"/favorites/restaurants/{RestaurantId}");
    }
}
