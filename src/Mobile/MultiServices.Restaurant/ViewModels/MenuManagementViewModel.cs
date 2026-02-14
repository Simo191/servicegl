using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api;
namespace MultiServices.Restaurant.ViewModels;
public partial class MenuManagementViewModel : BaseViewModel
{
    private readonly ApiService _api;
    [ObservableProperty] private ObservableCollection<MenuCategoryDto> _categories = new();
    [ObservableProperty] private MenuCategoryDto? _selectedCategory;
    [ObservableProperty] private ObservableCollection<MenuItemDto> _items = new();
    public MenuManagementViewModel(ApiService api) { _api = api; Title = "Gestion du Menu"; }

    [RelayCommand] private async Task LoadMenuAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<List<MenuCategoryDto>>("/restaurant/menu");
        if (r.Success && r.Data != null) { Categories = new(r.Data); if (r.Data.Any()) { SelectedCategory = r.Data[0]; Items = new(r.Data[0].Items); } }
    }); }
    [RelayCommand] private async Task SelectCategoryAsync(MenuCategoryDto cat) { SelectedCategory = cat; Items = new(cat.Items); }
    [RelayCommand] private async Task AddCategoryAsync() => await Shell.Current.GoToAsync("categoryform");
    [RelayCommand] private async Task EditCategoryAsync(MenuCategoryDto cat) => await Shell.Current.GoToAsync($"categoryform?id={cat.Id}");
    [RelayCommand] private async Task DeleteCategoryAsync(MenuCategoryDto cat) { if (await Shell.Current.DisplayAlert("Confirmer", $"Supprimer la catégorie '{cat.Name}' et tous ses plats?", "Oui", "Non")) { await _api.DeleteAsync<object>($"/restaurant/menu/categories/{cat.Id}"); await LoadMenuAsync(); } }
    [RelayCommand] private async Task AddItemAsync() => await Shell.Current.GoToAsync($"menuitemform?categoryId={SelectedCategory?.Id}");
    [RelayCommand] private async Task EditItemAsync(MenuItemDto item) => await Shell.Current.GoToAsync($"menuitemform?id={item.Id}");
    [RelayCommand] private async Task ToggleAvailabilityAsync(MenuItemDto item) { await _api.PostAsync<object>($"/restaurant/menu/items/{item.Id}/toggle-availability"); item.IsAvailable = !item.IsAvailable; }
    [RelayCommand] private async Task TogglePopularAsync(MenuItemDto item) { await _api.PutAsync<object>($"/restaurant/menu/items/{item.Id}", new { isPopular = !item.IsPopular }); item.IsPopular = !item.IsPopular; }
    [RelayCommand] private async Task DeleteItemAsync(MenuItemDto item) { if (await Shell.Current.DisplayAlert("Confirmer", $"Supprimer '{item.Name}'?", "Oui", "Non")) { await _api.DeleteAsync<object>($"/restaurant/menu/items/{item.Id}"); await LoadMenuAsync(); } }
    [RelayCommand] private async Task UploadItemPhotoAsync(MenuItemDto item) { var result = await MediaPicker.Default.PickPhotoAsync(); if (result != null) { using var stream = await result.OpenReadAsync(); await _api.UploadAsync<object>($"/restaurant/menu/items/{item.Id}/image", stream, result.FileName); await LoadMenuAsync(); } }
}