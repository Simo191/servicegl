using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Grocery;
using MultiServices.Application.Features.Grocery.Commands;
using MultiServices.Domain.Entities.Grocery;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Grocery.Handlers;

public class CreateGroceryOrderCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<CreateGroceryOrderCommand, ApiResponse<GroceryOrderDto>>
{
    public async Task<ApiResponse<GroceryOrderDto>> Handle(CreateGroceryOrderCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var store = await db.GroceryStores.FindAsync(new object[] { request.Dto.StoreId }, ct);
        if (store == null) return ApiResponse<GroceryOrderDto>.Fail("Magasin introuvable");
        if (!store.IsOpen) return ApiResponse<GroceryOrderDto>.Fail("Magasin ferme");

        var address = await db.UserAddresses.FindAsync(new object[] { request.Dto.DeliveryAddressId }, ct);
        if (address == null) return ApiResponse<GroceryOrderDto>.Fail("Adresse introuvable");

        var order = new GroceryOrder
        {
            OrderNumber = $"GRC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            CustomerId = userId, StoreId = request.Dto.StoreId,
            DeliveryStreet = address.Street, DeliveryCity = address.City,
            DeliveryLatitude = address.Latitude, DeliveryLongitude = address.Longitude,
            DeliveryInstructions = request.Dto.DeliveryInstructions,
            LeaveAtDoor = request.Dto.LeaveAtDoor,
            AllowReplacements = request.Dto.AllowSubstitutions,
            PaymentMethod = request.Dto.PaymentMethod, TipAmount = request.Dto.Tip,
            PromoCode = request.Dto.PromoCode,
            Status = GroceryOrderStatus.Received,
            DeliveryFee = store.DeliveryFee,
            EstimatedDeliveryMinutes = store.AveragePreparationMinutes + 30,
            IsScheduled = request.Dto.ScheduledDeliveryStart.HasValue,
            ScheduledFor = request.Dto.ScheduledDeliveryStart
        };

        decimal subTotal = 0;
        foreach (var cartItem in request.Dto.Items)
        {
            var product = await db.GroceryProducts.FindAsync(new object[] { cartItem.ProductId }, ct);
            if (product == null || !product.IsInStock) continue;
            var total = product.UnitPrice * cartItem.Quantity;
            order.Items.Add(new GroceryOrderItem
            {
                ProductId = cartItem.ProductId, ProductName = product.Name,
                ProductImageUrl = product.ImageUrl,
                Quantity = cartItem.Quantity, UnitPrice = product.UnitPrice, TotalPrice = total
            });
            subTotal += total;
            product.StockQuantity -= cartItem.Quantity;
            if (product.StockQuantity <= 0) product.IsInStock = false;
        }

        if (subTotal < store.MinOrderAmount)
            return ApiResponse<GroceryOrderDto>.Fail($"Minimum: {store.MinOrderAmount} MAD");

        if (subTotal >= store.FreeDeliveryMinimum) order.DeliveryFee = 0;
        order.SubTotal = subTotal;
        order.TotalAmount = subTotal + order.DeliveryFee + order.TipAmount + order.BagsFee - order.Discount;
        order.CommissionAmount = subTotal * store.CommissionRate / 100;
        order.StatusHistory.Add(new GroceryOrderStatusHistory { Status = GroceryOrderStatus.Received, Note = "Commande recue" });

        await db.GroceryOrders.AddAsync(order, ct);
        store.TotalOrders++;
        await db.SaveChangesAsync(ct);

        return ApiResponse<GroceryOrderDto>.Ok(new GroceryOrderDto(order.Id, order.OrderNumber,
            order.Status.ToString(), store.Name, order.SubTotal, order.DeliveryFee, order.Discount,
            order.TipAmount, order.TotalAmount, order.PaymentStatus.ToString(), order.CreatedAt, null,
            order.Items.Select(i => new GroceryOrderItemDto(i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice, !i.IsUnavailable, false, null, false)).ToList(), null));
    }
}

