using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;

namespace MultiServices.Domain.Entities.Grocery;

/// <summary>
/// Grocery Store entity - Module Courses en Ligne
/// </summary>
public class GroceryStoreEntity : AuditableEntity
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public GroceryStore StoreType { get; set; }
    public string LogoUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    
    // Location
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    // Contact
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    
    // Delivery
    public decimal DeliveryFee { get; set; }
    public decimal FreeDeliveryMinimum { get; set; }
    public double MaxDeliveryDistanceKm { get; set; }
    public int AveragePreparationMinutes { get; set; }
    public decimal MinOrderAmount { get; set; }
    
    // Rating
    public double AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int TotalOrders { get; set; } = 0;
    
    // Status
    public bool IsActive { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public bool IsOpen { get; set; } = false;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    
    // Commission
    public decimal CommissionRate { get; set; } = 12;

    // Navigation
    public virtual ApplicationUser Owner { get; set; } = null!;
    public virtual ICollection<GroceryStoreOpeningHours> OpeningHours { get; set; } = new List<GroceryStoreOpeningHours>();
    public virtual ICollection<GroceryDepartment> Departments { get; set; } = new List<GroceryDepartment>();
    public virtual ICollection<GroceryProduct> Products { get; set; } = new List<GroceryProduct>();
    public virtual ICollection<GroceryOrder> Orders { get; set; } = new List<GroceryOrder>();
    public virtual ICollection<GroceryPromotion> Promotions { get; set; } = new List<GroceryPromotion>();
}

public class GroceryStoreOpeningHours : BaseEntity
{
    public Guid StoreId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsClosed { get; set; } = false;

    public virtual GroceryStoreEntity Store { get; set; } = null!;
}

public class GroceryDepartment : AuditableEntity
{
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public GroceryCategory Category { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual GroceryStoreEntity Store { get; set; } = null!;
    public virtual ICollection<GroceryProduct> Products { get; set; } = new List<GroceryProduct>();
}

public class GroceryProduct : AuditableEntity
{
    public Guid StoreId { get; set; }
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Barcode { get; set; }
    public string SKU { get; set; } = string.Empty;
    
    // Pricing
    public decimal UnitPrice { get; set; }
    public decimal? PricePerKg { get; set; }
    public decimal? PricePerLiter { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string? Unit { get; set; } // "kg", "L", "piece", "pack"
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
    
    // Images
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> AdditionalImages { get; set; } = new();
    
    // Stock
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; } = 5;
    public bool IsInStock { get; set; } = true;
    
    // Details
    public string? NutritionalInfoJson { get; set; }
    public List<string> Allergens { get; set; } = new();
    public string? Origin { get; set; }
    public int? AverageExpiryDays { get; set; }
    public bool IsBio { get; set; } = false;
    public bool IsHalal { get; set; } = true;
    public bool IsPromotion { get; set; } = false;
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool AllowReplacement { get; set; } = true;
    public int SortOrder { get; set; }
    
    // Stats
    public int TotalSold { get; set; } = 0;
    public double AverageRating { get; set; } = 0;

    // Navigation
    public virtual GroceryStoreEntity Store { get; set; } = null!;
    public virtual GroceryDepartment Department { get; set; } = null!;
}

// ==================== GROCERY ORDERS ====================
public class GroceryOrder : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty; // "GRC-20240101-001"
    public Guid CustomerId { get; set; }
    public Guid StoreId { get; set; }
    public Guid? DelivererId { get; set; }
    public Guid? PreparerId { get; set; }
    
    // Delivery Address
    public string DeliveryStreet { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public string? DeliveryApartment { get; set; }
    public string? DeliveryFloor { get; set; }
    public string? DeliveryInstructions { get; set; }
    public double DeliveryLatitude { get; set; }
    public double DeliveryLongitude { get; set; }
    
    // Status
    public GroceryOrderStatus Status { get; set; } = GroceryOrderStatus.Received;
    
    // Pricing
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal TipAmount { get; set; } = 0;
    public decimal BagsFee { get; set; } = 0;
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
    public DeliveryTimeSlot? PreferredTimeSlot { get; set; }
    
    // Preferences
    public bool LeaveAtDoor { get; set; } = false;
    public bool AllowReplacements { get; set; } = true;
    public BagType BagType { get; set; } = BagType.Plastic;
    public string? FreshnessPreferences { get; set; }
    
    // Times
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? PreparationStartedAt { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    
    // Rating
    public int? StoreRating { get; set; }
    public int? DeliveryRating { get; set; }
    public int? FreshnessRating { get; set; }
    public string? ReviewComment { get; set; }
    
    // Proof
    public string? DeliveryPhotoUrl { get; set; }

    // Navigation
    public virtual ApplicationUser Customer { get; set; } = null!;
    public virtual GroceryStoreEntity Store { get; set; } = null!;
    public virtual ApplicationUser? Deliverer { get; set; }
    public virtual ICollection<GroceryOrderItem> Items { get; set; } = new List<GroceryOrderItem>();
    public virtual ICollection<GroceryOrderStatusHistory> StatusHistory { get; set; } = new List<GroceryOrderStatusHistory>();
}

public class GroceryOrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsPicked { get; set; } = false;
    public bool IsUnavailable { get; set; } = false;
    
    // Replacement
    public Guid? ReplacementProductId { get; set; }
    public string? ReplacementProductName { get; set; }
    public decimal? ReplacementPrice { get; set; }
    public bool? ReplacementAccepted { get; set; }
    public string? Note { get; set; }

    public virtual GroceryOrder Order { get; set; } = null!;
    public virtual GroceryProduct Product { get; set; } = null!;
    public virtual GroceryProduct? ReplacementProduct { get; set; }
}

public class GroceryOrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; set; }
    public GroceryOrderStatus Status { get; set; }
    public string? Note { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public virtual GroceryOrder Order { get; set; } = null!;
}

// ==================== SHOPPING LISTS ====================
public class ShoppingList : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty; // "Courses hebdo", "Fête", "BBQ"
    public bool IsRecurring { get; set; } = false;
    public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
    public string? ShareCode { get; set; }
    public bool IsShared { get; set; } = false;

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
    public virtual ICollection<ShoppingListShare> SharedWith { get; set; } = new List<ShoppingListShare>();
}

public class ShoppingListItem : BaseEntity
{
    public Guid ListId { get; set; }
    public Guid? ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public bool IsChecked { get; set; } = false;
    public string? Note { get; set; }

    public virtual ShoppingList List { get; set; } = null!;
    public virtual GroceryProduct? Product { get; set; }
}

public class ShoppingListShare : BaseEntity
{
    public Guid ListId { get; set; }
    public Guid SharedWithUserId { get; set; }
    public bool CanEdit { get; set; } = true;

    public virtual ShoppingList List { get; set; } = null!;
    public virtual ApplicationUser SharedWithUser { get; set; } = null!;
}

// ==================== GROCERY PROMOTIONS ====================
public class GroceryPromotion : AuditableEntity
{
    public Guid StoreId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public string DiscountType { get; set; } = "Percentage";
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
    
    // Buy X Get Y
    public int? BuyQuantity { get; set; }
    public int? GetQuantity { get; set; }
    public Guid? ApplicableProductId { get; set; }
    
    // Time-based
    public TimeOnly? ActiveFrom { get; set; }
    public TimeOnly? ActiveUntil { get; set; }

    public virtual GroceryStoreEntity Store { get; set; } = null!;
}
