using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Restaurant;

// ==================== LIST & DETAIL ====================
public record RestaurantListDto(Guid Id, string Name, string? LogoUrl, string? CoverImageUrl,
    CuisineType CuisineType, PriceRange PriceRange, double Rating, int ReviewCount,
    decimal MinOrderAmount, decimal DeliveryFee, int EstimatedDeliveryMinutes,
    double DistanceKm, bool IsOpen, bool HasPromotion, double Latitude, double Longitude, string Phone);

public record RestaurantDetailDto(Guid Id, string Name, string? Description, string? LogoUrl,
    string? CoverImageUrl, CuisineType CuisineType, PriceRange PriceRange, double Rating,
    int ReviewCount, decimal MinOrderAmount, decimal DeliveryFee, int EstimatedDeliveryMinutes,
    string Address, double Latitude, double Longitude, string Phone, bool IsOpen,
    bool AcceptsOnlinePayment, bool AcceptsCashPayment, double MaxDeliveryDistanceKm,
    List<MenuCategoryDto> Menu, List<WorkingHoursDto> WorkingHours, List<PromotionDto> ActivePromotions);

/// <summary>Alias used by IApplicationServices</summary>
public record RestaurantDto(Guid Id, string Name, string? Description, string? LogoUrl,
    string? CoverImageUrl, CuisineType CuisineType, PriceRange PriceRange, double Rating,
    int ReviewCount, decimal MinOrderAmount, decimal DeliveryFee, int EstimatedDeliveryMinutes,
    string Address, double Latitude, double Longitude, string Phone, bool IsOpen,
    bool AcceptsOnlinePayment, bool AcceptsCashPayment, double MaxDeliveryDistanceKm,
    List<MenuCategoryDto> Menu, List<WorkingHoursDto> WorkingHours, List<PromotionDto> ActivePromotions);

// ==================== MENU ====================
public record MenuCategoryDto(Guid Id, string Name, string? Description, string? ImageUrl, List<MenuItemDto> Items);

public record MenuItemDto(Guid Id, string Name, string? Description, decimal Price, string? ImageUrl,
    bool IsAvailable, bool IsPopular, int PreparationTimeMinutes, string? Allergens, int CalorieCount,
    List<MenuItemOptionDto> Options);

public record MenuItemOptionDto(Guid Id, string GroupName, string Name, decimal AdditionalPrice,
    bool IsRequired, bool IsMultiSelect, int MaxSelections);

public record WorkingHoursDto(DayOfWeek DayOfWeek, string OpenTime, string CloseTime, bool IsClosed);
public record PromotionDto(Guid Id, string Title, string? Description, string? PromoCode,
    decimal? DiscountPercentage, decimal? DiscountAmount, bool FreeDelivery, DateTime EndDate);

// ==================== SEARCH ====================
/// <summary>Used by IRestaurantService.SearchRestaurantsAsync</summary>
public class RestaurantSearchRequest
{
    public string? Query { get; set; }
    public CuisineType? CuisineType { get; set; }
    public PriceRange? PriceRange { get; set; }
    public double? MinRating { get; set; }
    public bool? IsOpen { get; set; }
    public bool? HasPromotion { get; set; }
    public bool? FreeDelivery { get; set; }
    public double? UserLatitude { get; set; }
    public double? UserLongitude { get; set; }
    public double? MaxDistanceKm { get; set; }
    public string? SortBy { get; set; } = "distance";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ==================== CREATE / UPDATE RESTAURANT ====================
public record CreateRestaurantDto(string Name, string? Description, CuisineType CuisineType,
    PriceRange PriceRange, decimal MinOrderAmount, decimal DeliveryFee, int EstimatedDeliveryMinutes,
    double MaxDeliveryDistanceKm, string Street, string City, string PostalCode,
    double Latitude, double Longitude, string Phone, string CoverImageUrl, string LogoUrl);

/// <summary>Used by IRestaurantManagementService.CreateRestaurantAsync</summary>
public class CreateRestaurantRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public CuisineType CuisineType { get; set; }
    public PriceRange PriceRange { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal FreeDeliveryMinimum { get; set; }
    public int AveragePreparationMinutes { get; set; }
    public double MaxDeliveryDistanceKm { get; set; }
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string LogoUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
}

/// <summary>Used by IRestaurantManagementService.UpdateRestaurantAsync</summary>
public class UpdateRestaurantRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public CuisineType? CuisineType { get; set; }
    public PriceRange? PriceRange { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? DeliveryFee { get; set; }
    public decimal? FreeDeliveryMinimum { get; set; }
    public int? AveragePreparationMinutes { get; set; }
    public double? MaxDeliveryDistanceKm { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string LogoUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

// ==================== MENU MANAGEMENT ====================
public record CreateMenuCategoryDto(string Name, string? Description, int SortOrder);

/// <summary>Used by IRestaurantManagementService.CreateMenuCategoryAsync / UpdateMenuCategoryAsync</summary>
public class CreateMenuCategoryRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
}

public record CreateMenuItemDto(Guid CategoryId, string Name, string? Description, decimal Price,
    int PreparationTimeMinutes, string? Allergens, string? Ingredients, int CalorieCount,
    List<CreateMenuItemOptionDto>? Options);

/// <summary>Used by IRestaurantManagementService.CreateMenuItemAsync / UpdateMenuItemAsync</summary>
public class CreateMenuItemRequest
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int PreparationMinutes { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsPopular { get; set; }
    public List<string>? Allergens { get; set; }
    public List<CreateMenuItemOptionDto>? Options { get; set; }
}

public record CreateMenuItemOptionDto(string GroupName, string Name, decimal AdditionalPrice,
    bool IsRequired, bool IsMultiSelect, int MaxSelections);

// ==================== ORDERS ====================
public record CreateRestaurantOrderDto(Guid RestaurantId, List<OrderItemDto> Items,
    string? SpecialInstructions, bool IncludeCutlery, PaymentMethod PaymentMethod,
    string? PromoCode, decimal Tip, Guid DeliveryAddressId, DateTime? ScheduledDeliveryTime);

/// <summary>Used by IRestaurantOrderService.CreateOrderAsync</summary>
public class CreateRestaurantOrderRequest
{
    public Guid RestaurantId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public string? SpecialInstructions { get; set; }
    public bool RequestCutlery { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? PromoCode { get; set; }
    public decimal TipAmount { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? ScheduledFor { get; set; }
}

public record OrderItemDto(Guid MenuItemId, int Quantity, List<Guid>? SelectedOptionIds, string? SpecialInstructions);

public record RestaurantOrderDto(Guid Id, string OrderNumber, string Status, string RestaurantName,
    decimal SubTotal, decimal DeliveryFee, decimal Discount, decimal Tip, decimal TotalAmount,
    string PaymentMethod, string PaymentStatus, DateTime CreatedAt, DateTime? EstimatedDeliveryTime,
    List<RestaurantOrderItemDto> Items, DeliveryDriverInfoDto? Driver);

public record RestaurantOrderItemDto(string ItemName, int Quantity, decimal UnitPrice, decimal TotalPrice,
    string? SelectedOptions, string? SpecialInstructions);

public record DeliveryDriverInfoDto(string Name, string? PhotoUrl, string Phone,
    double? Latitude, double? Longitude);

// ==================== REVIEWS ====================
public record RestaurantReviewDto(Guid Id, string CustomerName, int FoodRating, int DeliveryRating,
    int OverallRating, string? Comment, string? Reply, DateTime CreatedAt);
public record CreateReviewDto(Guid OrderId, int FoodRating, int DeliveryRating, int OverallRating, string? Comment);
