using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Restaurant;

namespace MultiServices.Infrastructure.Data.Configurations;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.OwnerId);
        builder.HasIndex(r => r.Name);
        builder.HasIndex(r => r.CuisineType);
        builder.HasIndex(r => r.IsActive);

        builder.Property(r => r.Name).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(2000);
        builder.Property(r => r.DeliveryFee).HasPrecision(18, 2);
        builder.Property(r => r.MinOrderAmount).HasPrecision(18, 2);
        builder.Property(r => r.CommissionRate).HasPrecision(5, 4);

        builder.OwnsOne(r => r.Address, a =>
        {
            a.Property(p => p.Street).HasMaxLength(500);
            a.Property(p => p.City).HasMaxLength(200);
            a.Property(p => p.PostalCode).HasMaxLength(20);
            a.Property(p => p.Country).HasMaxLength(10);
            a.HasIndex(p => new { p.Latitude, p.Longitude });
        });

        builder.OwnsOne(r => r.Phone, p =>
        {
            p.Property(pp => pp.CountryCode).HasMaxLength(10);
            p.Property(pp => pp.Number).HasMaxLength(20);
        });

        builder.OwnsOne(r => r.Rating, r =>
        {
            r.Property(p => p.Value).HasPrecision(3, 2);
        });

        builder.Property(r => r.RowVersion).IsRowVersion();

        builder.HasMany(r => r.MenuCategories).WithOne(c => c.Restaurant).HasForeignKey(c => c.RestaurantId);
        builder.HasMany(r => r.Orders).WithOne(o => o.Restaurant).HasForeignKey(o => o.RestaurantId);
        builder.HasMany(r => r.Reviews).WithOne(r2 => r2.Restaurant).HasForeignKey(r2 => r2.RestaurantId);
        builder.HasMany(r => r.WorkingHours).WithOne(w => w.Restaurant).HasForeignKey(w => w.RestaurantId);
        builder.HasMany(r => r.Promotions).WithOne(p => p.Restaurant).HasForeignKey(p => p.RestaurantId);
    }
}

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasIndex(m => m.RestaurantId);
        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Price).HasPrecision(18, 2);
        builder.HasMany(m => m.Options).WithOne(o => o.MenuItem).HasForeignKey(o => o.MenuItemId);
    }
}

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
        builder.Property(o => o.SubTotal).HasPrecision(18, 2);
        builder.Property(o => o.DeliveryFee).HasPrecision(18, 2);
        builder.Property(o => o.Discount).HasPrecision(18, 2);
        builder.Property(o => o.Tip).HasPrecision(18, 2);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Property(o => o.CommissionAmount).HasPrecision(18, 2);
        builder.Property(o => o.RowVersion).IsRowVersion();

        builder.OwnsOne(o => o.DeliveryAddress, a =>
        {
            a.Property(p => p.Street).HasMaxLength(500);
            a.Property(p => p.City).HasMaxLength(200);
        });

        builder.HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId);
    }
}
