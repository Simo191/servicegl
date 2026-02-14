using CommunityToolkit.Mvvm.ComponentModel; using CommunityToolkit.Mvvm.Input;
using MultiServices.Restaurant.Models; using MultiServices.Restaurant.Services.Api; using MultiServices.Restaurant.Services.Auth;
namespace MultiServices.Restaurant.ViewModels;
public partial class SettingsViewModel : BaseViewModel
{
    private readonly ApiService _api; private readonly AuthService _auth;
    // Restaurant info
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _cuisineType = string.Empty;
    [ObservableProperty] private string _priceRange = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _address = string.Empty;
    // Delivery settings
    [ObservableProperty] private string _deliveryFee = string.Empty;
    [ObservableProperty] private string _minOrder = string.Empty;
    [ObservableProperty] private string _maxDistance = string.Empty;
    [ObservableProperty] private string _estimatedDelivery = string.Empty;
    // Mode
    [ObservableProperty] private bool _vacationMode;
    [ObservableProperty] private bool _soundEnabled = true;
    [ObservableProperty] private bool _autoAccept;
    public SettingsViewModel(ApiService api, AuthService auth) { _api = api; _auth = auth; Title = "Paramètres"; }

    [RelayCommand] private async Task LoadAsync() { await ExecuteAsync(async () => {
        var r = await _api.GetAsync<RestaurantProfileDto>("/restaurant/me");
        if (r.Success && r.Data != null) { var d = r.Data; Name = d.Name; Description = d.Description ?? ""; CuisineType = d.CuisineType; PriceRange = d.PriceRange; Phone = d.Phone; Address = d.Address;
            DeliveryFee = d.DeliveryFee.ToString(); MinOrder = d.MinimumOrder.ToString(); MaxDistance = d.MaxDeliveryDistanceKm.ToString(); EstimatedDelivery = d.EstimatedDeliveryMinutes.ToString(); }
    }); }

    [RelayCommand] private async Task SaveAsync() { await _api.PutAsync<object>("/restaurant/me", new {
        name = Name, description = Description, cuisineType = CuisineType, priceRange = PriceRange, phone = Phone, address = Address,
        deliveryFee = decimal.TryParse(DeliveryFee, out var df) ? df : 0, minimumOrder = decimal.TryParse(MinOrder, out var mo) ? mo : 0,
        maxDeliveryDistanceKm = double.TryParse(MaxDistance, out var md) ? md : 5, estimatedDeliveryMinutes = int.TryParse(EstimatedDelivery, out var ed) ? ed : 30
    }); await Shell.Current.DisplayAlert("OK", "Paramètres enregistrés", "OK"); }

    [RelayCommand] private async Task UploadLogoAsync() { var r = await MediaPicker.Default.PickPhotoAsync(); if (r != null) { using var s = await r.OpenReadAsync(); await _api.UploadAsync<object>("/restaurant/me/logo", s, r.FileName); } }
    [RelayCommand] private async Task UploadCoverAsync() { var r = await MediaPicker.Default.PickPhotoAsync(); if (r != null) { using var s = await r.OpenReadAsync(); await _api.UploadAsync<object>("/restaurant/me/cover", s, r.FileName); } }
    [RelayCommand] private async Task ManageHoursAsync() => await Shell.Current.GoToAsync("openinghours");
    [RelayCommand] private async Task ManageStaffAsync() => await Shell.Current.GoToAsync("staffmanagement");
    [RelayCommand] private async Task ToggleVacationAsync() { await _api.PostAsync<object>("/restaurant/vacation-mode", new { enabled = VacationMode }); }
    [RelayCommand] private async Task LogoutAsync() => await _auth.LogoutAsync();
}