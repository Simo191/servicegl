using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using MultiServices.Maui.Helpers;

namespace MultiServices.Maui.ViewModels.Services;

public partial class ServicesViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<ServiceProviderListDto> _providers = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private string? _selectedCategory;
    [ObservableProperty] private string? _selectedCity;
    [ObservableProperty] private bool _showFilters;
    [ObservableProperty] private ObservableCollection<CategoryItem> _categories = new();

    public ServicesViewModel(ApiService api)
    {
        _api = api;
        Title = "Services";
        foreach (var cat in AppConstants.ServiceCategories.All)
            Categories.Add(new CategoryItem { Key = cat.Key, Label = cat.Label, Icon = cat.Icon });
    }

    [RelayCommand]
    private async Task LoadProvidersAsync()
    {
        await ExecuteAsync(async () =>
        {
            // FIX: Backend params match ServicesController.SearchProviders:
            // category, minRating, city, maxPrice, sortBy, page, pageSize
            // Note: Backend does NOT have a "query" param for text search
            var queryParams = new Dictionary<string, string>
            {
                ["category"] = SelectedCategory ?? "",
                ["city"] = SelectedCity ?? "",
                ["page"] = "1",
                ["pageSize"] = "20"
            };

            var result = await _api.GetAsync<PaginatedResult<ServiceProviderListDto>>("/services/providers", queryParams);
            if (result.Success && result.Data != null)
            {
                Providers = new ObservableCollection<ServiceProviderListDto>(result.Data.Items);
                IsEmpty = !Providers.Any();
            }
        });
    }

    [RelayCommand]
    private async Task SelectCategoryAsync(string category)
    {
        SelectedCategory = category == SelectedCategory ? null : category;
        await LoadProvidersAsync();
    }

    [RelayCommand]
    private async Task SelectProviderAsync(ServiceProviderListDto provider)
    {
        await Shell.Current.GoToAsync($"servicedetail?id={provider.Id}");
    }
}

public class CategoryItem
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
