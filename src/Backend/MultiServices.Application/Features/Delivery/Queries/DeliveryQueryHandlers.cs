using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Delivery;
using MultiServices.Application.Features.Delivery.Queries;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Delivery.Handlers;

public class GetDriverProfileQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetDriverProfileQuery, ApiResponse<DeliveryDriverDto>>
{
    public async Task<ApiResponse<DeliveryDriverDto>> Handle(GetDriverProfileQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var d = await db.Deliverers.FirstOrDefaultAsync(x => x.UserId == userId, ct);
        if (d == null) return ApiResponse<DeliveryDriverDto>.Fail("Livreur introuvable");
        return ApiResponse<DeliveryDriverDto>.Ok(new DeliveryDriverDto(d.Id,
            d.FirstName + " " + d.LastName, d.PhotoUrl, d.AverageRating,
            d.TotalDeliveries, d.TotalEarnings,
            d.Status != DelivererStatus.Offline, d.Status == DelivererStatus.Online,
            d.VehicleType.ToString()));
    }
}

public class GetAvailableDeliveriesQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetAvailableDeliveriesQuery, ApiResponse<List<AvailableDeliveryDto>>>
{
    public async Task<ApiResponse<List<AvailableDeliveryDto>>> Handle(GetAvailableDeliveriesQuery r, CancellationToken ct)
    {
        var result = new List<AvailableDeliveryDto>();
        // Restaurant orders ready for pickup
        var restOrders = await db.RestaurantOrders.Include(o => o.Restaurant)
            .Where(o => o.Status == RestaurantOrderStatus.Ready && o.DelivererId == null)
            .Take(20).ToListAsync(ct);
        foreach (var o in restOrders)
            result.Add(new AvailableDeliveryDto(o.Id, "Restaurant",
                $"{o.Restaurant.Street}, {o.Restaurant.City}",
                $"{o.DeliveryStreet}, {o.DeliveryCity}", o.DeliveryFee, 0,
                o.Restaurant.Name, null));

        // Grocery orders ready for pickup
        var grocOrders = await db.GroceryOrders.Include(o => o.Store)
            .Where(o => o.Status == GroceryOrderStatus.Ready && o.DelivererId == null)
            .Take(20).ToListAsync(ct);
        foreach (var o in grocOrders)
            result.Add(new AvailableDeliveryDto(o.Id, "Grocery",
                $"{o.Store.Street}, {o.Store.City}",
                $"{o.DeliveryStreet}, {o.DeliveryCity}", o.DeliveryFee, 0,
                null, o.Store.Name));

        return ApiResponse<List<AvailableDeliveryDto>>.Ok(result);
    }
}

public class GetDriverEarningsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetDriverEarningsQuery, ApiResponse<List<DeliveryEarningDto>>>
{
    public async Task<ApiResponse<List<DeliveryEarningDto>>> Handle(GetDriverEarningsQuery r, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var deliverer = await db.Deliverers.FirstOrDefaultAsync(d => d.UserId == userId, ct);
        if (deliverer == null) return ApiResponse<List<DeliveryEarningDto>>.Fail("Livreur introuvable");
        var q = db.DelivererEarnings.Where(e => e.DelivererId == deliverer.Id).AsQueryable();
        if (r.From.HasValue) q = q.Where(e => e.CreatedAt >= r.From.Value);
        if (r.To.HasValue) q = q.Where(e => e.CreatedAt <= r.To.Value.AddDays(1));
        var earnings = await q.OrderByDescending(e => e.CreatedAt).ToListAsync(ct);
        var dtos = earnings.Select(e => new DeliveryEarningDto(e.Id, e.BaseFee, e.TipAmount,
            e.BonusAmount, e.TotalEarning, e.OrderType, e.CreatedAt, false)).ToList();
        return ApiResponse<List<DeliveryEarningDto>>.Ok(dtos);
    }
}

public class GetDriverStatsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetDriverStatsQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetDriverStatsQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var d = await db.Deliverers.FirstOrDefaultAsync(x => x.UserId == userId, ct);
        if (d == null) return ApiResponse<object>.Fail("Livreur introuvable");
        var todayEarnings = await db.DelivererEarnings.Where(e => e.DelivererId == d.Id && e.CreatedAt.Date == DateTime.UtcNow.Date)
            .SumAsync(e => e.TotalEarning, ct);
        var weekEarnings = await db.DelivererEarnings.Where(e => e.DelivererId == d.Id && e.CreatedAt >= DateTime.UtcNow.AddDays(-7))
            .SumAsync(e => e.TotalEarning, ct);
        var pendingPayout = await db.DelivererPayouts.Where(p => p.DelivererId == d.Id && p.Status == "Pending")
            .SumAsync(p => p.Amount, ct);
        return ApiResponse<object>.Ok(new
        {
            d.TotalDeliveries, d.TotalEarnings, d.AverageRating, d.AcceptanceRate,
            TodayEarnings = todayEarnings, WeekEarnings = weekEarnings, PendingPayout = pendingPayout
        });
    }
}

public class GetActiveDeliveryQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetActiveDeliveryQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetActiveDeliveryQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        // Check restaurant orders in transit
        var restOrder = await db.RestaurantOrders.Include(o => o.Restaurant)
            .FirstOrDefaultAsync(o => o.DelivererId == userId && o.Status == RestaurantOrderStatus.InTransit, ct);
        if (restOrder != null)
            return ApiResponse<object>.Ok(new
            {
                Type = "Restaurant", OrderId = restOrder.Id, restOrder.OrderNumber,
                PickupAddress = $"{restOrder.Restaurant.Street}, {restOrder.Restaurant.City}",
                DeliveryAddress = $"{restOrder.DeliveryStreet}, {restOrder.DeliveryCity}",
                restOrder.Restaurant.Name, Status = restOrder.Status.ToString()
            });
        // Check grocery orders in transit
        var grocOrder = await db.GroceryOrders.Include(o => o.Store)
            .FirstOrDefaultAsync(o => o.DelivererId == userId && o.Status == GroceryOrderStatus.InTransit, ct);
        if (grocOrder != null)
            return ApiResponse<object>.Ok(new
            {
                Type = "Grocery", OrderId = grocOrder.Id, grocOrder.OrderNumber,
                PickupAddress = $"{grocOrder.Store.Street}, {grocOrder.Store.City}",
                DeliveryAddress = $"{grocOrder.DeliveryStreet}, {grocOrder.DeliveryCity}",
                grocOrder.Store.Name, Status = grocOrder.Status.ToString()
            });
        return ApiResponse<object>.Ok(null!);
    }
}
