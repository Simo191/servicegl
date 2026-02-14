using System.Collections.ObjectModel; using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Store.Models; using MultiServices.Store.Services.Api;
namespace MultiServices.Store.ViewModels;
public partial class StatsViewModel : BaseViewModel {
    private readonly ApiService _api;
    [ObservableProperty] private string _period = "month"; [ObservableProperty] private decimal _revenue; [ObservableProperty] private int _orderCount;
    [ObservableProperty] private decimal _avgBasket; [ObservableProperty] private int _outOfStock; [ObservableProperty] private decimal _commissions;
    [ObservableProperty] private ObservableCollection<TopProductDto> _topProducts = new();
    public StatsViewModel(ApiService api) { _api = api; Title = "Statistiques"; }
    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var from = Period switch { "day" => DateTime.Today, "week" => DateTime.Today.AddDays(-7), _ => DateTime.Today.AddDays(-30) };
        var r = await _api.GetAsync<StoreStatsDto>("/grocery/store/stats", new() { ["from"] = from.ToString("yyyy-MM-dd") });
        if (r.Success && r.Data != null) { Revenue = r.Data.TotalRevenue; OrderCount = r.Data.TotalOrders; AvgBasket = r.Data.AverageBasket; OutOfStock = r.Data.OutOfStockCount; Commissions = r.Data.CommissionsTotal; TopProducts = new(r.Data.TopProducts); }
    }); }
    [RelayCommand] private async Task SetPeriodAsync(string p) { Period = p; await LoadAsync(); }
    [RelayCommand] private async Task ExportAsync() { await _api.GetAsync<string>("/grocery/store/stats/export"); await Shell.Current.DisplayAlert("Export", "Rapport envoyé par email", "OK"); }
}