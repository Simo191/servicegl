using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Restaurant;
using MultiServices.Application.Features.Restaurants.Queries;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Restaurants.Handlers;

public class SearchRestaurantsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<SearchRestaurantsQuery, ApiResponse<PaginatedList<RestaurantListDto>>>
{
    public async Task<ApiResponse<PaginatedList<RestaurantListDto>>> Handle(SearchRestaurantsQuery r, CancellationToken ct)
    {
        var q = db.Restaurants.Where(x => x.IsActive && !x.IsDeleted).AsQueryable();
        if (!string.IsNullOrEmpty(r.Query)) q = q.Where(x => x.Name.Contains(r.Query) || x.Description.Contains(r.Query));
        if (r.Cuisine.HasValue) q = q.Where(x => x.CuisineType == r.Cuisine.Value);
        if (r.Price.HasValue) q = q.Where(x => x.PriceRange == r.Price.Value);
        if (r.MinRating.HasValue) q = q.Where(x => x.AverageRating >= r.MinRating.Value);
        if (r.HasPromo == true) q = q.Where(x => x.Promotions.Any(p => p.IsActive && p.EndDate > DateTime.UtcNow));

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.AverageRating)
            .Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(x => new RestaurantListDto(x.Id, x.Name, x.LogoUrl, x.CoverImageUrl,
                x.CuisineType, x.PriceRange, x.AverageRating, x.TotalReviews,
                x.MinOrderAmount, x.DeliveryFee, x.AverageDeliveryMinutes, 0, x.IsOpen,
                x.Promotions.Any(p => p.IsActive && p.EndDate > DateTime.UtcNow)))
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<RestaurantListDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetRestaurantDetailQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetRestaurantDetailQuery, ApiResponse<RestaurantDetailDto>>
{
    public async Task<ApiResponse<RestaurantDetailDto>> Handle(GetRestaurantDetailQuery r, CancellationToken ct)
    {
        var e = await db.Restaurants
            .Include(x => x.OpeningHours)
            .Include(x => x.MenuCategories.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Options)
            .Include(x => x.Promotions.Where(p => p.IsActive && p.EndDate > DateTime.UtcNow))
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct);
        if (e == null) return ApiResponse<RestaurantDetailDto>.Fail("Restaurant introuvable");

        return ApiResponse<RestaurantDetailDto>.Ok(new RestaurantDetailDto(e.Id, e.Name, e.Description, e.LogoUrl,
            e.CoverImageUrl, e.CuisineType, e.PriceRange, e.AverageRating, e.TotalReviews,
            e.MinOrderAmount, e.DeliveryFee, e.AverageDeliveryMinutes,
            $"{e.Street}, {e.City}", e.Latitude, e.Longitude, e.Phone, e.IsOpen,
            true, e.AcceptingOrders, e.MaxDeliveryDistanceKm,
            e.MenuCategories.OrderBy(c => c.SortOrder).Select(c => new MenuCategoryDto(c.Id, c.Name, c.Description, c.ImageUrl,
                c.Items.OrderBy(i => i.SortOrder).Select(i => new MenuItemDto(i.Id, i.Name, i.Description, i.Price, i.ImageUrl,
                    i.IsAvailable, i.IsPopular, i.PreparationMinutes, string.Join(",", i.Allergens), 0,
                    i.Options.Select(o => new MenuItemOptionDto(o.Id, o.GroupName, o.Name, o.PriceModifier, false, false, 1)).ToList()
                )).ToList()
            )).ToList(),
            e.OpeningHours.Select(h => new WorkingHoursDto(h.DayOfWeek, h.OpenTime.ToString(), h.CloseTime.ToString(), h.IsClosed)).ToList(),
            e.Promotions.Select(p => new PromotionDto(p.Id, p.Title, p.Description, p.Code,
                p.DiscountType == "Percentage" ? p.DiscountValue : null,
                p.DiscountType == "Fixed" ? p.DiscountValue : null,
                p.FreeDelivery, p.EndDate)).ToList()));
    }
}

public class GetNearbyRestaurantsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetNearbyRestaurantsQuery, ApiResponse<List<RestaurantListDto>>>
{
    public async Task<ApiResponse<List<RestaurantListDto>>> Handle(GetNearbyRestaurantsQuery r, CancellationToken ct)
    {
        var items = await db.Restaurants.Where(x => x.IsActive && x.IsOpen)
            .OrderByDescending(x => x.AverageRating).Take(20)
            .Select(x => new RestaurantListDto(x.Id, x.Name, x.LogoUrl, x.CoverImageUrl,
                x.CuisineType, x.PriceRange, x.AverageRating, x.TotalReviews,
                x.MinOrderAmount, x.DeliveryFee, x.AverageDeliveryMinutes, 0, x.IsOpen, false))
            .ToListAsync(ct);
        return ApiResponse<List<RestaurantListDto>>.Ok(items);
    }
}

