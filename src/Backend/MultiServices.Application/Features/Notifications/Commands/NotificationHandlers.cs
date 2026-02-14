using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.Features.Notifications.Commands;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Notifications.Handlers;

public class MarkNotificationReadCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<MarkNotificationReadCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var n = await db.Notifications.FirstOrDefaultAsync(x => x.Id == request.NotificationId && x.UserId == userId, ct);
        if (n == null) return ApiResponse.Fail("Notification introuvable");
        n.IsRead = true; n.ReadAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Notification lue");
    }
}

public class MarkAllNotificationsReadCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<MarkAllNotificationsReadCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(MarkAllNotificationsReadCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var unread = await db.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync(ct);
        foreach (var n in unread) { n.IsRead = true; n.ReadAt = DateTime.UtcNow; }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok($"{unread.Count} notifications marquees lues");
    }
}

public class UpdateNotificationPreferencesCommandHandler
    : IRequestHandler<UpdateNotificationPreferencesCommand, ApiResponse>
{
    public Task<ApiResponse> Handle(UpdateNotificationPreferencesCommand request, CancellationToken ct)
    {
        // TODO: Store preferences in user settings
        return Task.FromResult(ApiResponse.Ok("Preferences mises a jour"));
    }
}

public class RegisterDeviceTokenCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<RegisterDeviceTokenCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RegisterDeviceTokenCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var existing = await db.Set<UserDevice>().FirstOrDefaultAsync(d => d.UserId == userId && d.Platform == request.Platform, ct);
        if (existing != null)
        {
            existing.DeviceToken = request.Token;
            existing.IsActive = true;
        }
        else
        {
            await db.Set<UserDevice>().AddAsync(new UserDevice
            {
                UserId = userId, DeviceToken = request.Token, Platform = request.Platform, IsActive = true
            }, ct);
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Token enregistre");
    }
}

public class GetNotificationsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetNotificationsQuery, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var q = db.Notifications.Where(n => n.UserId == userId && !n.IsDeleted);
        if (request.UnreadOnly == true) q = q.Where(n => !n.IsRead);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(n => new { n.Id, n.Title, n.Message, n.Type, n.IsRead, n.ImageUrl, n.ActionUrl, n.CreatedAt })
            .ToListAsync(ct);
        return ApiResponse<object>.Ok(new { Items = items, Total = total });
    }
}

public class GetUnreadCountQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetUnreadCountQuery, ApiResponse<int>>
{
    public async Task<ApiResponse<int>> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var count = await db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted, ct);
        return ApiResponse<int>.Ok(count);
    }
}
