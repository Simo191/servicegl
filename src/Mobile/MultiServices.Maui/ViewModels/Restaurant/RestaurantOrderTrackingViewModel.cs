using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;
using System.Collections.ObjectModel;

namespace MultiServices.Maui.ViewModels.Restaurant;

[QueryProperty(nameof(OrderId), "id")]
public partial class RestaurantOrderTrackingViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _orderId = string.Empty;
    [ObservableProperty] private RestaurantOrderDto? _order;
    [ObservableProperty] private ObservableCollection<OrderStatusHistoryDto> _statusHistory = new();
    [ObservableProperty] private double _delivererLat;
    [ObservableProperty] private double _delivererLng;
    [ObservableProperty] private bool _showMap;

    public RestaurantOrderTrackingViewModel(ApiService api)
    {
        _api = api;
        Title = "Suivi commande";
    }

    partial void OnOrderIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadOrderCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadOrderAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.GetAsync<RestaurantOrderDto>($"/restaurant-orders/{OrderId}");
            if (result.Success && result.Data != null)
            {
                Order = result.Data;
                StatusHistory = new ObservableCollection<OrderStatusHistoryDto>(result.Data.StatusHistory);
                if (result.Data.DelivererLatitude.HasValue)
                {
                    DelivererLat = result.Data.DelivererLatitude.Value;
                    DelivererLng = result.Data.DelivererLongitude!.Value;
                    ShowMap = true;
                }
            }
        });
    }

    [RelayCommand]
    private async Task CallDelivererAsync()
    {
        if (Order?.DelivererPhone != null)
        {
            try { PhoneDialer.Default.Open(Order.DelivererPhone); }
            catch { await Shell.Current.DisplayAlert("Erreur", "Impossible d'appeler", "OK"); }
        }
    }

    [RelayCommand]
    private async Task RefreshTrackingAsync() => await LoadOrderAsync();
}
