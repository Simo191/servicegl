using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Grocery;

public record StoreListDto(Guid Id, string Name, string Brand, string? LogoUrl,
    double Rating, int ReviewCount, decimal MinOrderAmount, decimal DeliveryFee,
    decimal FreeDeliveryThreshold, double DistanceKm, bool IsOpen, bool HasPromotion);

public record StoreDetailDto(Guid Id, string Name, string Brand, string? Description,
    string? LogoUrl, string? CoverImageUrl, double Rating, int ReviewCount,
    decimal MinOrderAmount, decimal DeliveryFee, decimal FreeDeliveryThreshold,
    string Address, double Latitude, double Longitude, bool IsOpen,
    List<StoreCategoryDto> Categories, List<WorkingHoursDto> WorkingHours,
    List<StorePromotionDto> ActivePromotions);

public record StoreCategoryDto(Guid Id, GroceryCategory Category, string Name,
    string? ImageUrl, int ProductCount);

public record ProductListDto(Guid Id, string Name, string? Brand, decimal Price,
    decimal? PricePerUnit, string? UnitMeasure, string? ImageUrl, bool IsAvailable,
    bool IsBio, bool IsHalal, bool IsOnPromotion, decimal? PromotionPrice, bool IsPopular);

public record ProductDetailDto(Guid Id, string Name, string? Description, string? Brand,
    string? Barcode, decimal Price, decimal? PricePerUnit, string? UnitMeasure,
    string? ImageUrl, string? NutritionalInfo, string? Allergens, string? Origin,
    bool IsBio, bool IsHalal, int StockQuantity, bool IsAvailable,
    bool IsOnPromotion, decimal? PromotionPrice, List<ProductListDto>? SimilarProducts);

public record WorkingHoursDto(DayOfWeek DayOfWeek, string OpenTime, string CloseTime, bool IsClosed);

public record StorePromotionDto(Guid Id, string Title, string? Description,
    string? PromoCode, decimal? DiscountPercentage, decimal? DiscountAmount,
    bool FreeDelivery, DateTime EndDate);

public record CreateGroceryOrderDto(Guid StoreId, List<GroceryCartItemDto> Items,
    Guid DeliveryAddressId, PaymentMethod PaymentMethod, string? PromoCode,
    decimal Tip, string? DeliveryInstructions, bool LeaveAtDoor,
    bool AllowSubstitutions, string? BagPreference, string? FreshnessPreferences,
    DateTime? ScheduledDeliveryStart, DateTime? ScheduledDeliveryEnd);

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

public record ShoppingListDto(Guid Id, string Name, bool IsRecurring,
    string? RecurrencePattern, List<ShoppingListItemDto> Items);

public record ShoppingListItemDto(Guid Id, string Name, int Quantity,
    bool IsChecked, Guid? ProductId);

public record CreateShoppingListDto(string Name, bool IsRecurring,
    string? RecurrencePattern, List<CreateShoppingListItemDto> Items);
public record CreateShoppingListItemDto(string Name, int Quantity, Guid? ProductId);

// Store management DTOs
public record CreateProductDto(Guid CategoryId, string Name, string? Description,
    string? Brand, string? Barcode, decimal Price, decimal? PricePerUnit,
    string? UnitMeasure, string? NutritionalInfo, string? Allergens,
    string? Origin, bool IsBio, bool IsHalal, int StockQuantity);

public record UpdateProductDto(string? Name, string? Description, decimal? Price,
    int? StockQuantity, bool? IsAvailable, bool? IsOnPromotion, decimal? PromotionPrice);

public record BulkImportProductDto(string CsvBase64);
