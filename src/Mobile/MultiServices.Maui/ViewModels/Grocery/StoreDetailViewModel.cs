using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Grocery;

[QueryProperty(nameof(StoreId), "id")]
public partial class StoreDetailViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _storeId = string.Empty;
    [ObservableProperty] private GroceryStoreDto? _store;
    // Backend uses Categories (StoreCategoryDto), not Departments
    [ObservableProperty] private ObservableCollection<StoreCategoryDto> _categories = new();
    [ObservableProperty] private StoreCategoryDto? _selectedCategory;
    [ObservableProperty] private ObservableCollection<GroceryProductDto> _products = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private GroceryCart _cart = new();
    [ObservableProperty] private bool _showCart;
    [ObservableProperty] private bool _filterBio;
    [ObservableProperty] private bool _filterHalal;
    [ObservableProperty] private bool _filterPromo;

    public StoreDetailViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnStoreIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadStoreCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadStoreAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<GroceryStoreDto>($"/grocery/stores/{StoreId}");
            if (result.Success && result.Data != null)
            {
                Store = result.Data;
                Title = result.Data.Name;

                // Backend uses "Categories" not "Departments"
                Categories = new ObservableCollection<StoreCategoryDto>(result.Data.Categories);

                Cart.StoreId = result.Data.Id;
                Cart.StoreName = result.Data.Name;
                Cart.StoreLogoUrl = result.Data.LogoUrl;
                Cart.DeliveryFee = result.Data.DeliveryFee;
                Cart.MinimumOrder = result.Data.MinOrderAmount;
            }
        });
    }

    [RelayCommand]
    private async Task SelectCategoryAsync(StoreCategoryDto category)
    {
        SelectedCategory = category;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var queryParams = new Dictionary<string, string>
            {
                ["q"] = SearchQuery,
                ["categoryId"] = SelectedCategory?.Id.ToString() ?? "",
                ["isBio"] = FilterBio ? "true" : "",
                ["isHalal"] = FilterHalal ? "true" : "",
                ["onPromo"] = FilterPromo ? "true" : "",
                ["page"] = "1",
                ["pageSize"] = "20"
            };

            var result = await _api.GetAsync<PaginatedResult<GroceryProductDto>>(
                $"/grocery/stores/{StoreId}/products", queryParams);
            if (result.Success && result.Data != null)
                Products = new ObservableCollection<GroceryProductDto>(result.Data.Items);
        });
    }

    [RelayCommand]
    private async Task ToggleBioFilterAsync()
    {
        FilterBio = !FilterBio;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task ToggleHalalFilterAsync()
    {
        FilterHalal = !FilterHalal;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task TogglePromoFilterAsync()
    {
        FilterPromo = !FilterPromo;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private void AddToCart(GroceryProductDto product)
    {
        var existing = Cart.Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
            existing.Quantity++;
        else
            Cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.IsOnPromotion ? product.PromotionPrice ?? product.Price : product.Price,
                Quantity = 1
            });
        OnPropertyChanged(nameof(Cart));
    }

    [RelayCommand]
    private void RemoveFromCart(CartItem item)
    {
        if (item.Quantity > 1) item.Quantity--;
        else Cart.Items.Remove(item);
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
        await Shell.Current.GoToAsync("grocerycheckout");
    }

    [RelayCommand]
    private async Task ScanBarcodeAsync()
    {
        // ZXing scanner integration
        await Task.CompletedTask;
    }
}
