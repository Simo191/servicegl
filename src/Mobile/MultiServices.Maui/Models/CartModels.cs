namespace MultiServices.Maui.Models;

// Restaurant Cart
public class RestaurantCartItem
{
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public int Quantity { get; set; }
    public MenuItemSizeDto? SelectedSize { get; set; }
    public List<MenuItemExtraDto> SelectedExtras { get; set; } = new();
    public string? SpecialInstructions { get; set; }
    public decimal UnitPrice => BasePrice + (SelectedSize?.PriceAdjustment ?? 0) + SelectedExtras.Sum(e => e.Price);
    public decimal TotalPrice => UnitPrice * Quantity;
}

public class RestaurantCart
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantLogoUrl { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrder { get; set; }
    public List<RestaurantCartItem> Items { get; set; } = new();
    public string? PromoCode { get; set; }
    public decimal Discount { get; set; }
    public decimal? Tip { get; set; }
    public bool IncludeCutlery { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateTime? ScheduledDelivery { get; set; }
    
    public decimal SubTotal => Items.Sum(i => i.TotalPrice);
    public decimal Total => SubTotal + DeliveryFee - Discount + (Tip ?? 0);
    public int ItemCount => Items.Sum(i => i.Quantity);
    public bool MeetsMinimum => SubTotal >= MinimumOrder;
}

// Grocery Cart
public class GroceryCart
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? StoreLogoUrl { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrder { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public string? PromoCode { get; set; }
    public decimal Discount { get; set; }
    public decimal? Tip { get; set; }
    public string BagType { get; set; } = "plastic";
    public decimal BagsFee { get; set; }
    public bool AllowReplacements { get; set; } = true;
    public bool LeaveAtDoor { get; set; }
    public string? DeliveryInstructions { get; set; }
    public DateTime? ScheduledDelivery { get; set; }
    
    public decimal SubTotal => Items.Sum(i => i.Total);
    public decimal Total => SubTotal + DeliveryFee + BagsFee - Discount + (Tip ?? 0);
    public int ItemCount => Items.Sum(i => i.Quantity);
}
