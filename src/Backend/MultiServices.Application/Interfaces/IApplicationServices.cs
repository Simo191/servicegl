using MultiServices.Application.DTOs.Auth;
using MultiServices.Application.DTOs.Common;
using MultiServices.Application.DTOs.Restaurant;
using MultiServices.Application.DTOs.Service;
using MultiServices.Application.DTOs.Grocery;
using MultiServices.Application.DTOs.Admin;
using MultiServices.Domain.Enums;
using MultiServices.Application.Common.Models;

namespace MultiServices.Application.Interfaces;

// ==================== AUTH SERVICE ====================
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> SocialLoginAsync(SocialLoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
    Task<ApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default);
    Task<ApiResponse> VerifyPhoneAsync(VerifyPhoneRequest request, CancellationToken ct = default);
    Task<ApiResponse> SendPhoneVerificationAsync(string phoneNumber, CancellationToken ct = default);
    Task<ApiResponse> Enable2FAAsync(Guid userId, Enable2FARequest request, CancellationToken ct = default);
    Task<AuthResponse> Verify2FAAsync(Guid userId, Verify2FARequest request, CancellationToken ct = default);
    Task<ApiResponse> LogoutAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse> DeleteAccountAsync(Guid userId, string password, CancellationToken ct = default);
}

// ==================== PROFILE SERVICE ====================
public interface IProfileService
{
    Task<ApiResponse<UserDto>> GetProfileAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse<UserDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default);
    Task<ApiResponse<string>> UploadProfileImageAsync(Guid userId, Stream imageStream, string fileName, CancellationToken ct = default);
    Task<ApiResponse<List<AddressDto>>> GetAddressesAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse<AddressDto>> AddAddressAsync(Guid userId, CreateAddressRequest request, CancellationToken ct = default);
    Task<ApiResponse<AddressDto>> UpdateAddressAsync(Guid userId, Guid addressId, CreateAddressRequest request, CancellationToken ct = default);
    Task<ApiResponse> DeleteAddressAsync(Guid userId, Guid addressId, CancellationToken ct = default);
    Task<ApiResponse<List<NotificationDto>>> GetNotificationsAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<ApiResponse> MarkNotificationReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default);
    Task<ApiResponse> MarkAllNotificationsReadAsync(Guid userId, CancellationToken ct = default);
}

