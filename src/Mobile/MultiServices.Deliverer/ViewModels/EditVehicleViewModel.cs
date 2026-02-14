using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class EditVehicleViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly IAuthService _auth;

    [ObservableProperty] private string _vehicleType = string.Empty;
    [ObservableProperty] private string _plateNumber = string.Empty;
    [ObservableProperty] private string _vehicleModel = string.Empty;
    [ObservableProperty] private bool _plateRequired;
    [ObservableProperty] private List<VehicleOption> _vehicleOptions = new()
    {
        new() { Type = "Bicycle", Label = "Vélo", Icon = "🚲" },
        new() { Type = "Scooter", Label = "Scooter", Icon = "🛵" },
        new() { Type = "Motorcycle", Label = "Moto", Icon = "🏍️" },
        new() { Type = "Car", Label = "Voiture", Icon = "🚗" },
        new() { Type = "Van", Label = "Camionnette", Icon = "🚐" }
    };

    public EditVehicleViewModel(ApiService api, IAuthService auth)
    {
        _api = api;
        _auth = auth;
        Title = "Modifier véhicule";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var profile = await _auth.GetDelivererProfileAsync();
            if (profile != null)
            {
                VehicleType = profile.VehicleType;
                PlateNumber = profile.PlateNumber ?? string.Empty;
                VehicleModel = profile.VehicleModel ?? string.Empty;
                PlateRequired = profile.VehicleType != "Bicycle";
                foreach (var o in VehicleOptions)
                    o.IsSelected = o.Type == profile.VehicleType;
            }
        });
    }

    [RelayCommand]
    private void SelectVehicle(VehicleOption option)
    {
        VehicleType = option.Type;
        PlateRequired = option.Type != "Bicycle";
        foreach (var o in VehicleOptions) o.IsSelected = o.Type == option.Type;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (PlateRequired && string.IsNullOrEmpty(PlateNumber))
        {
            await Shell.Current.DisplayAlert("Erreur", "Plaque d'immatriculation requise", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _api.PutAsync<ApiResponse<object>>(AppConstants.DelivererVehicleEndpoint,
                new { VehicleType, PlateNumber, Model = VehicleModel });
            await Shell.Current.DisplayAlert("Succès", "Véhicule mis à jour", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }
}
