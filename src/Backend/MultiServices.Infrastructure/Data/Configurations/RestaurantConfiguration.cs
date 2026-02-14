using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Restaurant;

namespace MultiServices.Infrastructure.Data.Configurations;

// ==================== RESTAURANT ====================
public class RestaurantConfiguration : IEntityTypeConfiguration<RestaurantEntity>
{
    public void Configure(EntityTypeBuilder<RestaurantEntity> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.Slug).IsUnique();
        builder.HasIndex(r => r.OwnerId);
        builder.HasIndex(r => r.CuisineType);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => new { r.Latitude, r.Longitude });

        builder.Property(r => r.Name).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Slug).HasMaxLength(250).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(2000);
        builder.Property(r => r.LogoUrl).HasMaxLength(500);
        builder.Property(r => r.CoverImageUrl).HasMaxLength(500);
        builder.Property(r => r.GalleryUrls).HasColumnType("jsonb");
        builder.Property(r => r.AdditionalCuisines).HasColumnType("jsonb");

        // Location
        builder.Property(r => r.Street).HasMaxLength(300);
        builder.Property(r => r.City).HasMaxLength(100);
        builder.Property(r => r.PostalCode).HasMaxLength(10);

        // Contact
        builder.Property(r => r.Phone).HasMaxLength(20);
        builder.Property(r => r.Email).HasMaxLength(256);
        builder.Property(r => r.Website).HasMaxLength(500);

        // Pricing
        builder.Property(r => r.MinOrderAmount).HasPrecision(18, 2);
        builder.Property(r => r.DeliveryFee).HasPrecision(18, 2);
        builder.Property(r => r.FreeDeliveryMinimum).HasPrecision(18, 2);
        builder.Property(r => r.CommissionRate).HasPrecision(5, 2);

        // Tax
        builder.Property(r => r.TaxId).HasMaxLength(50);
        builder.Property(r => r.BusinessLicenseUrl).HasMaxLength(500);

        // Navigation
        builder.HasOne(r => r.Owner).WithMany().HasForeignKey(r => r.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(r => r.MenuCategories).WithOne(c => c.Restaurant).HasForeignKey(c => c.RestaurantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(r => r.OpeningHours).WithOne(h => h.Restaurant).HasForeignKey(h => h.RestaurantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(r => r.Orders).WithOne(o => o.Restaurant).HasForeignKey(o => o.RestaurantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(r => r.Promotions).WithOne(p => p.Restaurant).HasForeignKey(p => p.RestaurantId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== OPENING HOURS ====================
public class RestaurantOpeningHoursConfiguration : IEntityTypeConfiguration<RestaurantOpeningHours>
{
    public void Configure(EntityTypeBuilder<RestaurantOpeningHours> builder)
    {
        builder.HasKey(h => h.Id);
        builder.HasIndex(h => h.RestaurantId);
    }
}

// ==================== MENU CATEGORY ====================
public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.RestaurantId);

        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.ImageUrl).HasMaxLength(500);

        builder.HasMany(c => c.Items).WithOne(i => i.Category).HasForeignKey(i => i.CategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== MENU ITEM ====================
public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasIndex(m => m.CategoryId);
        builder.HasIndex(m => m.RestaurantId);

        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.ImageUrl).HasMaxLength(500);
        builder.Property(m => m.Price).HasPrecision(18, 2);
        builder.Property(m => m.DiscountedPrice).HasPrecision(18, 2);
        builder.Property(m => m.Allergens).HasColumnType("jsonb");
        builder.Property(m => m.AdditionalImages).HasColumnType("jsonb");

        builder.HasOne(m => m.Restaurant).WithMany().HasForeignKey(m => m.RestaurantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(m => m.Options).WithOne(o => o.MenuItem).HasForeignKey(o => o.MenuItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(m => m.Extras).WithOne(e => e.MenuItem).HasForeignKey(e => e.MenuItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(m => m.Sizes).WithOne(s => s.MenuItem).HasForeignKey(s => s.MenuItemId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== MENU ITEM SIZE ====================
public class MenuItemSizeConfiguration : IEntityTypeConfiguration<MenuItemSize>
{
    public void Configure(EntityTypeBuilder<MenuItemSize> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Price).HasPrecision(18, 2);
    }
}

// ==================== MENU ITEM EXTRA ====================
public class MenuItemExtraConfiguration : IEntityTypeConfiguration<MenuItemExtra>
{
    public void Configure(EntityTypeBuilder<MenuItemExtra> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Price).HasPrecision(18, 2);
    }
}

// ==================== MENU ITEM OPTION ====================
public class MenuItemOptionConfiguration : IEntityTypeConfiguration<MenuItemOption>
{
    public void Configure(EntityTypeBuilder<MenuItemOption> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.GroupName).HasMaxLength(100).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(100).IsRequired();
        builder.Property(o => o.PriceModifier).HasPrecision(18, 2);
    }
}

// ==================== RESTAURANT ORDER ====================
public class RestaurantOrderConfiguration : IEntityTypeConfiguration<RestaurantOrder>
{
    public void Configure(EntityTypeBuilder<RestaurantOrder> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.RestaurantId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);

        builder.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();
        
        // Delivery Address (flat)
        builder.Property(o => o.DeliveryStreet).HasMaxLength(300);
        builder.Property(o => o.DeliveryCity).HasMaxLength(100);
        builder.Property(o => o.DeliveryApartment).HasMaxLength(50);
        builder.Property(o => o.DeliveryFloor).HasMaxLength(10);
        builder.Property(o => o.DeliveryInstructions).HasMaxLength(500);

        // Pricing
        builder.Property(o => o.SubTotal).HasPrecision(18, 2);
        builder.Property(o => o.DeliveryFee).HasPrecision(18, 2);
        builder.Property(o => o.ServiceFee).HasPrecision(18, 2);
        builder.Property(o => o.Discount).HasPrecision(18, 2);
        builder.Property(o => o.TipAmount).HasPrecision(18, 2);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Property(o => o.CommissionAmount).HasPrecision(18, 2);
        builder.Property(o => o.PromoDiscount).HasPrecision(18, 2);

        // Payment
        builder.Property(o => o.PaymentIntentId).HasMaxLength(256);
        builder.Property(o => o.TransactionId).HasMaxLength(256);
        builder.Property(o => o.PromoCode).HasMaxLength(50);
        builder.Property(o => o.CancellationReason).HasMaxLength(500);
        builder.Property(o => o.SpecialInstructions).HasMaxLength(500);
        builder.Property(o => o.ReviewComment).HasMaxLength(2000);

        // Navigation
        builder.HasOne(o => o.Customer).WithMany().HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.Deliverer).WithMany().HasForeignKey(o => o.DelivererId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(o => o.StatusHistory).WithOne(h => h.Order).HasForeignKey(h => h.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== ORDER ITEM ====================
public class RestaurantOrderItemConfiguration : IEntityTypeConfiguration<RestaurantOrderItem>
{
    public void Configure(EntityTypeBuilder<RestaurantOrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.OrderId);

        builder.Property(i => i.Name).HasMaxLength(200).IsRequired();
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Property(i => i.TotalPrice).HasPrecision(18, 2);
        builder.Property(i => i.SizePrice).HasPrecision(18, 2);
        builder.Property(i => i.SelectedSize).HasMaxLength(50);
        builder.Property(i => i.SpecialInstructions).HasMaxLength(500);

        builder.HasOne(i => i.MenuItem).WithMany().HasForeignKey(i => i.MenuItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ==================== ORDER STATUS HISTORY ====================
public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.HasKey(h => h.Id);
        builder.HasIndex(h => h.OrderId);

        builder.Property(h => h.Status).HasMaxLength(50).IsRequired();
        builder.Property(h => h.Note).HasMaxLength(500);
    }
}

// ==================== RESTAURANT PROMOTION ====================
public class RestaurantPromotionConfiguration : IEntityTypeConfiguration<RestaurantPromotion>
{
    public void Configure(EntityTypeBuilder<RestaurantPromotion> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.RestaurantId);

        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Code).HasMaxLength(50);
        builder.Property(p => p.DiscountType).HasMaxLength(20);
        builder.Property(p => p.DiscountValue).HasPrecision(18, 2);
        builder.Property(p => p.MinOrderAmount).HasPrecision(18, 2);
        builder.Property(p => p.MaxDiscount).HasPrecision(18, 2);
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
    }
}
