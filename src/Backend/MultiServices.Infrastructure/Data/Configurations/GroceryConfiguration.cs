using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Grocery;

namespace MultiServices.Infrastructure.Data.Configurations;

// ==================== GROCERY STORE ====================
public class GroceryStoreConfiguration : IEntityTypeConfiguration<GroceryStoreEntity>
{
    public void Configure(EntityTypeBuilder<GroceryStoreEntity> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.Slug).IsUnique();
        builder.HasIndex(s => s.OwnerId);
        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => new { s.Latitude, s.Longitude });

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Slug).HasMaxLength(250).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(2000);
        builder.Property(s => s.LogoUrl).HasMaxLength(500);
        builder.Property(s => s.CoverImageUrl).HasMaxLength(500);

        // Location (flat)
        builder.Property(s => s.Street).HasMaxLength(300);
        builder.Property(s => s.City).HasMaxLength(100);
        builder.Property(s => s.PostalCode).HasMaxLength(10);

        // Contact (flat)
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Email).HasMaxLength(256);

        // Pricing
        builder.Property(s => s.DeliveryFee).HasPrecision(18, 2);
        builder.Property(s => s.FreeDeliveryMinimum).HasPrecision(18, 2);
        builder.Property(s => s.MinOrderAmount).HasPrecision(18, 2);
        builder.Property(s => s.CommissionRate).HasPrecision(5, 2);

        // Navigation
        builder.HasOne(s => s.Owner).WithMany().HasForeignKey(s => s.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(s => s.OpeningHours).WithOne(h => h.Store).HasForeignKey(h => h.StoreId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Departments).WithOne(d => d.Store).HasForeignKey(d => d.StoreId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Products).WithOne(p => p.Store).HasForeignKey(p => p.StoreId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Orders).WithOne(o => o.Store).HasForeignKey(o => o.StoreId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(s => s.Promotions).WithOne(p => p.Store).HasForeignKey(p => p.StoreId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== STORE OPENING HOURS ====================
public class GroceryStoreOpeningHoursConfiguration : IEntityTypeConfiguration<GroceryStoreOpeningHours>
{
    public void Configure(EntityTypeBuilder<GroceryStoreOpeningHours> builder)
    {
        builder.HasKey(h => h.Id);
        builder.HasIndex(h => h.StoreId);
    }
}

// ==================== GROCERY DEPARTMENT ====================
public class GroceryDepartmentConfiguration : IEntityTypeConfiguration<GroceryDepartment>
{
    public void Configure(EntityTypeBuilder<GroceryDepartment> builder)
    {
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.StoreId);

        builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.ImageUrl).HasMaxLength(500);

        builder.HasMany(d => d.Products).WithOne(p => p.Department).HasForeignKey(p => p.DepartmentId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== GROCERY PRODUCT ====================
public class GroceryProductConfiguration : IEntityTypeConfiguration<GroceryProduct>
{
    public void Configure(EntityTypeBuilder<GroceryProduct> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.StoreId);
        builder.HasIndex(p => p.DepartmentId);
        builder.HasIndex(p => p.Barcode);
        builder.HasIndex(p => p.SKU);

        builder.Property(p => p.Name).HasMaxLength(300).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Brand).HasMaxLength(100);
        builder.Property(p => p.Barcode).HasMaxLength(50);
        builder.Property(p => p.SKU).HasMaxLength(50);
        builder.Property(p => p.Unit).HasMaxLength(20);
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
        builder.Property(p => p.Origin).HasMaxLength(100);

        // Pricing
        builder.Property(p => p.UnitPrice).HasPrecision(18, 2);
        builder.Property(p => p.PricePerKg).HasPrecision(18, 2);
        builder.Property(p => p.PricePerLiter).HasPrecision(18, 2);
        builder.Property(p => p.DiscountedPrice).HasPrecision(18, 2);
        builder.Property(p => p.Weight).HasPrecision(18, 3);
        builder.Property(p => p.Volume).HasPrecision(18, 3);

        // JSON
        builder.Property(p => p.Allergens).HasColumnType("jsonb");
        builder.Property(p => p.AdditionalImages).HasColumnType("jsonb");
    }
}

// ==================== GROCERY ORDER ====================
public class GroceryOrderConfiguration : IEntityTypeConfiguration<GroceryOrder>
{
    public void Configure(EntityTypeBuilder<GroceryOrder> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.StoreId);
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
        builder.Property(o => o.BagsFee).HasPrecision(18, 2);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Property(o => o.CommissionAmount).HasPrecision(18, 2);
        builder.Property(o => o.PromoDiscount).HasPrecision(18, 2);

        // Payment
        builder.Property(o => o.PaymentIntentId).HasMaxLength(256);
        builder.Property(o => o.TransactionId).HasMaxLength(256);
        builder.Property(o => o.PromoCode).HasMaxLength(50);
        builder.Property(o => o.CancellationReason).HasMaxLength(500);
        builder.Property(o => o.FreshnessPreferences).HasMaxLength(500);
        builder.Property(o => o.ReviewComment).HasMaxLength(2000);
        builder.Property(o => o.DeliveryPhotoUrl).HasMaxLength(500);

        // Navigation
        builder.HasOne(o => o.Customer).WithMany().HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.Store).WithMany(s => s.Orders).HasForeignKey(o => o.StoreId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.Deliverer).WithMany().HasForeignKey(o => o.DelivererId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(o => o.StatusHistory).WithOne(h => h.Order).HasForeignKey(h => h.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== GROCERY ORDER ITEM ====================
public class GroceryOrderItemConfiguration : IEntityTypeConfiguration<GroceryOrderItem>
{
    public void Configure(EntityTypeBuilder<GroceryOrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.OrderId);

        builder.Property(i => i.ProductName).HasMaxLength(300).IsRequired();
        builder.Property(i => i.ProductImageUrl).HasMaxLength(500);
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Property(i => i.TotalPrice).HasPrecision(18, 2);
        builder.Property(i => i.ReplacementProductName).HasMaxLength(300);
        builder.Property(i => i.ReplacementPrice).HasPrecision(18, 2);
        builder.Property(i => i.Note).HasMaxLength(500);

        builder.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.ReplacementProduct).WithMany().HasForeignKey(i => i.ReplacementProductId).OnDelete(DeleteBehavior.SetNull);
    }
}

// ==================== GROCERY ORDER STATUS HISTORY ====================
public class GroceryOrderStatusHistoryConfiguration : IEntityTypeConfiguration<GroceryOrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<GroceryOrderStatusHistory> builder)
    {
        builder.HasKey(h => h.Id);
        builder.HasIndex(h => h.OrderId);

        builder.Property(h => h.Note).HasMaxLength(500);
    }
}

