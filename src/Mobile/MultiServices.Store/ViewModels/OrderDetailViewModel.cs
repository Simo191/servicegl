using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api;
namespace MultiServices.Store.ViewModels;
[QueryProperty(nameof(OrderId), "id")]
public partial class OrderDetailViewModel : BaseViewModel {
    private readonly ApiService _api; [ObservableProperty] private string _orderId = string.Empty; [ObservableProperty] private GroceryOrderDto? _order;
    [ObservableProperty] private bool _hasInstructions; [ObservableProperty] private int _pickedCount; [ObservableProperty] private int _totalItems;
    public OrderDetailViewModel(ApiService api) { _api = api; }
    partial void OnOrderIdChanged(string v) { if (!string.IsNullOrEmpty(v)) LoadCommand.ExecuteAsync(null); }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<GroceryOrderDto>($"/grocery/store/orders/{OrderId}");
        if (r.Success && r.Data != null) { Order = r.Data; HasInstructions = !string.IsNullOrEmpty(r.Data.DeliveryInstructions); TotalItems = r.Data.Items.Count; PickedCount = r.Data.Items.Count(i => i.IsPicked); }
    }); }
    [RelayCommand] private async Task AcceptAsync() { await _api.PostAsync<object>($"/grocery/store/orders/{OrderId}/accept"); await LoadAsync(); }
    [RelayCommand] private async Task StartPickingAsync() => await Shell.Current.GoToAsync($"picking?id={OrderId}");
    [RelayCommand] private async Task MarkReadyAsync() { await _api.PostAsync<object>($"/grocery/store/orders/{OrderId}/ready"); await Shell.Current.GoToAsync(".."); }
    [RelayCommand] private async Task MarkItemUnavailableAsync(GroceryOrderItemDto item) {
        // Proposer remplacement
        string sub = await Shell.Current.DisplayPromptAsync("Produit indisponible", $"Produit de remplacement pour '{item.ProductName}':", "Proposer", "Marquer indisponible");
        if (sub != null) await _api.PostAsync<object>($"/grocery/store/orders/{OrderId}/items/{item.Id}/substitute", new { substituteName = sub });
        else await _api.PostAsync<object>($"/grocery/store/orders/{OrderId}/items/{item.Id}/unavailable");
        await LoadAsync();
    }
}