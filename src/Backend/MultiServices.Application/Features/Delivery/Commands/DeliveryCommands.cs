using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Delivery;

namespace MultiServices.Application.Features.Delivery.Commands;

public record RegisterDriverCommand(DriverRegistrationDto Dto) : IRequest<ApiResponse<Guid>>;
public record GoOnlineCommand : IRequest<ApiResponse>;
public record GoOfflineCommand : IRequest<ApiResponse>;
public record UpdateDriverLocationCommand(UpdateLocationDto Dto) : IRequest<ApiResponse>;
public record AcceptDeliveryCommand(Guid DeliveryId, string Type) : IRequest<ApiResponse>;
public record RejectDeliveryCommand(Guid DeliveryId, string Type) : IRequest<ApiResponse>;
public record UpdateDeliveryStatusCommand(Guid DeliveryId, string Type, UpdateDeliveryStatusDto Dto) : IRequest<ApiResponse>;
public record SubmitDeliveryProofCommand(Guid DeliveryId, string Type, Stream Image, string FileName) : IRequest<ApiResponse<string>>;
public record RequestPayoutCommand(decimal Amount) : IRequest<ApiResponse>;
public record TriggerSosCommand(double Latitude, double Longitude) : IRequest<ApiResponse>;
