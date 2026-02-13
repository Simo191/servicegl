using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;

namespace MultiServices.Domain.Entities.Service;

/// <summary>
/// Service Provider entity - Module Services à Domicile
/// </summary>
public class ServiceProvider : AuditableEntity
{
    public Guid OwnerId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    
    // Contact
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Website { get; set; }
    
    // Location
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double MaxInterventionRadiusKm { get; set; } = 20;
    public List<string> InterventionCities { get; set; } = new();
    public List<string> InterventionQuarters { get; set; } = new();
    
    // Categories
    public ServiceCategory PrimaryCategory { get; set; }
    public List<ServiceCategory> AdditionalCategories { get; set; } = new();
    
    // Details
    public int YearsOfExperience { get; set; }
    public List<string> Certifications { get; set; } = new();
    public List<string> Qualifications { get; set; } = new();
    public string? InsuranceDocumentUrl { get; set; }
    public string? ProfessionalLicenseUrl { get; set; }
    public decimal TravelFee { get; set; } = 0;
    
    // Rating
    public double AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int TotalInterventions { get; set; } = 0;
    
    // Status
    public bool IsActive { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public bool IsAvailable { get; set; } = true;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    
    // Commission
    public decimal CommissionRate { get; set; } = 20;
    
    // Tax
    public string? TaxId { get; set; }
    public string? BusinessRegistrationNumber { get; set; }

    // Navigation
    public virtual ApplicationUser Owner { get; set; } = null!;
    public virtual ICollection<ServiceOffering> Services { get; set; } = new List<ServiceOffering>();
    public virtual ICollection<ServiceProviderAvailability> Availabilities { get; set; } = new List<ServiceProviderAvailability>();
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    public virtual ICollection<PortfolioItem> Portfolio { get; set; } = new List<PortfolioItem>();
    public virtual ICollection<ServiceIntervention> Interventions { get; set; } = new List<ServiceIntervention>();
}

public class ServiceOffering : AuditableEntity
{
    public Guid ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceCategory Category { get; set; }
    public ServiceSubCategory SubCategory { get; set; }
    public PricingType PricingType { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? FixedPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public string? MaterialIncluded { get; set; }
    public string? MaterialRequired { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }

    public virtual ServiceProvider Provider { get; set; } = null!;
}

public class ServiceProviderAvailability : BaseEntity
{
    public Guid ProviderId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;

    public virtual ServiceProvider Provider { get; set; } = null!;
}

public class ServiceProviderBlockedSlot : BaseEntity
{
    public Guid ProviderId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? Reason { get; set; }
    public bool IsRecurring { get; set; } = false;

    public virtual ServiceProvider Provider { get; set; } = null!;
}

public class TeamMember : AuditableEntity
{
    public Guid ProviderId { get; set; }
    public Guid? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Specialization { get; set; }
    public bool IsActive { get; set; } = true;
    public double AverageRating { get; set; } = 0;
    public int TotalInterventions { get; set; } = 0;

    public virtual ServiceProvider Provider { get; set; } = null!;
    public virtual ApplicationUser? User { get; set; }
}

public class PortfolioItem : BaseEntity
{
    public Guid ProviderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> BeforeImageUrls { get; set; } = new();
    public List<string> AfterImageUrls { get; set; } = new();
    public ServiceCategory Category { get; set; }
    public DateTime CompletedDate { get; set; }

    public virtual ServiceProvider Provider { get; set; } = null!;
}

// ==================== SERVICE INTERVENTIONS ====================
public class ServiceIntervention : AuditableEntity
{
    public string InterventionNumber { get; set; } = string.Empty; // "SVC-20240101-001"
    public Guid CustomerId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid ServiceOfferingId { get; set; }
    public Guid? AssignedTeamMemberId { get; set; }
    
    // Problem Description
    public string ProblemDescription { get; set; } = string.Empty;
    public List<string> ProblemImageUrls { get; set; } = new();
    
    // Address
    public string InterventionStreet { get; set; } = string.Empty;
    public string InterventionCity { get; set; } = string.Empty;
    public string? InterventionApartment { get; set; }
    public string? InterventionFloor { get; set; }
    public double InterventionLatitude { get; set; }
    public double InterventionLongitude { get; set; }
    
    // Schedule
    public DateTime ScheduledDate { get; set; }
    public TimeOnly ScheduledStartTime { get; set; }
    public TimeOnly ScheduledEndTime { get; set; }
    public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
    
    // Status
    public InterventionStatus Status { get; set; } = InterventionStatus.Reserved;
    
    // Pricing
    public decimal EstimatedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public decimal TravelFee { get; set; } = 0;
    public decimal? MaterialCost { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Payment
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? PaymentIntentId { get; set; }
    public string? TransactionId { get; set; }
    public bool PayBeforeIntervention { get; set; } = false;
    
    // Quote
    public bool QuoteRequested { get; set; } = false;
    public decimal? QuoteAmount { get; set; }
    public bool? QuoteAccepted { get; set; }
    public DateTime? QuoteSentAt { get; set; }
    public DateTime? QuoteRespondedAt { get; set; }
    
    // Times
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? EnRouteAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public int? ActualDurationMinutes { get; set; }
    
    // Before/After
    public List<string> BeforeImageUrls { get; set; } = new();
    public List<string> AfterImageUrls { get; set; } = new();
    public string? InterventionReport { get; set; }
    public string? IntervenantNotes { get; set; }
    
    // Rating
    public int? ProviderRating { get; set; }
    public int? IntervenantRating { get; set; }
    public string? ReviewComment { get; set; }
    
    // Warranty
    public bool HasWarranty { get; set; } = false;
    public int? WarrantyDays { get; set; }
    public DateTime? WarrantyExpiresAt { get; set; }

    // Navigation
    public virtual ApplicationUser Customer { get; set; } = null!;
    public virtual ServiceProvider Provider { get; set; } = null!;
    public virtual ServiceOffering ServiceOffering { get; set; } = null!;
    public virtual TeamMember? AssignedTeamMember { get; set; }
    public virtual ICollection<InterventionStatusHistory> StatusHistory { get; set; } = new List<InterventionStatusHistory>();
}

public class InterventionStatusHistory : BaseEntity
{
    public Guid InterventionId { get; set; }
    public InterventionStatus Status { get; set; }
    public string? Note { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public virtual ServiceIntervention Intervention { get; set; } = null!;
}
