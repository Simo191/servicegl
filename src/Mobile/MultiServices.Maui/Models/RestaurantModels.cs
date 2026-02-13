namespace MultiServices.Maui.Models;

public class RestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public List<string> GalleryUrls { get; set; } = new();
    public string CuisineType { get; set; } = string.Empty;
    public string PriceRange { get; set; } = "€€";
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal MinimumOrder { get; set; }
    public decimal DeliveryFee { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    public bool IsOpen { get; set; }
    public bool HasActivePromotions { get; set; }
    public double? Distance { get; set; }
    public List<MenuCategoryDto> MenuCategories { get; set; } = new();
}

public class RestaurantListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string CuisineType { get; set; } = string.Empty;
    public string PriceRange { get; set; } = "€€";
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public decimal DeliveryFee { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    public bool IsOpen { get; set; }
    public bool HasActivePromotions { get; set; }
    public double? Distance { get; set; }
}

public class MenuCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
}

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsPopular { get; set; }
    public List<string> Allergens { get; set; } = new();
    public string? NutritionalInfo { get; set; }
    public List<MenuItemSizeDto> Sizes { get; set; } = new();
    public List<MenuItemExtraDto> Extras { get; set; } = new();
}

public class MenuItemSizeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}

public class MenuItemExtraDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class RestaurantOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantLogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Tip { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? SpecialInstructions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public string? DelivererName { get; set; }
    public string? DelivererPhone { get; set; }
    public double? DelivererLatitude { get; set; }
    public double? DelivererLongitude { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
}

public class OrderItemDto
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SizeName { get; set; }
    public List<string> Extras { get; set; } = new();
    public string? SpecialInstructions { get; set; }
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
