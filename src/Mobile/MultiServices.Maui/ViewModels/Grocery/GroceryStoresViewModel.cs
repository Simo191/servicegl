using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Grocery;

public partial class GroceryStoresViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<GroceryStoreListDto> _stores = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private string? _selectedBrand;

    public string[] Brands => Helpers.AppConstants.GroceryBrands.All;

    public GroceryStoresViewModel(ApiService api)
    {
        _api = api;
        Title = "Courses";
    }

    [RelayCommand]
    private async Task LoadStoresAsync()
    {
        await ExecuteAsync(async () =>
        {
            var queryParams = new Dictionary<string, string>
            {
                ["query"] = SearchQuery,
                ["brand"] = SelectedBrand ?? ""
            };
            var result = await _api.GetAsync<PaginatedResult<GroceryStoreListDto>>("/grocery/stores", queryParams);
            if (result.Success && result.Data != null)
            {
                Stores = new ObservableCollection<GroceryStoreListDto>(result.Data.Items);
                IsEmpty = !Stores.Any();
            }
        });
    }

    [RelayCommand]
    private async Task SelectBrandAsync(string brand)
    {
        SelectedBrand = brand == SelectedBrand ? null : brand;
        await LoadStoresAsync();
    }

    [RelayCommand]
    private async Task SelectStoreAsync(GroceryStoreListDto store)
    {
        await Shell.Current.GoToAsync($"storedetail?id={store.Id}");
    }
}
