using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Identity;

namespace MultiServices.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Role);

        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.WalletBalance).HasPrecision(18, 2);
        builder.Property(u => u.RowVersion).IsRowVersion();

        builder.OwnsOne(u => u.Phone, p =>
        {
            p.Property(pp => pp.CountryCode).HasMaxLength(10);
            p.Property(pp => pp.Number).HasMaxLength(20);
        });

        builder.HasMany(u => u.Addresses).WithOne(a => a.User).HasForeignKey(a => a.UserId);
        builder.HasMany(u => u.Documents).WithOne(d => d.User).HasForeignKey(d => d.UserId);
        builder.HasMany(u => u.Devices).WithOne(d => d.User).HasForeignKey(d => d.UserId);
    }
}
