using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Admin;

//public record DashboardDto(int TotalUsers, int TotalRestaurants, int TotalStores, int TotalProviders, int TotalDrivers,
//    decimal TotalRevenue, decimal RestaurantRevenue, decimal GroceryRevenue, decimal ServiceRevenue, int PendingApprovals);
//public record ApprovalDto(Guid EntityId, string EntityType, string Name, DateTime CreatedAt);
//public record UserListDto(Guid Id, string Email, string FirstName, string LastName, string? Phone, bool IsActive, DateTime CreatedAt);
//public record FinancialReportDto(decimal TotalRevenue, decimal TotalCommissions, decimal RestaurantRevenue, decimal GroceryRevenue, decimal ServiceRevenue, int TotalOrders, DateTime StartDate, DateTime EndDate);
//public record CommissionSettingsDto(string Module, decimal Rate);
//public record ApproveEntityDto(Guid EntityId, string EntityType);
//public record CreatePromotionDto(string Code, string DiscountType, decimal DiscountValue, decimal? MaxDiscount, int MaxUsage, DateTime ExpiresAt);
// These DTOs are referenced by AdminQueries/AdminCommands but were missing
public record ApprovalDto(Guid Id, string EntityType, string Name, string? OwnerName,
    string Status, DateTime CreatedAt);

public record UserListDto(Guid Id, string FullName, string Email, string? Phone,
    bool IsActive, bool IsVerified, List<string> Roles, DateTime CreatedAt, int TotalOrders);

public record FinancialReportDto(decimal TotalRevenue, decimal RestaurantRevenue,
    decimal GroceryRevenue, decimal ServiceRevenue, decimal TotalCommissions,
    decimal TotalRefunds, int TotalOrders, int TotalPaidOrders);

public record CommissionSettingsDto(string EntityType, Guid EntityId, decimal NewRate);

public record CreatePromotionDto(string Code, string Title, string? Description,
    string DiscountType, decimal DiscountValue, decimal? MinOrderAmount,
    decimal? MaxDiscount, DateTime StartDate, DateTime EndDate,
    int? MaxUsages, string ApplicableModule, bool FreeDelivery);

public record ApproveEntityDto(string EntityType, Guid EntityId, bool Approved, string? RejectionReason);

// Missing Grocery Store DTO for CreateStorePromotionCommand
public record StorePromotionDto(string Title, string? Description, string? Code,
    string DiscountType, decimal DiscountValue, DateTime StartDate, DateTime EndDate, bool FreeDelivery);
// ==================== DASHBOARD ====================
public class DashboardDto
{
    public int TotalOrders { get; set; }
    public int TotalOrdersToday { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RevenueToday { get; set; }
    public int ActiveClients { get; set; }
    public int ActiveRestaurants { get; set; }
    public int ActiveServiceProviders { get; set; }
    public int ActiveGroceryStores { get; set; }
    public int ActiveDeliverers { get; set; }
    public int PendingApprovals { get; set; }
    public int OpenTickets { get; set; }
    public List<RevenueChartData> RevenueChart { get; set; } = new();
    public List<OrderChartData> OrderChart { get; set; } = new();
    public ModuleStats RestaurantStats { get; set; } = new();
    public ModuleStats ServiceStats { get; set; } = new();
    public ModuleStats GroceryStats { get; set; } = new();
}

public class RevenueChartData
{
    public string Date { get; set; } = "";
    public decimal Restaurant { get; set; }
    public decimal Service { get; set; }
    public decimal Grocery { get; set; }
}

public class OrderChartData
{
    public string Date { get; set; } = "";
    public int Restaurant { get; set; }
    public int Service { get; set; }
    public int Grocery { get; set; }
}

public class ModuleStats
{
    public int TotalOrders { get; set; }
    public decimal Revenue { get; set; }
    public decimal Commission { get; set; }
    public double AvgRating { get; set; }
}

// ==================== USERS ====================
public class AdminUserListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int TotalOrders { get; set; }
}

/// <summary>Used by GetAllUsersQuery</summary>
//public class UserListDto
//{
//    public Guid Id { get; set; }
//    public string FullName { get; set; } = "";
//    public string Email { get; set; } = "";
//    public string? Phone { get; set; }
//    public bool IsActive { get; set; }
//    public bool IsVerified { get; set; }
//    public List<string> Roles { get; set; } = new();
//    public DateTime CreatedAt { get; set; }
//    public DateTime? LastLoginAt { get; set; }
//    public int TotalOrders { get; set; }
//    public decimal TotalSpent { get; set; }
//}

