using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api;
namespace MultiServices.Restaurant.ViewModels;
public partial class OrdersKanbanViewModel : BaseViewModel
{
    private readonly ApiService _api;
    [ObservableProperty] private ObservableCollection<RestaurantOrderDto> _newOrders = new();
    [ObservableProperty] private ObservableCollection<RestaurantOrderDto> _acceptedOrders = new();
    [ObservableProperty] private ObservableCollection<RestaurantOrderDto> _preparingOrders = new();
    [ObservableProperty] private ObservableCollection<RestaurantOrderDto> _readyOrders = new();
    [ObservableProperty] private string _selectedFilter = "";
    [ObservableProperty] private int _newCount; [ObservableProperty] private int _acceptedCount;
    [ObservableProperty] private int _preparingCount; [ObservableProperty] private int _readyCount;

    public OrdersKanbanViewModel(ApiService api) { _api = api; Title = "Commandes"; }

    [RelayCommand]
    private async Task LoadOrdersAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Charger chaque colonne du Kanban
            var n = await _api.GetAsync<PaginatedResult<RestaurantOrderDto>>("/restaurant/orders", new() { ["status"] = "Pending", ["pageSize"] = "50" });
            if (n.Success && n.Data != null) { NewOrders = new(n.Data.Items); NewCount = n.Data.TotalCount; }

            var a = await _api.GetAsync<PaginatedResult<RestaurantOrderDto>>("/restaurant/orders", new() { ["status"] = "Accepted", ["pageSize"] = "50" });
            if (a.Success && a.Data != null) { AcceptedOrders = new(a.Data.Items); AcceptedCount = a.Data.TotalCount; }

            var p = await _api.GetAsync<PaginatedResult<RestaurantOrderDto>>("/restaurant/orders", new() { ["status"] = "Preparing", ["pageSize"] = "50" });
            if (p.Success && p.Data != null) { PreparingOrders = new(p.Data.Items); PreparingCount = p.Data.TotalCount; }

            var r = await _api.GetAsync<PaginatedResult<RestaurantOrderDto>>("/restaurant/orders", new() { ["status"] = "Ready", ["pageSize"] = "50" });
            if (r.Success && r.Data != null) { ReadyOrders = new(r.Data.Items); ReadyCount = r.Data.TotalCount; }
        });
    }

    [RelayCommand] private async Task AcceptAsync(RestaurantOrderDto o) { string m = await Shell.Current.DisplayPromptAsync("Temps", "Minutes:", keyboard: Keyboard.Numeric, initialValue: "20"); if (m != null) { await _api.PostAsync<object>($"/restaurant/orders/{o.Id}/accept", new { estimatedMinutes = int.Parse(m) }); await LoadOrdersAsync(); } }
    [RelayCommand] private async Task RejectAsync(RestaurantOrderDto o) { string r = await Shell.Current.DisplayPromptAsync("Refus", "Motif:"); if (r != null) { await _api.PostAsync<object>($"/restaurant/orders/{o.Id}/reject", new { reason = r }); await LoadOrdersAsync(); } }
    [RelayCommand] private async Task StartPrepAsync(RestaurantOrderDto o) { await _api.PostAsync<object>($"/restaurant/orders/{o.Id}/status", new { status = "Preparing" }); await LoadOrdersAsync(); }
    [RelayCommand] private async Task MarkReadyAsync(RestaurantOrderDto o) { await _api.PostAsync<object>($"/restaurant/orders/{o.Id}/status", new { status = "Ready" }); await LoadOrdersAsync(); }
    [RelayCommand] private async Task MarkPickedUpAsync(RestaurantOrderDto o) { await _api.PostAsync<object>($"/restaurant/orders/{o.Id}/status", new { status = "PickedUp" }); await LoadOrdersAsync(); }
    [RelayCommand] private async Task ViewOrderAsync(RestaurantOrderDto o) => await Shell.Current.GoToAsync($"orderdetail?id={o.Id}");
}