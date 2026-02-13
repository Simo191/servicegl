#!/bin/bash
BASE="/home/claude/MultiServicesApp/src/Mobile/MultiServices.Maui"

# ============================================================
# 1. PROJECT FILE
# ============================================================
cat > "$BASE/MultiServices.Maui.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>MultiServices.Maui</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <ApplicationTitle>MultiServices</ApplicationTitle>
    <ApplicationIdGuid>A1B2C3D4-E5F6-7890-ABCD-EF1234567890</ApplicationIdGuid>
    <ApplicationId>com.multiservices.app</ApplicationId>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <ApplicationVersion>1</ApplicationVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">24.0</SupportedOSPlatformVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">15.0</SupportedOSPlatformVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageReference Include="CommunityToolkit.Maui" Version="9.0.0" />
    <PackageReference Include="Microsoft.Maui.Controls.Maps" Version="9.0.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="9.0.0" />
    <PackageReference Include="Plugin.LocalNotification" Version="11.1.4" />
    <PackageReference Include="ZXing.Net.Maui.Controls" Version="0.4.0" />
  </ItemGroup>
  <ItemGroup>
    <MauiImage Include="Resources\Images\*" />
    <MauiFont Include="Resources\Fonts\*" />
    <MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
  </ItemGroup>
</Project>
EOF

# ============================================================
# 2. MODELS
# ============================================================
cat > "$BASE/Models/ApiResponse.cs" << 'EOF'
namespace MultiServices.Maui.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UserDto User { get; set; } = new();
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public int LoyaltyPoints { get; set; }
    public string LoyaltyTier { get; set; } = "Bronze";
}

public class AddressDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public string? Apartment { get; set; }
    public string? Instructions { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; }
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WalletDto
{
    public decimal Balance { get; set; }
    public List<WalletTransactionDto> RecentTransactions { get; set; } = new();
}

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
EOF

cat > "$BASE/Models/RestaurantModels.cs" << 'EOF'
namespace MultiServices.Maui.Models;

public class RestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public List<string> GalleryUrls { get; set; } = new();
    public string CuisineType { get; set; } = string.Empty;
    public string PriceRange { get; set; } = "€€";
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal MinimumOrder { get; set; }
    public decimal DeliveryFee { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    public bool IsOpen { get; set; }
    public bool HasActivePromotions { get; set; }
    public double? Distance { get; set; }
    public List<MenuCategoryDto> MenuCategories { get; set; } = new();
}

public class RestaurantListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string CuisineType { get; set; } = string.Empty;
    public string PriceRange { get; set; } = "€€";
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public decimal DeliveryFee { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    public bool IsOpen { get; set; }
    public bool HasActivePromotions { get; set; }
    public double? Distance { get; set; }
}

public class MenuCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
}

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsPopular { get; set; }
    public List<string> Allergens { get; set; } = new();
    public string? NutritionalInfo { get; set; }
    public List<MenuItemSizeDto> Sizes { get; set; } = new();
    public List<MenuItemExtraDto> Extras { get; set; } = new();
}

public class MenuItemSizeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}

public class MenuItemExtraDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class RestaurantOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantLogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Tip { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? SpecialInstructions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public string? DelivererName { get; set; }
    public string? DelivererPhone { get; set; }
    public double? DelivererLatitude { get; set; }
    public double? DelivererLongitude { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
}

public class OrderItemDto
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SizeName { get; set; }
    public List<string> Extras { get; set; } = new();
    public string? SpecialInstructions { get; set; }
}

public class OrderStatusHistoryDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Note { get; set; }
}

