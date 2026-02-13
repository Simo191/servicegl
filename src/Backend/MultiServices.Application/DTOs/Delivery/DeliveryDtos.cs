namespace MultiServices.Application.DTOs.Delivery;

public record DeliveryDriverDto(Guid Id, string Name, string? PhotoUrl,
    double Rating, int TotalDeliveries, decimal TotalEarnings, bool IsOnline,
    bool IsAvailable, string? VehicleType);

public record AvailableDeliveryDto(Guid Id, string Type, string PickupAddress,
    string DeliveryAddress, decimal EstimatedEarning, double DistanceKm,
    string? RestaurantName, string? StoreName);

public record DeliveryEarningDto(Guid Id, decimal BaseAmount, decimal TipAmount,
    decimal BonusAmount, decimal TotalAmount, string Type, DateTime Date, bool IsPaid);

public record UpdateLocationDto(double Latitude, double Longitude);
public record UpdateDeliveryStatusDto(string Status, string? ProofImageUrl);
public record DriverRegistrationDto(string FirstName, string LastName, string Phone,
    string? VehicleType, string? VehiclePlate);
