using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Entities.Grocery;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Entities.Restaurant;
using MultiServices.Domain.Entities.Service;
using MultiServices.Infrastructure.Data.Interceptors;
using System.Linq.Expressions;

namespace MultiServices.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly AuditableEntityInterceptor _auditInterceptor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, AuditableEntityInterceptor auditInterceptor) : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    // Identity
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<UserDocument> UserDocuments => Set<UserDocument>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<WalletAccount> Wallets => Set<WalletAccount>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<LoyaltyAccount> LoyaltyAccounts => Set<LoyaltyAccount>();
    public DbSet<LoyaltyTransaction> LoyaltyTransactions => Set<LoyaltyTransaction>();
    public DbSet<SavedPaymentMethod> SavedPaymentMethods => Set<SavedPaymentMethod>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Referral> Referrals => Set<Referral>();

    // Restaurant Module
    public DbSet<RestaurantEntity> Restaurants => Set<RestaurantEntity>();
    public DbSet<RestaurantOpeningHours> RestaurantOpeningHours => Set<RestaurantOpeningHours>();
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemSize> MenuItemSizes => Set<MenuItemSize>();
    public DbSet<MenuItemExtra> MenuItemExtras => Set<MenuItemExtra>();
    public DbSet<MenuItemOption> MenuItemOptions => Set<MenuItemOption>();
    public DbSet<RestaurantOrder> RestaurantOrders => Set<RestaurantOrder>();
    public DbSet<RestaurantOrderItem> RestaurantOrderItems => Set<RestaurantOrderItem>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<RestaurantPromotion> RestaurantPromotions => Set<RestaurantPromotion>();

    // Service Module
    public DbSet<ServiceProvider> ServiceProviders => Set<ServiceProvider>();
    public DbSet<ServiceOffering> ServiceOfferings => Set<ServiceOffering>();
    public DbSet<ServiceProviderAvailability> ServiceProviderAvailabilities => Set<ServiceProviderAvailability>();
    public DbSet<ServiceProviderBlockedSlot> ServiceProviderBlockedSlots => Set<ServiceProviderBlockedSlot>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<PortfolioItem> PortfolioItems => Set<PortfolioItem>();
    public DbSet<ServiceIntervention> ServiceInterventions => Set<ServiceIntervention>();
    public DbSet<InterventionStatusHistory> InterventionStatusHistories => Set<InterventionStatusHistory>();

    // Grocery Module
    public DbSet<GroceryStoreEntity> GroceryStores => Set<GroceryStoreEntity>();
    public DbSet<GroceryStoreOpeningHours> GroceryStoreOpeningHours => Set<GroceryStoreOpeningHours>();
    public DbSet<GroceryDepartment> GroceryDepartments => Set<GroceryDepartment>();
    public DbSet<GroceryProduct> GroceryProducts => Set<GroceryProduct>();
    public DbSet<GroceryOrder> GroceryOrders => Set<GroceryOrder>();
    public DbSet<GroceryOrderItem> GroceryOrderItems => Set<GroceryOrderItem>();
    public DbSet<GroceryOrderStatusHistory> GroceryOrderStatusHistories => Set<GroceryOrderStatusHistory>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<ShoppingListShare> ShoppingListShares => Set<ShoppingListShare>();
    public DbSet<GroceryPromotion> GroceryPromotions => Set<GroceryPromotion>();

    // Common
    public DbSet<Deliverer> Deliverers => Set<Deliverer>();
    public DbSet<DelivererEarning> DelivererEarnings => Set<DelivererEarning>();
    public DbSet<DelivererPayout> DelivererPayouts => Set<DelivererPayout>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<PromoCodeUsage> PromoCodeUsages => Set<PromoCodeUsage>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<SupportMessage> SupportMessages => Set<SupportMessage>();
    public DbSet<MarketingCampaign> MarketingCampaigns => Set<MarketingCampaign>();
    public DbSet<BannerPromotion> BannerPromotions => Set<BannerPromotion>();
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public DbSet<GeographicZone> GeographicZones => Set<GeographicZone>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for soft delete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // e =>
                var parameter = Expression.Parameter(entityType.ClrType, "e");

                // ((BaseEntity)e).IsDeleted
                var isDeletedProperty = Expression.Property(
                    Expression.Convert(parameter, typeof(BaseEntity)),
                    nameof(BaseEntity.IsDeleted));

                // !((BaseEntity)e).IsDeleted
                var body = Expression.Not(isDeletedProperty);

                // Expression<Func<TEntity, bool>>
                var lambda = Expression.Lambda(body, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        // Indexes
        builder.Entity<RestaurantEntity>().HasIndex(r => r.Slug).IsUnique();
        builder.Entity<RestaurantEntity>().HasIndex(r => new { r.Latitude, r.Longitude });
        builder.Entity<RestaurantEntity>().HasIndex(r => r.CuisineType);
        builder.Entity<RestaurantEntity>().HasIndex(r => r.IsActive);

        builder.Entity<ServiceProvider>().HasIndex(s => s.Slug).IsUnique();
        builder.Entity<ServiceProvider>().HasIndex(s => s.PrimaryCategory);

        builder.Entity<GroceryStoreEntity>().HasIndex(g => g.Slug).IsUnique();
        builder.Entity<GroceryProduct>().HasIndex(p => p.Barcode);
        builder.Entity<GroceryProduct>().HasIndex(p => p.SKU);

        builder.Entity<RestaurantOrder>().HasIndex(o => o.OrderNumber).IsUnique();
        builder.Entity<ServiceIntervention>().HasIndex(i => i.InterventionNumber).IsUnique();
        builder.Entity<GroceryOrder>().HasIndex(o => o.OrderNumber).IsUnique();

        builder.Entity<PromoCode>().HasIndex(p => p.Code).IsUnique();
        builder.Entity<SupportTicket>().HasIndex(t => t.TicketNumber).IsUnique();

        builder.Entity<Deliverer>().HasIndex(d => new { d.CurrentLatitude, d.CurrentLongitude });
        builder.Entity<Deliverer>().HasIndex(d => d.Status);

        // Precision for decimal properties
        foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(18);
            property.SetScale(2);
        }

        // JSON columns for lists
        builder.Entity<RestaurantEntity>().Property(r => r.GalleryUrls).HasColumnType("jsonb");
        builder.Entity<RestaurantEntity>().Property(r => r.AdditionalCuisines).HasColumnType("jsonb");
        builder.Entity<ServiceProvider>().Property(s => s.InterventionCities).HasColumnType("jsonb");
        builder.Entity<ServiceProvider>().Property(s => s.InterventionQuarters).HasColumnType("jsonb");
        builder.Entity<ServiceProvider>().Property(s => s.Certifications).HasColumnType("jsonb");
        builder.Entity<ServiceProvider>().Property(s => s.Qualifications).HasColumnType("jsonb");
        builder.Entity<GroceryProduct>().Property(p => p.Allergens).HasColumnType("jsonb");
        builder.Entity<GroceryProduct>().Property(p => p.AdditionalImages).HasColumnType("jsonb");
        builder.Entity<MenuItem>().Property(m => m.Allergens).HasColumnType("jsonb");
        builder.Entity<MenuItem>().Property(m => m.AdditionalImages).HasColumnType("jsonb");
        builder.Entity<ServiceIntervention>().Property(i => i.ProblemImageUrls).HasColumnType("jsonb");
        builder.Entity<ServiceIntervention>().Property(i => i.BeforeImageUrls).HasColumnType("jsonb");
        builder.Entity<ServiceIntervention>().Property(i => i.AfterImageUrls).HasColumnType("jsonb");
        builder.Entity<PortfolioItem>().Property(p => p.BeforeImageUrls).HasColumnType("jsonb");
        builder.Entity<PortfolioItem>().Property(p => p.AfterImageUrls).HasColumnType("jsonb");
        builder.Entity<Review>().Property(r => r.ImageUrls).HasColumnType("jsonb");
        builder.Entity<SupportTicket>().Property(t => t.AttachmentUrls).HasColumnType("jsonb");
        builder.Entity<SupportMessage>().Property(m => m.AttachmentUrls).HasColumnType("jsonb");
    }
}
