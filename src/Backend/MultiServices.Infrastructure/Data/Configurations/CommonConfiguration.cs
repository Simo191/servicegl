using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Common;

namespace MultiServices.Infrastructure.Data.Configurations;

// ==================== DELIVERER ====================
public class DelivererConfiguration : IEntityTypeConfiguration<Deliverer>
{
    public void Configure(EntityTypeBuilder<Deliverer> builder)
    {
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => new { d.CurrentLatitude, d.CurrentLongitude });

        builder.Property(d => d.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.LastName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Phone).HasMaxLength(20).IsRequired();
        builder.Property(d => d.PhotoUrl).HasMaxLength(500);
        builder.Property(d => d.VehiclePlateNumber).HasMaxLength(20);
        builder.Property(d => d.VehicleModel).HasMaxLength(100);
        builder.Property(d => d.EmergencyContactName).HasMaxLength(100);
        builder.Property(d => d.EmergencyContactPhone).HasMaxLength(20);

        builder.Property(d => d.TotalEarnings).HasPrecision(18, 2);
        builder.Property(d => d.DeliveryBaseFee).HasPrecision(18, 2);
        builder.Property(d => d.PerKmFee).HasPrecision(18, 2);

        builder.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(d => d.Earnings).WithOne(e => e.Deliverer).HasForeignKey(e => e.DelivererId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(d => d.Payouts).WithOne(p => p.Deliverer).HasForeignKey(p => p.DelivererId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== DELIVERER EARNING ====================
public class DelivererEarningConfiguration : IEntityTypeConfiguration<DelivererEarning>
{
    public void Configure(EntityTypeBuilder<DelivererEarning> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.DelivererId);

        builder.Property(e => e.OrderType).HasMaxLength(20).IsRequired();
        builder.Property(e => e.BaseFee).HasPrecision(18, 2);
        builder.Property(e => e.DistanceFee).HasPrecision(18, 2);
        builder.Property(e => e.TipAmount).HasPrecision(18, 2);
        builder.Property(e => e.BonusAmount).HasPrecision(18, 2);
        builder.Property(e => e.TotalEarning).HasPrecision(18, 2);
    }
}

// ==================== DELIVERER PAYOUT ====================
public class DelivererPayoutConfiguration : IEntityTypeConfiguration<DelivererPayout>
{
    public void Configure(EntityTypeBuilder<DelivererPayout> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.DelivererId);

        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.Property(p => p.Status).HasMaxLength(20);
        builder.Property(p => p.BankAccount).HasMaxLength(100);
        builder.Property(p => p.TransactionId).HasMaxLength(256);
    }
}

// ==================== PROMO CODE ====================
public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Code).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.DiscountType).HasMaxLength(20);
        builder.Property(p => p.DiscountValue).HasPrecision(18, 2);
        builder.Property(p => p.MinOrderAmount).HasPrecision(18, 2);
        builder.Property(p => p.MaxDiscount).HasPrecision(18, 2);
        builder.Property(p => p.ApplicableModule).HasMaxLength(20);
    }
}

// ==================== PROMO CODE USAGE ====================
public class PromoCodeUsageConfiguration : IEntityTypeConfiguration<PromoCodeUsage>
{
    public void Configure(EntityTypeBuilder<PromoCodeUsage> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => new { u.PromoCodeId, u.UserId });

        builder.Property(u => u.OrderType).HasMaxLength(20).IsRequired();
        builder.Property(u => u.DiscountApplied).HasPrecision(18, 2);
    }
}

// ==================== SUPPORT TICKET ====================
public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.TicketNumber).IsUnique();
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.Status);

        builder.Property(t => t.TicketNumber).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Subject).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(2000).IsRequired();
        builder.Property(t => t.Category).HasMaxLength(50);
        builder.Property(t => t.OrderType).HasMaxLength(20);
        builder.Property(t => t.Status).HasMaxLength(20);
        builder.Property(t => t.Priority).HasMaxLength(20);
        builder.Property(t => t.AttachmentUrls).HasColumnType("jsonb");

        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.AssignedTo).WithMany().HasForeignKey(t => t.AssignedToId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(t => t.Messages).WithOne(m => m.Ticket).HasForeignKey(m => m.TicketId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== SUPPORT MESSAGE ====================
public class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
{
    public void Configure(EntityTypeBuilder<SupportMessage> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasIndex(m => m.TicketId);

        builder.Property(m => m.Message).HasMaxLength(5000).IsRequired();
        builder.Property(m => m.AttachmentUrls).HasColumnType("jsonb");

        builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ==================== MARKETING CAMPAIGN ====================
public class MarketingCampaignConfiguration : IEntityTypeConfiguration<MarketingCampaign>
{
    public void Configure(EntityTypeBuilder<MarketingCampaign> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Type).HasMaxLength(20);
        builder.Property(c => c.Subject).HasMaxLength(200);
        builder.Property(c => c.Content).HasMaxLength(10000);
        builder.Property(c => c.ImageUrl).HasMaxLength(500);
        builder.Property(c => c.Status).HasMaxLength(20);
    }
}

// ==================== BANNER PROMOTION ====================
public class BannerPromotionConfiguration : IEntityTypeConfiguration<BannerPromotion>
{
    public void Configure(EntityTypeBuilder<BannerPromotion> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).HasMaxLength(200).IsRequired();
        builder.Property(b => b.ImageUrl).HasMaxLength(500).IsRequired();
        builder.Property(b => b.ActionUrl).HasMaxLength(500);
        builder.Property(b => b.TargetModule).HasMaxLength(20);
        builder.Property(b => b.Placement).HasMaxLength(20);
    }
}

// ==================== SYSTEM CONFIGURATION ====================
public class SystemConfigurationConfiguration : IEntityTypeConfiguration<SystemConfiguration>
{
    public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Key).IsUnique();

        builder.Property(c => c.Key).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Value).HasMaxLength(1000).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.Category).HasMaxLength(50);
        builder.Property(c => c.ValueType).HasMaxLength(20);
    }
}

// ==================== GEOGRAPHIC ZONE ====================
public class GeographicZoneConfiguration : IEntityTypeConfiguration<GeographicZone>
{
    public void Configure(EntityTypeBuilder<GeographicZone> builder)
    {
        builder.HasKey(z => z.Id);

        builder.Property(z => z.Name).HasMaxLength(100).IsRequired();
        builder.Property(z => z.City).HasMaxLength(100);
        builder.Property(z => z.SurchargeAmount).HasPrecision(18, 2);
    }
}

// ==================== AUDIT LOG ====================
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => new { a.EntityType, a.EntityId });

        builder.Property(a => a.Action).HasMaxLength(100).IsRequired();
        builder.Property(a => a.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(a => a.EntityId).HasMaxLength(100);
        builder.Property(a => a.IpAddress).HasMaxLength(45);
        builder.Property(a => a.UserAgent).HasMaxLength(500);
    }
}

// ==================== PAYMENT TRANSACTION ====================
public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => new { t.OrderType, t.OrderId });

        builder.Property(t => t.OrderType).HasMaxLength(20).IsRequired();
        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.Currency).HasMaxLength(3);
        builder.Property(t => t.StripePaymentIntentId).HasMaxLength(256);
        builder.Property(t => t.StripeChargeId).HasMaxLength(256);
        builder.Property(t => t.PayPalTransactionId).HasMaxLength(256);
        builder.Property(t => t.FailureReason).HasMaxLength(500);
        builder.Property(t => t.ReceiptUrl).HasMaxLength(500);
        builder.Property(t => t.RefundedAmount).HasPrecision(18, 2);

        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
