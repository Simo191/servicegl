using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.Services;

/// <summary>
/// Service qui gère l'adresse de livraison et la validation de la zone de couverture.
/// Stocke l'adresse localement SANS nécessiter de compte.
/// </summary>
public class AddressService
{
    private readonly ApiService _api;
    private DeliveryAddressDto? _currentAddress;

    public AddressService(ApiService api) { _api = api; }

    public DeliveryAddressDto? CurrentAddress => _currentAddress;
    public bool HasAddress => _currentAddress != null;

    public void SetAddress(DeliveryAddressDto address)
    {
        _currentAddress = address;
        // Sauvegarder localement
        Preferences.Default.Set("last_address", address.FullAddress);
        Preferences.Default.Set("last_lat", address.Latitude);
        Preferences.Default.Set("last_lng", address.Longitude);
    }

    public DeliveryAddressDto? GetSavedAddress()
    {
        var addr = Preferences.Default.Get("last_address", string.Empty);
        if (string.IsNullOrEmpty(addr)) return null;
        return new DeliveryAddressDto
        {
            FullAddress = addr,
            Latitude = Preferences.Default.Get("last_lat", 0.0),
            Longitude = Preferences.Default.Get("last_lng", 0.0),
            IsCovered = true
        };
    }

    public async Task<CoverageCheckResult> CheckCoverageAsync(double lat, double lng)
    {
        var result = await _api.GetAsync<CoverageCheckResult>("/coverage/check",
            new() { ["lat"] = lat.ToString(), ["lng"] = lng.ToString() });
        return result.Data ?? new CoverageCheckResult { IsCovered = false, Message = "Impossible de vérifier la couverture" };
    }
}
