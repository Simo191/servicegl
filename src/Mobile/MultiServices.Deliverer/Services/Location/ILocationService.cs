namespace MultiServices.Deliverer.Services.Location;

public interface ILocationService
{
    Task<bool> RequestPermissionAsync();
    Task<(double lat, double lng)?> GetCurrentLocationAsync();
    Task StartTrackingAsync(Func<double, double, Task> onLocationUpdate);
    Task StopTrackingAsync();
    bool IsTracking { get; }
}
