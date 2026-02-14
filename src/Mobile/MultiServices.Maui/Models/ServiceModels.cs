namespace MultiServices.Maui.Models;

/// <summary>
/// Aligned with backend ServiceProviderDetailDto / ServiceProviderDto
/// Key changes: ReviewCount, YearsOfExperience, Services (List of ServiceOfferingDto)
/// </summary>
public class ServiceProviderDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string Phone { get; set; } = string.Empty;

    // Backend fields
    public int YearsOfExperience { get; set; }  // Was: YearsExperience
    public double Rating { get; set; }
    public int ReviewCount { get; set; }  // Was: TotalReviews
    public bool IsAvailable { get; set; }

    public List<ServiceOfferingDto> Services { get; set; } = new();
    public List<string> Certifications { get; set; } = new();
    public List<PortfolioItemDto> Portfolio { get; set; } = new();
    public List<ServiceProviderReviewDto> Reviews { get; set; } = new();
    public List<string> ServiceZones { get; set; } = new();
    public List<AvailabilitySlotDto> Availability { get; set; } = new();

    // Backward compat computed
    public int YearsExperience => YearsOfExperience;
    public int TotalReviews => ReviewCount;
    public double? Distance { get; set; }
}

/// <summary>
/// Aligned with backend ServiceProviderListDto
/// Key changes: ServiceCategories (List), ReviewCount, YearsOfExperience, StartingPrice
/// </summary>
public class ServiceProviderListDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }

    // Backend uses List<string> not single string
    public List<string> ServiceCategories { get; set; } = new();
    public double Rating { get; set; }
    public int ReviewCount { get; set; }  // Was: TotalReviews
    public int YearsOfExperience { get; set; }  // Was: YearsExperience
    public decimal? StartingPrice { get; set; }  // Was: StartingPrice (decimal)
    public List<string> ServiceZones { get; set; } = new();
    public bool IsAvailable { get; set; }
    public double? Distance { get; set; }

    // Backward compat computed
    public string Category => ServiceCategories?.FirstOrDefault() ?? string.Empty;
    public int TotalReviews => ReviewCount;
    public int YearsExperience => YearsOfExperience;
}

/// <summary>
/// Aligned with backend ServiceOfferingDto
/// </summary>
public class ServiceOfferingDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? FixedPrice { get; set; }
    public decimal? TravelFee { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public bool IsAvailable { get; set; }

    // Computed
    public string PricingType => HourlyRate.HasValue ? "hourly" : "fixed";
}

public class PortfolioItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? BeforeImageUrl { get; set; }

    // Backward compat
    public string? AfterImageUrl => ImageUrl;
}

/// <summary>
/// Aligned with backend ServiceProviderReviewDto
/// </summary>
public class ServiceProviderReviewDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int OverallRating { get; set; }
    public int QualityRating { get; set; }
    public int PunctualityRating { get; set; }
    public string? Comment { get; set; }
    public string? Reply { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Aligned with backend AvailabilitySlotDto (schedule config)
/// </summary>
public class AvailabilitySlotDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

/// <summary>
/// Aligned with backend AvailableSlotDto (bookable time slots)
/// </summary>
public class AvailableSlotDto
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string Display => $"{Date:dd/MM} {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
}

/// <summary>
/// Aligned with backend InterventionDto
/// </summary>
public class InterventionDto
{
    public Guid Id { get; set; }
    public string InterventionNumber { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderLogoUrl { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ProblemDescription { get; set; } = string.Empty;
    public List<string> ProblemPhotos { get; set; } = new();
    public DateTime ScheduledDate { get; set; }
    public string ScheduledStartTime { get; set; } = string.Empty;
    public string ScheduledEndTime { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal EstimatedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public decimal? QuoteAmount { get; set; }
    public bool? QuoteAccepted { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? AssignedTeamMemberName { get; set; }
    public string? AssignedTeamMemberPhone { get; set; }
    public string? InterventionReport { get; set; }
    public List<string> BeforePhotos { get; set; } = new();
    public List<string> AfterPhotos { get; set; } = new();
    public int? ProviderRating { get; set; }
    public string? ReviewComment { get; set; }
    public bool HasWarranty { get; set; }
    public DateTime? WarrantyExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<InterventionStatusHistoryDto> StatusHistory { get; set; } = new();

    // Backward compat
    public decimal? EstimatedCost => EstimatedPrice;
    public decimal? FinalCost => FinalPrice;
    public string? IntervenantName => AssignedTeamMemberName;
    public string? IntervenantPhone => AssignedTeamMemberPhone;
    public string? Report => InterventionReport;
    public DateTime? WarrantyEndDate => WarrantyExpiresAt;
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
    public string? City { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