public class GetPopularRestaurantsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetPopularRestaurantsQuery, ApiResponse<List<RestaurantListDto>>>
{
    public async Task<ApiResponse<List<RestaurantListDto>>> Handle(GetPopularRestaurantsQuery r, CancellationToken ct)
    {
        var items = await db.Restaurants.Where(x => x.IsActive)
            .OrderByDescending(x => x.TotalOrders).Take(r.Count)
            .Select(x => new RestaurantListDto(x.Id, x.Name, x.LogoUrl, x.CoverImageUrl,
                x.CuisineType, x.PriceRange, x.AverageRating, x.TotalReviews,
                x.MinOrderAmount, x.DeliveryFee, x.AverageDeliveryMinutes, 0, x.IsOpen, false))
            .ToListAsync(ct);
        return ApiResponse<List<RestaurantListDto>>.Ok(items);
    }
}

public class GetFavoriteRestaurantsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetFavoriteRestaurantsQuery, ApiResponse<List<RestaurantListDto>>>
{
    public async Task<ApiResponse<List<RestaurantListDto>>> Handle(GetFavoriteRestaurantsQuery r, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var favIds = await db.UserFavorites.Where(f => f.UserId == userId && f.EntityType == "Restaurant")
            .Select(f => f.EntityId).ToListAsync(ct);
        var items = await db.Restaurants.Where(x => favIds.Contains(x.Id))
            .Select(x => new RestaurantListDto(x.Id, x.Name, x.LogoUrl, x.CoverImageUrl,
                x.CuisineType, x.PriceRange, x.AverageRating, x.TotalReviews,
                x.MinOrderAmount, x.DeliveryFee, x.AverageDeliveryMinutes, 0, x.IsOpen, false))
            .ToListAsync(ct);
        return ApiResponse<List<RestaurantListDto>>.Ok(items);
    }
}

public class GetRestaurantMenuQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetRestaurantMenuQuery, ApiResponse<List<MenuCategoryDto>>>
{
    public async Task<ApiResponse<List<MenuCategoryDto>>> Handle(GetRestaurantMenuQuery r, CancellationToken ct)
    {
        var cats = await db.MenuCategories.Where(c => c.RestaurantId == r.RestaurantId && !c.IsDeleted)
            .Include(c => c.Items.Where(i => !i.IsDeleted)).ThenInclude(i => i.Options)
            .OrderBy(c => c.SortOrder).ToListAsync(ct);
        var dtos = cats.Select(c => new MenuCategoryDto(c.Id, c.Name, c.Description, c.ImageUrl,
            c.Items.OrderBy(i => i.SortOrder).Select(i => new MenuItemDto(i.Id, i.Name, i.Description, i.Price, i.ImageUrl,
                i.IsAvailable, i.IsPopular, i.PreparationMinutes, string.Join(",", i.Allergens), 0,
                i.Options.Select(o => new MenuItemOptionDto(o.Id, o.GroupName, o.Name, o.PriceModifier, false, false, 1)).ToList()
            )).ToList()
        )).ToList();
        return ApiResponse<List<MenuCategoryDto>>.Ok(dtos);
    }
}

