using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Api;

namespace MultiServices.Maui.ViewModels.Common;

public partial class OrdersViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] private ObservableCollection<OrderSummary> _orders = new();
    [ObservableProperty] private string _activeTab = "active";

    public OrdersViewModel(ApiService api)
    {
        _api = api;
        Title = "Mes commandes";
    }

    [RelayCommand]
    private async Task LoadOrdersAsync()
    {
        await ExecuteAsync(async () =>
        {
            // FIX: Backend has NO unified /orders endpoint.
            // Must query 3 separate endpoints and merge results:
            // - /restaurant-orders/my-orders?status=...
            // - /services/bookings/my-bookings?status=...
            // - /grocery/orders/my-orders?status=...
            var status = ActiveTab == "active" ? "active" : "completed";
            var allOrders = new List<OrderSummary>();

            // Restaurant orders
            var restaurantTask = _api.GetAsync<PaginatedResult<RestaurantOrderDto>>(
                "/restaurant-orders/my-orders",
                new Dictionary<string, string> { ["status"] = status, ["page"] = "1", ["pageSize"] = "10" });

            // Service bookings
            var serviceTask = _api.GetAsync<PaginatedResult<InterventionDto>>(
                "/services/bookings/my-bookings",
                new Dictionary<string, string> { ["status"] = status, ["page"] = "1", ["pageSize"] = "10" });

            // Grocery orders
            var groceryTask = _api.GetAsync<PaginatedResult<GroceryOrderDto>>(
                "/grocery/orders/my-orders",
                new Dictionary<string, string> { ["status"] = status, ["page"] = "1", ["pageSize"] = "10" });

            await Task.WhenAll(restaurantTask, serviceTask, groceryTask);

            var restaurantResult = await restaurantTask;
            if (restaurantResult.Success && restaurantResult.Data != null)
            {
                allOrders.AddRange(restaurantResult.Data.Items.Select(o => new OrderSummary
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    Type = "restaurant",
                    ProviderName = o.RestaurantName,
                    ProviderLogoUrl = o.RestaurantLogoUrl,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt
                }));
            }

            var serviceResult = await serviceTask;
            if (serviceResult.Success && serviceResult.Data != null)
            {
                allOrders.AddRange(serviceResult.Data.Items.Select(o => new OrderSummary
                {
                    Id = o.Id,
                    OrderNumber = o.InterventionNumber,
                    Type = "service",
                    ProviderName = o.ProviderName,
                    ProviderLogoUrl = o.ProviderLogoUrl,
                    Status = o.Status,
                    TotalAmount = o.FinalPrice ?? o.EstimatedPrice,
                    CreatedAt = o.CreatedAt
                }));
            }

            var groceryResult = await groceryTask;
            if (groceryResult.Success && groceryResult.Data != null)
            {
                allOrders.AddRange(groceryResult.Data.Items.Select(o => new OrderSummary
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    Type = "grocery",
                    ProviderName = o.StoreName,
                    ProviderLogoUrl = o.StoreLogoUrl,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt
                }));
            }

            // Sort by most recent
            Orders = new ObservableCollection<OrderSummary>(
                allOrders.OrderByDescending(o => o.CreatedAt));
            IsEmpty = !Orders.Any();
        });
    }

    [RelayCommand]
    private async Task SetTabAsync(string tab)
    {
        ActiveTab = tab;
        await LoadOrdersAsync();
    }

    [RelayCommand]
    private async Task ViewOrderAsync(OrderSummary order)
    {
        var route = order.Type switch
        {
            "restaurant" => $"restaurantordertracking?id={order.Id}",
            "service" => $"interventiontracking?id={order.Id}",
            "grocery" => $"grocerycheckout?id={order.Id}",  // FIX: groceryordertracking doesn't exist in routes
            _ => ""
        };
        if (!string.IsNullOrEmpty(route))
            await Shell.Current.GoToAsync(route);
    }
}

public class OrderSummary
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderLogoUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TypeIcon => Type switch
    {
        "restaurant" => "🍔",
        "service" => "🛠️",
        "grocery" => "🛒",
        _ => "📦"
    };
    public Color TypeColor => Type switch
    {
        "restaurant" => Color.FromArgb("#F59E0B"),
        "service" => Color.FromArgb("#3B82F6"),
        "grocery" => Color.FromArgb("#10B981"),
        _ => Color.FromArgb("#6B7280")
    };
}
