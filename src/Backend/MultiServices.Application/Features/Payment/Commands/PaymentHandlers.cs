using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.Features.Payment.Commands;
using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Payment.Handlers;

public class CreatePaymentCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<CreatePaymentCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var transaction = new PaymentTransaction
        {
            UserId = userId, OrderType = request.OrderType, OrderId = request.OrderId,
            Amount = request.Amount, Method = request.Method,
            Status = PaymentStatus.Pending, Currency = "MAD"
        };

        if (request.Method == PaymentMethod.Wallet)
        {
            var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
            if (wallet == null || wallet.Balance < request.Amount)
                return ApiResponse<string>.Fail("Solde portefeuille insuffisant");
            wallet.Balance -= request.Amount;
            await db.WalletTransactions.AddAsync(new WalletTransaction
            {
                WalletId = wallet.Id, Amount = -request.Amount, Type = "Debit",
                Description = $"Paiement {request.OrderType} #{request.OrderId}"
            }, ct);
            transaction.Status = PaymentStatus.Completed;
        }
        else
        {
            // Simulate payment intent for card/CMI
            transaction.StripePaymentIntentId = $"pi_{Guid.NewGuid():N}";
        }

        await db.PaymentTransactions.AddAsync(transaction, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<string>.Ok(transaction.StripePaymentIntentId ?? "wallet_payment_ok");
    }
}

public class ConfirmPaymentCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ConfirmPaymentCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ConfirmPaymentCommand request, CancellationToken ct)
    {
        var tx = await db.PaymentTransactions.FirstOrDefaultAsync(t => t.StripePaymentIntentId == request.PaymentIntentId, ct);
        if (tx == null) return ApiResponse.Fail("Transaction introuvable");
        tx.Status = PaymentStatus.Completed;

        // Update order payment status
        if (tx.OrderType == "Restaurant")
        {
            var order = await db.RestaurantOrders.FindAsync(new object[] { tx.OrderId }, ct);
            if (order != null) order.PaymentStatus = PaymentStatus.Completed;
        }
        else if (tx.OrderType == "Grocery")
        {
            var order = await db.GroceryOrders.FindAsync(new object[] { tx.OrderId }, ct);
            if (order != null) order.PaymentStatus = PaymentStatus.Completed;
        }
        else if (tx.OrderType == "Service")
        {
            var intervention = await db.ServiceInterventions.FindAsync(new object[] { tx.OrderId }, ct);
            if (intervention != null) intervention.PaymentStatus = PaymentStatus.Completed;
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Paiement confirme");
    }
}

public class SavePaymentMethodCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<SavePaymentMethodCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(SavePaymentMethodCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var method = new SavedPaymentMethod
        {
            UserId = userId, Type = PaymentMethod.CreditCard,
            TokenizedCardId = request.CardToken,
            Last4Digits = request.CardToken[^4..],
            CardBrand = "Visa", IsDefault = false
        };
        await db.SavedPaymentMethods.AddAsync(method, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Methode de paiement enregistree");
    }
}

public class DeletePaymentMethodCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<DeletePaymentMethodCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeletePaymentMethodCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var method = await db.SavedPaymentMethods.FirstOrDefaultAsync(m => m.Id == request.PaymentMethodId && m.UserId == userId, ct);
        if (method == null) return ApiResponse.Fail("Methode introuvable");
        method.IsDeleted = true; method.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Methode supprimee");
    }
}

public class TopUpWalletCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<TopUpWalletCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(TopUpWalletCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
        if (wallet == null)
        {
            wallet = new WalletAccount { UserId = userId, Balance = 0, Currency = "MAD" };
            await db.Wallets.AddAsync(wallet, ct);
            await db.SaveChangesAsync(ct);
        }
        wallet.Balance += request.Amount;
        await db.WalletTransactions.AddAsync(new WalletTransaction
        {
            WalletId = wallet.Id, Amount = request.Amount, Type = "Credit",
            Description = $"Rechargement portefeuille ({request.Method})"
        }, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok($"Portefeuille recharge. Solde: {wallet.Balance} MAD");
    }
}

public class ApplyPromoCodeCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ApplyPromoCodeCommand, ApiResponse<decimal>>
{
    public async Task<ApiResponse<decimal>> Handle(ApplyPromoCodeCommand request, CancellationToken ct)
    {
        var promo = await db.PromoCodes.FirstOrDefaultAsync(p => p.Code == request.Code && p.IsActive, ct);
        if (promo == null) return ApiResponse<decimal>.Fail("Code promo invalide");
        if (promo.EndDate < DateTime.UtcNow) return ApiResponse<decimal>.Fail("Code promo expire");
        if (promo.MaxUsages.HasValue && promo.CurrentUsages >= promo.MaxUsages.Value)
            return ApiResponse<decimal>.Fail("Code promo epuise");
        if (promo.MinOrderAmount.HasValue && request.OrderAmount < promo.MinOrderAmount.Value)
            return ApiResponse<decimal>.Fail($"Montant minimum: {promo.MinOrderAmount} MAD");
        if (promo.ApplicableModule != "All" && promo.ApplicableModule != request.OrderType)
            return ApiResponse<decimal>.Fail("Code non applicable a ce module");

        decimal discount = promo.DiscountType == "Percentage"
            ? request.OrderAmount * promo.DiscountValue / 100
            : promo.DiscountValue;
        if (promo.MaxDiscount.HasValue && discount > promo.MaxDiscount.Value)
            discount = promo.MaxDiscount.Value;
        return ApiResponse<decimal>.Ok(discount);
    }
}

// Query handlers
public class GetSavedPaymentMethodsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetSavedPaymentMethodsQuery, ApiResponse<List<object>>>
{
    public async Task<ApiResponse<List<object>>> Handle(GetSavedPaymentMethodsQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var methods = await db.SavedPaymentMethods.Where(m => m.UserId == userId && !m.IsDeleted)
            .Select(m => (object)new { m.Id, Type = m.Type.ToString(), m.Last4Digits, m.CardBrand, m.IsDefault })
            .ToListAsync(ct);
        return ApiResponse<List<object>>.Ok(methods);
    }
}

public class GetWalletBalanceQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetWalletBalanceQuery, ApiResponse<decimal>>
{
    public async Task<ApiResponse<decimal>> Handle(GetWalletBalanceQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
        return ApiResponse<decimal>.Ok(wallet?.Balance ?? 0);
    }
}

public class GetWalletTransactionsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetWalletTransactionsQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetWalletTransactionsQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var wallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
        if (wallet == null) return ApiResponse<object>.Ok(new { Items = new List<object>(), Total = 0 });
        var q = db.WalletTransactions.Where(t => t.WalletId == wallet.Id);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(t => new { t.Id, t.Amount, t.Type, t.Description, t.CreatedAt }).ToListAsync(ct);
        return ApiResponse<object>.Ok(new { Items = items, Total = total });
    }
}

public class GetPaymentHistoryQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetPaymentHistoryQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetPaymentHistoryQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var q = db.PaymentTransactions.Where(t => t.UserId == userId);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(t => new { t.Id, t.OrderType, t.Amount, Status = t.Status.ToString(), Method = t.Method.ToString(), t.CreatedAt })
            .ToListAsync(ct);
        return ApiResponse<object>.Ok(new { Items = items, Total = total });
    }
}
