using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Delivery;
using MultiServices.Application.Features.Delivery.Commands;
using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Delivery.Handlers;

public class RegisterDriverCommandHandler(ApplicationDbContext db)
    : IRequestHandler<RegisterDriverCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(RegisterDriverCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        VehicleType vt = VehicleType.Scooter;
        if (!string.IsNullOrEmpty(d.VehicleType)) Enum.TryParse<VehicleType>(d.VehicleType, out vt);
        var deliverer = new Deliverer
        {
            FirstName = d.FirstName, LastName = d.LastName, Phone = d.Phone,
            VehicleType = vt, VehiclePlateNumber = d.VehiclePlate,
            Status = DelivererStatus.Offline,
            VerificationStatus = VerificationStatus.Pending
        };
        await db.Deliverers.AddAsync(deliverer, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(deliverer.Id);
    }
}

public class GoOnlineCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GoOnlineCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(GoOnlineCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var deliverer = await db.Deliverers.FirstOrDefaultAsync(d => d.UserId == userId, ct);
        if (deliverer == null) return ApiResponse.Fail("Livreur introuvable");
        if (!deliverer.IsVerified) return ApiResponse.Fail("Compte non verifie");
        deliverer.Status = DelivererStatus.Online;
        deliverer.LastLocationUpdate = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("En ligne");
    }
}

public class GoOfflineCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GoOfflineCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(GoOfflineCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var deliverer = await db.Deliverers.FirstOrDefaultAsync(d => d.UserId == userId, ct);
        if (deliverer == null) return ApiResponse.Fail("Livreur introuvable");
        deliverer.Status = DelivererStatus.Offline;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Hors ligne");
    }
}

public class UpdateDriverLocationCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<UpdateDriverLocationCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateDriverLocationCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var deliverer = await db.Deliverers.FirstOrDefaultAsync(d => d.UserId == userId, ct);
        if (deliverer == null) return ApiResponse.Fail("Livreur introuvable");
        deliverer.CurrentLatitude = request.Dto.Latitude;
        deliverer.CurrentLongitude = request.Dto.Longitude;
        deliverer.LastLocationUpdate = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Position mise a jour");
    }
}

public class AcceptDeliveryCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<AcceptDeliveryCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(AcceptDeliveryCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var deliverer = await db.Deliverers.FirstOrDefaultAsync(d => d.UserId == userId, ct);
        if (deliverer == null) return ApiResponse.Fail("Livreur introuvable");
        deliverer.Status = DelivererStatus.Busy;

        if (request.Type == "Restaurant")
        {
            var order = await db.RestaurantOrders.FindAsync(new object[] { request.DeliveryId }, ct);
            if (order == null) return ApiResponse.Fail("Commande introuvable");
            order.DelivererId = userId;
            order.StatusHistory.Add(new Domain.Entities.Restaurant.OrderStatusHistory
            { Status = RestaurantOrderStatus.InTransit.ToString(), Note = $"Livreur: {deliverer.FirstName}" });
        }
        else if (request.Type == "Grocery")
        {
            var order = await db.GroceryOrders.FindAsync(new object[] { request.DeliveryId }, ct);
            if (order == null) return ApiResponse.Fail("Commande introuvable");
            order.DelivererId = userId;
            order.StatusHistory.Add(new Domain.Entities.Grocery.GroceryOrderStatusHistory
            { Status = GroceryOrderStatus.InTransit, Note = $"Livreur: {deliverer.FirstName}" });
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Livraison acceptee");
    }
}

public class RejectDeliveryCommandHandler(ApplicationDbContext db)
    : IRequestHandler<RejectDeliveryCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RejectDeliveryCommand request, CancellationToken ct)
    {
        // Just log rejection, order stays available for other drivers
        return await Task.FromResult(ApiResponse.Ok("Livraison refusee"));
    }
}

public class UpdateDeliveryStatusCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<UpdateDeliveryStatusCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateDeliveryStatusCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var deliverer = await db.Deliverers.FirstOrDefaultAsync(d => d.UserId == userId, ct);

        if (request.Type == "Restaurant")
        {
            var order = await db.RestaurantOrders.FindAsync(new object[] { request.DeliveryId }, ct);
            if (order == null) return ApiResponse.Fail("Commande introuvable");
            if (Enum.TryParse<RestaurantOrderStatus>(request.Dto.Status, out var rStatus))
            {
                order.Status = rStatus;
                if (rStatus == RestaurantOrderStatus.InTransit) order.PickedUpAt = DateTime.UtcNow;
                if (rStatus == RestaurantOrderStatus.Delivered)
                {
                    order.DeliveredAt = DateTime.UtcNow;
                    //order.DeliveryPhotoUrl = request.Dto.ProofImageUrl;
                    if (deliverer != null)
                    {
                        deliverer.Status = DelivererStatus.Online;
                        deliverer.TotalDeliveries++;
                        var earning = new DelivererEarning
                        {
                            DelivererId = deliverer.Id, OrderType = "Restaurant", OrderId = order.Id,
                            BaseFee = deliverer.DeliveryBaseFee, DistanceFee = 0,
                            TipAmount = order.TipAmount, BonusAmount = 0,
                            TotalEarning = deliverer.DeliveryBaseFee + order.TipAmount
                        };
                        await db.DelivererEarnings.AddAsync(earning, ct);
                        deliverer.TotalEarnings += earning.TotalEarning;
                    }
                }
                order.StatusHistory.Add(new Domain.Entities.Restaurant.OrderStatusHistory { Status = rStatus.ToString() });
            }
        }
        else if (request.Type == "Grocery")
        {
            var order = await db.GroceryOrders.FindAsync(new object[] { request.DeliveryId }, ct);
            if (order == null) return ApiResponse.Fail("Commande introuvable");
            if (Enum.TryParse<GroceryOrderStatus>(request.Dto.Status, out var gStatus))
            {
                order.Status = gStatus;
                if (gStatus == GroceryOrderStatus.InTransit) order.PickedUpAt = DateTime.UtcNow;
                if (gStatus == GroceryOrderStatus.Delivered)
                {
                    order.DeliveredAt = DateTime.UtcNow;
                    order.DeliveryPhotoUrl = request.Dto.ProofImageUrl;
                    if (deliverer != null)
                    {
                        deliverer.Status = DelivererStatus.Online;
                        deliverer.TotalDeliveries++;
                        var earning = new DelivererEarning
                        {
                            DelivererId = deliverer.Id, OrderType = "Grocery", OrderId = order.Id,
                            BaseFee = deliverer.DeliveryBaseFee, TipAmount = order.TipAmount,
                            TotalEarning = deliverer.DeliveryBaseFee + order.TipAmount
                        };
                        await db.DelivererEarnings.AddAsync(earning, ct);
                        deliverer.TotalEarnings += earning.TotalEarning;
                    }
                }
                order.StatusHistory.Add(new Domain.Entities.Grocery.GroceryOrderStatusHistory { Status = gStatus });
            }
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Statut mis a jour");
    }
}

public class SubmitDeliveryProofCommandHandler : IRequestHandler<SubmitDeliveryProofCommand, ApiResponse<string>>
{
    public Task<ApiResponse<string>> Handle(SubmitDeliveryProofCommand request, CancellationToken ct)
    {
        var url = $"/uploads/delivery-proof/{request.DeliveryId}/{Guid.NewGuid()}/{request.FileName}";
        return Task.FromResult(ApiResponse<string>.Ok(url));
    }
}

public class RequestPayoutCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<RequestPayoutCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RequestPayoutCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var deliverer = await db.Deliverers.FirstOrDefaultAsync(d => d.UserId == userId, ct);
        if (deliverer == null) return ApiResponse.Fail("Livreur introuvable");
        var unpaid = await db.DelivererEarnings.Where(e => e.DelivererId == deliverer.Id)
            .SumAsync(e => e.TotalEarning, ct);
        var alreadyPaid = await db.DelivererPayouts.Where(p => p.DelivererId == deliverer.Id && p.Status != "Failed")
            .SumAsync(p => p.Amount, ct);
        var available = unpaid - alreadyPaid;
        if (request.Amount > available) return ApiResponse.Fail($"Solde insuffisant. Disponible: {available} MAD");
        var payout = new DelivererPayout
        {
            DelivererId = deliverer.Id, Amount = request.Amount, Status = "Pending"
        };
        await db.DelivererPayouts.AddAsync(payout, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Demande de paiement soumise");
    }
}

public class TriggerSosCommandHandler : IRequestHandler<TriggerSosCommand, ApiResponse>
{
    public Task<ApiResponse> Handle(TriggerSosCommand request, CancellationToken ct)
    {
        // TODO: Send emergency notification to admin + emergency contacts
        return Task.FromResult(ApiResponse.Ok("SOS envoye. Equipe alertee."));
    }
}
