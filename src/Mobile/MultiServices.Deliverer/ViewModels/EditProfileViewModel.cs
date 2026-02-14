using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
using MultiServices.Deliverer.Services.Auth;
using MultiServices.Deliverer.ViewModels.Base;

namespace MultiServices.Deliverer.ViewModels;

public partial class EditProfileViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly IAuthService _authService;

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private string? _licensePlate;
    [ObservableProperty] private int _selectedVehicleIndex;
    [ObservableProperty] private string? _emergencyContactName;
    [ObservableProperty] private string? _emergencyContactPhone;

    public List<string> VehicleTypes { get; } = new() { "Vélo", "Scooter", "Moto", "Voiture", "Van" };
    private static readonly string[] VehicleCodes = { "Bicycle", "Scooter", "Motorcycle", "Car", "Van" };

    public EditProfileViewModel(ApiService api, IAuthService authService)
    {
        _api = api;
        _authService = authService;
        Title = "Modifier le profil";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var profile = await _authService.GetDelivererProfileAsync();
            if (profile != null)
            {
                FirstName = profile.FirstName;
                LastName = profile.LastName;
                PhoneNumber = profile.PhoneNumber;
                City = profile.City;
                LicensePlate = profile.LicensePlate;
                EmergencyContactName = profile.EmergencyContactName;
                EmergencyContactPhone = profile.EmergencyContactPhone;
                SelectedVehicleIndex = Array.IndexOf(VehicleCodes, profile.VehicleType);
                if (SelectedVehicleIndex < 0) SelectedVehicleIndex = 1;
            }
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await ExecuteAsync(async () =>
        {
            await _api.PutAsync<ApiResponse<object>>(AppConstants.DelivererProfileEndpoint, new
            {
                FirstName, LastName, PhoneNumber, City, LicensePlate,
                VehicleType = VehicleCodes[Math.Clamp(SelectedVehicleIndex, 0, VehicleCodes.Length - 1)],
                EmergencyContactName, EmergencyContactPhone
            });
            await Shell.Current.DisplayAlert("Succès", "Profil mis à jour", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }
}