namespace MultiServices.Store.Models;

// Store profile (from GroceryStoreEntity)
public class GroceryStoreDto
{
    public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; public string StoreType { get; set; } = string.Empty; // Marjane, Carrefour, AswakAssalam, Acima, LabelVie
    public string? LogoUrl { get; set; } public string? CoverImageUrl { get; set; }
    public string Address { get; set; } = string.Empty; public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty; public string? Email { get; set; }
    public bool IsOpen { get; set; } public double AverageRating { get; set; } public int TotalReviews { get; set; } public int TotalOrders { get; set; }
    public decimal DeliveryFee { get; set; } public decimal FreeDeliveryMinimum { get; set; } public decimal MinOrderAmount { get; set; }
    public int AveragePreparationMinutes { get; set; } public double MaxDeliveryDistanceKm { get; set; } public decimal CommissionRate { get; set; }
    public List<WorkingHoursDto> OpeningHours { get; set; } = new();
}
public class WorkingHoursDto { public DayOfWeek DayOfWeek { get; set; } public string OpenTime { get; set; } = string.Empty; public string CloseTime { get; set; } = string.Empty; public bool IsClosed { get; set; } }

// Departments (from GroceryDepartment — Fruits&Légumes, Viandes, Épicerie, Boissons, Produits laitiers, Surgelés, Hygiène, Entretien, Bébé)
public class DepartmentDto
{
    public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public string? Description { get; set; }
    public string? ImageUrl { get; set; } public string Category { get; set; } = string.Empty; public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true; public int ProductCount { get; set; }
}

// Products (from GroceryProduct)
public class ProductDto
{
    public Guid Id { get; set; } public Guid DepartmentId { get; set; } public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } public string? Brand { get; set; } public string? Barcode { get; set; } public string SKU { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } public List<string> AdditionalImages { get; set; } = new();
    public decimal Price { get; set; } public decimal? PricePerKg { get; set; } public decimal? PricePerLiter { get; set; }
    public decimal? DiscountedPrice { get; set; } public string? Unit { get; set; } public decimal? Weight { get; set; }
    public string? NutritionalInfo { get; set; } public List<string> Allergens { get; set; } = new(); public string? Origin { get; set; }
    public bool IsBio { get; set; } public bool IsHalal { get; set; } public bool IsAvailable { get; set; } = true;
    public int StockQuantity { get; set; } public int LowStockThreshold { get; set; } = 5;
    public bool IsPromotion { get; set; } public bool AllowReplacement { get; set; } = true;
    public int TotalSold { get; set; } public double AverageRating { get; set; }
}

// Orders (from GroceryOrder, GroceryOrderItem)
public class GroceryOrderDto
{
    public Guid Id { get; set; } public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty; public string? CustomerPhone { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty; public string? DeliveryInstructions { get; set; }
    public bool LeaveAtDoor { get; set; } public bool AllowSubstitutions { get; set; }
    public string? BagPreference { get; set; } public string? FreshnessPreferences { get; set; }
    public string Status { get; set; } = string.Empty; // Received, Preparing, ProductUnavailable, Ready, InTransit, Delivered, Cancelled
    public decimal SubTotal { get; set; } public decimal DeliveryFee { get; set; } public decimal Discount { get; set; }
    public decimal Tip { get; set; } public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } public DateTime? EstimatedDelivery { get; set; }
    public string? PreparerName { get; set; } public string? DelivererName { get; set; } public string? DelivererPhone { get; set; }
    public List<GroceryOrderItemDto> Items { get; set; } = new();
}
public class GroceryOrderItemDto
{
    public Guid Id { get; set; } public Guid ProductId { get; set; } public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; } public int Quantity { get; set; } public decimal UnitPrice { get; set; } public decimal TotalPrice { get; set; }
    public bool IsPicked { get; set; } public bool IsUnavailable { get; set; }
    public Guid? ReplacementProductId { get; set; } public string? ReplacementProductName { get; set; }
    public decimal? ReplacementPrice { get; set; } public bool? ReplacementAccepted { get; set; } public string? Note { get; set; }
}

// CRUD Requests (from CreateProductDto, UpdateProductDto, BulkImportProductDto)
public class CreateProductRequest
{
    public Guid DepartmentId { get; set; } public string Name { get; set; } = string.Empty; public string? Description { get; set; }
    public string? Brand { get; set; } public string? Barcode { get; set; } public decimal Price { get; set; }
    public decimal? PricePerUnit { get; set; } public string? UnitMeasure { get; set; } public string? NutritionalInfo { get; set; }
    public string? Allergens { get; set; } public string? Origin { get; set; } public bool IsBio { get; set; } public bool IsHalal { get; set; } = true;
    public int StockQuantity { get; set; }
}
public class UpdateProductRequest { public string? Name { get; set; } public string? Description { get; set; } public decimal? Price { get; set; } public int? StockQuantity { get; set; } public bool? IsAvailable { get; set; } public bool? IsOnPromotion { get; set; } public decimal? PromotionPrice { get; set; } }

// Promotions (from StorePromotionDto)
public class StorePromotionDto
{
    public Guid Id { get; set; } public string Title { get; set; } = string.Empty; public string? Description { get; set; }
    public string? PromoCode { get; set; } public decimal? DiscountPercentage { get; set; } public decimal? DiscountAmount { get; set; }
    public bool FreeDelivery { get; set; } public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public bool IsActive { get; set; }
    public List<Guid>? ProductIds { get; set; }
}

// Stats
public class StoreStatsDto
{
    public decimal TotalRevenue { get; set; } public int TotalOrders { get; set; } public decimal AverageBasket { get; set; }
    public int OutOfStockCount { get; set; } public int LowStockCount { get; set; } public decimal CommissionsTotal { get; set; }
    public List<TopProductDto> TopProducts { get; set; } = new();
    public Dictionary<string, int> OrdersByHour { get; set; } = new();
}
public class TopProductDto { public string Name { get; set; } = string.Empty; public int SoldCount { get; set; } public decimal Revenue { get; set; } }
