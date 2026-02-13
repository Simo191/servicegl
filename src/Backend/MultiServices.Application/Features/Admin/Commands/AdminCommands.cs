using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Admin;

namespace MultiServices.Application.Features.Admin.Commands;

public record ApproveEntityCommand(ApproveEntityDto Dto) : IRequest<ApiResponse>;
public record SuspendUserCommand(Guid UserId, string Reason) : IRequest<ApiResponse>;
public record ActivateUserCommand(Guid UserId) : IRequest<ApiResponse>;
public record UpdateCommissionSettingsCommand(CommissionSettingsDto Dto) : IRequest<ApiResponse>;
public record CreateGlobalPromotionCommand(CreatePromotionDto Dto) : IRequest<ApiResponse<Guid>>;
public record SendBulkNotificationCommand(string Title, string Body, string? TargetRole, List<Guid>? TargetUserIds) : IRequest<ApiResponse>;
public record CancelOrderAdminCommand(Guid OrderId, string OrderType, string Reason) : IRequest<ApiResponse>;
public record ReassignDriverCommand(Guid OrderId, string OrderType, Guid NewDriverId) : IRequest<ApiResponse>;
public record ProcessRefundCommand(Guid OrderId, string OrderType, decimal Amount, string Reason) : IRequest<ApiResponse>;
public record UpdateGeoZonesCommand(List<object> Zones) : IRequest<ApiResponse>;
public record ExportFinancialReportCommand(DateTime StartDate, DateTime EndDate, string Format) : IRequest<ApiResponse<byte[]>>;
