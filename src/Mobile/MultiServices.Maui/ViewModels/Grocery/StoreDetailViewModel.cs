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
    [ObservableProperty] private ObservableCollection<GroceryDepartmentDto> _departments = new();
    [ObservableProperty] private GroceryDepartmentDto? _selectedDepartment;
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
                Departments = new ObservableCollection<GroceryDepartmentDto>(result.Data.Departments);
                Cart.StoreId = result.Data.Id;
                Cart.StoreName = result.Data.Name;
                Cart.StoreLogoUrl = result.Data.LogoUrl;
                Cart.DeliveryFee = result.Data.DeliveryFee;
                Cart.MinimumOrder = result.Data.MinimumOrder;
            }
        });
    }

    [RelayCommand]
    private async Task SelectDepartmentAsync(GroceryDepartmentDto dept)
    {
        SelectedDepartment = dept;
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var queryParams = new Dictionary<string, string>
            {
                ["query"] = SearchQuery,
                ["departmentId"] = SelectedDepartment?.Id.ToString() ?? "",
                ["isBio"] = FilterBio ? "true" : "",
                ["isHalal"] = FilterHalal ? "true" : "",
                ["isOnPromotion"] = FilterPromo ? "true" : ""
            };
            var result = await _api.GetAsync<PaginatedResult<GroceryProductDto>>($"/grocery/stores/{StoreId}/products", queryParams);
            if (result.Success && result.Data != null)
                Products = new ObservableCollection<GroceryProductDto>(result.Data.Items);
        });
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
        await Shell.Current.GoToAsync("grocerycheckout");
    }
}
