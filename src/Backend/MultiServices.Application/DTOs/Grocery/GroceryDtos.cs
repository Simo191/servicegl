using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Grocery;

// ==================== STORE ====================
public record StoreListDto(Guid Id, string Name, string Brand, string? LogoUrl,
    double Rating, int ReviewCount, decimal MinOrderAmount, decimal DeliveryFee,
    decimal FreeDeliveryThreshold, double DistanceKm, bool IsOpen, bool HasPromotion);

public record StoreDetailDto(Guid Id, string Name, string Brand, string? Description,
    string? LogoUrl, string? CoverImageUrl, double Rating, int ReviewCount,
    decimal MinOrderAmount, decimal DeliveryFee, decimal FreeDeliveryThreshold,
    string Address, double Latitude, double Longitude, bool IsOpen,
    List<StoreCategoryDto> Categories, List<WorkingHoursDto> WorkingHours,
    List<StorePromotionDto> ActivePromotions);

/// <summary>Alias used by IApplicationServices (IGroceryService)</summary>
public record GroceryStoreListDto(Guid Id, string Name, string Brand, string? LogoUrl,
    double Rating, int ReviewCount, decimal MinOrderAmount, decimal DeliveryFee,
    decimal FreeDeliveryThreshold, double DistanceKm, bool IsOpen, bool HasPromotion);

/// <summary>Alias used by IApplicationServices (IGroceryService)</summary>
public record GroceryStoreDto(Guid Id, string Name, string Brand, string? Description,
    string? LogoUrl, string? CoverImageUrl, double Rating, int ReviewCount,
    decimal MinOrderAmount, decimal DeliveryFee, decimal FreeDeliveryThreshold,
    string Address, double Latitude, double Longitude, bool IsOpen,
    List<StoreCategoryDto> Categories, List<WorkingHoursDto> WorkingHours,
    List<StorePromotionDto> ActivePromotions);

public record StoreCategoryDto(Guid Id, GroceryCategory Category, string Name,
    string? ImageUrl, int ProductCount);

// ==================== PRODUCT ====================
public record ProductListDto(Guid Id, string Name, string? Brand, decimal Price,
    decimal? PricePerUnit, string? UnitMeasure, string? ImageUrl, bool IsAvailable,
    bool IsBio, bool IsHalal, bool IsOnPromotion, decimal? PromotionPrice, bool IsPopular);

public record ProductDetailDto(Guid Id, string Name, string? Description, string? Brand,
    string? Barcode, decimal Price, decimal? PricePerUnit, string? UnitMeasure,
    string? ImageUrl, string? NutritionalInfo, string? Allergens, string? Origin,
    bool IsBio, bool IsHalal, int StockQuantity, bool IsAvailable,
    bool IsOnPromotion, decimal? PromotionPrice, List<ProductListDto>? SimilarProducts);

/// <summary>Alias used by IApplicationServices (IGroceryService)</summary>
public record GroceryProductListDto(Guid Id, string Name, string? Brand, decimal Price,
    decimal? PricePerUnit, string? UnitMeasure, string? ImageUrl, bool IsAvailable,
    bool IsBio, bool IsHalal, bool IsOnPromotion, decimal? PromotionPrice, bool IsPopular);

/// <summary>Alias used by IApplicationServices (IGroceryService)</summary>
public record GroceryProductDto(Guid Id, string Name, string? Description, string? Brand,
    string? Barcode, decimal Price, decimal? PricePerUnit, string? UnitMeasure,
    string? ImageUrl, string? NutritionalInfo, string? Allergens, string? Origin,
    bool IsBio, bool IsHalal, int StockQuantity, bool IsAvailable,
    bool IsOnPromotion, decimal? PromotionPrice, List<ProductListDto>? SimilarProducts);

