using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Grocery;
using MultiServices.Domain.Enums;

namespace MultiServices.Application.Features.Grocery.Queries;

public record SearchStoresQuery(string? Query, double? MaxDistance, double? Lat, double? Lng,
    bool? HasPromo, bool? FreeDelivery, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedList<StoreListDto>>>;

public record GetStoreDetailQuery(Guid Id) : IRequest<ApiResponse<StoreDetailDto>>;
public record GetStoreCategoriesQuery(Guid StoreId) : IRequest<ApiResponse<List<StoreCategoryDto>>>;
public record SearchProductsQuery(Guid StoreId, string? Query, Guid? CategoryId, string? Brand,
    bool? IsBio, bool? IsHalal, bool? OnPromo, string? SortBy, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedList<ProductListDto>>>;
public record GetProductDetailQuery(Guid ProductId) : IRequest<ApiResponse<ProductDetailDto>>;
public record ScanBarcodeQuery(string Barcode, Guid StoreId) : IRequest<ApiResponse<ProductDetailDto>>;
public record GetGroceryOrdersQuery(Guid? StoreId, string? Status, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedList<GroceryOrderDto>>>;
public record GetGroceryOrderDetailQuery(Guid OrderId) : IRequest<ApiResponse<GroceryOrderDto>>;
public record GetMyGroceryOrdersQuery(string? Status, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedList<GroceryOrderDto>>>;
public record GetMyShoppingListsQuery : IRequest<ApiResponse<List<ShoppingListDto>>>;
public record GetShoppingListQuery(Guid Id) : IRequest<ApiResponse<ShoppingListDto>>;
public record GetStoreStatsQuery(Guid StoreId, DateTime? From, DateTime? To) : IRequest<ApiResponse<object>>;
public record GetFavoriteStoresQuery : IRequest<ApiResponse<List<StoreListDto>>>;
