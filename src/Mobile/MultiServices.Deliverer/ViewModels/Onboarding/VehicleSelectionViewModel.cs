using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels.Onboarding;

public partial class VehicleSelectionViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private string _selectedVehicleType = string.Empty;
    [ObservableProperty] private string _plateNumber = string.Empty;
    [ObservableProperty] private string _vehicleModel = string.Empty;
    [ObservableProperty] private bool _plateRequired;
    [ObservableProperty] private List<VehicleOption> _vehicleOptions = new()
    {
        new() { Type = "Bicycle", Label = "Vélo", Icon = "🚲", Description = "Idéal en ville" },
        new() { Type = "Scooter", Label = "Scooter", Icon = "🛵", Description = "Rapide et pratique" },
        new() { Type = "Motorcycle", Label = "Moto", Icon = "🏍️", Description = "Pour longues distances" },
        new() { Type = "Car", Label = "Voiture", Icon = "🚗", Description = "Grosses commandes" },
        new() { Type = "Van", Label = "Camionnette", Icon = "🚐", Description = "Gros volumes" }
    };

    public VehicleSelectionViewModel(ApiService api)
    {
        _api = api;
        Title = "Véhicule";
    }

    [RelayCommand]
    private void SelectVehicle(VehicleOption option)
    {
        SelectedVehicleType = option.Type;
        PlateRequired = option.Type != "Bicycle";
        foreach (var o in VehicleOptions)
            o.IsSelected = o.Type == option.Type;
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (string.IsNullOrEmpty(SelectedVehicleType))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un véhicule", "OK");
            return;
        }
        if (PlateRequired && string.IsNullOrEmpty(PlateNumber))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez saisir la plaque d'immatriculation", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _api.PostAsync<ApiResponse<object>>(Helpers.AppConstants.DelivererVehicleEndpoint,
                new { VehicleType = SelectedVehicleType, PlateNumber, Model = VehicleModel });
            await Shell.Current.GoToAsync("documentUpload");
        });
    }
}