// ==================== ORDERS ====================
public class AdminOrderListDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string OrderType { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public string? DelivererName { get; set; }
    public string Status { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ==================== APPROVALS ====================
/// <summary>Used by GetPendingApprovalsQuery</summary>
//public class ApprovalDto
//{
//    public Guid EntityId { get; set; }
//    public string EntityType { get; set; } = "";
//    public string Name { get; set; } = "";
//    public string OwnerName { get; set; } = "";
//    public string? Email { get; set; }
//    public string? Phone { get; set; }
//    public string? City { get; set; }
//    public DateTime RequestedAt { get; set; }
//    public string Status { get; set; } = "Pending";
//    public List<string> Documents { get; set; } = new();
//}

///// <summary>Used by ApproveEntityCommand</summary>
//public class ApproveEntityDto
//{
//    public Guid EntityId { get; set; }
//    public string EntityType { get; set; } = "";
//    public bool Approved { get; set; }
//    public string? RejectionReason { get; set; }
//}

// ==================== FINANCE ====================
public class AdminFinanceDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalCommissions { get; set; }
    public decimal TotalPayoutsRestaurants { get; set; }
    public decimal TotalPayoutsServices { get; set; }
    public decimal TotalPayoutsGroceries { get; set; }
    public decimal TotalPayoutsDeliverers { get; set; }
    public decimal TotalRefunds { get; set; }
    public List<FinanceChartData> MonthlyData { get; set; } = new();
}

public class FinanceChartData
{
    public string Month { get; set; } = "";
    public decimal Revenue { get; set; }
    public decimal Commissions { get; set; }
    public decimal Payouts { get; set; }
}

/// <summary>Used by GetFinancialReportQuery</summary>
//public class FinancialReportDto
//{
//    public DateTime StartDate { get; set; }
//    public DateTime EndDate { get; set; }
//    public decimal TotalRevenue { get; set; }
//    public decimal TotalCommissions { get; set; }
//    public decimal TotalPayouts { get; set; }
//    public decimal TotalRefunds { get; set; }
//    public decimal NetProfit { get; set; }
//    public List<ModuleFinanceDto> ByModule { get; set; } = new();
//    public List<FinanceChartData> MonthlyData { get; set; } = new();
//}

public class ModuleFinanceDto
{
    public string Module { get; set; } = "";
    public decimal Revenue { get; set; }
    public decimal Commissions { get; set; }
    public decimal Payouts { get; set; }
    public int OrderCount { get; set; }
}

// ==================== COMMISSIONS ====================
public class CommissionConfigDto
{
    public string Module { get; set; } = "";
    public decimal Rate { get; set; }
}

/// <summary>Used by UpdateCommissionSettingsCommand</summary>
//public class CommissionSettingsDto
//{
//    public decimal RestaurantCommissionRate { get; set; }
//    public decimal ServiceCommissionRate { get; set; }
//    public decimal GroceryCommissionRate { get; set; }
//    public decimal DeliveryFeePerKm { get; set; }
//    public decimal DeliveryBaseFee { get; set; }
//}

// ==================== PROMOTIONS ====================
/// <summary>Used by CreateGlobalPromotionCommand</summary>
//public class CreatePromotionDto
//{
//    public string Code { get; set; } = "";
//    public string Title { get; set; } = "";
//    public string? Description { get; set; }
//    public string DiscountType { get; set; } = "Percentage";
//    public decimal DiscountValue { get; set; }
//    public decimal? MinOrderAmount { get; set; }
//    public decimal? MaxDiscount { get; set; }
//    public DateTime StartDate { get; set; }
//    public DateTime EndDate { get; set; }
//    public int? MaxUsages { get; set; }
//    public string ApplicableModule { get; set; } = "All";
//    public bool FreeDelivery { get; set; }
//}

public class CreatePromotionRequest
{
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? MaxUsages { get; set; }
    public string ApplicableModule { get; set; } = "All";
    public bool FreeDelivery { get; set; }
}

// ==================== SEARCH ====================
public class AdminSearchRequest
{
    public string? Query { get; set; }
    public string? Status { get; set; }
    public string? OrderType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

