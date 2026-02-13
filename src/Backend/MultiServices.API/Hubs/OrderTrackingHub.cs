using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MultiServices.API.Hubs;

[Authorize]
public class OrderTrackingHub : Hub
{
    public async Task JoinOrderGroup(string orderId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"order_{orderId}");

    public async Task LeaveOrderGroup(string orderId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order_{orderId}");

    public async Task SendOrderStatusUpdate(string orderId, string status, object? data = null)
        => await Clients.Group($"order_{orderId}").SendAsync("OrderStatusUpdated", new { orderId, status, data });

    public async Task SendDriverLocation(string orderId, double lat, double lng)
        => await Clients.Group($"order_{orderId}").SendAsync("DriverLocationUpdated", new { orderId, lat, lng });
}

[Authorize]
public class DeliveryTrackingHub : Hub
{
    public async Task JoinDeliveryGroup(string deliveryId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"delivery_{deliveryId}");

    public async Task UpdateLocation(double lat, double lng)
        => await Clients.All.SendAsync("LocationUpdated", new { driverId = Context.UserIdentifier, lat, lng });
}

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await base.OnConnectedAsync();
    }

    public async Task SendNotification(string userId, string title, string body, object? data = null)
        => await Clients.Group($"user_{userId}").SendAsync("NewNotification", new { title, body, data });
}
