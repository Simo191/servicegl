namespace MultiServices.Maui.Models;

public class GroceryStoreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public decimal MinimumOrder { get; set; }
    public decimal DeliveryFee { get; set; }
    public bool IsOpen { get; set; }
    public bool HasFreeDelivery { get; set; }
    public bool HasActivePromotions { get; set; }
    public double? Distance { get; set; }
    public List<GroceryDepartmentDto> Departments { get; set; } = new();
}

public class GroceryStoreListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public double Rating { get; set; }
    public decimal DeliveryFee { get; set; }
    public bool HasFreeDelivery { get; set; }
    public bool HasActivePromotions { get; set; }
    public double? Distance { get; set; }
}

public class GroceryDepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int ProductCount { get; set; }
}

public class GroceryProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Barcode { get; set; }
    public string Department { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public decimal? PricePerUnit { get; set; }
    public string? UnitMeasure { get; set; }
    public string? NutritionalInfo { get; set; }
    public List<string> Allergens { get; set; } = new();
    public string? Origin { get; set; }
    public bool IsInStock { get; set; }
    public bool IsBio { get; set; }
    public bool IsHalal { get; set; }
    public bool IsOnPromotion { get; set; }
    public decimal? PromotionPrice { get; set; }
    public string? PromotionLabel { get; set; }
}

public class GroceryOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string? StoreLogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal BagsFee { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Tip { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? DeliveryInstructions { get; set; }
    public bool LeaveAtDoor { get; set; }
    public bool AllowReplacements { get; set; }
    public string BagType { get; set; } = "plastic";
    public DateTime CreatedAt { get; set; }
    public DateTime? ScheduledDelivery { get; set; }
    public string? DelivererName { get; set; }
    public double? DelivererLatitude { get; set; }
    public double? DelivererLongitude { get; set; }
    public List<GroceryOrderItemDto> Items { get; set; } = new();
    public List<GroceryReplacementDto> Replacements { get; set; } = new();
}

public class GroceryOrderItemDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsAvailable { get; set; } = true;
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

public class CartItem
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}