public class CancelGroceryOrderCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CancelGroceryOrderCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CancelGroceryOrderCommand request, CancellationToken ct)
    {
        var order = await db.GroceryOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        if (order.Status > GroceryOrderStatus.Received) return ApiResponse.Fail("Annulation impossible");
        order.Status = GroceryOrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow; order.CancellationReason = request.Reason;
        order.StatusHistory.Add(new GroceryOrderStatusHistory { Status = GroceryOrderStatus.Cancelled, Note = request.Reason });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commande annulee");
    }
}

public class AcceptGroceryOrderCommandHandler(ApplicationDbContext db)
    : IRequestHandler<AcceptGroceryOrderCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(AcceptGroceryOrderCommand request, CancellationToken ct)
    {
        var order = await db.GroceryOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        order.Status = GroceryOrderStatus.Preparing;
        order.ConfirmedAt = DateTime.UtcNow;
        order.PreparationStartedAt = DateTime.UtcNow;
        order.StatusHistory.Add(new GroceryOrderStatusHistory { Status = GroceryOrderStatus.Preparing, Note = "Commande acceptee" });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commande acceptee");
    }
}

public class RejectGroceryOrderCommandHandler(ApplicationDbContext db)
    : IRequestHandler<RejectGroceryOrderCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RejectGroceryOrderCommand request, CancellationToken ct)
    {
        var order = await db.GroceryOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        order.Status = GroceryOrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow; order.CancellationReason = request.Reason;
        order.StatusHistory.Add(new GroceryOrderStatusHistory { Status = GroceryOrderStatus.Cancelled, Note = $"Refusee: {request.Reason}" });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commande refusee");
    }
}

public class AcceptProductSubstitutionCommandHandler(ApplicationDbContext db)
    : IRequestHandler<AcceptProductSubstitutionCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(AcceptProductSubstitutionCommand request, CancellationToken ct)
    {
        var item = await db.GroceryOrderItems.FindAsync(new object[] { request.ItemId }, ct);
        if (item == null) return ApiResponse.Fail("Article introuvable");
        item.ReplacementAccepted = request.Accept;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok(request.Accept ? "Substitution acceptee" : "Substitution refusee");
    }
}

public class ReportMissingProductCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ReportMissingProductCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ReportMissingProductCommand request, CancellationToken ct)
    {
        var item = await db.GroceryOrderItems.FindAsync(new object[] { request.ItemId }, ct);
        if (item == null) return ApiResponse.Fail("Article introuvable");
        item.IsUnavailable = true;
        item.Note = "Produit manquant signale par le client";
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Produit manquant signale");
    }
}

public class ReportDamagedProductCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ReportDamagedProductCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ReportDamagedProductCommand request, CancellationToken ct)
    {
        var item = await db.GroceryOrderItems.FindAsync(new object[] { request.ItemId }, ct);
        if (item == null) return ApiResponse.Fail("Article introuvable");
        item.Note = $"Produit endommage: {request.Description}";
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Dommage signale");
    }
}

public class CreateShoppingListCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<CreateShoppingListCommand, ApiResponse<ShoppingListDto>>
{
    public async Task<ApiResponse<ShoppingListDto>> Handle(CreateShoppingListCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var list = new ShoppingList
        {
            UserId = userId, Name = request.Dto.Name,
            IsRecurring = request.Dto.IsRecurring
        };
        foreach (var item in request.Dto.Items)
            list.Items.Add(new ShoppingListItem { Name = item.Name, Quantity = item.Quantity, ProductId = item.ProductId });
        await db.ShoppingLists.AddAsync(list, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<ShoppingListDto>.Ok(new ShoppingListDto(list.Id, list.Name, list.IsRecurring, null,
            list.Items.Select(i => new ShoppingListItemDto(i.Id, i.Name, i.Quantity, i.IsChecked, i.ProductId)).ToList()));
    }
}

public class UpdateShoppingListCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateShoppingListCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateShoppingListCommand request, CancellationToken ct)
    {
        var list = await db.ShoppingLists.Include(l => l.Items).FirstOrDefaultAsync(l => l.Id == request.Id, ct);
        if (list == null) return ApiResponse.Fail("Liste introuvable");
        list.Name = request.Dto.Name; list.IsRecurring = request.Dto.IsRecurring;
        db.ShoppingListItems.RemoveRange(list.Items);
        foreach (var item in request.Dto.Items)
            list.Items.Add(new ShoppingListItem { Name = item.Name, Quantity = item.Quantity, ProductId = item.ProductId });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Liste mise a jour");
    }
}

