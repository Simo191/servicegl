using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api;
namespace MultiServices.Store.ViewModels;
public partial class StockViewModel : BaseViewModel {
    private readonly ApiService _api;
    [ObservableProperty] private ObservableCollection<ProductDto> _lowStockProducts = new();
    [ObservableProperty] private ObservableCollection<ProductDto> _outOfStockProducts = new();
    [ObservableProperty] private ObservableCollection<ProductDto> _allProducts = new();
    [ObservableProperty] private string _viewMode = "alerts"; // alerts, all, inventory
    public StockViewModel(ApiService api) { _api = api; Title = "Stock"; }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var low = await _api.GetAsync<PaginatedResult<ProductDto>>("/grocery/store/products", new() { ["lowStock"] = "true", ["pageSize"] = "50" });
        if (low.Success && low.Data != null) LowStockProducts = new(low.Data.Items);
        var oos = await _api.GetAsync<PaginatedResult<ProductDto>>("/grocery/store/products", new() { ["outOfStock"] = "true", ["pageSize"] = "50" });
        if (oos.Success && oos.Data != null) OutOfStockProducts = new(oos.Data.Items);
    }); }
    [RelayCommand] private async Task UpdateStockAsync(ProductDto p) {
        string qty = await Shell.Current.DisplayPromptAsync("Stock", $"Nouvelle quantité pour {p.Name}:", keyboard: Keyboard.Numeric, initialValue: p.StockQuantity.ToString());
        if (qty != null && int.TryParse(qty, out int q)) { await _api.PutAsync<object>($"/grocery/store/products/{p.Id}", new { stockQuantity = q }); await LoadAsync(); }
    }
    [RelayCommand] private async Task MarkAvailableAsync(ProductDto p) { await _api.PostAsync<object>($"/grocery/store/products/{p.Id}/toggle-availability"); await LoadAsync(); }
    [RelayCommand] private async Task BulkRestockAsync() {
        // Réassort en masse
        bool ok = await Shell.Current.DisplayAlert("Réassort", "Remettre tous les produits en rupture à stock=10?", "Oui", "Non");
        if (ok) { foreach (var p in OutOfStockProducts) await _api.PutAsync<object>($"/grocery/store/products/{p.Id}", new { stockQuantity = 10 }); await LoadAsync(); }
    }
    [RelayCommand] private async Task StartInventoryAsync() { await Shell.Current.DisplayAlert("Inventaire", "Fonction d'inventaire périodique - Scannez les produits pour vérifier le stock", "Démarrer"); }
}