using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Grocery;
using MultiServices.Application.Features.Grocery.Queries;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Grocery.Handlers;

public class SearchStoresQueryHandler(ApplicationDbContext db)
    : IRequestHandler<SearchStoresQuery, ApiResponse<PaginatedList<StoreListDto>>>
{
    public async Task<ApiResponse<PaginatedList<StoreListDto>>> Handle(SearchStoresQuery r, CancellationToken ct)
    {
        var q = db.GroceryStores.Where(s => s.IsActive && !s.IsDeleted).AsQueryable();
        if (!string.IsNullOrEmpty(r.Query)) q = q.Where(s => s.Name.Contains(r.Query));
        if (r.HasPromo == true) q = q.Where(s => s.Promotions.Any(p => p.IsActive && p.EndDate > DateTime.UtcNow));
        if (r.FreeDelivery == true) q = q.Where(s => s.FreeDeliveryMinimum == 0);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(s => s.AverageRating)
            .Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(s => new StoreListDto(s.Id, s.Name, s.StoreType.ToString(), s.LogoUrl,
                s.AverageRating, s.TotalReviews, s.MinOrderAmount, s.DeliveryFee,
                s.FreeDeliveryMinimum, 0, s.IsOpen,
                s.Promotions.Any(p => p.IsActive && p.EndDate > DateTime.UtcNow)))
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<StoreListDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetStoreDetailQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetStoreDetailQuery, ApiResponse<StoreDetailDto>>
{
    public async Task<ApiResponse<StoreDetailDto>> Handle(GetStoreDetailQuery r, CancellationToken ct)
    {
        var e = await db.GroceryStores
            .Include(s => s.OpeningHours).Include(s => s.Departments.Where(d => d.IsActive))
            .Include(s => s.Promotions.Where(p => p.IsActive && p.EndDate > DateTime.UtcNow))
            .FirstOrDefaultAsync(s => s.Id == r.Id, ct);
        if (e == null) return ApiResponse<StoreDetailDto>.Fail("Magasin introuvable");
        return ApiResponse<StoreDetailDto>.Ok(new StoreDetailDto(e.Id, e.Name, e.StoreType.ToString(), e.Description,
            e.LogoUrl, e.CoverImageUrl, e.AverageRating, e.TotalReviews,
            e.MinOrderAmount, e.DeliveryFee, e.FreeDeliveryMinimum,
            $"{e.Street}, {e.City}", e.Latitude, e.Longitude, e.IsOpen,
            e.Departments.OrderBy(d => d.SortOrder).Select(d => new StoreCategoryDto(d.Id, d.Category, d.Name, d.ImageUrl,
                db.GroceryProducts.Count(p => p.DepartmentId == d.Id && p.IsActive && !p.IsDeleted))).ToList(),
            e.OpeningHours.Select(h => new WorkingHoursDto(h.DayOfWeek, h.OpenTime.ToString(), h.CloseTime.ToString(), h.IsClosed)).ToList(),
            e.Promotions.Select(p => new StorePromotionDto(p.Id, p.Title, p.Description, p.Code,
                p.DiscountType == "Percentage" ? p.DiscountValue : null,
                p.DiscountType == "Fixed" ? p.DiscountValue : null,
                p.FreeDelivery, p.EndDate, p.StartDate, p.DiscountType)).ToList()));
    }
}

public class GetStoreCategoriesQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetStoreCategoriesQuery, ApiResponse<List<StoreCategoryDto>>>
{
    public async Task<ApiResponse<List<StoreCategoryDto>>> Handle(GetStoreCategoriesQuery r, CancellationToken ct)
    {
        var cats = await db.GroceryDepartments.Where(d => d.StoreId == r.StoreId && d.IsActive)
            .OrderBy(d => d.SortOrder).ToListAsync(ct);
        var dtos = new List<StoreCategoryDto>();
        foreach (var d in cats)
        {
            var count = await db.GroceryProducts.CountAsync(p => p.DepartmentId == d.Id && p.IsActive && !p.IsDeleted, ct);
            dtos.Add(new StoreCategoryDto(d.Id, d.Category, d.Name, d.ImageUrl, count));
        }
        return ApiResponse<List<StoreCategoryDto>>.Ok(dtos);
    }
}

public class SearchProductsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<SearchProductsQuery, ApiResponse<PaginatedList<ProductListDto>>>
{
    public async Task<ApiResponse<PaginatedList<ProductListDto>>> Handle(SearchProductsQuery r, CancellationToken ct)
    {
        var q = db.GroceryProducts.Where(p => p.StoreId == r.StoreId && p.IsActive && !p.IsDeleted).AsQueryable();
        if (!string.IsNullOrEmpty(r.Query)) q = q.Where(p => p.Name.Contains(r.Query) || (p.Brand != null && p.Brand.Contains(r.Query)));
        if (r.CategoryId.HasValue) q = q.Where(p => p.DepartmentId == r.CategoryId.Value);
        if (!string.IsNullOrEmpty(r.Brand)) q = q.Where(p => p.Brand == r.Brand);
        if (r.IsBio == true) q = q.Where(p => p.IsBio);
        if (r.IsHalal == true) q = q.Where(p => p.IsHalal);
        if (r.OnPromo == true) q = q.Where(p => p.IsPromotion);
        q = r.SortBy switch
        {
            "price_asc" => q.OrderBy(p => p.UnitPrice),
            "price_desc" => q.OrderByDescending(p => p.UnitPrice),
            "popular" => q.OrderByDescending(p => p.TotalSold),
            _ => q.OrderBy(p => p.SortOrder)
        };
        var total = await q.CountAsync(ct);
        var items = await q.Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(p => new ProductListDto(p.Id, p.Name, p.Brand, p.UnitPrice,
                p.PricePerKg ?? p.PricePerLiter, p.Unit, p.ImageUrl, p.IsInStock,
                p.IsBio, p.IsHalal, p.IsPromotion, p.DiscountedPrice, p.TotalSold > 50))
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<ProductListDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetProductDetailQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetProductDetailQuery, ApiResponse<ProductDetailDto>>
{
    public async Task<ApiResponse<ProductDetailDto>> Handle(GetProductDetailQuery r, CancellationToken ct)
    {
        var p = await db.GroceryProducts.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == r.ProductId, ct);
        if (p == null) return ApiResponse<ProductDetailDto>.Fail("Produit introuvable");
        var similar = await db.GroceryProducts.Where(x => x.DepartmentId == p.DepartmentId && x.Id != p.Id && x.IsActive)
            .Take(5).Select(x => new ProductListDto(x.Id, x.Name, x.Brand, x.UnitPrice,
                x.PricePerKg ?? x.PricePerLiter, x.Unit, x.ImageUrl, x.IsInStock,
                x.IsBio, x.IsHalal, x.IsPromotion, x.DiscountedPrice, x.TotalSold > 50))
            .ToListAsync(ct);
        return ApiResponse<ProductDetailDto>.Ok(new ProductDetailDto(p.Id, p.Name, p.Description, p.Brand,
            p.Barcode, p.UnitPrice, p.PricePerKg ?? p.PricePerLiter, p.Unit, p.ImageUrl,
            p.NutritionalInfoJson, string.Join(",", p.Allergens), p.Origin,
            p.IsBio, p.IsHalal, p.StockQuantity, p.IsInStock,
            p.IsPromotion, p.DiscountedPrice, similar));
    }
}

public class ScanBarcodeQueryHandler(ApplicationDbContext db)
    : IRequestHandler<ScanBarcodeQuery, ApiResponse<ProductDetailDto>>
{
    public async Task<ApiResponse<ProductDetailDto>> Handle(ScanBarcodeQuery r, CancellationToken ct)
    {
        var p = await db.GroceryProducts.FirstOrDefaultAsync(x => x.Barcode == r.Barcode && x.StoreId == r.StoreId, ct);
        if (p == null) return ApiResponse<ProductDetailDto>.Fail("Produit introuvable");
        return ApiResponse<ProductDetailDto>.Ok(new ProductDetailDto(p.Id, p.Name, p.Description, p.Brand,
            p.Barcode, p.UnitPrice, p.PricePerKg ?? p.PricePerLiter, p.Unit, p.ImageUrl,
            p.NutritionalInfoJson, string.Join(",", p.Allergens), p.Origin,
            p.IsBio, p.IsHalal, p.StockQuantity, p.IsInStock,
            p.IsPromotion, p.DiscountedPrice, null));
    }
}

public class GetGroceryOrdersQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetGroceryOrdersQuery, ApiResponse<PaginatedList<GroceryOrderDto>>>
{
    public async Task<ApiResponse<PaginatedList<GroceryOrderDto>>> Handle(GetGroceryOrdersQuery r, CancellationToken ct)
    {
        var q = db.GroceryOrders.Include(o => o.Items).Include(o => o.Store).AsQueryable();
        if (r.StoreId.HasValue) q = q.Where(o => o.StoreId == r.StoreId.Value);
        if (!string.IsNullOrEmpty(r.Status) && Enum.TryParse<GroceryOrderStatus>(r.Status, out var s))
            q = q.Where(o => o.Status == s);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(o => new GroceryOrderDto(o.Id, o.OrderNumber, o.Status.ToString(), o.Store.Name,
                o.SubTotal, o.DeliveryFee, o.Discount, o.TipAmount, o.TotalAmount,
                o.PaymentStatus.ToString(), o.CreatedAt, o.ScheduledFor,
                o.Items.Select(i => new GroceryOrderItemDto(i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice,
                    !i.IsUnavailable, i.ReplacementProductId.HasValue, i.ReplacementProductName,
                    i.ReplacementAccepted ?? false)).ToList(), null))
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<GroceryOrderDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetGroceryOrderDetailQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetGroceryOrderDetailQuery, ApiResponse<GroceryOrderDto>>
{
    public async Task<ApiResponse<GroceryOrderDto>> Handle(GetGroceryOrderDetailQuery r, CancellationToken ct)
    {
        var o = await db.GroceryOrders.Include(x => x.Items).Include(x => x.Store)
            .FirstOrDefaultAsync(x => x.Id == r.OrderId, ct);
        if (o == null) return ApiResponse<GroceryOrderDto>.Fail("Commande introuvable");
        DeliveryDriverInfoDto? driver = null;
        if (o.DelivererId.HasValue)
        {
            var d = await db.Deliverers.FirstOrDefaultAsync(x => x.UserId == o.DelivererId.Value, ct);
            if (d != null) driver = new DeliveryDriverInfoDto(d.FirstName + " " + d.LastName, d.PhotoUrl, d.Phone, d.CurrentLatitude, d.CurrentLongitude);
        }
        return ApiResponse<GroceryOrderDto>.Ok(new GroceryOrderDto(o.Id, o.OrderNumber, o.Status.ToString(), o.Store.Name,
            o.SubTotal, o.DeliveryFee, o.Discount, o.TipAmount, o.TotalAmount,
            o.PaymentStatus.ToString(), o.CreatedAt, o.ScheduledFor,
            o.Items.Select(i => new GroceryOrderItemDto(i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice,
                !i.IsUnavailable, i.ReplacementProductId.HasValue, i.ReplacementProductName,
                i.ReplacementAccepted ?? false)).ToList(), driver));
    }
}

public class GetMyGroceryOrdersQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetMyGroceryOrdersQuery, ApiResponse<PaginatedList<GroceryOrderDto>>>
{
    public async Task<ApiResponse<PaginatedList<GroceryOrderDto>>> Handle(GetMyGroceryOrdersQuery r, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var q = db.GroceryOrders.Include(o => o.Items).Include(o => o.Store).Where(o => o.CustomerId == userId);
        if (!string.IsNullOrEmpty(r.Status) && Enum.TryParse<GroceryOrderStatus>(r.Status, out var s))
            q = q.Where(o => o.Status == s);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(o => new GroceryOrderDto(o.Id, o.OrderNumber, o.Status.ToString(), o.Store.Name,
                o.SubTotal, o.DeliveryFee, o.Discount, o.TipAmount, o.TotalAmount,
                o.PaymentStatus.ToString(), o.CreatedAt, null,
                o.Items.Select(i => new GroceryOrderItemDto(i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice, !i.IsUnavailable, false, null, false)).ToList(), null))
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<GroceryOrderDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetMyShoppingListsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetMyShoppingListsQuery, ApiResponse<List<ShoppingListDto>>>
{
    public async Task<ApiResponse<List<ShoppingListDto>>> Handle(GetMyShoppingListsQuery r, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var lists = await db.ShoppingLists.Include(l => l.Items).Where(l => l.UserId == userId && !l.IsDeleted)
            .Select(l => new ShoppingListDto(l.Id, l.Name, l.IsRecurring, l.Recurrence.ToString(),
                l.Items.Select(i => new ShoppingListItemDto(i.Id, i.Name, i.Quantity, i.IsChecked, i.ProductId)).ToList()))
            .ToListAsync(ct);
        return ApiResponse<List<ShoppingListDto>>.Ok(lists);
    }
}

public class GetShoppingListQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetShoppingListQuery, ApiResponse<ShoppingListDto>>
{
    public async Task<ApiResponse<ShoppingListDto>> Handle(GetShoppingListQuery r, CancellationToken ct)
    {
        var l = await db.ShoppingLists.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == r.Id, ct);
        if (l == null) return ApiResponse<ShoppingListDto>.Fail("Liste introuvable");
        return ApiResponse<ShoppingListDto>.Ok(new ShoppingListDto(l.Id, l.Name, l.IsRecurring, l.Recurrence.ToString(),
            l.Items.Select(i => new ShoppingListItemDto(i.Id, i.Name, i.Quantity, i.IsChecked, i.ProductId)).ToList()));
    }
}

public class GetStoreStatsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetStoreStatsQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetStoreStatsQuery r, CancellationToken ct)
    {
        var q = db.GroceryOrders.Where(o => o.StoreId == r.StoreId && o.Status != GroceryOrderStatus.Cancelled);
        if (r.From.HasValue) q = q.Where(o => o.CreatedAt >= r.From.Value);
        if (r.To.HasValue) q = q.Where(o => o.CreatedAt <= r.To.Value.AddDays(1));
        var orders = await q.ToListAsync(ct);
        var lowStock = await db.GroceryProducts.CountAsync(p => p.StoreId == r.StoreId && p.IsActive && p.StockQuantity <= p.LowStockThreshold && p.StockQuantity > 0, ct);
        var outOfStock = await db.GroceryProducts.CountAsync(p => p.StoreId == r.StoreId && p.IsActive && !p.IsInStock, ct);
        return ApiResponse<object>.Ok(new
        {
            TotalOrders = orders.Count, TotalRevenue = orders.Sum(o => o.TotalAmount),
            AverageBasket = orders.Count > 0 ? orders.Average(o => o.TotalAmount) : 0,
            TotalCommissions = orders.Sum(o => o.CommissionAmount), LowStockCount = lowStock, OutOfStockCount = outOfStock
        });
    }
}

public class GetFavoriteStoresQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetFavoriteStoresQuery, ApiResponse<List<StoreListDto>>>
{
    public async Task<ApiResponse<List<StoreListDto>>> Handle(GetFavoriteStoresQuery r, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var favIds = await db.UserFavorites.Where(f => f.UserId == userId && f.EntityType == "GroceryStore")
            .Select(f => f.EntityId).ToListAsync(ct);
        var items = await db.GroceryStores.Where(s => favIds.Contains(s.Id))
            .Select(s => new StoreListDto(s.Id, s.Name, s.StoreType.ToString(), s.LogoUrl,
                s.AverageRating, s.TotalReviews, s.MinOrderAmount, s.DeliveryFee,
                s.FreeDeliveryMinimum, 0, s.IsOpen, false))
            .ToListAsync(ct);
        return ApiResponse<List<StoreListDto>>.Ok(items);
    }
}