// ==================== SEARCH ====================
/// <summary>Used by IGroceryService.SearchProductsAsync</summary>
public class GrocerySearchRequest
{
    public string? Query { get; set; }
    public GroceryCategory? Category { get; set; }
    public bool? IsBio { get; set; }
    public bool? IsHalal { get; set; }
    public bool? IsOnPromotion { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } = "name";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ==================== WORKING HOURS / PROMOTIONS ====================
public record WorkingHoursDto(DayOfWeek DayOfWeek, string OpenTime, string CloseTime, bool IsClosed);

public record StorePromotionDto(Guid Id, string Title, string? Description,
    string? PromoCode, decimal? DiscountPercentage, decimal? DiscountAmount,
    bool FreeDelivery, DateTime EndDate);

// ==================== ORDERS ====================
public record CreateGroceryOrderDto(Guid StoreId, List<GroceryCartItemDto> Items,
    Guid DeliveryAddressId, PaymentMethod PaymentMethod, string? PromoCode,
    decimal Tip, string? DeliveryInstructions, bool LeaveAtDoor,
    bool AllowSubstitutions, string? BagPreference, string? FreshnessPreferences,
    DateTime? ScheduledDeliveryStart, DateTime? ScheduledDeliveryEnd);

/// <summary>Used by IGroceryOrderService.CreateOrderAsync</summary>
public class CreateGroceryOrderRequest
{
    public Guid StoreId { get; set; }
    public List<GroceryCartItemDto> Items { get; set; } = new();
    public Guid DeliveryAddressId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? PromoCode { get; set; }
    public decimal TipAmount { get; set; }
    public string? DeliveryInstructions { get; set; }
    public bool LeaveAtDoor { get; set; }
    public bool AllowReplacements { get; set; } = true;
    public BagType BagType { get; set; } = BagType.Plastic;
    public string? FreshnessPreferences { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? ScheduledFor { get; set; }
}

public record GroceryCartItemDto(Guid ProductId, int Quantity);

public record GroceryOrderDto(Guid Id, string OrderNumber, string Status,
    string StoreName, decimal SubTotal, decimal DeliveryFee, decimal Discount,
    decimal Tip, decimal TotalAmount, string PaymentStatus, DateTime CreatedAt,
    DateTime? EstimatedDeliveryTime, List<GroceryOrderItemDto> Items,
    DeliveryDriverInfoDto? Driver);

public record GroceryOrderItemDto(string ProductName, int Quantity, decimal UnitPrice,
    decimal TotalPrice, bool IsAvailable, bool WasSubstituted,
    string? SubstitutedProductName, bool SubstitutionAccepted);

public record DeliveryDriverInfoDto(string Name, string? PhotoUrl, string Phone,
    double? Latitude, double? Longitude);

// ==================== SHOPPING LISTS ====================
public record ShoppingListDto(Guid Id, string Name, bool IsRecurring,
    string? RecurrencePattern, List<ShoppingListItemDto> Items);

public record ShoppingListItemDto(Guid Id, string Name, int Quantity,
    bool IsChecked, Guid? ProductId);

public record CreateShoppingListDto(string Name, bool IsRecurring,
    string? RecurrencePattern, List<CreateShoppingListItemDto> Items);

/// <summary>Used by IShoppingListService.CreateListAsync</summary>
public class CreateShoppingListRequest
{
    public string Name { get; set; } = "";
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public List<CreateShoppingListItemDto> Items { get; set; } = new();
}

public record CreateShoppingListItemDto(string Name, int Quantity, Guid? ProductId);

// ==================== STORE MANAGEMENT ====================
public record CreateProductDto(Guid CategoryId, string Name, string? Description,
    string? Brand, string? Barcode, decimal Price, decimal? PricePerUnit,
    string? UnitMeasure, string? NutritionalInfo, string? Allergens,
    string? Origin, bool IsBio, bool IsHalal, int StockQuantity);

public record UpdateProductDto(string? Name, string? Description, decimal? Price,
    int? StockQuantity, bool? IsAvailable, bool? IsOnPromotion, decimal? PromotionPrice);

public record BulkImportProductDto(string CsvBase64);

/// <summary>Used by IGroceryStoreManagementService.UpdateStockAsync</summary>
public class StockUpdateRequest
{
    public Guid ProductId { get; set; }
    public int NewQuantity { get; set; }
}

/// <summary>Used by IGroceryStoreManagementService.SuggestReplacementAsync</summary>
public class ReplacementSuggestionRequest
{
    public Guid OrderId { get; set; }
    public Guid OriginalItemId { get; set; }
    public Guid ReplacementProductId { get; set; }
    public decimal? ReplacementPrice { get; set; }
}
