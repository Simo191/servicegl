using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Delivery;

namespace MultiServices.Application.Features.Delivery.Queries;

public record GetDriverProfileQuery : IRequest<ApiResponse<DeliveryDriverDto>>;
public record GetAvailableDeliveriesQuery(double Lat, double Lng, double RadiusKm = 10) : IRequest<ApiResponse<List<AvailableDeliveryDto>>>;
public record GetDriverEarningsQuery(DateTime? From, DateTime? To) : IRequest<ApiResponse<List<DeliveryEarningDto>>>;
public record GetDriverStatsQuery : IRequest<ApiResponse<object>>;
public record GetActiveDeliveryQuery : IRequest<ApiResponse<object>>;
