using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Restaurant;
using MultiServices.Application.Features.Restaurants.Commands;
using MultiServices.Domain.Entities.Restaurant;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Restaurants.Handlers;

public class CreateRestaurantOrderCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<CreateRestaurantOrderCommand, ApiResponse<RestaurantOrderDto>>
{
    public async Task<ApiResponse<RestaurantOrderDto>> Handle(CreateRestaurantOrderCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var restaurant = await db.Restaurants.FindAsync(new object[] { request.Dto.RestaurantId }, ct);
        if (restaurant == null) return ApiResponse<RestaurantOrderDto>.Fail("Restaurant introuvable");
        if (!restaurant.IsOpen || !restaurant.AcceptingOrders) return ApiResponse<RestaurantOrderDto>.Fail("Restaurant ferme");

        var address = await db.UserAddresses.FindAsync(new object[] { request.Dto.DeliveryAddressId }, ct);
        if (address == null) return ApiResponse<RestaurantOrderDto>.Fail("Adresse introuvable");

        var order = new RestaurantOrder
        {
            OrderNumber = $"RST-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            CustomerId = userId, RestaurantId = request.Dto.RestaurantId,
            DeliveryStreet = address.Street, DeliveryCity = address.City,
            DeliveryLatitude = address.Latitude, DeliveryLongitude = address.Longitude,
            SpecialInstructions = request.Dto.SpecialInstructions,
            RequestCutlery = request.Dto.IncludeCutlery,
            PaymentMethod = request.Dto.PaymentMethod, TipAmount = request.Dto.Tip,
            PromoCode = request.Dto.PromoCode,
            Status = RestaurantOrderStatus.Received,
            DeliveryFee = restaurant.DeliveryFee,
            EstimatedDeliveryMinutes = restaurant.AverageDeliveryMinutes,
            IsScheduled = request.Dto.ScheduledDeliveryTime.HasValue,
            ScheduledFor = request.Dto.ScheduledDeliveryTime
        };

        decimal subTotal = 0;
        foreach (var item in request.Dto.Items)
        {
            var menuItem = await db.MenuItems.FindAsync(new object[] { item.MenuItemId }, ct);
            if (menuItem == null || !menuItem.IsAvailable) continue;
            var total = menuItem.Price * item.Quantity;
            order.Items.Add(new RestaurantOrderItem
            {
                MenuItemId = item.MenuItemId, Name = menuItem.Name,
                Quantity = item.Quantity, UnitPrice = menuItem.Price, TotalPrice = total,
                SpecialInstructions = item.SpecialInstructions
            });
            subTotal += total;
        }

        if (subTotal < restaurant.MinOrderAmount)
            return ApiResponse<RestaurantOrderDto>.Fail($"Minimum de commande: {restaurant.MinOrderAmount} MAD");

        if (subTotal >= restaurant.FreeDeliveryMinimum) order.DeliveryFee = 0;
        order.SubTotal = subTotal;
        order.TotalAmount = subTotal + order.DeliveryFee + order.TipAmount - order.Discount;
        order.CommissionAmount = subTotal * restaurant.CommissionRate / 100;
        order.StatusHistory.Add(new OrderStatusHistory { Status = RestaurantOrderStatus.Received.ToString(), Note = "Commande recue" });

        await db.RestaurantOrders.AddAsync(order, ct);
        restaurant.TotalOrders++;
        await db.SaveChangesAsync(ct);

        return ApiResponse<RestaurantOrderDto>.Ok(new RestaurantOrderDto(order.Id, order.OrderNumber,
            order.Status.ToString(), restaurant.Name, order.SubTotal, order.DeliveryFee, order.Discount,
            order.TipAmount, order.TotalAmount, order.PaymentMethod.ToString(), order.PaymentStatus.ToString(),
            order.CreatedAt, null,
            order.Items.Select(i => new RestaurantOrderItemDto(i.Name, i.Quantity, i.UnitPrice, i.TotalPrice, null, i.SpecialInstructions)).ToList(), null));
    }
}

