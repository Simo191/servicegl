using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Restaurant;

public record RestaurantListDto(Guid Id, string Name, string? LogoUrl, string? CoverImageUrl,
    CuisineType CuisineType, PriceRange PriceRange, double Rating, int ReviewCount,
    decimal MinOrderAmount, decimal DeliveryFee, int EstimatedDeliveryMinutes,
    double DistanceKm, bool IsOpen, bool HasPromotion);

public record RestaurantDetailDto(Guid Id, string Name, string? Description, string? LogoUrl,
    string? CoverImageUrl, CuisineType CuisineType, PriceRange PriceRange, double Rating,
    int ReviewCount, decimal MinOrderAmount, decimal DeliveryFee, int EstimatedDeliveryMinutes,
    string Address, double Latitude, double Longitude, string Phone, bool IsOpen,
    bool AcceptsOnlinePayment, bool AcceptsCashPayment, double MaxDeliveryDistanceKm,
    List<MenuCategoryDto> Menu, List<WorkingHoursDto> WorkingHours, List<PromotionDto> ActivePromotions);

public record MenuCategoryDto(Guid Id, string Name, string? Description, string? ImageUrl, List<MenuItemDto> Items);

public record MenuItemDto(Guid Id, string Name, string? Description, decimal Price, string? ImageUrl,
    bool IsAvailable, bool IsPopular, int PreparationTimeMinutes, string? Allergens, int CalorieCount,
    List<MenuItemOptionDto> Options);

public record MenuItemOptionDto(Guid Id, string GroupName, string Name, decimal AdditionalPrice,
    bool IsRequired, bool IsMultiSelect, int MaxSelections);

public record WorkingHoursDto(DayOfWeek DayOfWeek, string OpenTime, string CloseTime, bool IsClosed);
public record PromotionDto(Guid Id, string Title, string? Description, string? PromoCode,
    decimal? DiscountPercentage, decimal? DiscountAmount, bool FreeDelivery, DateTime EndDate);

public record CreateRestaurantDto(string Name, string? Description, CuisineType CuisineType,
    PriceRange PriceRange, decimal MinOrderAmount, decimal DeliveryFee, int EstimatedDeliveryMinutes,
    double MaxDeliveryDistanceKm, string Street, string City, string PostalCode,
    double Latitude, double Longitude, string Phone);

public record CreateMenuCategoryDto(string Name, string? Description, int SortOrder);
public record CreateMenuItemDto(Guid CategoryId, string Name, string? Description, decimal Price,
    int PreparationTimeMinutes, string? Allergens, string? Ingredients, int CalorieCount,
    List<CreateMenuItemOptionDto>? Options);
public record CreateMenuItemOptionDto(string GroupName, string Name, decimal AdditionalPrice,
    bool IsRequired, bool IsMultiSelect, int MaxSelections);

public record CreateRestaurantOrderDto(Guid RestaurantId, List<OrderItemDto> Items,
    string? SpecialInstructions, bool IncludeCutlery, PaymentMethod PaymentMethod,
    string? PromoCode, decimal Tip, Guid DeliveryAddressId, DateTime? ScheduledDeliveryTime);

public record OrderItemDto(Guid MenuItemId, int Quantity, List<Guid>? SelectedOptionIds, string? SpecialInstructions);

public record RestaurantOrderDto(Guid Id, string OrderNumber, string Status, string RestaurantName,
    decimal SubTotal, decimal DeliveryFee, decimal Discount, decimal Tip, decimal TotalAmount,
    string PaymentMethod, string PaymentStatus, DateTime CreatedAt, DateTime? EstimatedDeliveryTime,
    List<RestaurantOrderItemDto> Items, DeliveryDriverInfoDto? Driver);

public record RestaurantOrderItemDto(string ItemName, int Quantity, decimal UnitPrice, decimal TotalPrice,
    string? SelectedOptions, string? SpecialInstructions);

public record DeliveryDriverInfoDto(string Name, string? PhotoUrl, string Phone,
    double? Latitude, double? Longitude);

public record RestaurantReviewDto(Guid Id, string CustomerName, int FoodRating, int DeliveryRating,
    int OverallRating, string? Comment, string? Reply, DateTime CreatedAt);
public record CreateReviewDto(Guid OrderId, int FoodRating, int DeliveryRating, int OverallRating, string? Comment);
