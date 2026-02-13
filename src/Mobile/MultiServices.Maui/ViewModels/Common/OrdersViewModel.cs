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
            var status = ActiveTab == "active" ? "active" : "completed";
            var result = await _api.GetAsync<List<OrderSummary>>("/orders", new Dictionary<string, string> { ["status"] = status });
            if (result.Success && result.Data != null)
            {
                Orders = new ObservableCollection<OrderSummary>(result.Data);
                IsEmpty = !Orders.Any();
            }
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
            "grocery" => $"groceryordertracking?id={order.Id}",
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
