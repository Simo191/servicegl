using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Service;
using MultiServices.Domain.Enums;

namespace MultiServices.Application.Features.Services.Queries;

public record SearchServiceProvidersQuery(ServiceCategory? Category, double? MinRating,
    string? City, decimal? MaxPrice, string? SortBy, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedList<ServiceProviderListDto>>>;

public record GetServiceProviderDetailQuery(Guid Id) : IRequest<ApiResponse<ServiceProviderDetailDto>>;
public record GetAvailableProvidersQuery(ServiceCategory Category, DateTime Date, string City)
    : IRequest<ApiResponse<List<ServiceProviderListDto>>>;
public record GetServiceBookingsQuery(Guid? ProviderId, string? Status, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedList<ServiceBookingDto>>>;
public record GetServiceBookingDetailQuery(Guid BookingId) : IRequest<ApiResponse<ServiceBookingDto>>;
public record GetMyServiceBookingsQuery(string? Status, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedList<ServiceBookingDto>>>;
public record GetMyServiceProviderQuery : IRequest<ApiResponse<ServiceProviderDetailDto>>;
public record GetProviderStatsQuery(Guid ProviderId, DateTime? From, DateTime? To) : IRequest<ApiResponse<object>>;
public record GetProviderEarningsQuery(Guid ProviderId, DateTime? From, DateTime? To) : IRequest<ApiResponse<object>>;
public record GetServiceCategoriesQuery : IRequest<ApiResponse<List<object>>>;
