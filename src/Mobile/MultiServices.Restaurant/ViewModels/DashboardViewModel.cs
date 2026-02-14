using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api; using MultiServices.Restaurant.Services.Auth;
namespace MultiServices.Restaurant.ViewModels;
public partial class DashboardViewModel : BaseViewModel
{
    private readonly ApiService _api; private readonly AuthService _auth;
    // Restaurant info
    [ObservableProperty] private string _restaurantName = string.Empty;
    [ObservableProperty] private string? _logoUrl;
    [ObservableProperty] private bool _isOpen;
    [ObservableProperty] private string _statusText = "Fermé";
    [ObservableProperty] private Color _statusColor = Colors.Red;
    // KPIs
    [ObservableProperty] private int _pendingOrdersCount;
    [ObservableProperty] private int _todayOrdersCount;
    [ObservableProperty] private decimal _todayRevenue;
    [ObservableProperty] private double _averageRating;
    [ObservableProperty] private int _totalReviews;
    [ObservableProperty] private decimal _commissionsToday;
    // Commandes en attente
    [ObservableProperty] private ObservableCollection<RestaurantOrderDto> _pendingOrders = new();
    [ObservableProperty] private ObservableCollection<RestaurantOrderDto> _preparingOrders = new();
    [ObservableProperty] private ObservableCollection<RestaurantOrderDto> _readyOrders = new();

    public DashboardViewModel(ApiService api, AuthService auth) { _api = api; _auth = auth; Title = "Dashboard"; }

    partial void OnIsOpenChanged(bool value) { StatusText = value ? "Ouvert" : "Fermé"; StatusColor = value ? Colors.Green : Colors.Red; }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Restaurant profile
            var r = await _api.GetAsync<RestaurantProfileDto>("/restaurant/me");
            if (r.Success && r.Data != null) { RestaurantName = r.Data.Name; LogoUrl = r.Data.LogoUrl; IsOpen = r.Data.IsOpen; AverageRating = r.Data.AverageRating; TotalReviews = r.Data.TotalReviews; }

            // Pending orders (notification sonore forte pour nouvelles)
            var pending = await _api.GetAsync<PaginatedResult<RestaurantOrderDto>>("/restaurant/orders", new() { ["status"] = "Pending", ["pageSize"] = "50" });
            if (pending.Success && pending.Data != null) { PendingOrders = new(pending.Data.Items); PendingOrdersCount = pending.Data.TotalCount; }

            // Preparing orders
            var preparing = await _api.GetAsync<PaginatedResult<RestaurantOrderDto>>("/restaurant/orders", new() { ["status"] = "Preparing", ["pageSize"] = "50" });
            if (preparing.Success && preparing.Data != null) PreparingOrders = new(preparing.Data.Items);

            // Ready orders
            var ready = await _api.GetAsync<PaginatedResult<RestaurantOrderDto>>("/restaurant/orders", new() { ["status"] = "Ready", ["pageSize"] = "50" });
            if (ready.Success && ready.Data != null) ReadyOrders = new(ready.Data.Items);

            // Stats du jour
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            var stats = await _api.GetAsync<RestaurantStatsDto>("/restaurant/stats", new() { ["from"] = today, ["to"] = today });
            if (stats.Success && stats.Data != null) { TodayOrdersCount = stats.Data.TotalOrders; TodayRevenue = stats.Data.TotalRevenue; CommissionsToday = stats.Data.CommissionsTotal; }
        });
    }

    [RelayCommand] private async Task ToggleOpenAsync() { await _api.PostAsync<object>("/restaurant/toggle-status"); IsOpen = !IsOpen; }

    [RelayCommand]
    private async Task AcceptOrderAsync(RestaurantOrderDto order)
    {
        // Timer acceptation 3 min — estimation minutes
        string result = await Shell.Current.DisplayPromptAsync("Temps de préparation", "Estimation en minutes:", "Accepter", "Annuler", "20", keyboard: Keyboard.Numeric);
        if (result != null && int.TryParse(result, out int mins))
        { await _api.PostAsync<object>($"/restaurant/orders/{order.Id}/accept", new { estimatedMinutes = mins }); await LoadDataAsync(); }
    }

    [RelayCommand]
    private async Task RejectOrderAsync(RestaurantOrderDto order)
    {
        // Motif: rupture stock, surcharge, fermeture imminente
        string action = await Shell.Current.DisplayActionSheet("Motif du refus", "Annuler", null, "Rupture de stock", "Surcharge de commandes", "Fermeture imminente", "Autre");
        if (action != null && action != "Annuler")
        {
            string detail = action == "Autre" ? await Shell.Current.DisplayPromptAsync("Motif", "Précisez:") ?? "" : action;
            await _api.PostAsync<object>($"/restaurant/orders/{order.Id}/reject", new { reason = detail });
            await LoadDataAsync();
        }
    }

    [RelayCommand] private async Task MarkPreparingAsync(RestaurantOrderDto order) { await _api.PostAsync<object>($"/restaurant/orders/{order.Id}/status", new { status = "Preparing" }); await LoadDataAsync(); }
    [RelayCommand] private async Task MarkReadyAsync(RestaurantOrderDto order)
    {
        // Notification livreur + note optionnelle
        string note = await Shell.Current.DisplayPromptAsync("Note pour le livreur", "Note (optionnel):", "Prêt!", "Annuler");
        await _api.PostAsync<object>($"/restaurant/orders/{order.Id}/status", new { status = "Ready", note = note ?? "" });
        await LoadDataAsync();
    }

    [RelayCommand] private async Task ViewOrderAsync(RestaurantOrderDto order) => await Shell.Current.GoToAsync($"orderdetail?id={order.Id}");
    [RelayCommand] private async Task GoToSettingsAsync() => await Shell.Current.GoToAsync("settings");
    [RelayCommand] private async Task GoToReviewsAsync() => await Shell.Current.GoToAsync("reviews");
}