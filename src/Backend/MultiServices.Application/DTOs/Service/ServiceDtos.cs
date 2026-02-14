using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Service;

// ==================== PROVIDER ====================
public record ServiceProviderListDto(Guid Id, string CompanyName, string? LogoUrl,
    double Rating, int ReviewCount, int YearsOfExperience, bool IsAvailable,
    List<string> ServiceCategories, decimal? StartingPrice, List<string> ServiceZones);

public record ServiceProviderDetailDto(Guid Id, string CompanyName, string? Description,
    string? LogoUrl, string? CoverImageUrl, double Rating, int ReviewCount,
    int YearsOfExperience, bool IsAvailable, string Phone,
    List<ServiceOfferingDto> Services, List<string> Certifications,
    List<PortfolioItemDto> Portfolio, List<ServiceProviderReviewDto> Reviews,
    List<string> ServiceZones, List<AvailabilitySlotDto> Availability);

/// <summary>Used by IServiceProviderService and IServiceProviderManagementService</summary>
public record ServiceProviderDto(Guid Id, string CompanyName, string? Description,
    string? LogoUrl, string? CoverImageUrl, double Rating, int ReviewCount,
    int YearsOfExperience, bool IsAvailable, string Phone,
    List<ServiceOfferingDto> Services, List<string> Certifications,
    List<PortfolioItemDto> Portfolio, List<ServiceProviderReviewDto> Reviews,
    List<string> ServiceZones, List<AvailabilitySlotDto> Availability);

// ==================== SEARCH ====================
/// <summary>Used by IServiceProviderService.SearchProvidersAsync</summary>
public class ServiceSearchRequest
{
    public string? Query { get; set; }
    public ServiceCategory? Category { get; set; }
    public double? MinRating { get; set; }
    public string? City { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsAvailable { get; set; }
    public double? UserLatitude { get; set; }
    public double? UserLongitude { get; set; }
    public double? MaxDistanceKm { get; set; }
    public string? SortBy { get; set; } = "rating";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ==================== SERVICE OFFERINGS ====================
public record ServiceOfferingDto(Guid Id, ServiceCategory Category, string Name,
    string? Description, decimal? HourlyRate, decimal? FixedPrice,
    decimal? TravelFee, int EstimatedDurationMinutes);

public record PortfolioItemDto(Guid Id, string Title, string? Description,
    string ImageUrl, string? BeforeImageUrl);

public record ServiceProviderReviewDto(Guid Id, string CustomerName, int OverallRating,
    int QualityRating, int PunctualityRating, string? Comment, string? Reply, DateTime CreatedAt);

// ==================== AVAILABILITY ====================
public record AvailabilitySlotDto(DayOfWeek DayOfWeek, string StartTime, string EndTime, bool IsAvailable);

/// <summary>Used by IServiceProviderService.GetAvailableSlotsAsync and IServiceProviderManagementService.UpdateAvailabilityAsync</summary>
public class AvailableSlotDto
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
}

// ==================== INTERVENTIONS ====================
/// <summary>Used by IInterventionService and IServiceProviderManagementService</summary>
public class InterventionDto
{
    public Guid Id { get; set; }
    public string InterventionNumber { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public string? ProviderLogoUrl { get; set; }
    public string ServiceName { get; set; } = "";
    public string Status { get; set; } = "";
    public string ProblemDescription { get; set; } = "";
    public List<string> ProblemPhotos { get; set; } = new();
    public DateTime ScheduledDate { get; set; }
    public string ScheduledStartTime { get; set; } = "";
    public string ScheduledEndTime { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal EstimatedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public decimal? QuoteAmount { get; set; }
    public bool? QuoteAccepted { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string PaymentStatus { get; set; } = "";
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
}

/// <summary>Used by IInterventionService.CreateInterventionAsync</summary>
public class CreateInterventionRequest
{
    public Guid ProviderId { get; set; }
    public Guid ServiceOfferingId { get; set; }
    public string ProblemDescription { get; set; } = "";
    public DateTime ScheduledDate { get; set; }
    public TimeSpan ScheduledStartTime { get; set; }
    public Guid InterventionAddressId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public bool PayBeforeIntervention { get; set; }
}

// ==================== PROVIDER MANAGEMENT ====================
/// <summary>Used by IServiceProviderManagementService.CreateProviderAsync / UpdateProviderAsync</summary>
public class CreateServiceProviderRequest
{
    public string CompanyName { get; set; } = "";
    public string? Description { get; set; }
    public ServiceCategory PrimaryCategory { get; set; }
    public List<ServiceCategory>? AdditionalCategories { get; set; }
    public int YearsOfExperience { get; set; }
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string City { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double MaxInterventionRadiusKm { get; set; } = 20;
    public List<string>? InterventionCities { get; set; }
    public decimal TravelFee { get; set; }
}

/// <summary>Used by IServiceProviderManagementService.SendQuoteAsync</summary>
public class SendQuoteRequest
{
    public Guid InterventionId { get; set; }
    public decimal EstimatedPrice { get; set; }
    public string? Details { get; set; }
    public decimal? MaterialCost { get; set; }
}

/// <summary>Used by IServiceProviderManagementService.UpdateInterventionStatusAsync</summary>
public class InterventionUpdateRequest
{
    public string Status { get; set; } = "";
    public string? Notes { get; set; }
    public decimal? FinalPrice { get; set; }
    public decimal? MaterialCost { get; set; }
    public string? InterventionReport { get; set; }
}

// ==================== BOOKINGS (legacy) ====================
public record CreateServiceBookingDto(Guid ProviderId, Guid ServiceOfferingId,
    string ProblemDescription, DateTime ScheduledDate, string ScheduledTime,
    Guid InterventionAddressId, PaymentMethod PaymentMethod, bool PayBeforeIntervention);

public record ServiceBookingDto(Guid Id, string BookingNumber, string Status,
    string ProviderName, string ServiceName, string ProblemDescription,
    decimal EstimatedPrice, decimal? FinalPrice, DateTime ScheduledDate,
    string ScheduledTime, string Address, string PaymentStatus,
    WorkerInfoDto? AssignedWorker, string? InterventionReport,
    string? BeforePhotos, string? AfterPhotos);

public record WorkerInfoDto(string Name, string? PhotoUrl, string Phone,
    double? Latitude, double? Longitude);

public record CreateServiceProviderDto(string CompanyName, string? Description,
    List<ServiceCategory> Categories, int YearsOfExperience, string Phone,
    List<CreateServiceOfferingDto> Services, List<string> ServiceZoneCities);

public record CreateServiceOfferingDto(ServiceCategory Category, string Name,
    string? Description, decimal? HourlyRate, decimal? FixedPrice,
    decimal? TravelFee, int EstimatedDurationMinutes);

public record SendQuoteDto(Guid BookingId, decimal EstimatedPrice, string? Details);
public record UpdateBookingStatusDto(Guid BookingId, string Status, string? Notes);