public class RestaurantSearchRequest
{
    public string? Query { get; set; }
    public string? CuisineType { get; set; }
    public string? PriceRange { get; set; }
    public double? MinRating { get; set; }
    public double? MaxDistance { get; set; }
    public int? MaxDeliveryTime { get; set; }
    public bool? HasPromotions { get; set; }
    public string? SortBy { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
EOF

cat > "$BASE/Models/ServiceModels.cs" << 'EOF'
namespace MultiServices.Maui.Models;

public class ServiceProviderDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public int CompletedInterventions { get; set; }
    public double InterventionRadius { get; set; }
    public List<string> Certifications { get; set; } = new();
    public List<ServiceOfferingDto> Services { get; set; } = new();
    public List<PortfolioItemDto> Portfolio { get; set; } = new();
    public List<ReviewDto> Reviews { get; set; } = new();
    public double? Distance { get; set; }
}

public class ServiceProviderListDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public int YearsExperience { get; set; }
    public decimal StartingPrice { get; set; }
    public string PricingType { get; set; } = string.Empty;
    public double? Distance { get; set; }
}

public class ServiceOfferingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PricingType { get; set; } = string.Empty;
    public decimal? HourlyRate { get; set; }
    public decimal? FixedPrice { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public bool IsAvailable { get; set; }
}

public class PortfolioItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BeforeImageUrl { get; set; }
    public string? AfterImageUrl { get; set; }
}

public class AvailableSlotDto
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Display => $"{Date:dd/MM} {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
}

