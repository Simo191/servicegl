using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Admin;

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

public class RevenueChartData { public string Date { get; set; } = ""; public decimal Restaurant { get; set; } public decimal Service { get; set; } public decimal Grocery { get; set; } }
public class OrderChartData { public string Date { get; set; } = ""; public int Restaurant { get; set; } public int Service { get; set; } public int Grocery { get; set; } }
public class ModuleStats { public int TotalOrders { get; set; } public decimal Revenue { get; set; } public decimal Commission { get; set; } public double AvgRating { get; set; } }

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

public class AdminOrderListDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string OrderType { get; set; } = ""; // Restaurant, Service, Grocery
    public string CustomerName { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public string? DelivererName { get; set; }
    public string Status { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

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

public class FinanceChartData { public string Month { get; set; } = ""; public decimal Revenue { get; set; } public decimal Commissions { get; set; } public decimal Payouts { get; set; } }

public class CommissionConfigDto
{
    public string Module { get; set; } = "";
    public decimal Rate { get; set; }
}

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
