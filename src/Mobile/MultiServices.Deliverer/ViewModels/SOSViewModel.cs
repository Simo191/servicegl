using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.Services.Location;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class SOSViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly ILocationService _locationService;

    [ObservableProperty] private bool _sosSent;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private int _countdown = 5;
    [ObservableProperty] private bool _isCounting;

    public SOSViewModel(ApiService api, ILocationService locationService)
    {
        _api = api;
        _locationService = locationService;
        Title = "🆘 Urgence";
    }

    [RelayCommand]
    private async Task SendSOSAsync()
    {
        IsCounting = true;
        for (int i = 5; i > 0; i--)
        {
            Countdown = i;
            await Task.Delay(1000);
        }

        await ExecuteAsync(async () =>
        {
            var location = await _locationService.GetCurrentLocationAsync();
            await _api.PostAsync<ApiResponse<object>>(AppConstants.SOSEndpoint, new SOSRequest
            {
                Latitude = location?.Lat ?? 0,
                Longitude = location?.Lng ?? 0,
                Message = Message
            });
            SosSent = true;
        });
    }

    [RelayCommand]
    private async Task CancelSOSAsync()
    {
        IsCounting = false;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CallEmergencyAsync()
    {
        try { PhoneDialer.Open("15"); } // SAMU Maroc
        catch { await Shell.Current.DisplayAlert("Erreur", "Impossible d'appeler les urgences", "OK"); }
    }

    [RelayCommand]
    private async Task CallSupportAsync()
    {
        try { PhoneDialer.Open("0800123456"); }
        catch { }
    }
}