public class CancelRestaurantOrderCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CancelRestaurantOrderCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CancelRestaurantOrderCommand request, CancellationToken ct)
    {
        var order = await db.RestaurantOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        if (order.Status > RestaurantOrderStatus.Received)
            return ApiResponse.Fail("Impossible d'annuler apres preparation");
        order.Status = RestaurantOrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow; order.CancellationReason = request.Reason;
        order.StatusHistory.Add(new OrderStatusHistory { Status = RestaurantOrderStatus.Cancelled.ToString(), Note = request.Reason });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commande annulee");
    }
}

public class AcceptRestaurantOrderCommandHandler(ApplicationDbContext db)
    : IRequestHandler<AcceptRestaurantOrderCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(AcceptRestaurantOrderCommand request, CancellationToken ct)
    {
        var order = await db.RestaurantOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        order.Status = RestaurantOrderStatus.Preparing;
        order.ConfirmedAt = DateTime.UtcNow;
        order.EstimatedDeliveryMinutes = request.EstimatedMinutes;
        order.StatusHistory.Add(new OrderStatusHistory { Status = RestaurantOrderStatus.Preparing.ToString(), Note = "Commande acceptee" });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commande acceptee");
    }
}

public class RejectRestaurantOrderCommandHandler(ApplicationDbContext db)
    : IRequestHandler<RejectRestaurantOrderCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RejectRestaurantOrderCommand request, CancellationToken ct)
    {
        var order = await db.RestaurantOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        order.Status = RestaurantOrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow; order.CancellationReason = request.Reason;
        order.StatusHistory.Add(new OrderStatusHistory { Status = RestaurantOrderStatus.Cancelled.ToString(), Note = $"Refusee: {request.Reason}" });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Commande refusee");
    }
}

public class UpdateOrderStatusCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateOrderStatusCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await db.RestaurantOrders.FindAsync(new object[] { request.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        if (!Enum.TryParse<RestaurantOrderStatus>(request.Status, out var newStatus))
            return ApiResponse.Fail("Statut invalide");
        order.Status = newStatus;
        switch (newStatus)
        {
            case RestaurantOrderStatus.Preparing: order.PreparingAt = DateTime.UtcNow; break;
            case RestaurantOrderStatus.Ready: order.ReadyAt = DateTime.UtcNow; break;
            case RestaurantOrderStatus.InTransit: order.PickedUpAt = DateTime.UtcNow; break;
            case RestaurantOrderStatus.Delivered: order.DeliveredAt = DateTime.UtcNow; break;
        }
        order.StatusHistory.Add(new OrderStatusHistory { Status = newStatus.ToString() });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok($"Statut: {newStatus}");
    }
}

public class CreateRestaurantReviewCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<CreateRestaurantReviewCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CreateRestaurantReviewCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var order = await db.RestaurantOrders.FindAsync(new object[] { request.Dto.OrderId }, ct);
        if (order == null) return ApiResponse.Fail("Commande introuvable");
        order.RestaurantRating = request.Dto.FoodRating;
        order.DeliveryRating = request.Dto.DeliveryRating;
        order.ReviewComment = request.Dto.Comment;
        var review = new Review
        {
            UserId = userId, EntityType = "Restaurant", EntityId = order.RestaurantId,
            OrderId = order.Id, Stars = request.Dto.OverallRating, Comment = request.Dto.Comment
        };
        await db.Reviews.AddAsync(review, ct);
        // Update restaurant average
        var restaurant = await db.Restaurants.FindAsync(new object[] { order.RestaurantId }, ct);
        if (restaurant != null)
        {
            var allReviews = await db.Reviews.Where(r => r.EntityId == restaurant.Id && r.EntityType == "Restaurant").ToListAsync(ct);
            restaurant.AverageRating = allReviews.Average(r => r.Stars);
            restaurant.TotalReviews = allReviews.Count;
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Avis soumis");
    }
}

public class ToggleFavoriteRestaurantCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<ToggleFavoriteRestaurantCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ToggleFavoriteRestaurantCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var existing = await db.UserFavorites.FirstOrDefaultAsync(f => f.UserId == userId && f.EntityType == "Restaurant" && f.EntityId == request.RestaurantId, ct);
        if (existing != null) { db.UserFavorites.Remove(existing); await db.SaveChangesAsync(ct); return ApiResponse.Ok("Retire des favoris"); }
        await db.UserFavorites.AddAsync(new UserFavorite { UserId = userId, EntityType = "Restaurant", EntityId = request.RestaurantId }, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Ajoute aux favoris");
    }
}

public class CreateRestaurantCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<CreateRestaurantCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateRestaurantCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var r = request.Dto;
        var restaurant = new RestaurantEntity
        {
            OwnerId = userId, Name = r.Name, Slug = r.Name.ToLower().Replace(" ", "-"),
            Description = r.Description ?? "", CuisineType = r.CuisineType,
            PriceRange = r.PriceRange, MinOrderAmount = r.MinOrderAmount,
            DeliveryFee = r.DeliveryFee, AverageDeliveryMinutes = r.EstimatedDeliveryMinutes,
            MaxDeliveryDistanceKm = r.MaxDeliveryDistanceKm,
            Street = r.Street, City = r.City, PostalCode = r.PostalCode,
            Latitude = r.Latitude, Longitude = r.Longitude, Phone = r.Phone,
            VerificationStatus = VerificationStatus.Pending
        };
        await db.Restaurants.AddAsync(restaurant, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(restaurant.Id);
    }
}

public class UpdateRestaurantCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateRestaurantCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateRestaurantCommand request, CancellationToken ct)
    {
        var restaurant = await db.Restaurants.FindAsync(new object[] { request.Id }, ct);
        if (restaurant == null) return ApiResponse.Fail("Restaurant introuvable");
        var r = request.Dto;
        restaurant.Name = r.Name; restaurant.Description = r.Description ?? "";
        restaurant.CuisineType = r.CuisineType; restaurant.PriceRange = r.PriceRange;
        restaurant.MinOrderAmount = r.MinOrderAmount; restaurant.DeliveryFee = r.DeliveryFee;
        restaurant.AverageDeliveryMinutes = r.EstimatedDeliveryMinutes;
        restaurant.Street = r.Street; restaurant.City = r.City; restaurant.Phone = r.Phone;
        restaurant.Latitude = r.Latitude; restaurant.Longitude = r.Longitude;
        restaurant.CoverImageUrl = r.CoverImageUrl; restaurant.LogoUrl = r.LogoUrl;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Restaurant mis a jour");
    }
}

public class ToggleRestaurantOpenCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ToggleRestaurantOpenCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ToggleRestaurantOpenCommand request, CancellationToken ct)
    {
        var r = await db.Restaurants.FindAsync(new object[] { request.RestaurantId }, ct);
        if (r == null) return ApiResponse.Fail("Introuvable");
        r.IsOpen = !r.IsOpen;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok(r.IsOpen ? "Ouvert" : "Ferme");
    }
}

public class CreateMenuCategoryCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CreateMenuCategoryCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateMenuCategoryCommand request, CancellationToken ct)
    {
        var cat = new MenuCategory
        {
            RestaurantId = request.RestaurantId, Name = request.Dto.Name,
            Description = request.Dto.Description, SortOrder = request.Dto.SortOrder
        };
        await db.MenuCategories.AddAsync(cat, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(cat.Id);
    }
}

public class UpdateMenuCategoryCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateMenuCategoryCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateMenuCategoryCommand request, CancellationToken ct)
    {
        var cat = await db.MenuCategories.FindAsync(new object[] { request.Id }, ct);
        if (cat == null) return ApiResponse.Fail("Categorie introuvable");
        cat.Name = request.Dto.Name; cat.Description = request.Dto.Description;
        cat.SortOrder = request.Dto.SortOrder;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Categorie mise a jour");
    }
}