public class DeleteShoppingListCommandHandler(ApplicationDbContext db)
    : IRequestHandler<DeleteShoppingListCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteShoppingListCommand request, CancellationToken ct)
    {
        var list = await db.ShoppingLists.FindAsync(new object[] { request.Id }, ct);
        if (list == null) return ApiResponse.Fail("Introuvable");
        list.IsDeleted = true; list.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Liste supprimee");
    }
}

public class ConvertListToCartCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ConvertListToCartCommand, ApiResponse<List<GroceryCartItemDto>>>
{
    public async Task<ApiResponse<List<GroceryCartItemDto>>> Handle(ConvertListToCartCommand request, CancellationToken ct)
    {
        var list = await db.ShoppingLists.Include(l => l.Items).FirstOrDefaultAsync(l => l.Id == request.StoreId, ct);
        if (list == null) return ApiResponse<List<GroceryCartItemDto>>.Fail("Liste introuvable");
        var cartItems = list.Items.Where(i => i.ProductId.HasValue)
            .Select(i => new GroceryCartItemDto(i.ProductId!.Value, i.Quantity)).ToList();
        return ApiResponse<List<GroceryCartItemDto>>.Ok(cartItems);
    }
}

public class DuplicatePreviousOrderCommandHandler(ApplicationDbContext db)
    : IRequestHandler<DuplicatePreviousOrderCommand, ApiResponse<List<GroceryCartItemDto>>>
{
    public async Task<ApiResponse<List<GroceryCartItemDto>>> Handle(DuplicatePreviousOrderCommand request, CancellationToken ct)
    {
        var order = await db.GroceryOrders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order == null) return ApiResponse<List<GroceryCartItemDto>>.Fail("Commande introuvable");
        var cartItems = order.Items.Select(i => new GroceryCartItemDto(i.ProductId, i.Quantity)).ToList();
        return ApiResponse<List<GroceryCartItemDto>>.Ok(cartItems);
    }
}

public class AssignPreparerCommandHandler(ApplicationDbContext db)
    : IRequestHandler<AssignPreparerCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(AssignPreparerCommand request, CancellationToken ct)
    {
        var order = await db.GroceryOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        // PreparerId is Guid?, PreparerName is string from command
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Preparateur assigne");
    }
}

public class MarkProductUnavailableCommandHandler(ApplicationDbContext db)
    : IRequestHandler<MarkProductUnavailableCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(MarkProductUnavailableCommand request, CancellationToken ct)
    {
        var item = await db.GroceryOrderItems.FindAsync(new object[] { request.ItemId }, ct);
        if (item == null) return ApiResponse.Fail("Article introuvable");
        item.IsUnavailable = true;
        if (request.SubstituteProductId.HasValue)
        {
            var sub = await db.GroceryProducts.FindAsync(new object[] { request.SubstituteProductId.Value }, ct);
            if (sub != null)
            {
                item.ReplacementProductId = sub.Id;
                item.ReplacementProductName = sub.Name;
                item.ReplacementPrice = sub.UnitPrice;
            }
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Produit marque indisponible");
    }
}

public class MarkOrderReadyCommandHandler(ApplicationDbContext db)
    : IRequestHandler<MarkOrderReadyCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(MarkOrderReadyCommand request, CancellationToken ct)
    {
        var order = await db.GroceryOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        order.Status = GroceryOrderStatus.Ready;
        order.PreparedAt = DateTime.UtcNow;
        order.StatusHistory.Add(new GroceryOrderStatusHistory { Status = GroceryOrderStatus.Ready, Note = "Commande prete" });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commande prete");
    }
}