public class InterventionDto
{
    public Guid Id { get; set; }
    public string InterventionNumber { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderLogoUrl { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ProblemDescription { get; set; }
    public List<string> ProblemPhotos { get; set; } = new();
    public DateTime ScheduledDate { get; set; }
    public TimeSpan ScheduledStartTime { get; set; }
    public TimeSpan? ScheduledEndTime { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? FinalCost { get; set; }
    public string? IntervenantName { get; set; }
    public string? IntervenantPhone { get; set; }
    public double? IntervenantLatitude { get; set; }
    public double? IntervenantLongitude { get; set; }
    public List<string> BeforePhotos { get; set; } = new();
    public List<string> AfterPhotos { get; set; } = new();
    public string? Report { get; set; }
    public int? ActualDurationMinutes { get; set; }
    public bool HasWarranty { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public List<InterventionStatusHistoryDto> StatusHistory { get; set; } = new();
}

public class InterventionStatusHistoryDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Note { get; set; }
}

public class ServiceSearchRequest
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public double? MinRating { get; set; }
    public double? MaxDistance { get; set; }
    public string? PricingType { get; set; }
    public string? SortBy { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
EOF

cat > "$BASE/Models/GroceryModels.cs" << 'EOF'
namespace MultiServices.Maui.Models;

public class GroceryStoreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public decimal MinimumOrder { get; set; }
    public decimal DeliveryFee { get; set; }
    public bool IsOpen { get; set; }
    public bool HasFreeDelivery { get; set; }
    public bool HasActivePromotions { get; set; }
    public double? Distance { get; set; }
    public List<GroceryDepartmentDto> Departments { get; set; } = new();
}

public class GroceryStoreListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public double Rating { get; set; }
    public decimal DeliveryFee { get; set; }
    public bool HasFreeDelivery { get; set; }
    public bool HasActivePromotions { get; set; }
    public double? Distance { get; set; }
}

public class GroceryDepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int ProductCount { get; set; }
}

public class GroceryProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Barcode { get; set; }
    public string Department { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public decimal? PricePerUnit { get; set; }
    public string? UnitMeasure { get; set; }
    public string? NutritionalInfo { get; set; }
    public List<string> Allergens { get; set; } = new();
    public string? Origin { get; set; }
    public bool IsInStock { get; set; }
    public bool IsBio { get; set; }
    public bool IsHalal { get; set; }
    public bool IsOnPromotion { get; set; }
    public decimal? PromotionPrice { get; set; }
    public string? PromotionLabel { get; set; }
}

public class GroceryOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string? StoreLogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal BagsFee { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Tip { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? DeliveryInstructions { get; set; }
    public bool LeaveAtDoor { get; set; }
    public bool AllowReplacements { get; set; }
    public string BagType { get; set; } = "plastic";
    public DateTime CreatedAt { get; set; }
    public DateTime? ScheduledDelivery { get; set; }
    public string? DelivererName { get; set; }
    public double? DelivererLatitude { get; set; }
    public double? DelivererLongitude { get; set; }
    public List<GroceryOrderItemDto> Items { get; set; } = new();
    public List<GroceryReplacementDto> Replacements { get; set; } = new();
}

public class GroceryOrderItemDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class GroceryReplacementDto
{
    public Guid OriginalProductId { get; set; }
    public string OriginalProductName { get; set; } = string.Empty;
    public Guid ReplacementProductId { get; set; }
    public string ReplacementProductName { get; set; } = string.Empty;
    public decimal PriceDifference { get; set; }
    public string Status { get; set; } = "Pending";
}

public class ShoppingListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int CheckedCount { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsed { get; set; }
    public List<ShoppingListItemDto> Items { get; set; } = new();
    public List<ShoppingListShareDto> SharedWith { get; set; } = new();
}

public class ShoppingListItemDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public bool IsChecked { get; set; }
    public Guid? ProductId { get; set; }
}

public class ShoppingListShareDto
{
    public string UserName { get; set; } = string.Empty;
    public string Permission { get; set; } = "view";
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}
EOF

cat > "$BASE/Models/CartModels.cs" << 'EOF'
namespace MultiServices.Maui.Models;

// Restaurant Cart
public class RestaurantCartItem
{
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public int Quantity { get; set; }
    public MenuItemSizeDto? SelectedSize { get; set; }
    public List<MenuItemExtraDto> SelectedExtras { get; set; } = new();
    public string? SpecialInstructions { get; set; }
    public decimal UnitPrice => BasePrice + (SelectedSize?.PriceAdjustment ?? 0) + SelectedExtras.Sum(e => e.Price);
    public decimal TotalPrice => UnitPrice * Quantity;
}

public class RestaurantCart
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantLogoUrl { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrder { get; set; }
    public List<RestaurantCartItem> Items { get; set; } = new();
    public string? PromoCode { get; set; }
    public decimal Discount { get; set; }
    public decimal? Tip { get; set; }
    public bool IncludeCutlery { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateTime? ScheduledDelivery { get; set; }
    
    public decimal SubTotal => Items.Sum(i => i.TotalPrice);
    public decimal Total => SubTotal + DeliveryFee - Discount + (Tip ?? 0);
    public int ItemCount => Items.Sum(i => i.Quantity);
    public bool MeetsMinimum => SubTotal >= MinimumOrder;
}

// Grocery Cart
public class GroceryCart
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? StoreLogoUrl { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrder { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public string? PromoCode { get; set; }
    public decimal Discount { get; set; }
    public decimal? Tip { get; set; }
    public string BagType { get; set; } = "plastic";
    public decimal BagsFee { get; set; }
    public bool AllowReplacements { get; set; } = true;
    public bool LeaveAtDoor { get; set; }
    public string? DeliveryInstructions { get; set; }
    public DateTime? ScheduledDelivery { get; set; }
    
    public decimal SubTotal => Items.Sum(i => i.Total);
    public decimal Total => SubTotal + DeliveryFee + BagsFee - Discount + (Tip ?? 0);
    public int ItemCount => Items.Sum(i => i.Quantity);
}
EOF

echo "✅ Models created"

# ============================================================
# 3. SERVICES
# ============================================================
cat > "$BASE/Services/Api/ApiService.cs" << 'EOF'
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using MultiServices.Maui.Models;

namespace MultiServices.Maui.Services.Api;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorageService _secureStorage;
    private const string BaseUrl = "https://api.multiservices.ma/api/v1";

    public ApiService(HttpClient httpClient, ISecureStorageService secureStorage)
    {
        _httpClient = httpClient;
        _secureStorage = secureStorage;
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _secureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            await SetAuthHeaderAsync();
            var url = BuildUrl(endpoint, queryParams);
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            await SetAuthHeaderAsync();
            var content = data != null
                ? new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                : null;
            var response = await _httpClient.PostAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            await SetAuthHeaderAsync();
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync(endpoint);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> UploadAsync<T>(string endpoint, Stream fileStream, string fileName)
    {
        try
        {
            await SetAuthHeaderAsync();
            using var formData = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            formContent.Add(streamContent, "file", fileName);
            var response = await _httpClient.PostAsync(endpoint, formData);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<ApiResponse<T>>(json);
            return result ?? new ApiResponse<T> { Success = true };
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshed = await RefreshTokenAsync();
            if (!refreshed)
            {
                await Shell.Current.GoToAsync("//login");
                return new ApiResponse<T> { Success = false, Message = "Session expirée" };
            }
        }

        return new ApiResponse<T>
        {
            Success = false,
            Message = $"Erreur {(int)response.StatusCode}: {response.ReasonPhrase}"
        };
    }

    private async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _secureStorage.GetAsync("refresh_token");
        if (string.IsNullOrEmpty(refreshToken)) return false;

        try
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(new { refreshToken }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/auth/refresh", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<AuthResponse>>(json);
                if (result?.Data != null)
                {
                    await _secureStorage.SetAsync("auth_token", result.Data.Token);
                    await _secureStorage.SetAsync("refresh_token", result.Data.RefreshToken);
                    return true;
                }
            }
        }
        catch { }
        return false;
    }

    private string BuildUrl(string endpoint, Dictionary<string, string>? queryParams)
    {
        if (queryParams == null || queryParams.Count == 0) return endpoint;
        var query = string.Join("&", queryParams
            .Where(kv => !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        return $"{endpoint}?{query}";
    }
}
EOF

# Fix the typo in UploadAsync
sed -i 's/formContent.Add/formData.Add/' "$BASE/Services/Api/ApiService.cs"

cat > "$BASE/Services/Auth/AuthService.cs" << 'EOF'
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.Services.Auth;

public class AuthService
{
    private readonly ApiService _api;
    private readonly ISecureStorageService _secureStorage;
    private UserDto? _currentUser;

    public AuthService(ApiService api, ISecureStorageService secureStorage)
    {
        _api = api;
        _secureStorage = secureStorage;
    }

    public UserDto? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        var result = await _api.PostAsync<AuthResponse>("/auth/login", new { email, password });
        if (result.Success && result.Data != null)
        {
            await SaveTokensAsync(result.Data);
            _currentUser = result.Data.User;
            return (true, null);
        }
        return (false, result.Message ?? "Identifiants incorrects");
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string firstName, string lastName, string email, string password, string phone)
    {
        var result = await _api.PostAsync<AuthResponse>("/auth/register", new
        {
            firstName, lastName, email, password, phoneNumber = phone
        });
        if (result.Success && result.Data != null)
        {
            await SaveTokensAsync(result.Data);
            _currentUser = result.Data.User;
            return (true, null);
        }
        return (false, result.Message ?? "Erreur lors de l'inscription");
    }

    public async Task<(bool Success, string? Error)> SocialLoginAsync(string provider, string token)
    {
        var result = await _api.PostAsync<AuthResponse>("/auth/social-login", new { provider, token });
        if (result.Success && result.Data != null)
        {
            await SaveTokensAsync(result.Data);
            _currentUser = result.Data.User;
            return (true, null);
        }
        return (false, result.Message ?? "Erreur de connexion sociale");
    }

    public async Task<bool> CheckAuthAsync()
    {
        var token = await _secureStorage.GetAsync("auth_token");
        if (string.IsNullOrEmpty(token)) return false;

        var result = await _api.GetAsync<UserDto>("/auth/me");
        if (result.Success && result.Data != null)
        {
            _currentUser = result.Data;
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        await _api.PostAsync<object>("/auth/logout");
        _secureStorage.Remove("auth_token");
        _secureStorage.Remove("refresh_token");
        _secureStorage.Remove("user_data");
        _currentUser = null;
        await Shell.Current.GoToAsync("//login");
    }

    public async Task<(bool Success, string? Error)> ForgotPasswordAsync(string email)
    {
        var result = await _api.PostAsync<object>("/auth/forgot-password", new { email });
        return (result.Success, result.Message);
    }

    public async Task<(bool Success, string? Error)> VerifyPhoneAsync(string code)
    {
        var result = await _api.PostAsync<object>("/auth/verify-phone", new { code });
        return (result.Success, result.Message);
    }

    private async Task SaveTokensAsync(AuthResponse auth)
    {
        await _secureStorage.SetAsync("auth_token", auth.Token);
        await _secureStorage.SetAsync("refresh_token", auth.RefreshToken);
    }
}
EOF

cat > "$BASE/Services/Storage/ISecureStorageService.cs" << 'EOF'
namespace MultiServices.Maui.Services.Storage;

public interface ISecureStorageService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    void Remove(string key);
    void RemoveAll();
}

public class SecureStorageService : ISecureStorageService
{
    public async Task<string?> GetAsync(string key)
    {
        try { return await SecureStorage.Default.GetAsync(key); }
        catch { return null; }
    }

    public async Task SetAsync(string key, string value)
    {
        try { await Task.Run(() => SecureStorage.Default.SetAsync(key, value)); }
        catch { Preferences.Default.Set(key, value); }
    }

    public void Remove(string key)
    {
        try { SecureStorage.Default.Remove(key); }
        catch { Preferences.Default.Remove(key); }
    }

    public void RemoveAll()
    {
        try { SecureStorage.Default.RemoveAll(); }
        catch { Preferences.Default.Clear(); }
    }
}
EOF

cat > "$BASE/Services/Location/LocationService.cs" << 'EOF'
namespace MultiServices.Maui.Services.Location;

public class LocationService
{
    private Microsoft.Maui.Devices.Sensors.Location? _lastLocation;

    public Microsoft.Maui.Devices.Sensors.Location? LastLocation => _lastLocation;

    public async Task<Microsoft.Maui.Devices.Sensors.Location?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    return null;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            _lastLocation = await Geolocation.Default.GetLocationAsync(request);
            return _lastLocation;
        }
        catch
        {
            try
            {
                _lastLocation = await Geolocation.Default.GetLastKnownLocationAsync();
                return _lastLocation;
            }
            catch { return null; }
        }
    }

    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var loc1 = new Microsoft.Maui.Devices.Sensors.Location(lat1, lon1);
        var loc2 = new Microsoft.Maui.Devices.Sensors.Location(lat2, lon2);
        return Microsoft.Maui.Devices.Sensors.Location.CalculateDistance(loc1, loc2, DistanceUnits.Kilometers);
    }
}
EOF

