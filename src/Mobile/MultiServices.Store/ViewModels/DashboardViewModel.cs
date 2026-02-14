using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api; using MultiServices.Store.Services.Auth;
namespace MultiServices.Store.ViewModels;
public partial class DashboardViewModel : BaseViewModel {
    private readonly ApiService _api; private readonly AuthService _auth;
    [ObservableProperty] private string _storeName = string.Empty; [ObservableProperty] private string _storeType = string.Empty;
    [ObservableProperty] private bool _isOpen; [ObservableProperty] private string _statusText = "Fermé"; [ObservableProperty] private Color _statusColor = Colors.Red;
    [ObservableProperty] private int _pendingOrdersCount; [ObservableProperty] private int _todayOrders; [ObservableProperty] private decimal _todayRevenue;
    [ObservableProperty] private int _lowStockAlerts; [ObservableProperty] private int _outOfStockAlerts;
    [ObservableProperty] private ObservableCollection<GroceryOrderDto> _pendingOrders = new();
    public DashboardViewModel(ApiService api, AuthService auth) { _api = api; _auth = auth; Title = "Dashboard"; }
    partial void OnIsOpenChanged(bool v) { StatusText = v ? "Ouvert" : "Fermé"; StatusColor = v ? Colors.Green : Colors.Red; }

    [RelayCommand] private async Task LoadDataAsync() { await ExecuteAsync(async () => {
        var store = await _api.GetAsync<GroceryStoreDto>("/grocery/store/me");
        if (store.Success && store.Data != null) { StoreName = store.Data.Name; StoreType = store.Data.StoreType; IsOpen = store.Data.IsOpen; }
        var orders = await _api.GetAsync<PaginatedResult<GroceryOrderDto>>("/grocery/store/orders", new() { ["status"] = "Received", ["pageSize"] = "50" });
        if (orders.Success && orders.Data != null) { PendingOrders = new(orders.Data.Items); PendingOrdersCount = orders.Data.TotalCount; }
        var stats = await _api.GetAsync<StoreStatsDto>("/grocery/store/stats", new() { ["from"] = DateTime.Today.ToString("yyyy-MM-dd") });
        if (stats.Success && stats.Data != null) { TodayOrders = stats.Data.TotalOrders; TodayRevenue = stats.Data.TotalRevenue; LowStockAlerts = stats.Data.LowStockCount; OutOfStockAlerts = stats.Data.OutOfStockCount; }
    }); }
    [RelayCommand] private async Task ToggleOpenAsync() { await _api.PostAsync<object>("/grocery/store/toggle-status"); IsOpen = !IsOpen; }
    [RelayCommand] private async Task AcceptOrderAsync(GroceryOrderDto o) { await _api.PostAsync<object>($"/grocery/store/orders/{o.Id}/accept"); await LoadDataAsync(); }
    [RelayCommand] private async Task RejectOrderAsync(GroceryOrderDto o) { string r = await Shell.Current.DisplayPromptAsync("Refus", "Motif:"); if (r != null) { await _api.PostAsync<object>($"/grocery/store/orders/{o.Id}/reject", new { reason = r }); await LoadDataAsync(); } }
    [RelayCommand] private async Task ViewOrderAsync(GroceryOrderDto o) => await Shell.Current.GoToAsync($"orderdetail?id={o.Id}");
}