public class DeleteMenuCategoryCommandHandler(ApplicationDbContext db)
    : IRequestHandler<DeleteMenuCategoryCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteMenuCategoryCommand request, CancellationToken ct)
    {
        var cat = await db.MenuCategories.FindAsync(new object[] { request.Id }, ct);
        if (cat == null) return ApiResponse.Fail("Introuvable");
        cat.IsDeleted = true; cat.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Categorie supprimee");
    }
}

public class CreateMenuItemCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CreateMenuItemCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateMenuItemCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var item = new MenuItem
        {
            RestaurantId = request.RestaurantId, CategoryId = d.CategoryId,
            Name = d.Name, Description = d.Description ?? "",
            Price = d.Price, PreparationMinutes = d.PreparationTimeMinutes,
            Allergens = d.Allergens?.Split(',').ToList() ?? new(),
            NutritionalInfoJson = d.Ingredients,
            IsAvailable = true, SortOrder = 0
        };
        if (d.Options != null)
        {
            foreach (var opt in d.Options)
                item.Options.Add(new MenuItemOption { GroupName = opt.GroupName, Name = opt.Name, PriceModifier = opt.AdditionalPrice });
        }
        await db.MenuItems.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(item.Id);
    }
}

public class UpdateMenuItemCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateMenuItemCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateMenuItemCommand request, CancellationToken ct)
    {
        var item = await db.MenuItems.FindAsync(new object[] { request.Id }, ct);
        if (item == null) return ApiResponse.Fail("Plat introuvable");
        var d = request.Dto;
        item.Name = d.Name; item.Description = d.Description ?? "";
        item.Price = d.Price; item.PreparationMinutes = d.PreparationTimeMinutes;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Plat mis a jour");
    }
}

public class DeleteMenuItemCommandHandler(ApplicationDbContext db)
    : IRequestHandler<DeleteMenuItemCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteMenuItemCommand request, CancellationToken ct)
    {
        var item = await db.MenuItems.FindAsync(new object[] { request.Id }, ct);
        if (item == null) return ApiResponse.Fail("Introuvable");
        item.IsDeleted = true; item.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Plat supprime");
    }
}

public class ToggleMenuItemAvailabilityCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ToggleMenuItemAvailabilityCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ToggleMenuItemAvailabilityCommand request, CancellationToken ct)
    {
        var item = await db.MenuItems.FindAsync(new object[] { request.Id }, ct);
        if (item == null) return ApiResponse.Fail("Introuvable");
        item.IsAvailable = !item.IsAvailable;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok(item.IsAvailable ? "Disponible" : "Indisponible");
    }
}

public class ReplyToReviewCommandHandler(ApplicationDbContext db)
    : IRequestHandler<ReplyToReviewCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ReplyToReviewCommand request, CancellationToken ct)
    {
        var review = await db.Reviews.FindAsync(new object[] { request.ReviewId }, ct);
        if (review == null) return ApiResponse.Fail("Avis introuvable");
        review.Reply = request.Reply; review.RepliedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Reponse ajoutee");
    }
}

public class CreateRestaurantPromotionCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CreateRestaurantPromotionCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateRestaurantPromotionCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var promo = new RestaurantPromotion
        {
            RestaurantId = request.RestaurantId, Title = d.Title, Description = d.Description,
            Code = d.PromoCode, DiscountType = d.DiscountPercentage.HasValue ? "Percentage" : "Fixed",
            DiscountValue = d.DiscountPercentage ?? d.DiscountAmount ?? 0,
            FreeDelivery = d.FreeDelivery, EndDate = d.EndDate,
            StartDate = DateTime.UtcNow, IsActive = true
        };
        await db.RestaurantPromotions.AddAsync(promo, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(promo.Id);
    }
}

public class UploadRestaurantImageCommandHandler : IRequestHandler<UploadRestaurantImageCommand, ApiResponse<string>>
{
    public Task<ApiResponse<string>> Handle(UploadRestaurantImageCommand request, CancellationToken ct)
    {
        var url = $"/uploads/restaurants/{request.RestaurantId}/{request.Type}/{Guid.NewGuid()}/{request.FileName}";
        return Task.FromResult(ApiResponse<string>.Ok(url));
    }
}
