#!/bin/bash
set -e
cd /home/claude/MultiServicesApp

# ============================================
# 1. DOMAIN LAYER - Project File
# ============================================
cat > src/Backend/MultiServices.Domain/MultiServices.Domain.csproj << 'CSPROJ'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.4.1" />
  </ItemGroup>
</Project>
CSPROJ

# ============================================
# DOMAIN - Common
# ============================================
cat > src/Backend/MultiServices.Domain/Common/BaseEntity.cs << 'EOF'
namespace MultiServices.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
EOF

cat > src/Backend/MultiServices.Domain/Common/AggregateRoot.cs << 'EOF'
namespace MultiServices.Domain.Common;

public abstract class AggregateRoot : BaseEntity
{
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(DomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
EOF

cat > src/Backend/MultiServices.Domain/Common/DomainEvent.cs << 'EOF'
using MediatR;

namespace MultiServices.Domain.Common;

public abstract class DomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}
EOF

cat > src/Backend/MultiServices.Domain/Common/Result.cs << 'EOF'
namespace MultiServices.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public List<string> Errors { get; } = new();

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public new static Result<T> Failure(string error) => new(false, default, error);
}
EOF

# ============================================
# DOMAIN - Enums
# ============================================
cat > src/Backend/MultiServices.Domain/Enums/OrderStatus.cs << 'EOF'
namespace MultiServices.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    Ready = 3,
    PickedUp = 4,
    InTransit = 5,
    Delivered = 6,
    Cancelled = 7,
    Refunded = 8
}

public enum ServiceOrderStatus
{
    Pending = 0,
    Confirmed = 1,
    ProviderEnRoute = 2,
    ProviderArrived = 3,
    InProgress = 4,
    Completed = 5,
    Cancelled = 6,
    Disputed = 7
}

public enum GroceryOrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Picking = 2,
    Ready = 3,
    InTransit = 4,
    Delivered = 5,
    Cancelled = 6,
    Refunded = 7
}
EOF

cat > src/Backend/MultiServices.Domain/Enums/UserRole.cs << 'EOF'
namespace MultiServices.Domain.Enums;

public enum UserRole
{
    Client = 0,
    RestaurantOwner = 1,
    ServiceProvider = 2,
    StoreManager = 3,
    DeliveryDriver = 4,
    ServiceWorker = 5,
    Admin = 6,
    SuperAdmin = 7
}
EOF

cat > src/Backend/MultiServices.Domain/Enums/PaymentMethod.cs << 'EOF'
namespace MultiServices.Domain.Enums;

public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    ApplePay = 2,
    GooglePay = 3,
    PayPal = 4,
    CashOnDelivery = 5,
    Wallet = 6
}

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    PartiallyRefunded = 5
}
EOF

cat > src/Backend/MultiServices.Domain/Enums/ServiceCategory.cs << 'EOF'
namespace MultiServices.Domain.Enums;

public enum ServiceCategory
{
    Plumbing = 0,
    Electrical = 1,
    Cleaning = 2,
    Painting = 3,
    Gardening = 4,
    AirConditioning = 5,
    Moving = 6,
    Repair = 7
}

public enum CuisineType
{
    Italian = 0,
    Asian = 1,
    Burger = 2,
    Pizza = 3,
    Moroccan = 4,
    French = 5,
    Sushi = 6,
    Mexican = 7,
    Indian = 8,
    Turkish = 9,
    Seafood = 10,
    Vegetarian = 11,
    FastFood = 12,
    Desserts = 13,
    Other = 99
}

public enum GroceryCategory
{
    FruitsVegetables = 0,
    MeatFish = 1,
    SavoryGrocery = 2,
    SweetGrocery = 3,
    Beverages = 4,
    Dairy = 5,
    Frozen = 6,
    HygieneBeauty = 7,
    Cleaning = 8,
    BabyChild = 9
}
EOF

cat > src/Backend/MultiServices.Domain/Enums/NotificationType.cs << 'EOF'
namespace MultiServices.Domain.Enums;

public enum NotificationType
{
    Push = 0,
    Email = 1,
    Sms = 2
}

public enum PriceRange
{
    Budget = 1,
    Medium = 2,
    Premium = 3
}

public enum DocumentType
{
    IdentityCard = 0,
    DrivingLicense = 1,
    ProfessionalLicense = 2,
    Certificate = 3,
    Insurance = 4,
    VehicleRegistration = 5
}

public enum VerificationStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Suspended = 3
}
EOF

# ============================================
# DOMAIN - Value Objects
# ============================================
cat > src/Backend/MultiServices.Domain/ValueObjects/Address.cs << 'EOF'
namespace MultiServices.Domain.ValueObjects;

public record Address
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = "MA";
    public string? Apartment { get; init; }
    public string? BuildingName { get; init; }
    public string? Floor { get; init; }
    public string? AdditionalInfo { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Label { get; init; } // "Maison", "Travail"
}
EOF

cat > src/Backend/MultiServices.Domain/ValueObjects/GeoLocation.cs << 'EOF'
namespace MultiServices.Domain.ValueObjects;

public record GeoLocation(double Latitude, double Longitude)
{
    public double DistanceTo(GeoLocation other)
    {
        const double R = 6371;
        var dLat = ToRad(other.Latitude - Latitude);
        var dLon = ToRad(other.Longitude - Longitude);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(Latitude)) * Math.Cos(ToRad(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
EOF

cat > src/Backend/MultiServices.Domain/ValueObjects/Money.cs << 'EOF'
namespace MultiServices.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "MAD";

    public Money(decimal amount, string currency = "MAD")
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative");
        Amount = amount;
        Currency = currency;
    }

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency) throw new InvalidOperationException("Cannot add different currencies");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency) throw new InvalidOperationException("Cannot subtract different currencies");
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static Money Zero => new(0);
}
EOF

cat > src/Backend/MultiServices.Domain/ValueObjects/PhoneNumber.cs << 'EOF'
namespace MultiServices.Domain.ValueObjects;

public record PhoneNumber
{
    public string CountryCode { get; init; } = "+212";
    public string Number { get; init; } = string.Empty;

    public string FullNumber => $"{CountryCode}{Number}";

    public static PhoneNumber Create(string countryCode, string number)
    {
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Phone number is required");
        return new PhoneNumber { CountryCode = countryCode, Number = number.Trim() };
    }
}
EOF

cat > src/Backend/MultiServices.Domain/ValueObjects/Rating.cs << 'EOF'
namespace MultiServices.Domain.ValueObjects;

public record Rating
{
    public double Value { get; init; }
    public int Count { get; init; }

    public Rating(double value, int count)
    {
        if (value < 0 || value > 5) throw new ArgumentException("Rating must be between 0 and 5");
        Value = value;
        Count = count;
    }

    public Rating AddRating(int newRating)
    {
        var totalValue = Value * Count + newRating;
        var newCount = Count + 1;
        return new Rating(totalValue / newCount, newCount);
    }

    public static Rating Empty => new(0, 0);
}
EOF

cat > src/Backend/MultiServices.Domain/ValueObjects/TimeSlot.cs << 'EOF'
namespace MultiServices.Domain.ValueObjects;

public record TimeSlot
{
    public DayOfWeek Day { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public bool IsAvailable { get; init; } = true;

    public bool Overlaps(TimeSlot other)
    {
        return Day == other.Day && StartTime < other.EndTime && EndTime > other.StartTime;
    }
}
EOF

echo "Domain layer created"
