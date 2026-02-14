using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services;
using MultiServices.Maui.Services.Location;

namespace MultiServices.Maui.ViewModels.Common;

/// <summary>
/// Premier écran : saisie d'adresse SANS login.
/// Valide la zone de couverture avant de laisser l'utilisateur naviguer.
/// </summary>
public partial class AddressEntryViewModel : BaseViewModel
{
    private readonly AddressService _addressService;
    private readonly LocationService _locationService;

    [ObservableProperty] private string _addressQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _suggestions = new();
    [ObservableProperty] private bool _showNotCovered;
    [ObservableProperty] private string _coverageMessage = string.Empty;

    public AddressEntryViewModel(AddressService addressService, LocationService locationService)
    {
        _addressService = addressService;
        _locationService = locationService;
        Title = "Adresse de livraison";

        // Vérifier s'il y a une adresse sauvegardée
        var saved = _addressService.GetSavedAddress();
        if (saved != null)
        {
            // Aller directement à l'accueil
            MainThread.BeginInvokeOnMainThread(async () => {
                _addressService.SetAddress(saved);
                await Shell.Current.GoToAsync("//main/home");
            });
        }
    }

    [RelayCommand]
    private async Task SearchAddressAsync()
    {
        if (string.IsNullOrWhiteSpace(AddressQuery)) return;
        // TODO: Intégrer Google Places Autocomplete API
        Suggestions = new ObservableCollection<string>(new[]
        {
            AddressQuery + ", Casablanca",
            AddressQuery + ", Rabat",
            AddressQuery + ", Marrakech",
        });
    }

    [RelayCommand]
    private async Task UseGPSAsync()
    {
        await ExecuteAsync(async () =>
        {
            var location = await _locationService.GetCurrentLocationAsync();
            if (location == null)
            {
                await Shell.Current.DisplayAlert("GPS", "Impossible d'obtenir votre position. Veuillez saisir votre adresse manuellement.", "OK");
                return;
            }
            await ValidateAndNavigate("Position GPS actuelle", location.Latitude, location.Longitude);
        });
    }

    [RelayCommand]
    private async Task SelectAddressAsync(string address)
    {
        if (string.IsNullOrEmpty(address)) return;
        // TODO: Geocoder l'adresse vers lat/lng via Google Geocoding API
        // Pour le moment, utiliser des coordonnées par défaut (Casablanca)
        await ValidateAndNavigate(address, 33.5731, -7.5898);
    }

    private async Task ValidateAndNavigate(string address, double lat, double lng)
    {
        var coverage = await _addressService.CheckCoverageAsync(lat, lng);
        if (coverage.IsCovered)
        {
            ShowNotCovered = false;
            _addressService.SetAddress(new DeliveryAddressDto
            {
                FullAddress = address,
                Latitude = lat,
                Longitude = lng,
                IsCovered = true
            });
            await Shell.Current.GoToAsync("//main/home");
        }
        else
        {
            ShowNotCovered = true;
            CoverageMessage = coverage.Message ?? "Désolé, nous ne livrons pas encore dans cette zone.";
            if (coverage.AvailableZones.Any())
                CoverageMessage += "\nZones disponibles : " + string.Join(", ", coverage.AvailableZones);
        }
    }
}
