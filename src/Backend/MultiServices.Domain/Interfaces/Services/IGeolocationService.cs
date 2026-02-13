using MultiServices.Domain.Common;
using MultiServices.Domain.ValueObjects;

namespace MultiServices.Domain.Interfaces.Services;

public interface IGeolocationService
{
    Task<Result<double>> CalculateDistanceAsync(GeoLocation from, GeoLocation to);
    Task<Result<int>> EstimateTravelTimeAsync(GeoLocation from, GeoLocation to, string mode = "driving");
    Task<Result<Address>> ReverseGeocodeAsync(double latitude, double longitude);
    Task<Result<GeoLocation>> GeocodeAsync(string address);
}
