namespace MultiServices.Maui.Models;

/// <summary>
/// Aligned with backend StoreDetailDto / GroceryStoreDto
/// Key changes: MinOrderAmount, ReviewCount, Categories (not Departments), FreeDeliveryThreshold
/// </summary>
public class GroceryStoreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }  // Was: TotalReviews
    public decimal MinOrderAmount { get; set; }  // Was: MinimumOrder
    public decimal DeliveryFee { get; set; }
    public decimal FreeDeliveryThreshold { get; set; }  // Was: HasFreeDelivery (bool)
    public bool IsOpen { get; set; }

    // Backend uses Categories (StoreCategoryDto), not Departments
    public List<StoreCategoryDto> Categories { get; set; } = new();
    public List<GroceryWorkingHoursDto> WorkingHours { get; set; } = new();
    public List<StorePromotionDto> ActivePromotions { get; set; } = new();

    // Computed backward compat
    public bool HasFreeDelivery => FreeDeliveryThreshold > 0;
    public bool HasActivePromotions => ActivePromotions?.Any() == true;
    public decimal MinimumOrder => MinOrderAmount;
    public int TotalReviews => ReviewCount;
    public double? Distance { get; set; }
}

/// <summary>
/// Aligned with backend StoreListDto / GroceryStoreListDto
/// </summary>
public class GroceryStoreListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal FreeDeliveryThreshold { get; set; }
    public double DistanceKm { get; set; }
    public bool IsOpen { get; set; }
    public bool HasPromotion { get; set; }

    // Computed backward compat
    public bool HasFreeDelivery => FreeDeliveryThreshold > 0;
    public bool HasActivePromotions => HasPromotion;
    public double? Distance => DistanceKm;
}

/// <summary>
/// Aligned with backend StoreCategoryDto
/// Replaces old GroceryDepartmentDto
/// </summary>
public class StoreCategoryDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int ProductCount { get; set; }
}

/// <summary>
/// Aligned with backend ProductDetailDto / GroceryProductDto
/// Key changes: IsAvailable (not IsInStock), Allergens (string, not List)
/// </summary>
public class GroceryProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Barcode { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public decimal? PricePerUnit { get; set; }
    public string? UnitMeasure { get; set; }
    public string? NutritionalInfo { get; set; }
    public string? Origin { get; set; }

    // Backend uses single string, not List
    public string? Allergens { get; set; }

    // Backend field name
    public bool IsAvailable { get; set; }  // Was: IsInStock
    public int StockQuantity { get; set; }
    public bool IsBio { get; set; }
    public bool IsHalal { get; set; }
    public bool IsOnPromotion { get; set; }
    public decimal? PromotionPrice { get; set; }

    // Backend doesn't have these, client-side:
    public string? PromotionLabel { get; set; }
    public List<GroceryProductDto>? SimilarProducts { get; set; }

    // Backward compat
    public bool IsInStock => IsAvailable;
}

/// <summary>
/// Aligned with backend GroceryOrderDto
/// </summary>
public class GroceryOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string? StoreLogoUrl { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Tip { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public List<GroceryOrderItemDto> Items { get; set; } = new();
    public DeliveryDriverInfoDto? Driver { get; set; }

    // Client-side extras
    public decimal BagsFee { get; set; }
}

/// <summary>
/// Aligned with backend GroceryOrderItemDto
/// </summary>
public class GroceryOrderItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsAvailable { get; set; }
    public bool WasSubstituted { get; set; }
    public string? SubstitutedProductName { get; set; }
    public bool SubstitutionAccepted { get; set; }
}

/// <summary>
/// Aligned with backend DeliveryDriverInfoDto
/// Shared between Restaurant and Grocery modules
/// </summary>
//public class DeliveryDriverInfoDto
//{
//    public string Name { get; set; } = string.Empty;
//    public string? PhotoUrl { get; set; }
//    public string Phone { get; set; } = string.Empty;
//    public double? Latitude { get; set; }
//    public double? Longitude { get; set; }
//}

public class GroceryWorkingHoursDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public string OpenTime { get; set; } = string.Empty;
    public string CloseTime { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
}

public class StorePromotionDto
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

/// <summary>Cart models (client-side only)</summary>
public class GroceryCart
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? StoreLogoUrl { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrder { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public decimal SubTotal => Items.Sum(i => i.TotalPrice);
    public decimal Total => SubTotal + DeliveryFee;
    public bool MeetsMinimum => SubTotal >= MinimumOrder;
    public int ItemCount => Items.Sum(i => i.Quantity);
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity;
}

/// <summary>
/// Aligned with backend ShoppingListDto
/// Backend record: (Guid Id, string Name, bool IsRecurring, string? RecurrencePattern, List Items)
/// </summary>
public class ShoppingListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int CheckedCount { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsed { get; set; }
    public List<ShoppingListItemDto> Items { get; set; } = new();
    public List<ShoppingListShareDto> SharedWith { get; set; } = new();
}

/// <summary>
/// Aligned with backend ShoppingListItemDto
/// Backend record: (Guid Id, string Name, int Quantity, bool IsChecked, Guid? ProductId)
/// </summary>
public class ShoppingListItemDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public bool IsChecked { get; set; }
    public Guid? ProductId { get; set; }
}

public class ShoppingListShareDto
{
    public string UserName { get; set; } = string.Empty;
    public string Permission { get; set; } = "view";
}

public class GroceryReplacementDto
{
    public Guid OriginalProductId { get; set; }
    public string OriginalProductName { get; set; } = string.Empty;
    public Guid ReplacementProductId { get; set; }
    public string ReplacementProductName { get; set; } = string.Empty;
    public decimal PriceDifference { get; set; }
    public string Status { get; set; } = "Pending";
}