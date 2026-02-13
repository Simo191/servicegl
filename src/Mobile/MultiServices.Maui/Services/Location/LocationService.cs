namespace MultiServices.Maui.Services.Location;

public class LocationService
{
    private Microsoft.Maui.Devices.Sensors.Location? _lastLocation;

    public Microsoft.Maui.Devices.Sensors.Location? LastLocation => _lastLocation;

    public async Task<Microsoft.Maui.Devices.Sensors.Location?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    return null;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            _lastLocation = await Geolocation.Default.GetLocationAsync(request);
            return _lastLocation;
        }
        catch
        {
            try
            {
                _lastLocation = await Geolocation.Default.GetLastKnownLocationAsync();
                return _lastLocation;
            }
            catch { return null; }
        }
    }

    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var loc1 = new Microsoft.Maui.Devices.Sensors.Location(lat1, lon1);
        var loc2 = new Microsoft.Maui.Devices.Sensors.Location(lat2, lon2);
        return Microsoft.Maui.Devices.Sensors.Location.CalculateDistance(loc1, loc2, DistanceUnits.Kilometers);
    }
}
