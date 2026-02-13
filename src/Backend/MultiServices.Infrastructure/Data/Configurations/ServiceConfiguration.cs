using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Service;

namespace MultiServices.Infrastructure.Data.Configurations;

public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.IsActive);

        builder.Property(p => p.CompanyName).HasMaxLength(200).IsRequired();
        builder.Property(p => p.CommissionRate).HasPrecision(5, 4);
        builder.Property(p => p.RowVersion).IsRowVersion();

        builder.OwnsOne(p => p.Phone);
        builder.OwnsOne(p => p.Rating, r => r.Property(p => p.Value).HasPrecision(3, 2));

        builder.HasMany(p => p.ServiceOfferings).WithOne(s => s.Provider).HasForeignKey(s => s.ProviderId);
        builder.HasMany(p => p.ServiceZones).WithOne(z => z.Provider).HasForeignKey(z => z.ProviderId);
        builder.HasMany(p => p.Workers).WithOne(w => w.Provider).HasForeignKey(w => w.ProviderId);
        builder.HasMany(p => p.Bookings).WithOne(b => b.Provider).HasForeignKey(b => b.ProviderId);
        builder.HasMany(p => p.Reviews).WithOne(r => r.Provider).HasForeignKey(r => r.ProviderId);
    }
}

public class ServiceBookingConfiguration : IEntityTypeConfiguration<ServiceBooking>
{
    public void Configure(EntityTypeBuilder<ServiceBooking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.BookingNumber).IsUnique();
        builder.HasIndex(b => b.CustomerId);
        builder.HasIndex(b => b.ProviderId);
        builder.HasIndex(b => b.Status);

        builder.Property(b => b.BookingNumber).HasMaxLength(50).IsRequired();
        builder.Property(b => b.EstimatedPrice).HasPrecision(18, 2);
        builder.Property(b => b.FinalPrice).HasPrecision(18, 2);
        builder.Property(b => b.CommissionAmount).HasPrecision(18, 2);
        builder.Property(b => b.RowVersion).IsRowVersion();

        builder.OwnsOne(b => b.InterventionAddress);
    }
}