cat > "$BASE/Services/Notification/NotificationService.cs" << 'EOF'
namespace MultiServices.Maui.Services.Notification;

public class NotificationService
{
    public async Task<bool> RequestPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }
            return status == PermissionStatus.Granted;
        }
        catch { return false; }
    }

    public async Task ShowLocalNotificationAsync(string title, string body)
    {
        // Using Plugin.LocalNotification
        var notification = new Plugin.LocalNotification.NotificationRequest
        {
            NotificationId = new Random().Next(1000),
            Title = title,
            Description = body,
            ReturningData = "notification_data",
        };
        await Plugin.LocalNotification.LocalNotificationCenter.Current.Show(notification);
    }
}
EOF

echo "✅ Services created"

# ============================================================
# 4. CONVERTERS
# ============================================================
cat > "$BASE/Converters/ValueConverters.cs" << 'EOF'
using System.Globalization;

namespace MultiServices.Maui.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return Color.FromArgb("#10B981");
        return Color.FromArgb("#EF4444");
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}

public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Pending" or "Reserved" => Color.FromArgb("#F59E0B"),
            "Confirmed" => Color.FromArgb("#3B82F6"),
            "Preparing" or "InProgress" => Color.FromArgb("#8B5CF6"),
            "Ready" => Color.FromArgb("#06B6D4"),
            "InTransit" or "EnRoute" => Color.FromArgb("#F97316"),
            "Delivered" or "Completed" => Color.FromArgb("#10B981"),
            "Cancelled" => Color.FromArgb("#EF4444"),
            "Refunded" => Color.FromArgb("#6B7280"),
            _ => Color.FromArgb("#6B7280")
        };
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class StatusToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Pending" => "En attente",
            "Confirmed" => "Confirmée",
            "Preparing" => "En préparation",
            "Ready" => "Prête",
            "InTransit" or "EnRoute" => "En route",
            "OnSite" => "Sur place",
            "InProgress" => "En cours",
            "Delivered" or "Completed" => "Terminée",
            "Cancelled" => "Annulée",
            "Refunded" => "Remboursée",
            "Reserved" => "Réservée",
            _ => value?.ToString() ?? ""
        };
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class RatingToStarsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double rating)
        {
            int full = (int)rating;
            return new string('★', full) + new string('☆', 5 - full);
        }
        return "☆☆☆☆☆";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class CurrencyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal d) return $"{d:N2} DH";
        return "0.00 DH";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class DateTimeToRelativeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            var diff = DateTime.Now - dt;
            if (diff.TotalMinutes < 1) return "À l'instant";
            if (diff.TotalMinutes < 60) return $"Il y a {(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24) return $"Il y a {(int)diff.TotalHours}h";
            if (diff.TotalDays < 7) return $"Il y a {(int)diff.TotalDays}j";
            return dt.ToString("dd/MM/yyyy");
        }
        return "";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class NullToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value != null;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}
