using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;
using MultiServices.Domain.ValueObjects;

namespace MultiServices.Domain.Entities.Restaurant;

/// <summary>
/// Restaurant entity - Module Restaurant
/// </summary>
public class RestaurantEntity : AuditableEntity
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<string> GalleryUrls { get; set; } = new();
    
    // Location
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    // Contact
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Website { get; set; }
    
    // Details
    public CuisineType CuisineType { get; set; }
    public List<CuisineType> AdditionalCuisines { get; set; } = new();
    public PriceRange PriceRange { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal FreeDeliveryMinimum { get; set; }
    public int AveragePreparationMinutes { get; set; }
    public int AverageDeliveryMinutes { get; set; }
    public double MaxDeliveryDistanceKm { get; set; }
    
    // Rating
    public double AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int TotalOrders { get; set; } = 0;
    
    // Status
    public bool IsActive { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public bool IsOpen { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public bool AcceptingOrders { get; set; } = true;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    
    // Commission
    public decimal CommissionRate { get; set; } = 15; // Percentage
    
    // Tax
    public string? TaxId { get; set; }
    public string? BusinessLicenseUrl { get; set; }

    // Navigation
    public virtual ApplicationUser Owner { get; set; } = null!;
    public virtual ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();
    public virtual ICollection<RestaurantOpeningHours> OpeningHours { get; set; } = new List<RestaurantOpeningHours>();
    public virtual ICollection<RestaurantOrder> Orders { get; set; } = new List<RestaurantOrder>();
    public virtual ICollection<RestaurantPromotion> Promotions { get; set; } = new List<RestaurantPromotion>();
}

public class RestaurantOpeningHours : BaseEntity
{
    public Guid RestaurantId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsClosed { get; set; } = false;

    public virtual RestaurantEntity Restaurant { get; set; } = null!;
}

public class MenuCategory : AuditableEntity
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual RestaurantEntity Restaurant { get; set; } = null!;
    public virtual ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
}

public class MenuItem : AuditableEntity
{
    public Guid CategoryId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> AdditionalImages { get; set; } = new();
    public bool IsAvailable { get; set; } = true;
    public bool IsPopular { get; set; } = false;
    public bool IsNew { get; set; } = false;
    public int PreparationMinutes { get; set; }
    public int SortOrder { get; set; }
    
    // Allergens & Nutrition
    public List<string> Allergens { get; set; } = new();
    public string? NutritionalInfoJson { get; set; } // Serialized NutritionalInfo
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsHalal { get; set; } = true;
    public bool IsSpicy { get; set; }
    public int? SpicyLevel { get; set; } // 1-5

    // Navigation
    public virtual MenuCategory Category { get; set; } = null!;
    public virtual RestaurantEntity Restaurant { get; set; } = null!;
    public virtual ICollection<MenuItemOption> Options { get; set; } = new List<MenuItemOption>();
    public virtual ICollection<MenuItemExtra> Extras { get; set; } = new List<MenuItemExtra>();
    public virtual ICollection<MenuItemSize> Sizes { get; set; } = new List<MenuItemSize>();
}

public class MenuItemSize : BaseEntity
{
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty; // "Small", "Medium", "Large"
    public decimal Price { get; set; }
    public int SortOrder { get; set; }
    public virtual MenuItem MenuItem { get; set; } = null!;
}

public class MenuItemExtra : BaseEntity
{
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty; // "Extra Cheese", "Bacon"
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int MaxQuantity { get; set; } = 5;
    public int SortOrder { get; set; }
    public virtual MenuItem MenuItem { get; set; } = null!;
}

public class MenuItemOption : BaseEntity
{
    public Guid MenuItemId { get; set; }
    public string GroupName { get; set; } = string.Empty; // "Remove ingredient"
    public string Name { get; set; } = string.Empty; // "No onion", "No tomato"
    public decimal PriceModifier { get; set; } = 0;
    public bool IsDefault { get; set; } = false;
    public virtual MenuItem MenuItem { get; set; } = null!;
}

// ==================== RESTAURANT ORDERS ====================
public class RestaurantOrder : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty; // e.g. "RST-20240101-001"
    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid? DelivererId { get; set; }
    
    // Delivery Address
    public string DeliveryStreet { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public string? DeliveryApartment { get; set; }
    public string? DeliveryFloor { get; set; }
    public string? DeliveryInstructions { get; set; }
    public double DeliveryLatitude { get; set; }
    public double DeliveryLongitude { get; set; }
    
    // Order Details
    public RestaurantOrderStatus Status { get; set; } = RestaurantOrderStatus.Received;
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal TipAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    
    // Payment
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? PaymentIntentId { get; set; }
    public string? TransactionId { get; set; }
    
    // Promo
    public string? PromoCode { get; set; }
    public decimal PromoDiscount { get; set; } = 0;
    
    // Scheduling
    public bool IsScheduled { get; set; } = false;
    public DateTime? ScheduledFor { get; set; }
    
    // Times
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? PreparingAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    
    // Extras
    public bool RequestCutlery { get; set; } = false;
    public bool RequestNapkins { get; set; } = false;
    public string? SpecialInstructions { get; set; }
    
    // Rating
    public int? RestaurantRating { get; set; }
    public int? DeliveryRating { get; set; }
    public string? ReviewComment { get; set; }

    // Navigation
    public virtual ApplicationUser Customer { get; set; } = null!;
    public virtual RestaurantEntity Restaurant { get; set; } = null!;
    public virtual ApplicationUser? Deliverer { get; set; }
    public virtual ICollection<RestaurantOrderItem> Items { get; set; } = new List<RestaurantOrderItem>();
    public virtual ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}

public class RestaurantOrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SelectedSize { get; set; }
    public decimal SizePrice { get; set; } = 0;
    public string? SpecialInstructions { get; set; }
    public string? SelectedExtrasJson { get; set; } // JSON
    public string? SelectedOptionsJson { get; set; } // JSON
    public string? RemovedIngredientsJson { get; set; } // JSON

    public virtual RestaurantOrder Order { get; set; } = null!;
    public virtual MenuItem MenuItem { get; set; } = null!;
}

public class OrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public virtual RestaurantOrder Order { get; set; } = null!;
}

public class RestaurantPromotion : AuditableEntity
{
    public Guid RestaurantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public string DiscountType { get; set; } = "Percentage"; // "Percentage", "Fixed"
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? MaxUsages { get; set; }
    public int CurrentUsages { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }
    public bool FreeDelivery { get; set; } = false;

    public virtual RestaurantEntity Restaurant { get; set; } = null!;
}
