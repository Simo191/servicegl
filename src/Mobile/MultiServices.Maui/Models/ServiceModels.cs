namespace MultiServices.Maui.Models;

public class ServiceProviderDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public int CompletedInterventions { get; set; }
    public double InterventionRadius { get; set; }
    public List<string> Certifications { get; set; } = new();
    public List<ServiceOfferingDto> Services { get; set; } = new();
    public List<PortfolioItemDto> Portfolio { get; set; } = new();
    public List<ReviewDto> Reviews { get; set; } = new();
    public double? Distance { get; set; }
}

public class ServiceProviderListDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public int YearsExperience { get; set; }
    public decimal StartingPrice { get; set; }
    public string PricingType { get; set; } = string.Empty;
    public double? Distance { get; set; }
}

public class ServiceOfferingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PricingType { get; set; } = string.Empty;
    public decimal? HourlyRate { get; set; }
    public decimal? FixedPrice { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public bool IsAvailable { get; set; }
}

public class PortfolioItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BeforeImageUrl { get; set; }
    public string? AfterImageUrl { get; set; }
}

public class AvailableSlotDto
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Display => $"{Date:dd/MM} {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
}

public class InterventionDto
{
    public Guid Id { get; set; }
    public string InterventionNumber { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderLogoUrl { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ProblemDescription { get; set; }
    public List<string> ProblemPhotos { get; set; } = new();
    public DateTime ScheduledDate { get; set; }
    public TimeSpan ScheduledStartTime { get; set; }
    public TimeSpan? ScheduledEndTime { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? FinalCost { get; set; }
    public string? IntervenantName { get; set; }
    public string? IntervenantPhone { get; set; }
    public double? IntervenantLatitude { get; set; }
    public double? IntervenantLongitude { get; set; }
    public List<string> BeforePhotos { get; set; } = new();
    public List<string> AfterPhotos { get; set; } = new();
    public string? Report { get; set; }
    public int? ActualDurationMinutes { get; set; }
    public bool HasWarranty { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public List<InterventionStatusHistoryDto> StatusHistory { get; set; } = new();
}

public class InterventionStatusHistoryDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Note { get; set; }
}

public class ServiceSearchRequest
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public double? MinRating { get; set; }
    public double? MaxDistance { get; set; }
    public string? PricingType { get; set; }
    public string? SortBy { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