public class GetRestaurantOrdersQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetRestaurantOrdersQuery, ApiResponse<PaginatedList<RestaurantOrderDto>>>
{
    public async Task<ApiResponse<PaginatedList<RestaurantOrderDto>>> Handle(GetRestaurantOrdersQuery r, CancellationToken ct)
    {
        var q = db.RestaurantOrders.Include(o => o.Items).Include(o => o.Restaurant).AsQueryable();
        if (r.RestaurantId.HasValue) q = q.Where(o => o.RestaurantId == r.RestaurantId.Value);
        if (!string.IsNullOrEmpty(r.Status) && Enum.TryParse<RestaurantOrderStatus>(r.Status, out var s))
            q = q.Where(o => o.Status == s);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(o => new RestaurantOrderDto(o.Id, o.OrderNumber, o.Status.ToString(), o.Restaurant.Name,
                o.SubTotal, o.DeliveryFee, o.Discount, o.TipAmount, o.TotalAmount,
                o.PaymentMethod.ToString(), o.PaymentStatus.ToString(), o.CreatedAt, o.ScheduledFor,
                o.Items.Select(i => new RestaurantOrderItemDto(i.Name, i.Quantity, i.UnitPrice, i.TotalPrice, i.SelectedOptionsJson, i.SpecialInstructions)).ToList(), null))
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<RestaurantOrderDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetRestaurantOrderDetailQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetRestaurantOrderDetailQuery, ApiResponse<RestaurantOrderDto>>
{
    public async Task<ApiResponse<RestaurantOrderDto>> Handle(GetRestaurantOrderDetailQuery r, CancellationToken ct)
    {
        var o = await db.RestaurantOrders.Include(x => x.Items).Include(x => x.Restaurant)
            .FirstOrDefaultAsync(x => x.Id == r.OrderId, ct);
        if (o == null) return ApiResponse<RestaurantOrderDto>.Fail("Commande introuvable");
        DeliveryDriverInfoDto? driver = null;
        if (o.DelivererId.HasValue)
        {
            var d = await db.Deliverers.FirstOrDefaultAsync(x => x.UserId == o.DelivererId.Value, ct);
            if (d != null) driver = new DeliveryDriverInfoDto(d.FirstName + " " + d.LastName, d.PhotoUrl, d.Phone, d.CurrentLatitude, d.CurrentLongitude);
        }
        return ApiResponse<RestaurantOrderDto>.Ok(new RestaurantOrderDto(o.Id, o.OrderNumber, o.Status.ToString(), o.Restaurant.Name,
            o.SubTotal, o.DeliveryFee, o.Discount, o.TipAmount, o.TotalAmount,
            o.PaymentMethod.ToString(), o.PaymentStatus.ToString(), o.CreatedAt, o.ScheduledFor,
            o.Items.Select(i => new RestaurantOrderItemDto(i.Name, i.Quantity, i.UnitPrice, i.TotalPrice, i.SelectedOptionsJson, i.SpecialInstructions)).ToList(), driver));
    }
}

public class GetMyRestaurantOrdersQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetMyRestaurantOrdersQuery, ApiResponse<PaginatedList<RestaurantOrderDto>>>
{
    public async Task<ApiResponse<PaginatedList<RestaurantOrderDto>>> Handle(GetMyRestaurantOrdersQuery r, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var q = db.RestaurantOrders.Include(o => o.Items).Include(o => o.Restaurant).Where(o => o.CustomerId == userId);
        if (!string.IsNullOrEmpty(r.Status) && Enum.TryParse<RestaurantOrderStatus>(r.Status, out var s))
            q = q.Where(o => o.Status == s);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(o => new RestaurantOrderDto(o.Id, o.OrderNumber, o.Status.ToString(), o.Restaurant.Name,
                o.SubTotal, o.DeliveryFee, o.Discount, o.TipAmount, o.TotalAmount,
                o.PaymentMethod.ToString(), o.PaymentStatus.ToString(), o.CreatedAt, null,
                o.Items.Select(i => new RestaurantOrderItemDto(i.Name, i.Quantity, i.UnitPrice, i.TotalPrice, null, null)).ToList(), null))
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<RestaurantOrderDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetRestaurantReviewsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetRestaurantReviewsQuery, ApiResponse<PaginatedList<RestaurantReviewDto>>>
{
    public async Task<ApiResponse<PaginatedList<RestaurantReviewDto>>> Handle(GetRestaurantReviewsQuery r, CancellationToken ct)
    {
        var q = db.Reviews.Include(rv => rv.User).Where(rv => rv.EntityId == r.RestaurantId && rv.EntityType == "Restaurant");
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(rv => rv.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(rv => new RestaurantReviewDto(rv.Id, rv.User.FirstName + " " + rv.User.LastName,
                rv.Stars, rv.Stars, rv.Stars, rv.Comment, rv.Reply, rv.CreatedAt))
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<RestaurantReviewDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetRestaurantStatsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetRestaurantStatsQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetRestaurantStatsQuery r, CancellationToken ct)
    {
        var q = db.RestaurantOrders.Where(o => o.RestaurantId == r.RestaurantId && o.Status != RestaurantOrderStatus.Cancelled);
        if (r.From.HasValue) q = q.Where(o => o.CreatedAt >= r.From.Value);
        if (r.To.HasValue) q = q.Where(o => o.CreatedAt <= r.To.Value.AddDays(1));
        var orders = await q.ToListAsync(ct);
        return ApiResponse<object>.Ok(new
        {
            TotalOrders = orders.Count,
            TotalRevenue = orders.Sum(o => o.TotalAmount),
            AverageBasket = orders.Count > 0 ? orders.Average(o => o.TotalAmount) : 0,
            TotalCommissions = orders.Sum(o => o.CommissionAmount),
            DeliveredCount = orders.Count(o => o.Status == RestaurantOrderStatus.Delivered),
            CancelledCount = await db.RestaurantOrders.CountAsync(o => o.RestaurantId == r.RestaurantId && o.Status == RestaurantOrderStatus.Cancelled, ct)
        });
    }
}

public class GetMyRestaurantsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetMyRestaurantsQuery, ApiResponse<List<RestaurantListDto>>>
{
    public async Task<ApiResponse<List<RestaurantListDto>>> Handle(GetMyRestaurantsQuery r, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var items = await db.Restaurants.Where(x => x.OwnerId == userId)
            .Select(x => new RestaurantListDto(x.Id, x.Name, x.LogoUrl, x.CoverImageUrl,
                x.CuisineType, x.PriceRange, x.AverageRating, x.TotalReviews,
                x.MinOrderAmount, x.DeliveryFee, x.AverageDeliveryMinutes, 0, x.IsOpen, false))
            .ToListAsync(ct);
        return ApiResponse<List<RestaurantListDto>>.Ok(items);
    }
}
