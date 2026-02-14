namespace MultiServices.Maui.Models;

public class DeliveryAddressDto
{
    public string FullAddress { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Label { get; set; } // Maison, Travail, Autre
    public bool IsCovered { get; set; }
}

public class CoverageCheckResult
{
    public bool IsCovered { get; set; }
    public string? Message { get; set; }
    public List<string> AvailableZones { get; set; } = new();
}

public class OtpRequest { public string PhoneNumber { get; set; } = string.Empty; }
public class OtpVerifyRequest { public string PhoneNumber { get; set; } = string.Empty; public string Code { get; set; } = string.Empty; }
public class QuickRegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}
