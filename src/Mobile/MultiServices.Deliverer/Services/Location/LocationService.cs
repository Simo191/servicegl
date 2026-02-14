using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Models;
using MultiServices.Deliverer.Services.Api;
namespace MultiServices.Deliverer.Services.Location;
public class LocationService
{
    private readonly ApiService _api; private CancellationTokenSource? _cts; private bool _tracking;
    public event EventHandler<Microsoft.Maui.Devices.Sensors.Location>? LocationChanged;
    public Microsoft.Maui.Devices.Sensors.Location? CurrentLocation { get; private set; }
    public LocationService(ApiService api) => _api = api;
    public async Task<bool> RequestPermissionAsync() { var s = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>(); if (s != PermissionStatus.Granted) s = await Permissions.RequestAsync<Permissions.LocationWhenInUse>(); return s == PermissionStatus.Granted; }
    public async Task<Microsoft.Maui.Devices.Sensors.Location?> GetCurrentAsync() { try { var r = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10)); CurrentLocation = await Geolocation.Default.GetLocationAsync(r); return CurrentLocation; } catch { return null; } }
    public async Task StartTrackingAsync() { if (_tracking) return; if (!await RequestPermissionAsync()) return; _tracking = true; _cts = new(); _ = Task.Run(async () => { while (!_cts.Token.IsCancellationRequested) { try { var loc = await GetCurrentAsync(); if (loc != null) { LocationChanged?.Invoke(this, loc); await _api.PostAsync(AppConstants.DelivererLocationEndpoint, new LocationUpdate { Latitude = loc.Latitude, Longitude = loc.Longitude }); } } catch { } await Task.Delay(TimeSpan.FromSeconds(AppConstants.LocationUpdateIntervalSeconds), _cts.Token); } }, _cts.Token); }
    public void StopTracking() { _tracking = false; _cts?.Cancel(); _cts?.Dispose(); _cts = null; }
}
