using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Grocery;

namespace MultiServices.Infrastructure.Data.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.OwnerId);
        builder.HasIndex(s => s.Brand);
        builder.HasIndex(s => s.IsActive);

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Brand).HasMaxLength(100);
        builder.Property(s => s.DeliveryFee).HasPrecision(18, 2);
        builder.Property(s => s.MinOrderAmount).HasPrecision(18, 2);
        builder.Property(s => s.FreeDeliveryThreshold).HasPrecision(18, 2);
        builder.Property(s => s.CommissionRate).HasPrecision(5, 4);
        builder.Property(s => s.RowVersion).IsRowVersion();

        builder.OwnsOne(s => s.Address, a => a.HasIndex(p => new { p.Latitude, p.Longitude }));
        builder.OwnsOne(s => s.Phone);
        builder.OwnsOne(s => s.Rating, r => r.Property(p => p.Value).HasPrecision(3, 2));

        builder.HasMany(s => s.Categories).WithOne(c => c.Store).HasForeignKey(c => c.StoreId);
        builder.HasMany(s => s.Orders).WithOne(o => o.Store).HasForeignKey(o => o.StoreId);
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.StoreId);
        builder.HasIndex(p => p.Barcode);
        builder.HasIndex(p => p.Name);

        builder.Property(p => p.Name).HasMaxLength(300).IsRequired();
        builder.Property(p => p.Price).HasPrecision(18, 2);
        builder.Property(p => p.PricePerUnit).HasPrecision(18, 2);
        builder.Property(p => p.PromotionPrice).HasPrecision(18, 2);
    }
}

public class GroceryOrderConfiguration : IEntityTypeConfiguration<GroceryOrder>
{
    public void Configure(EntityTypeBuilder<GroceryOrder> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.StoreId);
        builder.HasIndex(o => o.Status);

        builder.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(o => o.SubTotal).HasPrecision(18, 2);
        builder.Property(o => o.DeliveryFee).HasPrecision(18, 2);
        builder.Property(o => o.Discount).HasPrecision(18, 2);
        builder.Property(o => o.Tip).HasPrecision(18, 2);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Property(o => o.CommissionAmount).HasPrecision(18, 2);
        builder.Property(o => o.RowVersion).IsRowVersion();

        builder.OwnsOne(o => o.DeliveryAddress);
        builder.HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId);
    }
}