// ==================== SHOPPING LIST ====================
public class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingList>
{
    public void Configure(EntityTypeBuilder<ShoppingList> builder)
    {
        builder.HasKey(l => l.Id);
        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.ShareCode).IsUnique().HasFilter("\"ShareCode\" IS NOT NULL");

        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
        builder.Property(l => l.ShareCode).HasMaxLength(20);

        builder.HasOne(l => l.User).WithMany().HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(l => l.Items).WithOne(i => i.List).HasForeignKey(i => i.ListId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(l => l.SharedWith).WithOne(s => s.List).HasForeignKey(s => s.ListId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== SHOPPING LIST ITEM ====================
public class ShoppingListItemConfiguration : IEntityTypeConfiguration<ShoppingListItem>
{
    public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.ListId);

        builder.Property(i => i.Name).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Note).HasMaxLength(500);

        builder.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.SetNull);
    }
}

// ==================== SHOPPING LIST SHARE ====================
public class ShoppingListShareConfiguration : IEntityTypeConfiguration<ShoppingListShare>
{
    public void Configure(EntityTypeBuilder<ShoppingListShare> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.ListId);

        builder.HasOne(s => s.SharedWithUser).WithMany().HasForeignKey(s => s.SharedWithUserId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== GROCERY PROMOTION ====================
public class GroceryPromotionConfiguration : IEntityTypeConfiguration<GroceryPromotion>
{
    public void Configure(EntityTypeBuilder<GroceryPromotion> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.StoreId);

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
