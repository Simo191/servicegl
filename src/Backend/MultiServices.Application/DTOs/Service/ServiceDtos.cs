using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Service;

public record ServiceProviderListDto(Guid Id, string CompanyName, string? LogoUrl,
    double Rating, int ReviewCount, int YearsOfExperience, bool IsAvailable,
    List<string> ServiceCategories, decimal? StartingPrice, List<string> ServiceZones);

public record ServiceProviderDetailDto(Guid Id, string CompanyName, string? Description,
    string? LogoUrl, string? CoverImageUrl, double Rating, int ReviewCount,
    int YearsOfExperience, bool IsAvailable, string Phone,
    List<ServiceOfferingDto> Services, List<string> Certifications,
    List<PortfolioItemDto> Portfolio, List<ServiceProviderReviewDto> Reviews,
    List<string> ServiceZones, List<AvailabilitySlotDto> Availability);

public record ServiceOfferingDto(Guid Id, ServiceCategory Category, string Name,
    string? Description, decimal? HourlyRate, decimal? FixedPrice,
    decimal? TravelFee, int EstimatedDurationMinutes);

public record PortfolioItemDto(Guid Id, string Title, string? Description,
    string ImageUrl, string? BeforeImageUrl);

public record ServiceProviderReviewDto(Guid Id, string CustomerName, int OverallRating,
    int QualityRating, int PunctualityRating, string? Comment, string? Reply, DateTime CreatedAt);

public record AvailabilitySlotDto(DayOfWeek DayOfWeek, string StartTime, string EndTime, bool IsAvailable);

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
