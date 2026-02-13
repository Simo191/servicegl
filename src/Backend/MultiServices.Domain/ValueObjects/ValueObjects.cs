namespace MultiServices.Domain.ValueObjects;

public record Address
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = "Morocco";
    public string? Apartment { get; init; }
    public string? Floor { get; init; }
    public string? BuildingName { get; init; }
    public string? AdditionalInfo { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Label { get; init; } // "Maison", "Travail"
    public bool IsDefault { get; init; }
}

public record GeoLocation
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "MAD"; // Moroccan Dirham

    public static Money Zero => new() { Amount = 0 };
    public static Money operator +(Money a, Money b) => new() { Amount = a.Amount + b.Amount, Currency = a.Currency };
    public static Money operator -(Money a, Money b) => new() { Amount = a.Amount - b.Amount, Currency = a.Currency };
    public static Money operator *(Money a, decimal multiplier) => new() { Amount = a.Amount * multiplier, Currency = a.Currency };
}

public record TimeSlot
{
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public DayOfWeek Day { get; init; }
}

public record ContactInfo
{
    public string Phone { get; init; } = string.Empty;
    public string? AlternatePhone { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Website { get; init; }
}

public record Rating
{
    public double Average { get; init; }
    public int TotalReviews { get; init; }
    public int FiveStars { get; init; }
    public int FourStars { get; init; }
    public int ThreeStars { get; init; }
    public int TwoStars { get; init; }
    public int OneStar { get; init; }
}

public record DeliveryInfo
{
    public decimal DeliveryFee { get; init; }
    public decimal FreeDeliveryMinimum { get; init; }
    public int EstimatedMinutes { get; init; }
    public double MaxDistanceKm { get; init; }
}

public record NutritionalInfo
{
    public int Calories { get; init; }
    public decimal Protein { get; init; }
    public decimal Carbohydrates { get; init; }
    public decimal Fat { get; init; }
    public decimal Fiber { get; init; }
    public decimal Sugar { get; init; }
    public decimal Salt { get; init; }
}