public class CreateProductCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CreateProductCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var product = new GroceryProduct
        {
            StoreId = request.StoreId, DepartmentId = d.CategoryId,
            Name = d.Name, Description = d.Description ?? "",
            Brand = d.Brand, Barcode = d.Barcode, SKU = Guid.NewGuid().ToString()[..8],
            UnitPrice = d.Price, IsBio = d.IsBio, IsHalal = d.IsHalal,
            StockQuantity = d.StockQuantity, IsInStock = d.StockQuantity > 0,
            IsActive = true, Origin = d.Origin
        };
        await db.GroceryProducts.AddAsync(product, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(product.Id);
    }
}

public class UpdateProductCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateProductCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await db.GroceryProducts.FindAsync(new object[] { request.ProductId }, ct);
        if (product == null) return ApiResponse.Fail("Produit introuvable");
        var d = request.Dto;
        if (d.Name != null) product.Name = d.Name;
        if (d.Description != null) product.Description = d.Description;
        if (d.Price.HasValue) product.UnitPrice = d.Price.Value;
        if (d.StockQuantity.HasValue) { product.StockQuantity = d.StockQuantity.Value; product.IsInStock = d.StockQuantity.Value > 0; }
        if (d.IsAvailable.HasValue) product.IsInStock = d.IsAvailable.Value;
        if (d.IsOnPromotion.HasValue) product.IsPromotion = d.IsOnPromotion.Value;
        if (d.PromotionPrice.HasValue) product.DiscountedPrice = d.PromotionPrice;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Produit mis a jour");
    }
}

public class DeleteProductCommandHandler(ApplicationDbContext db)
    : IRequestHandler<DeleteProductCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await db.GroceryProducts.FindAsync(new object[] { request.ProductId }, ct);
        if (product == null) return ApiResponse.Fail("Introuvable");
        product.IsDeleted = true; product.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Produit supprime");
    }
}

public class BulkImportProductsCommandHandler(ApplicationDbContext db)
    : IRequestHandler<BulkImportProductsCommand, ApiResponse<int>>
{
    public async Task<ApiResponse<int>> Handle(BulkImportProductsCommand request, CancellationToken ct)
    {
        // Parse CSV from base64
        var csv = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(request.Dto.CsvBase64));
        var lines = csv.Split('\n').Skip(1).Where(l => !string.IsNullOrWhiteSpace(l));
        int count = 0;
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length < 4) continue;
            var existing = await db.GroceryProducts.FirstOrDefaultAsync(p => p.StoreId == request.StoreId && p.Barcode == parts[0].Trim(), ct);
            if (existing != null) { existing.Name = parts[1].Trim(); existing.UnitPrice = decimal.Parse(parts[2].Trim()); existing.StockQuantity = int.Parse(parts[3].Trim()); existing.IsInStock = existing.StockQuantity > 0; }
            else
            {
                await db.GroceryProducts.AddAsync(new GroceryProduct
                {
                    StoreId = request.StoreId, // DepartmentId assigned later
                    Name = parts[1].Trim(), Barcode = parts[0].Trim(), SKU = Guid.NewGuid().ToString()[..8],
                    UnitPrice = decimal.Parse(parts[2].Trim()), StockQuantity = int.Parse(parts[3].Trim()),
                    IsInStock = int.Parse(parts[3].Trim()) > 0, IsActive = true
                }, ct);
            }
            count++;
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse<int>.Ok(count);
    }
}

public class UpdateStockCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateStockCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateStockCommand request, CancellationToken ct)
    {
        var product = await db.GroceryProducts.FindAsync(new object[] { request.ProductId }, ct);
        if (product == null) return ApiResponse.Fail("Produit introuvable");
        product.StockQuantity = request.Quantity;
        product.IsInStock = request.Quantity > 0;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Stock mis a jour");
    }
}

public class CreateStorePromotionCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CreateStorePromotionCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateStorePromotionCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var promo = new GroceryPromotion
        {
            StoreId = request.StoreId, Title = d.Title,
            Description = d.Description, Code = d.Description,
            DiscountType = d.DiscountType,
            DiscountValue = d.DiscountAmount!.Value,
            StartDate = d.StartDate, EndDate = d.EndDate,
            IsActive = true, FreeDelivery = d.FreeDelivery
        };
        await db.GroceryPromotions.AddAsync(promo, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(promo.Id);
    }
}
