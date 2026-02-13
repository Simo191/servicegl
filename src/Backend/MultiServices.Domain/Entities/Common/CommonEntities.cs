using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;

namespace MultiServices.Domain.Entities.Common;

/// <summary>
/// Deliverer / Intervenant entity
/// </summary>
public class Deliverer : AuditableEntity
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    
    // Vehicle
    public VehicleType VehicleType { get; set; }
    public string? VehiclePlateNumber { get; set; }
    public string? VehicleModel { get; set; }
    
    // Location
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }
    
    // Status
    public DelivererStatus Status { get; set; } = DelivererStatus.Offline;
    public bool IsActive { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    
    // Stats
    public double AverageRating { get; set; } = 0;
    public int TotalDeliveries { get; set; } = 0;
    public int TotalInterventions { get; set; } = 0;
    public decimal TotalEarnings { get; set; } = 0;
    public double AcceptanceRate { get; set; } = 100;
    
    // Documents
    public bool HasTrainingCompleted { get; set; } = false;
    public DateTime? TrainingCompletedAt { get; set; }
    
    // Emergency
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    
    // Commission
    public decimal DeliveryBaseFee { get; set; }
    public decimal PerKmFee { get; set; }

    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<DelivererEarning> Earnings { get; set; } = new List<DelivererEarning>();
    public virtual ICollection<DelivererPayout> Payouts { get; set; } = new List<DelivererPayout>();
}

public class DelivererEarning : BaseEntity
{
    public Guid DelivererId { get; set; }
    public string OrderType { get; set; } = string.Empty; // "Restaurant", "Grocery", "Service"
    public Guid OrderId { get; set; }
    public decimal BaseFee { get; set; }
    public decimal DistanceFee { get; set; }
    public decimal TipAmount { get; set; }
    public decimal BonusAmount { get; set; }
    public decimal TotalEarning { get; set; }

    public virtual Deliverer Deliverer { get; set; } = null!;
}

public class DelivererPayout : BaseEntity
{
    public Guid DelivererId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Processing", "Completed", "Failed"
    public string? BankAccount { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public virtual Deliverer Deliverer { get; set; } = null!;
}

/// <summary>
/// Global Promotion Codes
/// </summary>
public class PromoCode : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? MaxUsages { get; set; }
    public int? MaxUsagesPerUser { get; set; }
    public int CurrentUsages { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string ApplicableModule { get; set; } = "All"; // "All", "Restaurant", "Service", "Grocery"
    public bool FreeDelivery { get; set; } = false;
}

public class PromoCodeUsage : BaseEntity
{
    public Guid PromoCodeId { get; set; }
    public Guid UserId { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public decimal DiscountApplied { get; set; }

    public virtual PromoCode PromoCode { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}

/// <summary>
/// Support Ticket
/// </summary>
public class SupportTicket : AuditableEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? OrderType { get; set; }
    public Guid? OrderId { get; set; }
    public ReportReason? ReportReason { get; set; }
    public string Status { get; set; } = "Open"; // "Open", "InProgress", "Resolved", "Closed"
    public string Priority { get; set; } = "Normal"; // "Low", "Normal", "High", "Urgent"
    public Guid? AssignedToId { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<string> AttachmentUrls { get; set; } = new();

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationUser? AssignedTo { get; set; }
    public virtual ICollection<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
}

public class SupportMessage : BaseEntity
{
    public Guid TicketId { get; set; }
    public Guid SenderId { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> AttachmentUrls { get; set; } = new();
    public bool IsFromAdmin { get; set; } = false;

    public virtual SupportTicket Ticket { get; set; } = null!;
    public virtual ApplicationUser Sender { get; set; } = null!;
}

/// <summary>
/// Marketing Campaign
/// </summary>
public class MarketingCampaign : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Email", "SMS", "Push", "Banner"
    public string? Subject { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? TargetAudience { get; set; } // JSON filter
    public DateTime? ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string Status { get; set; } = "Draft"; // "Draft", "Scheduled", "Sent", "Cancelled"
    public int TotalRecipients { get; set; } = 0;
    public int TotalOpened { get; set; } = 0;
    public int TotalClicked { get; set; } = 0;
}

/// <summary>
/// Banner Promotion
/// </summary>
public class BannerPromotion : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public string? TargetModule { get; set; } // "Restaurant", "Service", "Grocery", "All"
    public Guid? TargetEntityId { get; set; }
    public int SortOrder { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string Placement { get; set; } = "Home"; // "Home", "Restaurant", "Service", "Grocery"
}

/// <summary>
/// System Configuration
/// </summary>
public class SystemConfiguration : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty; // "General", "Payment", "Commission", etc.
    public string ValueType { get; set; } = "String"; // "String", "Number", "Boolean", "JSON"
}

/// <summary>
/// Geographic Zone
/// </summary>
public class GeographicZone : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? PolygonJson { get; set; } // GeoJSON polygon
    public bool IsActive { get; set; } = true;
    public decimal? SurchargeAmount { get; set; }
    public double? SurchargePercentage { get; set; }
}

/// <summary>
/// Audit Log
/// </summary>
public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

/// <summary>
/// Payment Transaction Log
/// </summary>
public class PaymentTransaction : AuditableEntity
{
    public Guid UserId { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MAD";
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? StripeChargeId { get; set; }
    public string? PayPalTransactionId { get; set; }
    public string? FailureReason { get; set; }
    public string? ReceiptUrl { get; set; }
    public bool Is3DSecure { get; set; } = false;
    public decimal? RefundedAmount { get; set; }
    public DateTime? RefundedAt { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}