// ==================== RESTAURANT SERVICE ====================
public interface IRestaurantService
{
    Task<ApiResponse<PagedResult<RestaurantListDto>>> SearchRestaurantsAsync(RestaurantSearchRequest request, CancellationToken ct = default);
    Task<ApiResponse<RestaurantDto>> GetRestaurantAsync(Guid id, double? userLat = null, double? userLon = null, CancellationToken ct = default);
    Task<ApiResponse<RestaurantDto>> GetRestaurantBySlugAsync(string slug, CancellationToken ct = default);
    Task<ApiResponse<List<MenuCategoryDto>>> GetMenuAsync(Guid restaurantId, CancellationToken ct = default);
    Task<ApiResponse<MenuItemDto>> GetMenuItemAsync(Guid menuItemId, CancellationToken ct = default);
    Task<ApiResponse<List<RestaurantListDto>>> GetFavoriteRestaurantsAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse> ToggleFavoriteAsync(Guid userId, Guid restaurantId, CancellationToken ct = default);
    Task<ApiResponse<List<RestaurantListDto>>> GetRecommendedRestaurantsAsync(Guid userId, double lat, double lon, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<ReviewDto>>> GetReviewsAsync(Guid restaurantId, int page = 1, int pageSize = 20, CancellationToken ct = default);
}

// ==================== RESTAURANT ORDER SERVICE ====================
public interface IRestaurantOrderService
{
    Task<ApiResponse<RestaurantOrderDto>> CreateOrderAsync(Guid userId, CreateRestaurantOrderRequest request, CancellationToken ct = default);
    Task<ApiResponse<RestaurantOrderDto>> GetOrderAsync(Guid userId, Guid orderId, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<RestaurantOrderDto>>> GetOrdersAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<ApiResponse> CancelOrderAsync(Guid userId, Guid orderId, string reason, CancellationToken ct = default);
    Task<ApiResponse> RateOrderAsync(Guid userId, Guid orderId, int restaurantRating, int deliveryRating, string? comment, CancellationToken ct = default);
    Task<ApiResponse<RestaurantOrderDto>> ReorderAsync(Guid userId, Guid previousOrderId, CancellationToken ct = default);
}

// ==================== RESTAURANT MANAGEMENT SERVICE ====================
public interface IRestaurantManagementService
{
    Task<ApiResponse<RestaurantDto>> CreateRestaurantAsync(Guid ownerId, CreateRestaurantRequest request, CancellationToken ct = default);
    Task<ApiResponse<RestaurantDto>> UpdateRestaurantAsync(Guid ownerId, Guid restaurantId, UpdateRestaurantRequest request, CancellationToken ct = default);
    Task<ApiResponse<string>> UploadImageAsync(Guid restaurantId, Stream imageStream, string fileName, string type, CancellationToken ct = default);
    Task<ApiResponse> ToggleOpenStatusAsync(Guid restaurantId, CancellationToken ct = default);
    // Menu management
    Task<ApiResponse<MenuCategoryDto>> CreateMenuCategoryAsync(Guid restaurantId, CreateMenuCategoryRequest request, CancellationToken ct = default);
    Task<ApiResponse<MenuCategoryDto>> UpdateMenuCategoryAsync(Guid restaurantId, Guid categoryId, CreateMenuCategoryRequest request, CancellationToken ct = default);
    Task<ApiResponse> DeleteMenuCategoryAsync(Guid restaurantId, Guid categoryId, CancellationToken ct = default);
    Task<ApiResponse<MenuItemDto>> CreateMenuItemAsync(Guid restaurantId, CreateMenuItemRequest request, CancellationToken ct = default);
    Task<ApiResponse<MenuItemDto>> UpdateMenuItemAsync(Guid restaurantId, Guid itemId, CreateMenuItemRequest request, CancellationToken ct = default);
    Task<ApiResponse> DeleteMenuItemAsync(Guid restaurantId, Guid itemId, CancellationToken ct = default);
    Task<ApiResponse> ToggleMenuItemAvailabilityAsync(Guid restaurantId, Guid itemId, CancellationToken ct = default);
    // Order management
    Task<ApiResponse<PagedResult<RestaurantOrderDto>>> GetRestaurantOrdersAsync(Guid restaurantId, AdminSearchRequest request, CancellationToken ct = default);
    Task<ApiResponse> AcceptOrderAsync(Guid restaurantId, Guid orderId, int estimatedMinutes, CancellationToken ct = default);
    Task<ApiResponse> RejectOrderAsync(Guid restaurantId, Guid orderId, string reason, CancellationToken ct = default);
    Task<ApiResponse> UpdateOrderStatusAsync(Guid restaurantId, Guid orderId, RestaurantOrderStatus status, CancellationToken ct = default);
    // Promotions
    Task<ApiResponse> CreatePromotionAsync(Guid restaurantId, CreatePromotionRequest request, CancellationToken ct = default);
    // Stats
    Task<ApiResponse<object>> GetStatisticsAsync(Guid restaurantId, DateTime from, DateTime to, CancellationToken ct = default);
}

// ==================== SERVICE PROVIDER SERVICE ====================
public interface IServiceProviderService
{
    Task<ApiResponse<PagedResult<ServiceProviderListDto>>> SearchProvidersAsync(ServiceSearchRequest request, CancellationToken ct = default);
    Task<ApiResponse<ServiceProviderDto>> GetProviderAsync(Guid id, double? userLat = null, double? userLon = null, CancellationToken ct = default);
    Task<ApiResponse<List<AvailableSlotDto>>> GetAvailableSlotsAsync(Guid providerId, DateTime date, Guid serviceId, CancellationToken ct = default);
    Task<ApiResponse<List<ServiceProviderListDto>>> GetFavoriteProvidersAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse> ToggleFavoriteAsync(Guid userId, Guid providerId, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<ReviewDto>>> GetReviewsAsync(Guid providerId, int page, int pageSize, CancellationToken ct = default);
}

// ==================== INTERVENTION SERVICE ====================
public interface IInterventionService
{
    Task<ApiResponse<InterventionDto>> CreateInterventionAsync(Guid userId, CreateInterventionRequest request, CancellationToken ct = default);
    Task<ApiResponse<InterventionDto>> GetInterventionAsync(Guid userId, Guid interventionId, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<InterventionDto>>> GetInterventionsAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<ApiResponse> CancelInterventionAsync(Guid userId, Guid interventionId, string reason, CancellationToken ct = default);
    Task<ApiResponse> AcceptQuoteAsync(Guid userId, Guid interventionId, CancellationToken ct = default);
    Task<ApiResponse> RejectQuoteAsync(Guid userId, Guid interventionId, CancellationToken ct = default);
    Task<ApiResponse> RateInterventionAsync(Guid userId, Guid interventionId, int providerRating, int? intervenantRating, string? comment, CancellationToken ct = default);
}

// ==================== SERVICE PROVIDER MANAGEMENT ====================
public interface IServiceProviderManagementService
{
    Task<ApiResponse<ServiceProviderDto>> CreateProviderAsync(Guid ownerId, CreateServiceProviderRequest request, CancellationToken ct = default);
    Task<ApiResponse<ServiceProviderDto>> UpdateProviderAsync(Guid ownerId, Guid providerId, CreateServiceProviderRequest request, CancellationToken ct = default);
    Task<ApiResponse> CreateServiceOfferingAsync(Guid providerId, ServiceOfferingDto request, CancellationToken ct = default);
    Task<ApiResponse> UpdateAvailabilityAsync(Guid providerId, List<AvailableSlotDto> slots, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<InterventionDto>>> GetInterventionsAsync(Guid providerId, AdminSearchRequest request, CancellationToken ct = default);
    Task<ApiResponse> AcceptInterventionAsync(Guid providerId, Guid interventionId, CancellationToken ct = default);
    Task<ApiResponse> RejectInterventionAsync(Guid providerId, Guid interventionId, string reason, CancellationToken ct = default);
    Task<ApiResponse> SendQuoteAsync(Guid providerId, SendQuoteRequest request, CancellationToken ct = default);
    Task<ApiResponse> UpdateInterventionStatusAsync(Guid providerId, Guid interventionId, InterventionUpdateRequest request, CancellationToken ct = default);
    Task<ApiResponse> AssignTeamMemberAsync(Guid providerId, Guid interventionId, Guid teamMemberId, CancellationToken ct = default);
    Task<ApiResponse<object>> GetStatisticsAsync(Guid providerId, DateTime from, DateTime to, CancellationToken ct = default);
}

// ==================== GROCERY SERVICE ====================
public interface IGroceryService
{
    Task<ApiResponse<PagedResult<GroceryStoreListDto>>> GetStoresAsync(double? lat, double? lon, string? query, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<ApiResponse<GroceryStoreDto>> GetStoreAsync(Guid storeId, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<GroceryProductListDto>>> SearchProductsAsync(Guid storeId, GrocerySearchRequest request, CancellationToken ct = default);
    Task<ApiResponse<GroceryProductDto>> GetProductAsync(Guid productId, CancellationToken ct = default);
    Task<ApiResponse<List<GroceryProductListDto>>> ScanBarcodeAsync(string barcode, Guid storeId, CancellationToken ct = default);
    Task<ApiResponse<List<GroceryStoreListDto>>> GetFavoriteStoresAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse> ToggleFavoriteAsync(Guid userId, Guid storeId, CancellationToken ct = default);
}

// ==================== GROCERY ORDER SERVICE ====================
public interface IGroceryOrderService
{
    Task<ApiResponse<GroceryOrderDto>> CreateOrderAsync(Guid userId, CreateGroceryOrderRequest request, CancellationToken ct = default);
    Task<ApiResponse<GroceryOrderDto>> GetOrderAsync(Guid userId, Guid orderId, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<GroceryOrderDto>>> GetOrdersAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<ApiResponse> CancelOrderAsync(Guid userId, Guid orderId, string reason, CancellationToken ct = default);
    Task<ApiResponse> AcceptReplacementAsync(Guid userId, Guid orderId, Guid itemId, bool accept, CancellationToken ct = default);
    Task<ApiResponse> ReportIssueAsync(Guid userId, Guid orderId, string issue, List<Guid>? affectedItemIds, CancellationToken ct = default);
    Task<ApiResponse> RateOrderAsync(Guid userId, Guid orderId, int storeRating, int deliveryRating, int? freshnessRating, string? comment, CancellationToken ct = default);
    Task<ApiResponse<GroceryOrderDto>> DuplicateOrderAsync(Guid userId, Guid previousOrderId, CancellationToken ct = default);
}

// ==================== SHOPPING LIST SERVICE ====================
public interface IShoppingListService
{
    Task<ApiResponse<List<ShoppingListDto>>> GetListsAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse<ShoppingListDto>> CreateListAsync(Guid userId, CreateShoppingListRequest request, CancellationToken ct = default);
    Task<ApiResponse<ShoppingListDto>> GetListAsync(Guid userId, Guid listId, CancellationToken ct = default);
    Task<ApiResponse> AddItemToListAsync(Guid userId, Guid listId, string name, int quantity, Guid? productId, CancellationToken ct = default);
    Task<ApiResponse> RemoveItemFromListAsync(Guid userId, Guid listId, Guid itemId, CancellationToken ct = default);
    Task<ApiResponse> ToggleItemCheckedAsync(Guid userId, Guid listId, Guid itemId, CancellationToken ct = default);
    Task<ApiResponse> DeleteListAsync(Guid userId, Guid listId, CancellationToken ct = default);
    Task<ApiResponse<string>> ShareListAsync(Guid userId, Guid listId, CancellationToken ct = default);
    Task<ApiResponse> ConvertListToCartAsync(Guid userId, Guid listId, Guid storeId, CancellationToken ct = default);
}

// ==================== GROCERY STORE MANAGEMENT ====================
public interface IGroceryStoreManagementService
{
    Task<ApiResponse> ImportProductsAsync(Guid storeId, Stream fileStream, string format, CancellationToken ct = default);
    Task<ApiResponse> UpdateStockAsync(Guid storeId, List<StockUpdateRequest> updates, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<GroceryOrderDto>>> GetOrdersAsync(Guid storeId, AdminSearchRequest request, CancellationToken ct = default);
    Task<ApiResponse> AcceptOrderAsync(Guid storeId, Guid orderId, CancellationToken ct = default);
    Task<ApiResponse> SuggestReplacementAsync(Guid storeId, ReplacementSuggestionRequest request, CancellationToken ct = default);
    Task<ApiResponse> MarkOrderReadyAsync(Guid storeId, Guid orderId, CancellationToken ct = default);
    Task<ApiResponse<object>> GetStatisticsAsync(Guid storeId, DateTime from, DateTime to, CancellationToken ct = default);
}

// ==================== DELIVERER SERVICE ====================
public interface IDelivererService
{
    Task<ApiResponse> GoOnlineAsync(Guid delivererId, CancellationToken ct = default);
    Task<ApiResponse> GoOfflineAsync(Guid delivererId, CancellationToken ct = default);
    Task<ApiResponse> UpdateLocationAsync(Guid delivererId, double lat, double lon, CancellationToken ct = default);
    Task<ApiResponse<List<object>>> GetAvailableOrdersAsync(Guid delivererId, CancellationToken ct = default);
    Task<ApiResponse> AcceptOrderAsync(Guid delivererId, string orderType, Guid orderId, CancellationToken ct = default);
    Task<ApiResponse> RejectOrderAsync(Guid delivererId, string orderType, Guid orderId, CancellationToken ct = default);
    Task<ApiResponse> UpdateDeliveryStatusAsync(Guid delivererId, string orderType, Guid orderId, string status, CancellationToken ct = default);
    Task<ApiResponse> UploadDeliveryProofAsync(Guid delivererId, Guid orderId, Stream imageStream, CancellationToken ct = default);
    Task<ApiResponse<object>> GetEarningsAsync(Guid delivererId, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<ApiResponse> RequestPayoutAsync(Guid delivererId, decimal amount, CancellationToken ct = default);
    Task<ApiResponse> TriggerSOSAsync(Guid delivererId, double lat, double lon, CancellationToken ct = default);
}

// ==================== ADMIN SERVICE ====================
public interface IAdminService
{
    Task<ApiResponse<DashboardDto>> GetDashboardAsync(CancellationToken ct = default);
    Task<ApiResponse<PagedResult<AdminUserListDto>>> GetUsersAsync(AdminSearchRequest request, CancellationToken ct = default);
    Task<ApiResponse> SuspendUserAsync(Guid userId, string reason, CancellationToken ct = default);
    Task<ApiResponse> ActivateUserAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse> ApproveProviderAsync(string providerType, Guid providerId, CancellationToken ct = default);
    Task<ApiResponse> RejectProviderAsync(string providerType, Guid providerId, string reason, CancellationToken ct = default);
    Task<ApiResponse<PagedResult<AdminOrderListDto>>> GetAllOrdersAsync(AdminSearchRequest request, CancellationToken ct = default);
    Task<ApiResponse> CancelOrderAsync(string orderType, Guid orderId, string reason, CancellationToken ct = default);
    Task<ApiResponse> ReassignDelivererAsync(string orderType, Guid orderId, Guid newDelivererId, CancellationToken ct = default);
    Task<ApiResponse<AdminFinanceDto>> GetFinancesAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<ApiResponse> UpdateCommissionRateAsync(string module, decimal rate, CancellationToken ct = default);
    Task<ApiResponse> CreateGlobalPromoAsync(CreatePromotionRequest request, CancellationToken ct = default);
    Task<ApiResponse> SendCampaignAsync(string type, string subject, string content, string? targetAudience, CancellationToken ct = default);
    Task<byte[]> ExportFinanceReportAsync(DateTime from, DateTime to, string format, CancellationToken ct = default);
}

// ==================== PAYMENT SERVICE ====================
public interface IPaymentAppService
{
    Task<ApiResponse<object>> CreatePaymentIntentAsync(Guid userId, string orderType, Guid orderId, CancellationToken ct = default);
    Task<ApiResponse> ConfirmPaymentAsync(string paymentIntentId, CancellationToken ct = default);
    Task<ApiResponse> RefundAsync(Guid userId, string orderType, Guid orderId, decimal? amount, string reason, CancellationToken ct = default);
    Task<ApiResponse<object>> GetWalletAsync(Guid userId, CancellationToken ct = default);
    Task<ApiResponse> TopUpWalletAsync(Guid userId, decimal amount, string paymentMethodId, CancellationToken ct = default);
}
