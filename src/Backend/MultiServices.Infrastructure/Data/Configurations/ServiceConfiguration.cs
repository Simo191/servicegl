using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiServices.Domain.Entities.Service;

namespace MultiServices.Infrastructure.Data.Configurations;

// ==================== SERVICE PROVIDER ====================
public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.OwnerId);
        builder.HasIndex(p => p.PrimaryCategory);
        builder.HasIndex(p => p.IsActive);

        builder.Property(p => p.CompanyName).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(250).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.LogoUrl).HasMaxLength(500);
        builder.Property(p => p.CoverImageUrl).HasMaxLength(500);

        // Contact (flat)
        builder.Property(p => p.Phone).HasMaxLength(20);
        builder.Property(p => p.Email).HasMaxLength(256);
        builder.Property(p => p.Website).HasMaxLength(500);

        // Location (flat)
        builder.Property(p => p.City).HasMaxLength(100);

        // JSON arrays
        builder.Property(p => p.InterventionCities).HasColumnType("jsonb");
        builder.Property(p => p.InterventionQuarters).HasColumnType("jsonb");
        builder.Property(p => p.AdditionalCategories).HasColumnType("jsonb");
        builder.Property(p => p.Certifications).HasColumnType("jsonb");
        builder.Property(p => p.Qualifications).HasColumnType("jsonb");

        // Documents
        builder.Property(p => p.InsuranceDocumentUrl).HasMaxLength(500);
        builder.Property(p => p.ProfessionalLicenseUrl).HasMaxLength(500);

        // Pricing
        builder.Property(p => p.TravelFee).HasPrecision(18, 2);
        builder.Property(p => p.CommissionRate).HasPrecision(5, 2);

        // Tax
        builder.Property(p => p.TaxId).HasMaxLength(50);
        builder.Property(p => p.BusinessRegistrationNumber).HasMaxLength(100);

        // Navigation
        builder.HasOne(p => p.Owner).WithMany().HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Services).WithOne(s => s.Provider).HasForeignKey(s => s.ProviderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Availabilities).WithOne(a => a.Provider).HasForeignKey(a => a.ProviderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.TeamMembers).WithOne(t => t.Provider).HasForeignKey(t => t.ProviderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Portfolio).WithOne(pi => pi.Provider).HasForeignKey(pi => pi.ProviderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Interventions).WithOne(i => i.Provider).HasForeignKey(i => i.ProviderId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ==================== SERVICE OFFERING ====================
public class ServiceOfferingConfiguration : IEntityTypeConfiguration<ServiceOffering>
{
    public void Configure(EntityTypeBuilder<ServiceOffering> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.ProviderId);

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.Property(s => s.HourlyRate).HasPrecision(18, 2);
        builder.Property(s => s.FixedPrice).HasPrecision(18, 2);
        builder.Property(s => s.MinPrice).HasPrecision(18, 2);
        builder.Property(s => s.MaxPrice).HasPrecision(18, 2);
        builder.Property(s => s.MaterialIncluded).HasMaxLength(500);
        builder.Property(s => s.MaterialRequired).HasMaxLength(500);
        builder.Property(s => s.ImageUrl).HasMaxLength(500);
    }
}

// ==================== AVAILABILITY ====================
public class ServiceProviderAvailabilityConfiguration : IEntityTypeConfiguration<ServiceProviderAvailability>
{
    public void Configure(EntityTypeBuilder<ServiceProviderAvailability> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.ProviderId);
    }
}

// ==================== BLOCKED SLOTS ====================
public class ServiceProviderBlockedSlotConfiguration : IEntityTypeConfiguration<ServiceProviderBlockedSlot>
{
    public void Configure(EntityTypeBuilder<ServiceProviderBlockedSlot> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.ProviderId);
        builder.Property(b => b.Reason).HasMaxLength(500);
    }
}

// ==================== TEAM MEMBER ====================
public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.ProviderId);

        builder.Property(t => t.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(t => t.LastName).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Phone).HasMaxLength(20).IsRequired();
        builder.Property(t => t.Email).HasMaxLength(256);
        builder.Property(t => t.PhotoUrl).HasMaxLength(500);
        builder.Property(t => t.Specialization).HasMaxLength(200);

        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.SetNull);
    }
}

// ==================== PORTFOLIO ITEM ====================
public class PortfolioItemConfiguration : IEntityTypeConfiguration<PortfolioItem>
{
    public void Configure(EntityTypeBuilder<PortfolioItem> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.ProviderId);

        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.BeforeImageUrls).HasColumnType("jsonb");
        builder.Property(p => p.AfterImageUrls).HasColumnType("jsonb");
    }
}

// ==================== SERVICE INTERVENTION ====================
public class ServiceInterventionConfiguration : IEntityTypeConfiguration<ServiceIntervention>
{
    public void Configure(EntityTypeBuilder<ServiceIntervention> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.InterventionNumber).IsUnique();
        builder.HasIndex(i => i.CustomerId);
        builder.HasIndex(i => i.ProviderId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.CreatedAt);

        builder.Property(i => i.InterventionNumber).HasMaxLength(50).IsRequired();
        builder.Property(i => i.ProblemDescription).HasMaxLength(2000).IsRequired();

        // Address (flat)
        builder.Property(i => i.InterventionStreet).HasMaxLength(300);
        builder.Property(i => i.InterventionCity).HasMaxLength(100);
        builder.Property(i => i.InterventionApartment).HasMaxLength(50);
        builder.Property(i => i.InterventionFloor).HasMaxLength(10);

        // JSON arrays
        builder.Property(i => i.ProblemImageUrls).HasColumnType("jsonb");
        builder.Property(i => i.BeforeImageUrls).HasColumnType("jsonb");
        builder.Property(i => i.AfterImageUrls).HasColumnType("jsonb");

        // Pricing
        builder.Property(i => i.EstimatedPrice).HasPrecision(18, 2);
        builder.Property(i => i.FinalPrice).HasPrecision(18, 2);
        builder.Property(i => i.TravelFee).HasPrecision(18, 2);
        builder.Property(i => i.MaterialCost).HasPrecision(18, 2);
        builder.Property(i => i.CommissionAmount).HasPrecision(18, 2);
        builder.Property(i => i.TotalAmount).HasPrecision(18, 2);
        builder.Property(i => i.QuoteAmount).HasPrecision(18, 2);

        // Payment
        builder.Property(i => i.PaymentIntentId).HasMaxLength(256);
        builder.Property(i => i.TransactionId).HasMaxLength(256);
        builder.Property(i => i.CancellationReason).HasMaxLength(500);
        builder.Property(i => i.InterventionReport).HasMaxLength(5000);
        builder.Property(i => i.IntervenantNotes).HasMaxLength(2000);
        builder.Property(i => i.ReviewComment).HasMaxLength(2000);

        // Navigation
        builder.HasOne(i => i.Customer).WithMany().HasForeignKey(i => i.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.ServiceOffering).WithMany().HasForeignKey(i => i.ServiceOfferingId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.AssignedTeamMember).WithMany().HasForeignKey(i => i.AssignedTeamMemberId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(i => i.StatusHistory).WithOne(h => h.Intervention).HasForeignKey(h => h.InterventionId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ==================== INTERVENTION STATUS HISTORY ====================
public class InterventionStatusHistoryConfiguration : IEntityTypeConfiguration<InterventionStatusHistory>
{
    public void Configure(EntityTypeBuilder<InterventionStatusHistory> builder)
    {
        builder.HasKey(h => h.Id);
        builder.HasIndex(h => h.InterventionId);

        builder.Property(h => h.Note).HasMaxLength(500);
    }
}
