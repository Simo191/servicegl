using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class StatsViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private DelivererStats? _stats;
    [ObservableProperty] private int _totalDeliveries;
    [ObservableProperty] private int _totalInterventions;
    [ObservableProperty] private double _averageRating;
    [ObservableProperty] private double _acceptanceRate;
    [ObservableProperty] private decimal _totalEarnings;
    [ObservableProperty] private double _totalDistanceKm;
    [ObservableProperty] private string _averageDeliveryTime = "--";
    [ObservableProperty] private int _deliveriesToday;
    [ObservableProperty] private int _deliveriesThisWeek;
    [ObservableProperty] private int _deliveriesThisMonth;

    public StatsViewModel(ApiService api)
    {
        _api = api;
        Title = "Mes statistiques";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<ApiResponse<DelivererStats>>(AppConstants.DelivererStatsEndpoint);
            if (result?.Data != null)
            {
                Stats = result.Data;
                TotalDeliveries = result.Data.TotalDeliveries;
                TotalInterventions = result.Data.TotalInterventions;
                AverageRating = result.Data.AverageRating;
                AcceptanceRate = result.Data.AcceptanceRate;
                TotalEarnings = result.Data.TotalEarnings;
                TotalDistanceKm = result.Data.TotalDistanceKm;
                AverageDeliveryTime = $"{result.Data.AverageDeliveryMinutes:F0} min";
                DeliveriesToday = result.Data.DeliveriesToday;
                DeliveriesThisWeek = result.Data.DeliveriesWeek;
                DeliveriesThisMonth = result.Data.DeliveriesMonth;
            }
        });
    }
}
