using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api;
namespace MultiServices.Store.ViewModels;
[QueryProperty(nameof(OrderId), "id")]
public partial class PickingViewModel : BaseViewModel {
    private readonly ApiService _api; [ObservableProperty] private string _orderId = string.Empty;
    [ObservableProperty] private GroceryOrderDto? _order; [ObservableProperty] private ObservableCollection<GroceryOrderItemDto> _items = new();
    [ObservableProperty] private int _pickedCount; [ObservableProperty] private int _totalCount; [ObservableProperty] private double _progress;
    public PickingViewModel(ApiService api) { _api = api; Title = "Picking"; }
    partial void OnOrderIdChanged(string v) { if (!string.IsNullOrEmpty(v)) LoadCommand.ExecuteAsync(null); }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<GroceryOrderDto>($"/grocery/store/orders/{OrderId}");
        if (r.Success && r.Data != null) { Order = r.Data; Items = new(r.Data.Items); TotalCount = r.Data.Items.Count; PickedCount = r.Data.Items.Count(i => i.IsPicked); Progress = TotalCount > 0 ? (double)PickedCount / TotalCount : 0; }
    }); }
    [RelayCommand] private async Task PickItemAsync(GroceryOrderItemDto item) {
        // Scanner ou cocher manuellement
        await _api.PostAsync<object>($"/grocery/store/orders/{OrderId}/items/{item.Id}/pick");
        item.IsPicked = true; PickedCount++; Progress = (double)PickedCount / TotalCount;
    }
    [RelayCommand] private async Task ScanBarcodeAsync() { /* ZXing scanner intégration */ await Shell.Current.DisplayAlert("Scanner", "Scanner le code-barre du produit...", "OK"); }
    [RelayCommand] private async Task MarkUnavailableAsync(GroceryOrderItemDto item) {
        string sub = await Shell.Current.DisplayPromptAsync("Indisponible", $"Remplacement pour '{item.ProductName}':", "Proposer", "Sans remplacement");
        if (sub != null) await _api.PostAsync<object>($"/grocery/store/orders/{OrderId}/items/{item.Id}/substitute", new { substituteName = sub, substitutePrice = 0 });
        else await _api.PostAsync<object>($"/grocery/store/orders/{OrderId}/items/{item.Id}/unavailable");
        await LoadAsync();
    }
    [RelayCommand] private async Task FinishPickingAsync() {
        if (PickedCount < TotalCount && !await Shell.Current.DisplayAlert("Attention", $"Seulement {PickedCount}/{TotalCount} produits ramassés. Continuer?", "Oui", "Non")) return;
        await _api.PostAsync<object>($"/grocery/store/orders/{OrderId}/ready");
        await Shell.Current.GoToAsync("../..");
    }
}