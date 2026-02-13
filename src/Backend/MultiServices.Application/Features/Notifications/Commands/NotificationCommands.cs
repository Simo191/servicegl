using MediatR;
using MultiServices.Application.Common.Models;

namespace MultiServices.Application.Features.Notifications.Commands;

public record MarkNotificationReadCommand(Guid NotificationId) : IRequest<ApiResponse>;
public record MarkAllNotificationsReadCommand : IRequest<ApiResponse>;
public record UpdateNotificationPreferencesCommand(bool PushEnabled, bool EmailEnabled, bool SmsEnabled) : IRequest<ApiResponse>;
public record RegisterDeviceTokenCommand(string Token, string Platform) : IRequest<ApiResponse>;

public record GetNotificationsQuery(bool? UnreadOnly, int Page = 1, int PageSize = 20) : IRequest<ApiResponse<object>>;
public record GetUnreadCountQuery : IRequest<ApiResponse<int>>;
