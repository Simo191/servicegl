using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Identity;

namespace MultiServices.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // ApplicationUser extends IdentityUser<Guid> — Id, Email, PhoneNumber, PasswordHash, etc. are inherited
        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);
        builder.Property(u => u.DeviceToken).HasMaxLength(500);
        builder.Property(u => u.RefreshToken).HasMaxLength(500);
        builder.Property(u => u.TwoFactorSecret).HasMaxLength(256);

        builder.HasIndex(u => u.IsActive);

        // Navigations
        builder.HasMany(u => u.Addresses).WithOne(a => a.User).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.Documents).WithOne(d => d.User).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.Favorites).WithOne(f => f.User).HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.Reviews).WithOne(r => r.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(u => u.Wallet).WithOne(w => w.User).HasForeignKey<WalletAccount>(w => w.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(u => u.LoyaltyAccount).WithOne(l => l.User).HasForeignKey<LoyaltyAccount>(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.Notifications).WithOne(n => n.User).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.SavedPaymentMethods).WithOne(p => p.User).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.Property(r => r.Description).HasMaxLength(500);
    }
}

public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.UserId);

        builder.Property(a => a.Label).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Street).HasMaxLength(300).IsRequired();
        builder.Property(a => a.City).HasMaxLength(100).IsRequired();
        builder.Property(a => a.PostalCode).HasMaxLength(10);
        builder.Property(a => a.Country).HasMaxLength(50);
        builder.Property(a => a.Apartment).HasMaxLength(50);
        builder.Property(a => a.Floor).HasMaxLength(10);
        builder.Property(a => a.BuildingName).HasMaxLength(100);
        builder.Property(a => a.AdditionalInfo).HasMaxLength(500);

        builder.HasIndex(a => new { a.Latitude, a.Longitude });
    }
}

public class UserDocumentConfiguration : IEntityTypeConfiguration<UserDocument>
{
    public void Configure(EntityTypeBuilder<UserDocument> builder)
    {
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => d.Status);

        builder.Property(d => d.DocumentUrl).HasMaxLength(500).IsRequired();
        builder.Property(d => d.DocumentNumber).HasMaxLength(100);
        builder.Property(d => d.RejectionReason).HasMaxLength(500);
        builder.Property(d => d.VerifiedBy).HasMaxLength(100);
    }
}

public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.UserId);

        builder.Property(d => d.DeviceToken).HasMaxLength(500).IsRequired();
        builder.Property(d => d.Platform).HasMaxLength(20).IsRequired();
        builder.Property(d => d.DeviceModel).HasMaxLength(100);
        builder.Property(d => d.AppVersion).HasMaxLength(20);

        builder.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class WalletAccountConfiguration : IEntityTypeConfiguration<WalletAccount>
{
    public void Configure(EntityTypeBuilder<WalletAccount> builder)
    {
        builder.HasKey(w => w.Id);
        builder.HasIndex(w => w.UserId).IsUnique();

        builder.Property(w => w.Balance).HasPrecision(18, 2);
        builder.Property(w => w.Currency).HasMaxLength(3);

        builder.HasMany(w => w.Transactions).WithOne(t => t.Wallet).HasForeignKey(t => t.WalletId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.WalletId);

        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.Type).HasMaxLength(20).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500).IsRequired();
        builder.Property(t => t.ReferenceId).HasMaxLength(100);
    }
}

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.HasKey(l => l.Id);
        builder.HasIndex(l => l.UserId).IsUnique();

        builder.Property(l => l.Tier).HasMaxLength(20);

        builder.HasMany(l => l.Transactions).WithOne(t => t.LoyaltyAccount).HasForeignKey(t => t.LoyaltyAccountId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.LoyaltyAccountId);

        builder.Property(t => t.Type).HasMaxLength(20).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500).IsRequired();
        builder.Property(t => t.OrderId).HasMaxLength(100);
    }
}

public class SavedPaymentMethodConfiguration : IEntityTypeConfiguration<SavedPaymentMethod>
{
    public void Configure(EntityTypeBuilder<SavedPaymentMethod> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.UserId);

        builder.Property(p => p.TokenizedCardId).HasMaxLength(256);
        builder.Property(p => p.Last4Digits).HasMaxLength(4);
        builder.Property(p => p.CardBrand).HasMaxLength(20);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);

        builder.Property(n => n.Title).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Message).HasMaxLength(1000).IsRequired();
        builder.Property(n => n.ImageUrl).HasMaxLength(500);
        builder.Property(n => n.ActionUrl).HasMaxLength(500);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => new { r.EntityType, r.EntityId });

        builder.Property(r => r.EntityType).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Comment).HasMaxLength(2000);
        builder.Property(r => r.Reply).HasMaxLength(2000);
        builder.Property(r => r.ImageUrls).HasColumnType("jsonb");
    }
}

public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    public void Configure(EntityTypeBuilder<Referral> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.ReferralCode).IsUnique();

        builder.Property(r => r.ReferralCode).HasMaxLength(20).IsRequired();
        builder.Property(r => r.ReferrerReward).HasPrecision(18, 2);
        builder.Property(r => r.ReferredReward).HasPrecision(18, 2);

        builder.HasOne(r => r.Referrer).WithMany().HasForeignKey(r => r.ReferrerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.ReferredUser).WithMany().HasForeignKey(r => r.ReferredUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
