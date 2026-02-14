using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.Services.Location;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly IAuthService _authService;
    private readonly ILocationService _locationService;

    [ObservableProperty] private bool _isOnline;
    [ObservableProperty] private string _statusText = "Hors ligne";
    [ObservableProperty] private Color _statusColor = Colors.Grey;
    [ObservableProperty] private decimal _todayEarnings;
    [ObservableProperty] private int _todayDeliveries;
    [ObservableProperty] private double _averageRating;
    [ObservableProperty] private double _acceptanceRate;
    [ObservableProperty] private double _currentLat;
    [ObservableProperty] private double _currentLng;
    [ObservableProperty] private ObservableCollection<AvailableDelivery> _availableDeliveries = new();
    [ObservableProperty] private ActiveDelivery? _activeDelivery;
    [ObservableProperty] private bool _hasActiveDelivery;
    [ObservableProperty] private string _delivererName = string.Empty;
    [ObservableProperty] private string? _delivererPhoto;
    [ObservableProperty] private ObservableCollection<ActiveBonus> _activeBonuses = new();

    public DashboardViewModel(ApiService api, IAuthService authService, ILocationService locationService)
    {
        _api = api;
        _authService = authService;
        _locationService = locationService;
        Title = "Accueil";
    }

    [RelayCommand]
    private async Task LoadDashboardAsync()
    {
        await ExecuteAsync(async () =>
        {
            var profile = await _authService.GetDelivererProfileAsync();
            if (profile != null)
            {
                DelivererName = profile.FullName;
                DelivererPhoto = profile.PhotoUrl;
                AverageRating = profile.AverageRating;
                AcceptanceRate = profile.AcceptanceRate;
                IsOnline = profile.Status == "Online";
                UpdateStatusDisplay();
            }

            var earnings = await _api.GetAsync<ApiResponse<EarningsSummary>>(AppConstants.EarningsSummaryEndpoint);
            if (earnings?.Data != null)
            {
                TodayEarnings = earnings.Data.TodayEarnings;
                TodayDeliveries = earnings.Data.TodayDeliveries;
            }

            if (IsOnline)
                await LoadAvailableDeliveriesAsync();

            var bonuses = await _api.GetAsync<ApiResponse<List<ActiveBonus>>>(AppConstants.ActiveBonusesEndpoint);
            if (bonuses?.Data != null)
                ActiveBonuses = new ObservableCollection<ActiveBonus>(bonuses.Data);
        });
    }

    [RelayCommand]
    private async Task ToggleOnlineAsync()
    {
        await ExecuteAsync(async () =>
        {
            IsOnline = !IsOnline;
            var newStatus = IsOnline ? "Online" : "Offline";

            await _api.PutAsync<ApiResponse<object>>(
                AppConstants.DelivererStatusEndpoint, new StatusUpdateRequest { Status = newStatus });

            UpdateStatusDisplay();

            if (IsOnline)
            {
                await _locationService.StartTrackingAsync(OnLocationUpdate);
                await LoadAvailableDeliveriesAsync();
            }
            else
            {
                await _locationService.StopTrackingAsync();
                AvailableDeliveries.Clear();
            }
        });
    }

    private void UpdateStatusDisplay()
    {
        StatusText = IsOnline ? "En ligne" : "Hors ligne";
        StatusColor = IsOnline ? Colors.Green : Colors.Grey;
    }

    private async Task OnLocationUpdate(double lat, double lng)
    {
        CurrentLat = lat;
        CurrentLng = lng;
        try
        {
            await _api.PutAsync<ApiResponse<object>>(
                AppConstants.DelivererLocationEndpoint,
                new LocationUpdate { Latitude = lat, Longitude = lng });
        }
        catch { }
    }

    private async Task LoadAvailableDeliveriesAsync()
    {
        var result = await _api.GetAsync<ApiResponse<List<AvailableDelivery>>>(AppConstants.AvailableDeliveriesEndpoint);
        if (result?.Data != null)
            AvailableDeliveries = new ObservableCollection<AvailableDelivery>(result.Data);
    }

    [RelayCommand]
    private async Task AcceptDeliveryAsync(AvailableDelivery delivery)
    {
        await ExecuteAsync(async () =>
        {
            var result = await _api.PostAsync<ApiResponse<ActiveDelivery>>(
                string.Format(AppConstants.AcceptDeliveryEndpoint, delivery.Id));

            if (result?.Data != null)
            {
                ActiveDelivery = result.Data;
                HasActiveDelivery = true;
                await Shell.Current.GoToAsync($"activeDelivery?id={delivery.Id}");
            }
        });
    }

    [RelayCommand]
    private async Task RejectDeliveryAsync(AvailableDelivery delivery)
    {
        var confirm = await Shell.Current.DisplayAlert("Refuser", "Refuser cette livraison ?", "Oui", "Non");
        if (!confirm) return;

        await ExecuteAsync(async () =>
        {
            await _api.PostAsync<ApiResponse<object>>(
                string.Format(AppConstants.RejectDeliveryEndpoint, delivery.Id));
            AvailableDeliveries.Remove(delivery);
        });
    }

    [RelayCommand]
    private async Task GoToActiveDeliveryAsync()
    {
        if (ActiveDelivery != null)
            await Shell.Current.GoToAsync($"activeDelivery?id={ActiveDelivery.Id}");
    }

    [RelayCommand]
    private async Task SOSAsync()
    {
        await Shell.Current.GoToAsync("sos");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDashboardAsync();
    }
}