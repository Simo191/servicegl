using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Admin;
using MultiServices.Application.Features.Admin.Commands;
using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;

namespace MultiServices.Application.Features.Admin.Handlers;

public class ApproveEntityCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ApproveEntityCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ApproveEntityCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var newStatus = d.Approved ? VerificationStatus.Approved : VerificationStatus.Rejected;

        switch (d.EntityType)
        {
            case "Restaurant":
                var r = await db.Restaurants.FindAsync(new object[] { d.EntityId }, ct);
                if (r == null) return ApiResponse.Fail("Restaurant introuvable");
                r.VerificationStatus = newStatus;
                r.IsActive = d.Approved; r.IsVerified = d.Approved;
                break;
            case "ServiceProvider":
                var sp = await db.ServiceProviders.FindAsync(new object[] { d.EntityId }, ct);
                if (sp == null) return ApiResponse.Fail("Prestataire introuvable");
                sp.VerificationStatus = newStatus;
                sp.IsActive = d.Approved; sp.IsVerified = d.Approved;
                break;
            case "GroceryStore":
                var gs = await db.GroceryStores.FindAsync(new object[] { d.EntityId }, ct);
                if (gs == null) return ApiResponse.Fail("Magasin introuvable");
                gs.VerificationStatus = newStatus;
                gs.IsActive = d.Approved; gs.IsVerified = d.Approved;
                break;
            case "Deliverer":
                var dl = await db.Deliverers.FindAsync(new object[] { d.EntityId }, ct);
                if (dl == null) return ApiResponse.Fail("Livreur introuvable");
                dl.VerificationStatus = newStatus;
                dl.IsActive = d.Approved; dl.IsVerified = d.Approved;
                break;
            default:
                return ApiResponse.Fail("Type d'entite inconnu");
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok(d.Approved ? "Approuve" : "Rejete");
    }
}

public class SuspendUserCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<SuspendUserCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(SuspendUserCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return ApiResponse.Fail("Utilisateur introuvable");
        user.IsActive = false;
        user.LockoutEndDate = DateTime.UtcNow.AddYears(100);
        await userManager.UpdateAsync(user);
        return ApiResponse.Ok("Utilisateur suspendu");
    }
}

public class ActivateUserCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ActivateUserCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ActivateUserCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return ApiResponse.Fail("Utilisateur introuvable");
        user.IsActive = true; user.LockoutEndDate = null;
        await userManager.UpdateAsync(user);
        return ApiResponse.Ok("Utilisateur active");
    }
}

public class UpdateCommissionSettingsCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateCommissionSettingsCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateCommissionSettingsCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        switch (d.EntityType)
        {
            case "Restaurant":
                var r = await db.Restaurants.FindAsync(new object[] { d.EntityId }, ct);
                if (r != null) r.CommissionRate = d.NewRate;
                break;
            case "ServiceProvider":
                var sp = await db.ServiceProviders.FindAsync(new object[] { d.EntityId }, ct);
                if (sp != null) sp.CommissionRate = d.NewRate;
                break;
            case "GroceryStore":
                var gs = await db.GroceryStores.FindAsync(new object[] { d.EntityId }, ct);
                if (gs != null) gs.CommissionRate = d.NewRate;
                break;
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commission mise a jour");
    }
}

public class CreateGlobalPromotionCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CreateGlobalPromotionCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateGlobalPromotionCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var promo = new PromoCode
        {
            Code = d.Code, Title = d.Title, Description = d.Description,
            DiscountType = d.DiscountType, DiscountValue = d.DiscountValue,
            MinOrderAmount = d.MinOrderAmount, MaxDiscount = d.MaxDiscount,
            StartDate = d.StartDate, EndDate = d.EndDate,
            MaxUsages = d.MaxUsages, ApplicableModule = d.ApplicableModule,
            FreeDelivery = d.FreeDelivery, IsActive = true
        };
        await db.PromoCodes.AddAsync(promo, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(promo.Id);
    }
}

