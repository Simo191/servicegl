using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class EarningsViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private EarningsSummary? _summary;
    [ObservableProperty] private ObservableCollection<EarningDetail> _recentEarnings = new();
    [ObservableProperty] private ObservableCollection<ActiveBonus> _activeBonuses = new();
    [ObservableProperty] private string _selectedPeriod = "today";

    public EarningsViewModel(ApiService api)
    {
        _api = api;
        Title = "Mes Gains";
    }

    [RelayCommand]
    private async Task LoadEarningsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<ApiResponse<EarningsSummary>>(AppConstants.EarningsSummaryEndpoint);
            Summary = result?.Data;

            var history = await _api.GetAsync<ApiResponse<List<EarningDetail>>>(
                $"{AppConstants.EarningsHistoryEndpoint}?period={SelectedPeriod}");
            if (history?.Data != null)
                RecentEarnings = new ObservableCollection<EarningDetail>(history.Data);

            var bonuses = await _api.GetAsync<ApiResponse<List<ActiveBonus>>>(AppConstants.ActiveBonusesEndpoint);
            if (bonuses?.Data != null)
                ActiveBonuses = new ObservableCollection<ActiveBonus>(bonuses.Data);
        });
    }

    [RelayCommand]
    private async Task ChangePeriodAsync(string period)
    {
        SelectedPeriod = period;
        await LoadEarningsAsync();
    }

    [RelayCommand]
    private async Task RequestPayoutAsync()
    {
        await Shell.Current.GoToAsync("payout");
    }

    [RelayCommand]
    private async Task RefreshAsync() => await LoadEarningsAsync();
}