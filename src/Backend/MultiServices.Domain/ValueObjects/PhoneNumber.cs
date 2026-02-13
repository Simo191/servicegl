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
