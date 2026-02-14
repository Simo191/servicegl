using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class DeliveryHistoryViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<DeliveryHistoryItem> _deliveries = new();
    [ObservableProperty] private string _selectedFilter = "all";
    [ObservableProperty] private int _totalDeliveries;
    [ObservableProperty] private decimal _totalEarnings;

    public DeliveryHistoryViewModel(ApiService api)
    {
        _api = api;
        Title = "Historique";
    }

    [RelayCommand]
    private async Task LoadHistoryAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<ApiResponse<PaginatedResponse<DeliveryHistoryItem>>>(
                $"{AppConstants.DeliveryHistoryEndpoint}?status={SelectedFilter}");
            if (result?.Data != null)
            {
                Deliveries = new ObservableCollection<DeliveryHistoryItem>(result.Data.Items);
                TotalDeliveries = result.Data.TotalCount;
                TotalEarnings = Deliveries.Sum(d => d.TotalEarning);
            }
        });
    }

    [RelayCommand]
    private async Task FilterAsync(string filter)
    {
        SelectedFilter = filter;
        await LoadHistoryAsync();
    }

    [RelayCommand]
    private async Task ViewDetailAsync(DeliveryHistoryItem item)
    {
        await Shell.Current.GoToAsync($"deliveryDetail?id={item.Id}");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadHistoryAsync();
    }
}