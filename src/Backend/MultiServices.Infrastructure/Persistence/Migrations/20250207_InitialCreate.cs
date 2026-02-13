using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable enable

namespace MultiServices.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // ==================== SCHEMAS ====================
        migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS restaurant;");
        migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS service;");
        migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS grocery;");
        migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS common;");
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"pg_trgm\";");

        // ==================== IDENTITY ====================
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_AspNetRoles", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                PreferredLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "fr"),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                SocialProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                SocialProviderId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                ReferralCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                PasswordHash = table.Column<string>(type: "text", nullable: true),
                SecurityStamp = table.Column<string>(type: "text", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));

        // Identity support tables
        migrationBuilder.CreateTable(name: "AspNetRoleClaims", columns: table => new
        {
            Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            RoleId = table.Column<Guid>(type: "uuid", nullable: false),
            ClaimType = table.Column<string>(type: "text", nullable: true),
            ClaimValue = table.Column<string>(type: "text", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id); table.ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "AspNetUserClaims", columns: table => new
        {
            Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            ClaimType = table.Column<string>(type: "text", nullable: true),
            ClaimValue = table.Column<string>(type: "text", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_AspNetUserClaims", x => x.Id); table.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "AspNetUserLogins", columns: table => new
        {
            LoginProvider = table.Column<string>(type: "text", nullable: false),
            ProviderKey = table.Column<string>(type: "text", nullable: false),
            ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
            UserId = table.Column<Guid>(type: "uuid", nullable: false)
        }, constraints: table => { table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey }); table.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "AspNetUserRoles", columns: table => new
        {
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            RoleId = table.Column<Guid>(type: "uuid", nullable: false)
        }, constraints: table => { table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId }); table.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", onDelete: ReferentialAction.Cascade); table.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "AspNetUserTokens", columns: table => new
        {
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            LoginProvider = table.Column<string>(type: "text", nullable: false),
            Name = table.Column<string>(type: "text", nullable: false),
            Value = table.Column<string>(type: "text", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name }); table.ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== USER ADDRESSES ====================
        migrationBuilder.CreateTable(
            name: "UserAddresses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Street = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                PostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Maroc"),
                Latitude = table.Column<double>(type: "double precision", nullable: false),
                Longitude = table.Column<double>(type: "double precision", nullable: false),
                AdditionalInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_UserAddresses", x => x.Id); table.ForeignKey("FK_UserAddresses_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== USER DOCUMENTS ====================
        migrationBuilder.CreateTable(name: "UserDocuments", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            DocumentUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
            RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_UserDocuments", x => x.Id); table.ForeignKey("FK_UserDocuments_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== WALLET ====================
        migrationBuilder.CreateTable(name: "Wallets", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            Balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "MAD"),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_Wallets", x => x.Id); table.ForeignKey("FK_Wallets_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "WalletTransactions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            WalletId = table.Column<Guid>(type: "uuid", nullable: false),
            Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_WalletTransactions", x => x.Id); table.ForeignKey("FK_WalletTransactions_Wallets_WalletId", x => x.WalletId, "Wallets", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== LOYALTY ====================
        migrationBuilder.CreateTable(name: "LoyaltyAccounts", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            TotalPoints = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            AvailablePoints = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            Tier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Bronze"),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_LoyaltyAccounts", x => x.Id); table.ForeignKey("FK_LoyaltyAccounts_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "LoyaltyTransactions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            LoyaltyAccountId = table.Column<Guid>(type: "uuid", nullable: false),
            Points = table.Column<int>(type: "integer", nullable: false),
            Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_LoyaltyTransactions", x => x.Id); table.ForeignKey("FK_LoyaltyTransactions_LoyaltyAccounts_LoyaltyAccountId", x => x.LoyaltyAccountId, "LoyaltyAccounts", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== NOTIFICATIONS ====================
        migrationBuilder.CreateTable(name: "Notifications", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Body = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
            Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            ReferenceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
            IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_Notifications", x => x.Id); table.ForeignKey("FK_Notifications_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== RESTAURANTS ====================
        migrationBuilder.CreateTable(name: "Restaurants", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
            LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            CoverImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            GalleryUrls = table.Column<string>(type: "jsonb", nullable: true),
            CuisineType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            AdditionalCuisines = table.Column<string>(type: "jsonb", nullable: true),
            PriceRange = table.Column<int>(type: "integer", nullable: false),
            Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
            Street = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
            City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            PostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
            Latitude = table.Column<double>(type: "double precision", nullable: false),
            Longitude = table.Column<double>(type: "double precision", nullable: false),
            AverageRating = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0),
            TotalReviews = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            MinOrderAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            FreeDeliveryThreshold = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            EstimatedDeliveryMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
            CommissionRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 15m),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsFeatured = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            AcceptsOnlinePayment = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            AcceptsCashPayment = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_Restaurants", x => x.Id); table.ForeignKey("FK_Restaurants_AspNetUsers_OwnerId", x => x.OwnerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); });

        migrationBuilder.CreateTable(name: "RestaurantOpeningHours", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
            DayOfWeek = table.Column<int>(type: "integer", nullable: false),
            OpenTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
            CloseTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
            IsClosed = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_RestaurantOpeningHours", x => x.Id); table.ForeignKey("FK_RestaurantOpeningHours_Restaurants_RestaurantId", x => x.RestaurantId, "Restaurants", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "MenuCategories", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            SortOrder = table.Column<int>(type: "integer", nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_MenuCategories", x => x.Id); table.ForeignKey("FK_MenuCategories_Restaurants_RestaurantId", x => x.RestaurantId, "Restaurants", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "MenuItems", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
            RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            DiscountedPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            AdditionalImages = table.Column<string>(type: "jsonb", nullable: true),
            Allergens = table.Column<string>(type: "jsonb", nullable: true),
            IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsPopular = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            PreparationTime = table.Column<int>(type: "integer", nullable: false, defaultValue: 15),
            Calories = table.Column<int>(type: "integer", nullable: true),
            SortOrder = table.Column<int>(type: "integer", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_MenuItems", x => x.Id); table.ForeignKey("FK_MenuItems_MenuCategories_CategoryId", x => x.CategoryId, "MenuCategories", "Id", onDelete: ReferentialAction.Cascade); table.ForeignKey("FK_MenuItems_Restaurants_RestaurantId", x => x.RestaurantId, "Restaurants", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "MenuItemSizes", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            PriceAdjustment = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            IsDefault = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_MenuItemSizes", x => x.Id); table.ForeignKey("FK_MenuItemSizes_MenuItems_MenuItemId", x => x.MenuItemId, "MenuItems", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "MenuItemExtras", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_MenuItemExtras", x => x.Id); table.ForeignKey("FK_MenuItemExtras_MenuItems_MenuItemId", x => x.MenuItemId, "MenuItems", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "MenuItemOptions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            IsRequired = table.Column<bool>(type: "boolean", nullable: false),
            MinSelection = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            MaxSelection = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_MenuItemOptions", x => x.Id); table.ForeignKey("FK_MenuItemOptions_MenuItems_MenuItemId", x => x.MenuItemId, "MenuItems", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== RESTAURANT ORDERS ====================
        migrationBuilder.CreateTable(name: "RestaurantOrders", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OrderNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
            RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
            DelivererId = table.Column<Guid>(type: "uuid", nullable: true),
            Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            DeliveryAddressStreet = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
            DeliveryAddressCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            DeliveryLatitude = table.Column<double>(type: "double precision", nullable: false),
            DeliveryLongitude = table.Column<double>(type: "double precision", nullable: false),
            SubTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Discount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            Tip = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Commission = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            PaymentStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            PromoCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            SpecialInstructions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            IsScheduled = table.Column<bool>(type: "boolean", nullable: false),
            ScheduledFor = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            EstimatedDeliveryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            ActualDeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            IncludeCutlery = table.Column<bool>(type: "boolean", nullable: false),
            DeliveryProofImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_RestaurantOrders", x => x.Id); table.ForeignKey("FK_RestaurantOrders_AspNetUsers_CustomerId", x => x.CustomerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); table.ForeignKey("FK_RestaurantOrders_Restaurants_RestaurantId", x => x.RestaurantId, "Restaurants", "Id", onDelete: ReferentialAction.Restrict); });

        migrationBuilder.CreateTable(name: "RestaurantOrderItems", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OrderId = table.Column<Guid>(type: "uuid", nullable: false),
            MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Quantity = table.Column<int>(type: "integer", nullable: false),
            UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            SizeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            ExtrasJson = table.Column<string>(type: "jsonb", nullable: true),
            SpecialInstructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_RestaurantOrderItems", x => x.Id); table.ForeignKey("FK_RestaurantOrderItems_RestaurantOrders_OrderId", x => x.OrderId, "RestaurantOrders", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "OrderStatusHistories", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OrderId = table.Column<Guid>(type: "uuid", nullable: false),
            Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_OrderStatusHistories", x => x.Id); table.ForeignKey("FK_OrderStatusHistories_RestaurantOrders_OrderId", x => x.OrderId, "RestaurantOrders", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "RestaurantPromotions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
            Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            DiscountType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            DiscountValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            MinOrderAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_RestaurantPromotions", x => x.Id); table.ForeignKey("FK_RestaurantPromotions_Restaurants_RestaurantId", x => x.RestaurantId, "Restaurants", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== SERVICE PROVIDERS ====================
        migrationBuilder.CreateTable(name: "ServiceProviders", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
            CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
            LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            CoverImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            PrimaryCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
            City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            InterventionCities = table.Column<string>(type: "jsonb", nullable: true),
            InterventionQuarters = table.Column<string>(type: "jsonb", nullable: true),
            Certifications = table.Column<string>(type: "jsonb", nullable: true),
            Qualifications = table.Column<string>(type: "jsonb", nullable: true),
            YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
            AverageRating = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0),
            TotalReviews = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            TotalInterventions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            CommissionRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 20m),
            HasInsurance = table.Column<bool>(type: "boolean", nullable: false),
            InsuranceDocumentUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsFeatured = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_ServiceProviders", x => x.Id); table.ForeignKey("FK_ServiceProviders_AspNetUsers_OwnerId", x => x.OwnerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); });

        migrationBuilder.CreateTable(name: "ServiceOfferings", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            PricingType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            HourlyRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            FixedPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            MinPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            TravelFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            EstimatedDurationMinutes = table.Column<int>(type: "integer", nullable: false),
            MaterialIncluded = table.Column<bool>(type: "boolean", nullable: false),
            MaterialDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_ServiceOfferings", x => x.Id); table.ForeignKey("FK_ServiceOfferings_ServiceProviders_ProviderId", x => x.ProviderId, "ServiceProviders", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "ServiceProviderAvailabilities", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
            DayOfWeek = table.Column<int>(type: "integer", nullable: false),
            StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
            EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_ServiceProviderAvailabilities", x => x.Id); table.ForeignKey("FK_ServiceProviderAvailabilities_ServiceProviders_ProviderId", x => x.ProviderId, "ServiceProviders", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "ServiceProviderBlockedSlots", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
            StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            Reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_ServiceProviderBlockedSlots", x => x.Id); table.ForeignKey("FK_ServiceProviderBlockedSlots_ServiceProviders_ProviderId", x => x.ProviderId, "ServiceProviders", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "TeamMembers", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: true),
            FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            Specialization = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_TeamMembers", x => x.Id); table.ForeignKey("FK_TeamMembers_ServiceProviders_ProviderId", x => x.ProviderId, "ServiceProviders", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "PortfolioItems", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
            Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            BeforeImageUrls = table.Column<string>(type: "jsonb", nullable: true),
            AfterImageUrls = table.Column<string>(type: "jsonb", nullable: true),
            CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_PortfolioItems", x => x.Id); table.ForeignKey("FK_PortfolioItems_ServiceProviders_ProviderId", x => x.ProviderId, "ServiceProviders", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== SERVICE INTERVENTIONS ====================
        migrationBuilder.CreateTable(name: "ServiceInterventions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            InterventionNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
            ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
            ServiceOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
            AssignedTeamMemberId = table.Column<Guid>(type: "uuid", nullable: true),
            Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            ProblemDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
            ProblemImageUrls = table.Column<string>(type: "jsonb", nullable: true),
            AddressStreet = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
            AddressCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            Latitude = table.Column<double>(type: "double precision", nullable: false),
            Longitude = table.Column<double>(type: "double precision", nullable: false),
            ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            ScheduledStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
            ScheduledEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
            ActualStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            ActualCompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            EstimatedCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            FinalCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            Commission = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            PaymentStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            PaymentTiming = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            BeforeImageUrls = table.Column<string>(type: "jsonb", nullable: true),
            AfterImageUrls = table.Column<string>(type: "jsonb", nullable: true),
            InterventionNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
            CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            WarrantyEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
            RecurrencePattern = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_ServiceInterventions", x => x.Id); table.ForeignKey("FK_ServiceInterventions_AspNetUsers_CustomerId", x => x.CustomerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); table.ForeignKey("FK_ServiceInterventions_ServiceProviders_ProviderId", x => x.ProviderId, "ServiceProviders", "Id", onDelete: ReferentialAction.Restrict); table.ForeignKey("FK_ServiceInterventions_ServiceOfferings_ServiceOfferingId", x => x.ServiceOfferingId, "ServiceOfferings", "Id", onDelete: ReferentialAction.Restrict); });

        migrationBuilder.CreateTable(name: "InterventionStatusHistories", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            InterventionId = table.Column<Guid>(type: "uuid", nullable: false),
            Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_InterventionStatusHistories", x => x.Id); table.ForeignKey("FK_InterventionStatusHistories_ServiceInterventions_InterventionId", x => x.InterventionId, "ServiceInterventions", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== GROCERY STORES ====================
        migrationBuilder.CreateTable(name: "GroceryStores", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
            LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            CoverImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
            Street = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
            City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            PostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
            Latitude = table.Column<double>(type: "double precision", nullable: false),
            Longitude = table.Column<double>(type: "double precision", nullable: false),
            AverageRating = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0),
            TotalReviews = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            MinOrderAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            FreeDeliveryThreshold = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            CommissionRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 10m),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            AcceptsOnlinePayment = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            AcceptsCashPayment = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_GroceryStores", x => x.Id); table.ForeignKey("FK_GroceryStores_AspNetUsers_OwnerId", x => x.OwnerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); });

        migrationBuilder.CreateTable(name: "GroceryStoreOpeningHours", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            StoreId = table.Column<Guid>(type: "uuid", nullable: false),
            DayOfWeek = table.Column<int>(type: "integer", nullable: false),
            OpenTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
            CloseTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
            IsClosed = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_GroceryStoreOpeningHours", x => x.Id); table.ForeignKey("FK_GroceryStoreOpeningHours_GroceryStores_StoreId", x => x.StoreId, "GroceryStores", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "GroceryDepartments", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            StoreId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            IconName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            SortOrder = table.Column<int>(type: "integer", nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_GroceryDepartments", x => x.Id); table.ForeignKey("FK_GroceryDepartments_GroceryStores_StoreId", x => x.StoreId, "GroceryStores", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "GroceryProducts", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            StoreId = table.Column<Guid>(type: "uuid", nullable: false),
            DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            Barcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            DiscountedPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            PricePerUnit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            UnitMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            Weight = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
            WeightUnit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
            ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            AdditionalImages = table.Column<string>(type: "jsonb", nullable: true),
            NutritionalInfo = table.Column<string>(type: "text", nullable: true),
            Allergens = table.Column<string>(type: "jsonb", nullable: true),
            Origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            IsBio = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsHalal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            StockQuantity = table.Column<int>(type: "integer", nullable: false),
            LowStockThreshold = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
            IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsPopular = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            AllowReplacement = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_GroceryProducts", x => x.Id); table.ForeignKey("FK_GroceryProducts_GroceryStores_StoreId", x => x.StoreId, "GroceryStores", "Id", onDelete: ReferentialAction.Restrict); table.ForeignKey("FK_GroceryProducts_GroceryDepartments_DepartmentId", x => x.DepartmentId, "GroceryDepartments", "Id", onDelete: ReferentialAction.Restrict); });

        // ==================== GROCERY ORDERS ====================
        migrationBuilder.CreateTable(name: "GroceryOrders", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OrderNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
            StoreId = table.Column<Guid>(type: "uuid", nullable: false),
            DelivererId = table.Column<Guid>(type: "uuid", nullable: true),
            PreparerId = table.Column<Guid>(type: "uuid", nullable: true),
            Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            DeliveryAddressStreet = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
            DeliveryAddressCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            DeliveryLatitude = table.Column<double>(type: "double precision", nullable: false),
            DeliveryLongitude = table.Column<double>(type: "double precision", nullable: false),
            SubTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Discount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            BagFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            Tip = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Commission = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            PaymentStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            PromoCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            DeliveryInstructions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            AllowReplacements = table.Column<bool>(type: "boolean", nullable: false),
            LeaveAtDoor = table.Column<bool>(type: "boolean", nullable: false),
            BagType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Plastic"),
            IsScheduled = table.Column<bool>(type: "boolean", nullable: false),
            ScheduledSlotStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            ScheduledSlotEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            EstimatedDeliveryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            ActualDeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            DeliveryProofImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_GroceryOrders", x => x.Id); table.ForeignKey("FK_GroceryOrders_AspNetUsers_CustomerId", x => x.CustomerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); table.ForeignKey("FK_GroceryOrders_GroceryStores_StoreId", x => x.StoreId, "GroceryStores", "Id", onDelete: ReferentialAction.Restrict); });

        migrationBuilder.CreateTable(name: "GroceryOrderItems", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OrderId = table.Column<Guid>(type: "uuid", nullable: false),
            ProductId = table.Column<Guid>(type: "uuid", nullable: false),
            ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Quantity = table.Column<int>(type: "integer", nullable: false),
            UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            IsPicked = table.Column<bool>(type: "boolean", nullable: false),
            IsUnavailable = table.Column<bool>(type: "boolean", nullable: false),
            ReplacementProductId = table.Column<Guid>(type: "uuid", nullable: true),
            ReplacementProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
            ReplacementAccepted = table.Column<bool>(type: "boolean", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_GroceryOrderItems", x => x.Id); table.ForeignKey("FK_GroceryOrderItems_GroceryOrders_OrderId", x => x.OrderId, "GroceryOrders", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "GroceryOrderStatusHistories", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            OrderId = table.Column<Guid>(type: "uuid", nullable: false),
            Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_GroceryOrderStatusHistories", x => x.Id); table.ForeignKey("FK_GroceryOrderStatusHistories_GroceryOrders_OrderId", x => x.OrderId, "GroceryOrders", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== SHOPPING LISTS ====================
        migrationBuilder.CreateTable(name: "ShoppingLists", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
            RecurrencePattern = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_ShoppingLists", x => x.Id); table.ForeignKey("FK_ShoppingLists_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "ShoppingListItems", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            ShoppingListId = table.Column<Guid>(type: "uuid", nullable: false),
            ProductId = table.Column<Guid>(type: "uuid", nullable: true),
            ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
            IsChecked = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_ShoppingListItems", x => x.Id); table.ForeignKey("FK_ShoppingListItems_ShoppingLists_ShoppingListId", x => x.ShoppingListId, "ShoppingLists", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "ShoppingListShares", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            ShoppingListId = table.Column<Guid>(type: "uuid", nullable: false),
            SharedWithUserId = table.Column<Guid>(type: "uuid", nullable: false),
            Permission = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "ReadWrite"),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_ShoppingListShares", x => x.Id); table.ForeignKey("FK_ShoppingListShares_ShoppingLists_ShoppingListId", x => x.ShoppingListId, "ShoppingLists", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "GroceryPromotions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            StoreId = table.Column<Guid>(type: "uuid", nullable: false),
            ProductId = table.Column<Guid>(type: "uuid", nullable: true),
            Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            DiscountType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            DiscountValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            BuyQuantity = table.Column<int>(type: "integer", nullable: true),
            GetQuantity = table.Column<int>(type: "integer", nullable: true),
            MinPurchaseAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_GroceryPromotions", x => x.Id); table.ForeignKey("FK_GroceryPromotions_GroceryStores_StoreId", x => x.StoreId, "GroceryStores", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== DELIVERERS ====================
        migrationBuilder.CreateTable(name: "Deliverers", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            VehicleType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            LicensePlate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Offline"),
            CurrentLatitude = table.Column<double>(type: "double precision", nullable: true),
            CurrentLongitude = table.Column<double>(type: "double precision", nullable: true),
            AverageRating = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0),
            TotalDeliveries = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            TotalReviews = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            IsVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_Deliverers", x => x.Id); table.ForeignKey("FK_Deliverers_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "DelivererEarnings", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            DelivererId = table.Column<Guid>(type: "uuid", nullable: false),
            OrderType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            OrderId = table.Column<Guid>(type: "uuid", nullable: false),
            BaseFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Tip = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            Bonus = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
            Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_DelivererEarnings", x => x.Id); table.ForeignKey("FK_DelivererEarnings_Deliverers_DelivererId", x => x.DelivererId, "Deliverers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "DelivererPayouts", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            DelivererId = table.Column<Guid>(type: "uuid", nullable: false),
            Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_DelivererPayouts", x => x.Id); table.ForeignKey("FK_DelivererPayouts_Deliverers_DelivererId", x => x.DelivererId, "Deliverers", "Id", onDelete: ReferentialAction.Cascade); });

        // ==================== COMMON TABLES ====================
        migrationBuilder.CreateTable(name: "UserFavorites", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            EntityType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            EntityId = table.Column<Guid>(type: "uuid", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_UserFavorites", x => x.Id); table.ForeignKey("FK_UserFavorites_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "Reviews", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            EntityType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            EntityId = table.Column<Guid>(type: "uuid", nullable: false),
            Rating = table.Column<int>(type: "integer", nullable: false),
            Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
            ImageUrls = table.Column<string>(type: "jsonb", nullable: true),
            IsVerified = table.Column<bool>(type: "boolean", nullable: false),
            OwnerResponse = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
            OwnerRespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_Reviews", x => x.Id); table.ForeignKey("FK_Reviews_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "SavedPaymentMethods", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            Last4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
            CardBrand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            ExpiryMonth = table.Column<int>(type: "integer", nullable: true),
            ExpiryYear = table.Column<int>(type: "integer", nullable: true),
            StripePaymentMethodId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            IsDefault = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_SavedPaymentMethods", x => x.Id); table.ForeignKey("FK_SavedPaymentMethods_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "Referrals", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            ReferrerId = table.Column<Guid>(type: "uuid", nullable: false),
            ReferredUserId = table.Column<Guid>(type: "uuid", nullable: false),
            RewardClaimed = table.Column<bool>(type: "boolean", nullable: false),
            RewardAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_Referrals", x => x.Id); table.ForeignKey("FK_Referrals_AspNetUsers_ReferrerId", x => x.ReferrerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); table.ForeignKey("FK_Referrals_AspNetUsers_ReferredUserId", x => x.ReferredUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); });

        migrationBuilder.CreateTable(name: "PromoCodes", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            DiscountType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            DiscountValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            MaxDiscount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            MinOrderAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            ApplicableModule = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
            MaxUsages = table.Column<int>(type: "integer", nullable: true),
            CurrentUsages = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            MaxUsagesPerUser = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
            StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_PromoCodes", x => x.Id));

        migrationBuilder.CreateTable(name: "PromoCodeUsages", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            PromoCodeId = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            OrderType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            OrderId = table.Column<Guid>(type: "uuid", nullable: false),
            DiscountApplied = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_PromoCodeUsages", x => x.Id); table.ForeignKey("FK_PromoCodeUsages_PromoCodes_PromoCodeId", x => x.PromoCodeId, "PromoCodes", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "SupportTickets", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            TicketNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
            Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Normal"),
            Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Open"),
            AssignedToUserId = table.Column<Guid>(type: "uuid", nullable: true),
            RelatedOrderType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
            RelatedOrderId = table.Column<Guid>(type: "uuid", nullable: true),
            AttachmentUrls = table.Column<string>(type: "jsonb", nullable: true),
            ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_SupportTickets", x => x.Id); table.ForeignKey("FK_SupportTickets_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "SupportMessages", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            TicketId = table.Column<Guid>(type: "uuid", nullable: false),
            SenderId = table.Column<Guid>(type: "uuid", nullable: false),
            Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
            AttachmentUrls = table.Column<string>(type: "jsonb", nullable: true),
            IsFromSupport = table.Column<bool>(type: "boolean", nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_SupportMessages", x => x.Id); table.ForeignKey("FK_SupportMessages_SupportTickets_TicketId", x => x.TicketId, "SupportTickets", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "PaymentTransactions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            OrderType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            OrderId = table.Column<Guid>(type: "uuid", nullable: false),
            Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "MAD"),
            PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            StripePaymentIntentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            StripeChargeId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            RefundedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
            RefundedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_PaymentTransactions", x => x.Id); table.ForeignKey("FK_PaymentTransactions_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict); });

        migrationBuilder.CreateTable(name: "MarketingCampaigns", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
            Content = table.Column<string>(type: "text", nullable: false),
            TargetModule = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
            ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            TotalRecipients = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            TotalSent = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            TotalOpened = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_MarketingCampaigns", x => x.Id));

        migrationBuilder.CreateTable(name: "BannerPromotions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            TargetUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            TargetModule = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
            Position = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
            SortOrder = table.Column<int>(type: "integer", nullable: false),
            StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false),
            TotalClicks = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            TotalViews = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_BannerPromotions", x => x.Id));

        migrationBuilder.CreateTable(name: "SystemConfigurations", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            Value = table.Column<string>(type: "text", nullable: false),
            Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            Module = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_SystemConfigurations", x => x.Id));

        migrationBuilder.CreateTable(name: "GeographicZones", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            DeliveryFeeMultiplier = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 1m),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_GeographicZones", x => x.Id));

        migrationBuilder.CreateTable(name: "AuditLogs", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: true),
            Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            EntityId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            OldValues = table.Column<string>(type: "text", nullable: true),
            NewValues = table.Column<string>(type: "text", nullable: true),
            IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_AuditLogs", x => x.Id));

        // ==================== INDEXES ====================
        migrationBuilder.CreateIndex(name: "IX_AspNetRoleClaims_RoleId", table: "AspNetRoleClaims", column: "RoleId");
        migrationBuilder.CreateIndex(name: "RoleNameIndex", table: "AspNetRoles", column: "NormalizedName", unique: true);
        migrationBuilder.CreateIndex(name: "IX_AspNetUserClaims_UserId", table: "AspNetUserClaims", column: "UserId");
        migrationBuilder.CreateIndex(name: "IX_AspNetUserLogins_UserId", table: "AspNetUserLogins", column: "UserId");
        migrationBuilder.CreateIndex(name: "IX_AspNetUserRoles_RoleId", table: "AspNetUserRoles", column: "RoleId");
        migrationBuilder.CreateIndex(name: "EmailIndex", table: "AspNetUsers", column: "NormalizedEmail");
        migrationBuilder.CreateIndex(name: "UserNameIndex", table: "AspNetUsers", column: "NormalizedUserName", unique: true);
        migrationBuilder.CreateIndex(name: "IX_UserAddresses_UserId", table: "UserAddresses", column: "UserId");
        migrationBuilder.CreateIndex(name: "IX_UserDocuments_UserId", table: "UserDocuments", column: "UserId");
        migrationBuilder.CreateIndex(name: "IX_Wallets_UserId", table: "Wallets", column: "UserId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_LoyaltyAccounts_UserId", table: "LoyaltyAccounts", column: "UserId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Notifications_UserId_IsRead", table: "Notifications", columns: new[] { "UserId", "IsRead" });

        // Restaurant indexes
        migrationBuilder.CreateIndex(name: "IX_Restaurants_OwnerId", table: "Restaurants", column: "OwnerId");
        migrationBuilder.CreateIndex(name: "IX_Restaurants_Slug", table: "Restaurants", column: "Slug", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Restaurants_CuisineType", table: "Restaurants", column: "CuisineType");
        migrationBuilder.CreateIndex(name: "IX_Restaurants_IsActive", table: "Restaurants", column: "IsActive");
        migrationBuilder.CreateIndex(name: "IX_Restaurants_Location", table: "Restaurants", columns: new[] { "Latitude", "Longitude" });
        migrationBuilder.CreateIndex(name: "IX_MenuCategories_RestaurantId", table: "MenuCategories", column: "RestaurantId");
        migrationBuilder.CreateIndex(name: "IX_MenuItems_CategoryId", table: "MenuItems", column: "CategoryId");
        migrationBuilder.CreateIndex(name: "IX_MenuItems_RestaurantId", table: "MenuItems", column: "RestaurantId");
        migrationBuilder.CreateIndex(name: "IX_RestaurantOrders_OrderNumber", table: "RestaurantOrders", column: "OrderNumber", unique: true);
        migrationBuilder.CreateIndex(name: "IX_RestaurantOrders_CustomerId", table: "RestaurantOrders", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_RestaurantOrders_RestaurantId", table: "RestaurantOrders", column: "RestaurantId");
        migrationBuilder.CreateIndex(name: "IX_RestaurantOrders_Status", table: "RestaurantOrders", column: "Status");

        // Service indexes
        migrationBuilder.CreateIndex(name: "IX_ServiceProviders_OwnerId", table: "ServiceProviders", column: "OwnerId");
        migrationBuilder.CreateIndex(name: "IX_ServiceProviders_Slug", table: "ServiceProviders", column: "Slug", unique: true);
        migrationBuilder.CreateIndex(name: "IX_ServiceProviders_PrimaryCategory", table: "ServiceProviders", column: "PrimaryCategory");
        migrationBuilder.CreateIndex(name: "IX_ServiceOfferings_ProviderId", table: "ServiceOfferings", column: "ProviderId");
        migrationBuilder.CreateIndex(name: "IX_ServiceInterventions_InterventionNumber", table: "ServiceInterventions", column: "InterventionNumber", unique: true);
        migrationBuilder.CreateIndex(name: "IX_ServiceInterventions_CustomerId", table: "ServiceInterventions", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_ServiceInterventions_ProviderId", table: "ServiceInterventions", column: "ProviderId");
        migrationBuilder.CreateIndex(name: "IX_ServiceInterventions_Status", table: "ServiceInterventions", column: "Status");

        // Grocery indexes
        migrationBuilder.CreateIndex(name: "IX_GroceryStores_OwnerId", table: "GroceryStores", column: "OwnerId");
        migrationBuilder.CreateIndex(name: "IX_GroceryStores_Slug", table: "GroceryStores", column: "Slug", unique: true);
        migrationBuilder.CreateIndex(name: "IX_GroceryDepartments_StoreId", table: "GroceryDepartments", column: "StoreId");
        migrationBuilder.CreateIndex(name: "IX_GroceryProducts_StoreId", table: "GroceryProducts", column: "StoreId");
        migrationBuilder.CreateIndex(name: "IX_GroceryProducts_DepartmentId", table: "GroceryProducts", column: "DepartmentId");
        migrationBuilder.CreateIndex(name: "IX_GroceryProducts_Barcode", table: "GroceryProducts", column: "Barcode");
        migrationBuilder.CreateIndex(name: "IX_GroceryProducts_SKU", table: "GroceryProducts", column: "SKU");
        migrationBuilder.CreateIndex(name: "IX_GroceryOrders_OrderNumber", table: "GroceryOrders", column: "OrderNumber", unique: true);
        migrationBuilder.CreateIndex(name: "IX_GroceryOrders_CustomerId", table: "GroceryOrders", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_GroceryOrders_StoreId", table: "GroceryOrders", column: "StoreId");

        // Deliverer indexes
        migrationBuilder.CreateIndex(name: "IX_Deliverers_UserId", table: "Deliverers", column: "UserId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Deliverers_Status", table: "Deliverers", column: "Status");
        migrationBuilder.CreateIndex(name: "IX_Deliverers_Location", table: "Deliverers", columns: new[] { "CurrentLatitude", "CurrentLongitude" });

        // Common indexes
        migrationBuilder.CreateIndex(name: "IX_UserFavorites_UserId_EntityType", table: "UserFavorites", columns: new[] { "UserId", "EntityType" });
        migrationBuilder.CreateIndex(name: "IX_Reviews_EntityType_EntityId", table: "Reviews", columns: new[] { "EntityType", "EntityId" });
        migrationBuilder.CreateIndex(name: "IX_PromoCodes_Code", table: "PromoCodes", column: "Code", unique: true);
        migrationBuilder.CreateIndex(name: "IX_SupportTickets_TicketNumber", table: "SupportTickets", column: "TicketNumber", unique: true);
        migrationBuilder.CreateIndex(name: "IX_SupportTickets_UserId", table: "SupportTickets", column: "UserId");
        migrationBuilder.CreateIndex(name: "IX_SystemConfigurations_Key", table: "SystemConfigurations", column: "Key", unique: true);
        migrationBuilder.CreateIndex(name: "IX_AuditLogs_UserId", table: "AuditLogs", column: "UserId");
        migrationBuilder.CreateIndex(name: "IX_AuditLogs_EntityType_EntityId", table: "AuditLogs", columns: new[] { "EntityType", "EntityId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop in reverse order
        migrationBuilder.DropTable(name: "AuditLogs");
        migrationBuilder.DropTable(name: "GeographicZones");
        migrationBuilder.DropTable(name: "SystemConfigurations");
        migrationBuilder.DropTable(name: "BannerPromotions");
        migrationBuilder.DropTable(name: "MarketingCampaigns");
        migrationBuilder.DropTable(name: "PaymentTransactions");
        migrationBuilder.DropTable(name: "SupportMessages");
        migrationBuilder.DropTable(name: "SupportTickets");
        migrationBuilder.DropTable(name: "PromoCodeUsages");
        migrationBuilder.DropTable(name: "PromoCodes");
        migrationBuilder.DropTable(name: "Referrals");
        migrationBuilder.DropTable(name: "SavedPaymentMethods");
        migrationBuilder.DropTable(name: "Reviews");
        migrationBuilder.DropTable(name: "UserFavorites");
        migrationBuilder.DropTable(name: "DelivererPayouts");
        migrationBuilder.DropTable(name: "DelivererEarnings");
        migrationBuilder.DropTable(name: "Deliverers");
        migrationBuilder.DropTable(name: "GroceryPromotions");
        migrationBuilder.DropTable(name: "ShoppingListShares");
        migrationBuilder.DropTable(name: "ShoppingListItems");
        migrationBuilder.DropTable(name: "ShoppingLists");
        migrationBuilder.DropTable(name: "GroceryOrderStatusHistories");
        migrationBuilder.DropTable(name: "GroceryOrderItems");
        migrationBuilder.DropTable(name: "GroceryOrders");
        migrationBuilder.DropTable(name: "GroceryProducts");
        migrationBuilder.DropTable(name: "GroceryDepartments");
        migrationBuilder.DropTable(name: "GroceryStoreOpeningHours");
        migrationBuilder.DropTable(name: "GroceryStores");
        migrationBuilder.DropTable(name: "InterventionStatusHistories");
        migrationBuilder.DropTable(name: "ServiceInterventions");
        migrationBuilder.DropTable(name: "PortfolioItems");
        migrationBuilder.DropTable(name: "TeamMembers");
        migrationBuilder.DropTable(name: "ServiceProviderBlockedSlots");
        migrationBuilder.DropTable(name: "ServiceProviderAvailabilities");
        migrationBuilder.DropTable(name: "ServiceOfferings");
        migrationBuilder.DropTable(name: "ServiceProviders");
        migrationBuilder.DropTable(name: "RestaurantPromotions");
        migrationBuilder.DropTable(name: "OrderStatusHistories");
        migrationBuilder.DropTable(name: "RestaurantOrderItems");
        migrationBuilder.DropTable(name: "RestaurantOrders");
        migrationBuilder.DropTable(name: "MenuItemOptions");
        migrationBuilder.DropTable(name: "MenuItemExtras");
        migrationBuilder.DropTable(name: "MenuItemSizes");
        migrationBuilder.DropTable(name: "MenuItems");
        migrationBuilder.DropTable(name: "MenuCategories");
        migrationBuilder.DropTable(name: "RestaurantOpeningHours");
        migrationBuilder.DropTable(name: "Restaurants");
        migrationBuilder.DropTable(name: "Notifications");
        migrationBuilder.DropTable(name: "LoyaltyTransactions");
        migrationBuilder.DropTable(name: "LoyaltyAccounts");
        migrationBuilder.DropTable(name: "WalletTransactions");
        migrationBuilder.DropTable(name: "Wallets");
        migrationBuilder.DropTable(name: "UserDocuments");
        migrationBuilder.DropTable(name: "UserAddresses");
        migrationBuilder.DropTable(name: "AspNetUserTokens");
        migrationBuilder.DropTable(name: "AspNetUserRoles");
        migrationBuilder.DropTable(name: "AspNetUserLogins");
        migrationBuilder.DropTable(name: "AspNetUserClaims");
        migrationBuilder.DropTable(name: "AspNetRoleClaims");
        migrationBuilder.DropTable(name: "AspNetUsers");
        migrationBuilder.DropTable(name: "AspNetRoles");
    }
}