public class SendBulkNotificationCommandHandler(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    : IRequestHandler<SendBulkNotificationCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(SendBulkNotificationCommand request, CancellationToken ct)
    {
        List<Guid> targetIds;
        if (request.TargetUserIds != null && request.TargetUserIds.Any())
            targetIds = request.TargetUserIds;
        else if (!string.IsNullOrEmpty(request.TargetRole))
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(request.TargetRole);
            targetIds = usersInRole.Select(u => u.Id).ToList();
        }
        else
            targetIds = await db.Users.Where(u => u.IsActive).Select(u => u.Id).ToListAsync(ct);

        foreach (var uid in targetIds)
        {
            await db.Notifications.AddAsync(new Notification
            {
                UserId = uid, Title = request.Title, Message = request.Body,
                Type = NotificationType.System
            }, ct);
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok($"{targetIds.Count} notifications envoyees");
    }
}

public class CancelOrderAdminCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CancelOrderAdminCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CancelOrderAdminCommand request, CancellationToken ct)
    {
        switch (request.OrderType)
        {
            case "Restaurant":
                var ro = await db.RestaurantOrders.FindAsync(new object[] { request.OrderId }, ct);
                if (ro == null) return ApiResponse.Fail("Commande introuvable");
                ro.Status = RestaurantOrderStatus.Cancelled;
                ro.CancelledAt = DateTime.UtcNow; ro.CancellationReason = request.Reason;
                break;
            case "Grocery":
                var go = await db.GroceryOrders.FindAsync(new object[] { request.OrderId }, ct);
                if (go == null) return ApiResponse.Fail("Commande introuvable");
                go.Status = GroceryOrderStatus.Cancelled;
                go.CancelledAt = DateTime.UtcNow; go.CancellationReason = request.Reason;
                break;
            case "Service":
                var si = await db.ServiceInterventions.FindAsync(new object[] { request.OrderId }, ct);
                if (si == null) return ApiResponse.Fail("Intervention introuvable");
                si.Status = InterventionStatus.Cancelled;
                si.CancelledAt = DateTime.UtcNow; si.CancellationReason = request.Reason;
                break;
            default: return ApiResponse.Fail("Type de commande inconnu");
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commande annulee par admin");
    }
}

public class ReassignDriverCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ReassignDriverCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ReassignDriverCommand request, CancellationToken ct)
    {
        switch (request.OrderType)
        {
            case "Restaurant":
                var ro = await db.RestaurantOrders.FindAsync(new object[] { request.OrderId }, ct);
                if (ro == null) return ApiResponse.Fail("Commande introuvable");
                ro.DelivererId = request.NewDriverId;
                break;
            case "Grocery":
                var go = await db.GroceryOrders.FindAsync(new object[] { request.OrderId }, ct);
                if (go == null) return ApiResponse.Fail("Commande introuvable");
                go.DelivererId = request.NewDriverId;
                break;
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Livreur reassigne");
    }
}

public class ProcessRefundCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ProcessRefundCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ProcessRefundCommand request, CancellationToken ct)
    {
        var tx = await db.PaymentTransactions.FirstOrDefaultAsync(
            t => t.OrderId == request.OrderId && t.OrderType == request.OrderType && t.Status == PaymentStatus.Completed, ct);
        if (tx == null) return ApiResponse.Fail("Transaction introuvable");
        tx.RefundedAmount = request.Amount;
        tx.RefundedAt = DateTime.UtcNow;

        // Credit wallet
        var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == tx.UserId, ct);
        if (wallet != null)
        {
            wallet.Balance += request.Amount;
            await db.WalletTransactions.AddAsync(new WalletTransaction
            {
                WalletId = wallet.Id, Amount = request.Amount, Type = "Credit",
                Description = $"Remboursement {request.OrderType}: {request.Reason}"
            }, ct);
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok($"Remboursement de {request.Amount} MAD effectue");
    }
}

public class UpdateGeoZonesCommandHandler : IRequestHandler<UpdateGeoZonesCommand, ApiResponse>
{
    public Task<ApiResponse> Handle(UpdateGeoZonesCommand request, CancellationToken ct)
    {
        // TODO: Update geographic zones
        return Task.FromResult(ApiResponse.Ok("Zones geographiques mises a jour"));
    }
}

public class ExportFinancialReportCommandHandler : IRequestHandler<ExportFinancialReportCommand, ApiResponse<byte[]>>
{
    public Task<ApiResponse<byte[]>> Handle(ExportFinancialReportCommand request, CancellationToken ct)
    {
        // TODO: Generate actual report
        return Task.FromResult(ApiResponse<byte[]>.Ok(Array.Empty<byte>()));
    }
}
