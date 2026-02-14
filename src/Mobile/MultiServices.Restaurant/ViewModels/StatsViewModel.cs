using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api;
namespace MultiServices.Restaurant.ViewModels;
public partial class StatsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    [ObservableProperty] private string _period = "month";
    [ObservableProperty] private decimal _revenue; [ObservableProperty] private int _orderCount;
    [ObservableProperty] private decimal _avgOrder; [ObservableProperty] private double _rating;
    [ObservableProperty] private decimal _commissions;
    [ObservableProperty] private ObservableCollection<TopItemDto> _topItems = new();
    [ObservableProperty] private Dictionary<string, int> _ordersByHour = new();
    [ObservableProperty] private Dictionary<string, decimal> _revenueByDay = new();
    public StatsViewModel(ApiService api) { _api = api; Title = "Statistiques"; }

    [RelayCommand] private async Task LoadStatsAsync() { await ExecuteAsync(async () => {
        var from = Period switch { "day" => DateTime.Today, "week" => DateTime.Today.AddDays(-7), "month" => DateTime.Today.AddDays(-30), _ => DateTime.Today.AddDays(-30) };
        var r = await _api.GetAsync<RestaurantStatsDto>("/restaurant/stats", new() { ["from"] = from.ToString("yyyy-MM-dd"), ["to"] = DateTime.Today.ToString("yyyy-MM-dd") });
        if (r.Success && r.Data != null) { Revenue = r.Data.TotalRevenue; OrderCount = r.Data.TotalOrders; AvgOrder = r.Data.AverageOrderValue; Rating = r.Data.AverageRating; Commissions = r.Data.CommissionsTotal; TopItems = new(r.Data.TopSellingItems); OrdersByHour = r.Data.OrdersByHour; RevenueByDay = r.Data.RevenueByDay; }
    }); }
    [RelayCommand] private async Task SetPeriodAsync(string p) { Period = p; await LoadStatsAsync(); }
    [RelayCommand] private async Task ExportCsvAsync() { var r = await _api.GetAsync<string>("/restaurant/stats/export", new() { ["format"] = "csv" }); await Shell.Current.DisplayAlert("Export", "Rapport envoyé par email", "OK"); }
    [RelayCommand] private async Task ExportPdfAsync() { var r = await _api.GetAsync<string>("/restaurant/stats/export", new() { ["format"] = "pdf" }); await Shell.Current.DisplayAlert("Export", "Rapport PDF envoyé par email", "OK"); }
}