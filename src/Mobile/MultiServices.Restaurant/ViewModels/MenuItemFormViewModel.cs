using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api;
namespace MultiServices.Restaurant.ViewModels;
[QueryProperty(nameof(ItemId), "id")]
[QueryProperty(nameof(CategoryIdParam), "categoryId")]
public partial class MenuItemFormViewModel : BaseViewModel
{
    private readonly ApiService _api;
    [ObservableProperty] private string? _itemId;
    [ObservableProperty] private string? _categoryIdParam;
    [ObservableProperty] private bool _isEditing;
    // Form fields
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _price = string.Empty;
    [ObservableProperty] private bool _isPopular;
    [ObservableProperty] private bool _isAvailable = true;
    [ObservableProperty] private string _allergensText = string.Empty;
    [ObservableProperty] private string _nutritionalInfo = string.Empty;
    [ObservableProperty] private string _calorieCount = string.Empty;
    // Sizes & Extras
    [ObservableProperty] private ObservableCollection<MenuItemSizeDto> _sizes = new();
    [ObservableProperty] private ObservableCollection<MenuItemExtraDto> _extras = new();

    public MenuItemFormViewModel(ApiService api) { _api = api; Title = "Nouveau plat"; }

    partial void OnItemIdChanged(string? v) { if (!string.IsNullOrEmpty(v)) { IsEditing = true; Title = "Modifier le plat"; LoadItemCommand.ExecuteAsync(null); } }

    [RelayCommand] private async Task LoadItemAsync() { if (ItemId == null) return; await ExecuteAsync(async () => {
        var r = await _api.GetAsync<MenuItemDto>($"/restaurant/menu/items/{ItemId}");
        if (r.Success && r.Data != null) { var d = r.Data; Name = d.Name; Description = d.Description ?? ""; Price = d.BasePrice.ToString(); IsPopular = d.IsPopular; IsAvailable = d.IsAvailable;
            AllergensText = string.Join(", ", d.Allergens); NutritionalInfo = d.NutritionalInfo ?? ""; CalorieCount = d.CalorieCount.ToString();
            Sizes = new(d.Sizes); Extras = new(d.Extras); }
    }); }

    [RelayCommand] private void AddSize() { Sizes.Add(new MenuItemSizeDto { Name = "", PriceAdjustment = 0 }); }
    [RelayCommand] private void RemoveSize(MenuItemSizeDto s) { Sizes.Remove(s); }
    [RelayCommand] private void AddExtra() { Extras.Add(new MenuItemExtraDto { Name = "", Price = 0 }); }
    [RelayCommand] private void RemoveExtra(MenuItemExtraDto e) { Extras.Remove(e); }

    [RelayCommand] private async Task UploadPhotoAsync() { var result = await MediaPicker.Default.PickPhotoAsync(); if (result != null && IsEditing) { using var s = await result.OpenReadAsync(); await _api.UploadAsync<object>($"/restaurant/menu/items/{ItemId}/image", s, result.FileName); } }

    [RelayCommand] private async Task SaveAsync() { await ExecuteAsync(async () => {
        var request = new CreateMenuItemRequest {
            CategoryId = Guid.TryParse(CategoryIdParam, out var cid) ? cid : Guid.Empty,
            Name = Name, Description = Description, BasePrice = decimal.TryParse(Price, out var p) ? p : 0, IsPopular = IsPopular,
            Allergens = AllergensText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            NutritionalInfo = NutritionalInfo, CalorieCount = int.TryParse(CalorieCount, out var cal) ? cal : 0,
            Sizes = Sizes.ToList(), Extras = Extras.ToList()
        };
        if (IsEditing) await _api.PutAsync<object>($"/restaurant/menu/items/{ItemId}", request);
        else await _api.PostAsync<MenuItemDto>("/restaurant/menu/items", request);
        await Shell.Current.GoToAsync("..");
    }); }
}