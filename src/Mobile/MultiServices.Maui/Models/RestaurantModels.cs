namespace MultiServices.Maui.Models;

/// <summary>
/// Aligned with backend RestaurantDetailDto / RestaurantDto
/// Key changes: Menu (not MenuCategories), MinOrderAmount, ReviewCount, enums as string
/// </summary>
public class RestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }

    // Backend uses enums, JSON serializes as string
    public string CuisineType { get; set; } = string.Empty;
    public string PriceRange { get; set; } = string.Empty;

    public double Rating { get; set; }
    public int ReviewCount { get; set; }  // Was: TotalReviews

    // Backend field names
    public decimal MinOrderAmount { get; set; }  // Was: MinimumOrder
    public decimal DeliveryFee { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }

    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public bool AcceptsOnlinePayment { get; set; }
    public bool AcceptsCashPayment { get; set; }
    public double MaxDeliveryDistanceKm { get; set; }
    public double? Distance { get; set; }

    // Backend uses "Menu" not "MenuCategories"
    public List<MenuCategoryDto> Menu { get; set; } = new();
    public List<WorkingHoursDto> WorkingHours { get; set; } = new();
    public List<PromotionDto> ActivePromotions { get; set; } = new();

    // Computed
    public bool HasActivePromotions => ActivePromotions?.Any() == true;
    public decimal MinimumOrder => MinOrderAmount; // backward compat
}

/// <summary>
/// Aligned with backend RestaurantListDto
/// </summary>
public class RestaurantListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string CuisineType { get; set; } = string.Empty;
    public string PriceRange { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewCount { get; set; }  // Was: TotalReviews
    public decimal MinOrderAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    public double DistanceKm { get; set; }
    public bool IsOpen { get; set; }
    public bool HasPromotion { get; set; }

    // Computed backward compat
    public bool HasActivePromotions => HasPromotion;
    public double? Distance => DistanceKm;
}

public class MenuCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
}

/// <summary>
/// Aligned with backend MenuItemDto
/// Key change: Price (not BasePrice), Options instead of Sizes/Extras
/// </summary>
public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // Backend uses "Price"
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsPopular { get; set; }
    public int PreparationTimeMinutes { get; set; }
    public string? Allergens { get; set; }
    public int CalorieCount { get; set; }

    // Backend uses Options
    public List<MenuItemOptionDto> Options { get; set; } = new();

    // Backward compat
    public decimal BasePrice => Price;
}

/// <summary>
/// Aligned with backend MenuItemOptionDto
/// </summary>
public class MenuItemOptionDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }
    public bool IsRequired { get; set; }
    public bool IsMultiSelect { get; set; }
    public int MaxSelections { get; set; }
}

public class WorkingHoursDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public string OpenTime { get; set; } = string.Empty;
    public string CloseTime { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
}

public class PromotionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PromoCode { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public bool FreeDelivery { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// Aligned with backend RestaurantOrderDto
/// </summary>
public class RestaurantOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantLogoUrl { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Tip { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? SpecialInstructions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public List<RestaurantOrderItemDto> Items { get; set; } = new();
    public DeliveryDriverInfoDto? Driver { get; set; }
    public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();

    // Backward compat
    public DateTime? EstimatedDelivery => EstimatedDeliveryTime;
    public string? DelivererName => Driver?.Name;
    public string? DelivererPhone => Driver?.Phone;
    public double? DelivererLatitude => Driver?.Latitude;
    public double? DelivererLongitude => Driver?.Longitude;
}

public class RestaurantOrderItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SelectedOptions { get; set; }
    public string? SpecialInstructions { get; set; }

    // Backward compat
    public string Name => ItemName;
}

public class DeliveryDriverInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string Phone { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class OrderStatusHistoryDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Note { get; set; }
}

public class RestaurantSearchRequest
{
    public string? Query { get; set; }
    public string? CuisineType { get; set; }
    public string? PriceRange { get; set; }
    public double? MinRating { get; set; }
    public double? MaxDistance { get; set; }
    public int? MaxDeliveryTime { get; set; }
    public bool? HasPromotions { get; set; }
    public string? SortBy { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>Cart models (client-side only, not from backend)</summary>
public class RestaurantCart
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantLogoUrl { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrder { get; set; }
    public List<RestaurantCartItem> Items { get; set; } = new();
    public decimal SubTotal => Items.Sum(i => i.TotalPrice);
    public decimal Total => SubTotal + DeliveryFee;
    public bool MeetsMinimum => SubTotal >= MinimumOrder;
    public int ItemCount => Items.Sum(i => i.Quantity);
}

public class RestaurantCartItem
{
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => BasePrice * Quantity;
    public List<Guid> SelectedOptionIds { get; set; } = new();
    public string? SpecialInstructions { get; set; }
}
