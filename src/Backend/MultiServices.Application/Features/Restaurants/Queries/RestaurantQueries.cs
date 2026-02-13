using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Restaurant;
using MultiServices.Domain.Enums;

namespace MultiServices.Application.Features.Restaurants.Queries;

public record SearchRestaurantsQuery(string? Query, CuisineType? Cuisine, PriceRange? Price,
    double? MaxDistance, double? Lat, double? Lng, double? MinRating, bool? HasPromo,
    string? SortBy, int Page = 1, int PageSize = 20) : IRequest<ApiResponse<PaginatedList<RestaurantListDto>>>;

public record GetRestaurantDetailQuery(Guid Id) : IRequest<ApiResponse<RestaurantDetailDto>>;
public record GetNearbyRestaurantsQuery(double Lat, double Lng, double RadiusKm = 5) : IRequest<ApiResponse<List<RestaurantListDto>>>;
public record GetPopularRestaurantsQuery(int Count = 10) : IRequest<ApiResponse<List<RestaurantListDto>>>;
public record GetFavoriteRestaurantsQuery : IRequest<ApiResponse<List<RestaurantListDto>>>;
public record GetRestaurantMenuQuery(Guid RestaurantId) : IRequest<ApiResponse<List<MenuCategoryDto>>>;
public record GetRestaurantOrdersQuery(Guid? RestaurantId, string? Status, int Page = 1, int PageSize = 20) : IRequest<ApiResponse<PaginatedList<RestaurantOrderDto>>>;
public record GetRestaurantOrderDetailQuery(Guid OrderId) : IRequest<ApiResponse<RestaurantOrderDto>>;
public record GetMyRestaurantOrdersQuery(string? Status, int Page = 1, int PageSize = 20) : IRequest<ApiResponse<PaginatedList<RestaurantOrderDto>>>;
public record GetRestaurantReviewsQuery(Guid RestaurantId, int Page = 1, int PageSize = 20) : IRequest<ApiResponse<PaginatedList<RestaurantReviewDto>>>;
public record GetRestaurantStatsQuery(Guid RestaurantId, DateTime? From, DateTime? To) : IRequest<ApiResponse<object>>;
public record GetMyRestaurantsQuery : IRequest<ApiResponse<List<RestaurantListDto>>>;
