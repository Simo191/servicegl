namespace MultiServices.Restaurant.Models;

// === Restaurant Profile ===
public class RestaurantProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string CuisineType { get; set; } = string.Empty;
    public string PriceRange { get; set; } = "€€";
    public bool IsOpen { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalOrders { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrder { get; set; }
    public double MaxDeliveryDistanceKm { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    public decimal CommissionRate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public List<OpeningHoursDto> OpeningHours { get; set; } = new();
}

public class OpeningHoursDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public string OpenTime { get; set; } = string.Empty;
    public string CloseTime { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
}

// === Menu (aligned with MenuCategoryDto, MenuItemDto from backend) ===
public class MenuCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<MenuItemDto> Items { get; set; } = new();
}

public class MenuItemDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsPopular { get; set; }
    public List<string> Allergens { get; set; } = new();
    public string? NutritionalInfo { get; set; }
    public int CalorieCount { get; set; }
    public List<MenuItemSizeDto> Sizes { get; set; } = new();
    public List<MenuItemExtraDto> Extras { get; set; } = new();
}

public class MenuItemSizeDto { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public decimal PriceAdjustment { get; set; } }
public class MenuItemExtraDto { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public decimal Price { get; set; } }

// === Orders (aligned with RestaurantOrderDto from backend) ===
public class RestaurantOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Tip { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public bool IncludeCutlery { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public int? EstimatedMinutes { get; set; }
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

public class OrderStatusHistoryDto { public string Status { get; set; } = string.Empty; public DateTime Timestamp { get; set; } public string? Note { get; set; } }

// === Create/Update Requests ===
public class CreateMenuCategoryRequest { public string Name { get; set; } = string.Empty; public string? Description { get; set; } public int SortOrder { get; set; } }
public class CreateMenuItemRequest
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsPopular { get; set; }
    public List<string> Allergens { get; set; } = new();
    public string? NutritionalInfo { get; set; }
    public int CalorieCount { get; set; }
    public List<MenuItemSizeDto> Sizes { get; set; } = new();
    public List<MenuItemExtraDto> Extras { get; set; } = new();
}
public class UpdateMenuItemRequest : CreateMenuItemRequest { public bool IsAvailable { get; set; } = true; }

// === Promotions ===
public class PromotionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty; // Percentage, FixedAmount, FreeDelivery, BuyXGetY, HappyHour
    public decimal DiscountValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public List<Guid>? MenuItemIds { get; set; }
}

// === Stats ===
public class RestaurantStatsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public double AverageRating { get; set; }
    public decimal CommissionsTotal { get; set; }
    public List<TopItemDto> TopSellingItems { get; set; } = new();
    public Dictionary<string, int> OrdersByHour { get; set; } = new();
    public Dictionary<string, decimal> RevenueByDay { get; set; } = new();
}
public class TopItemDto { public string Name { get; set; } = string.Empty; public int Count { get; set; } public decimal Revenue { get; set; } }

// === Reviews ===
public class ReviewDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerImageUrl { get; set; }
    public int FoodRating { get; set; }
    public int DeliveryRating { get; set; }
    public int OverallRating { get; set; }
    public string? Comment { get; set; }
    public string? Reply { get; set; }
    public DateTime CreatedAt { get; set; }
}

// === Staff ===
public class StaffMemberDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Manager, Caissier, Cuisinier
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
}
