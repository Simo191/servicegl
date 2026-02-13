#!/bin/bash
set -e

BASE="/home/claude/MultiServicesApp"
BACKEND="$BASE/src/Backend"
WEB="$BASE/src/Web"
MOBILE="$BASE/src/Mobile"

echo "=== Creating Solution & Project Files ==="

# Solution file
cat > "$BASE/MultiServicesApp.sln" << 'EOF'
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MultiServices.Domain", "src\Backend\MultiServices.Domain\MultiServices.Domain.csproj", "{A1B2C3D4-0001-0001-0001-000000000001}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MultiServices.Application", "src\Backend\MultiServices.Application\MultiServices.Application.csproj", "{A1B2C3D4-0002-0002-0002-000000000002}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MultiServices.Infrastructure", "src\Backend\MultiServices.Infrastructure\MultiServices.Infrastructure.csproj", "{A1B2C3D4-0003-0003-0003-000000000003}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MultiServices.API", "src\Backend\MultiServices.API\MultiServices.API.csproj", "{A1B2C3D4-0004-0004-0004-000000000004}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MultiServices.Maui", "src\Mobile\MultiServices.Maui\MultiServices.Maui.csproj", "{A1B2C3D4-0005-0005-0005-000000000005}"
EndProject
Global
  GlobalSection(SolutionConfigurationPlatforms) = preSolution
    Debug|Any CPU = Debug|Any CPU
    Release|Any CPU = Release|Any CPU
  EndGlobalSection
  GlobalSection(ProjectConfigurationPlatforms) = postSolution
    {A1B2C3D4-0001-0001-0001-000000000001}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A1B2C3D4-0001-0001-0001-000000000001}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {A1B2C3D4-0002-0002-0002-000000000002}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A1B2C3D4-0002-0002-0002-000000000002}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {A1B2C3D4-0003-0003-0003-000000000003}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A1B2C3D4-0003-0003-0003-000000000003}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {A1B2C3D4-0004-0004-0004-000000000004}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A1B2C3D4-0004-0004-0004-000000000004}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {A1B2C3D4-0005-0005-0005-000000000005}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A1B2C3D4-0005-0005-0005-000000000005}.Debug|Any CPU.Build.0 = Debug|Any CPU
  EndGlobalSection
EndGlobal
EOF

# Domain.csproj
cat > "$BACKEND/MultiServices.Domain/MultiServices.Domain.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.0" />
  </ItemGroup>
</Project>
EOF

# Application.csproj
cat > "$BACKEND/MultiServices.Application/MultiServices.Application.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\MultiServices.Domain\MultiServices.Domain.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="FluentValidation" Version="11.9.0" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.2.0" />
    <PackageReference Include="Microsoft.IdentityModel.Tokens" Version="7.2.0" />
  </ItemGroup>
</Project>
EOF

# Infrastructure.csproj
cat > "$BACKEND/MultiServices.Infrastructure/MultiServices.Infrastructure.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\MultiServices.Domain\MultiServices.Domain.csproj" />
    <ProjectReference Include="..\MultiServices.Application\MultiServices.Application.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="StackExchange.Redis" Version="2.7.27" />
    <PackageReference Include="Stripe.net" Version="43.16.0" />
    <PackageReference Include="FirebaseAdmin" Version="3.0.1" />
    <PackageReference Include="Azure.Storage.Blobs" Version="12.19.1" />
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="Serilog.Sinks.PostgreSQL" Version="2.3.0" />
  </ItemGroup>
</Project>
EOF

# API.csproj
cat > "$BACKEND/MultiServices.API/MultiServices.API.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\MultiServices.Infrastructure\MultiServices.Infrastructure.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.1.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
    <PackageReference Include="AspNetCoreRateLimit" Version="5.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Cors" Version="2.2.0" />
  </ItemGroup>
</Project>
EOF

echo "=== Project files created ==="

# DbContext
cat > "$BACKEND/MultiServices.Infrastructure/Data/ApplicationDbContext.cs" << 'CEOF'
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Entities.Restaurant;
using MultiServices.Domain.Entities.Service;
using MultiServices.Domain.Entities.Grocery;
using MultiServices.Domain.Entities.Common;
using MultiServices.Infrastructure.Data.Interceptors;

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
                builder.Entity(entityType.ClrType).HasQueryFilter(
                    (System.Linq.Expressions.Expression<Func<BaseEntity, bool>>)(e => !e.IsDeleted));
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
CEOF

echo "=== DbContext created ==="
echo "Done phase 1"