EOF

echo "✅ Converters created"

# ============================================================
# 5. HELPERS
# ============================================================
cat > "$BASE/Helpers/Constants.cs" << 'EOF'
namespace MultiServices.Maui.Helpers;

public static class AppConstants
{
    public const string ApiBaseUrl = "https://api.multiservices.ma/api/v1";
    public const string SignalRHubUrl = "https://api.multiservices.ma/hubs/tracking";

    public static class Colors
    {
        public const string Primary = "#6366F1";
        public const string Secondary = "#8B5CF6";
        public const string Success = "#10B981";
        public const string Warning = "#F59E0B";
        public const string Danger = "#EF4444";
        public const string Info = "#3B82F6";
        public const string Dark = "#1F2937";
        public const string Light = "#F9FAFB";
        public const string Restaurant = "#F59E0B";
        public const string Service = "#3B82F6";
        public const string Grocery = "#10B981";
    }

    public static class CuisineTypes
    {
        public static readonly string[] All = { "Marocain", "Italien", "Asiatique", "Burger", "Pizza", "Tacos", "Sushi", "Indien", "Turc", "Français" };
    }

    public static class ServiceCategories
    {
        public static readonly (string Key, string Label, string Icon)[] All =
        {
            ("Plomberie", "Plomberie", "🔧"),
            ("Electricite", "Électricité", "⚡"),
            ("Menage", "Ménage", "🧹"),
            ("Peinture", "Peinture", "🎨"),
            ("Jardinage", "Jardinage", "🌱"),
            ("Climatisation", "Climatisation", "❄️"),
            ("Demenagement", "Déménagement", "📦"),
            ("Reparation", "Réparation", "🔨")
        };
    }

    public static class GroceryBrands
    {
        public static readonly string[] All = { "Marjane", "Carrefour", "Aswak Assalam", "Acima", "Label'Vie" };
    }
}
EOF

echo "✅ Helpers created"

echo ""
echo "========================================="
echo "✅ MAUI Core files generated successfully"
echo "========================================="
