using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Grocery;

public partial class ShoppingListsViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<ShoppingListDto> _lists = new();
    [ObservableProperty] private string _newListName = string.Empty;
    [ObservableProperty] private bool _showCreateForm;

    public ShoppingListsViewModel(ApiService api)
    {
        _api = api;
        Title = "Mes listes";
    }

    [RelayCommand]
    private async Task LoadListsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<List<ShoppingListDto>>("/grocery/shopping-lists");
            if (result.Success && result.Data != null)
            {
                Lists = new ObservableCollection<ShoppingListDto>(result.Data);
                IsEmpty = !Lists.Any();
            }
        });
    }

    [RelayCommand]
    private async Task CreateListAsync()
    {
        if (string.IsNullOrWhiteSpace(NewListName)) return;
        await ExecuteAsync(async () =>
        {
            var result = await _api.PostAsync<ShoppingListDto>("/grocery/shopping-lists", new { name = NewListName });
            if (result.Success && result.Data != null)
            {
                Lists.Add(result.Data);
                NewListName = string.Empty;
                ShowCreateForm = false;
            }
        });
    }

    [RelayCommand]
    private async Task SelectListAsync(ShoppingListDto list)
    {
        await Shell.Current.GoToAsync($"shoppinglistdetail?id={list.Id}");
    }

    [RelayCommand]
    private async Task DeleteListAsync(ShoppingListDto list)
    {
        var confirm = await Shell.Current.DisplayAlert("Supprimer", $"Supprimer \"{list.Name}\" ?", "Oui", "Non");
        if (confirm)
        {
            await _api.DeleteAsync<object>($"/grocery/shopping-lists/{list.Id}");
            Lists.Remove(list);
        }
    }

    [RelayCommand]
    private async Task ConvertToCartAsync(ShoppingListDto list)
    {
        await Shell.Current.GoToAsync($"storedetail?id=select&listId={list.Id}");
    }
}
