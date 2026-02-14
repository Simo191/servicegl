using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Admin;
using MultiServices.Application.Features.Admin.Queries;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;

namespace MultiServices.Application.Features.Admin.Handlers;

public class GetDashboardQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetDashboardQuery, ApiResponse<DashboardDto>>
{
    public async Task<ApiResponse<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var dto = new DashboardDto
        {
            TotalOrders = await db.RestaurantOrders.CountAsync(ct) + await db.GroceryOrders.CountAsync(ct) + await db.ServiceInterventions.CountAsync(ct),
            TotalOrdersToday = await db.RestaurantOrders.CountAsync(o => o.CreatedAt.Date == today, ct)
                + await db.GroceryOrders.CountAsync(o => o.CreatedAt.Date == today, ct)
                + await db.ServiceInterventions.CountAsync(o => o.CreatedAt.Date == today, ct),
            TotalRevenue = await db.RestaurantOrders.Where(o => o.Status == RestaurantOrderStatus.Delivered).SumAsync(o => o.TotalAmount, ct)
                + await db.GroceryOrders.Where(o => o.Status == GroceryOrderStatus.Delivered).SumAsync(o => o.TotalAmount, ct)
                + await db.ServiceInterventions.Where(o => o.Status == InterventionStatus.Completed).SumAsync(o => o.TotalAmount, ct),
            ActiveClients = await db.Users.CountAsync(u => u.IsActive, ct),
            ActiveRestaurants = await db.Restaurants.CountAsync(r => r.IsActive, ct),
            ActiveServiceProviders = await db.ServiceProviders.CountAsync(s => s.IsActive, ct),
            ActiveGroceryStores = await db.GroceryStores.CountAsync(g => g.IsActive, ct),
            ActiveDeliverers = await db.Deliverers.CountAsync(d => d.IsActive, ct),
            PendingApprovals = await db.Restaurants.CountAsync(r => r.VerificationStatus == VerificationStatus.Pending, ct)
                + await db.ServiceProviders.CountAsync(s => s.VerificationStatus == VerificationStatus.Pending, ct)
                + await db.GroceryStores.CountAsync(g => g.VerificationStatus == VerificationStatus.Pending, ct)
                + await db.Deliverers.CountAsync(d => d.VerificationStatus == VerificationStatus.Pending, ct),
            OpenTickets = await db.SupportTickets.CountAsync(t => t.Status == "Open" || t.Status == "InProgress", ct)
        };
        return ApiResponse<DashboardDto>.Ok(dto);
    }
}

public class GetPendingApprovalsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetPendingApprovalsQuery, ApiResponse<PaginatedList<ApprovalDto>>>
{
    public async Task<ApiResponse<PaginatedList<ApprovalDto>>> Handle(GetPendingApprovalsQuery r, CancellationToken ct)
    {
        var items = new List<ApprovalDto>();
        if (r.EntityType == null || r.EntityType == "Restaurant")
            items.AddRange(await db.Restaurants.Include(x => x.Owner)
                .Where(x => x.VerificationStatus == VerificationStatus.Pending)
                .Select(x => new ApprovalDto(x.Id, "Restaurant", x.Name, x.Owner.FirstName + " " + x.Owner.LastName, "Pending", x.CreatedAt))
                .ToListAsync(ct));
        if (r.EntityType == null || r.EntityType == "ServiceProvider")
            items.AddRange(await db.ServiceProviders.Include(x => x.Owner)
                .Where(x => x.VerificationStatus == VerificationStatus.Pending)
                .Select(x => new ApprovalDto(x.Id, "ServiceProvider", x.CompanyName, x.Owner.FirstName + " " + x.Owner.LastName, "Pending", x.CreatedAt))
                .ToListAsync(ct));
        if (r.EntityType == null || r.EntityType == "GroceryStore")
            items.AddRange(await db.GroceryStores.Include(x => x.Owner)
                .Where(x => x.VerificationStatus == VerificationStatus.Pending)
                .Select(x => new ApprovalDto(x.Id, "GroceryStore", x.Name, x.Owner.FirstName + " " + x.Owner.LastName, "Pending", x.CreatedAt))
                .ToListAsync(ct));
        if (r.EntityType == null || r.EntityType == "Deliverer")
            items.AddRange(await db.Deliverers
                .Where(x => x.VerificationStatus == VerificationStatus.Pending)
                .Select(x => new ApprovalDto(x.Id, "Deliverer", x.FirstName + " " + x.LastName, null, "Pending", x.CreatedAt))
                .ToListAsync(ct));
        var total = items.Count;
        var paged = items.OrderByDescending(x => x.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize).ToList();
        return ApiResponse<PaginatedList<ApprovalDto>>.Ok(new(paged, total, r.Page, r.PageSize));
    }
}

public class GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    : IRequestHandler<GetAllUsersQuery, ApiResponse<PaginatedList<UserListDto>>>
{
    public async Task<ApiResponse<PaginatedList<UserListDto>>> Handle(GetAllUsersQuery r, CancellationToken ct)
    {
        var q = db.Users.AsQueryable();
        if (!string.IsNullOrEmpty(r.Search))
            q = q.Where(u => u.FirstName.Contains(r.Search) || u.LastName.Contains(r.Search) || u.Email!.Contains(r.Search));
        if (r.IsActive.HasValue) q = q.Where(u => u.IsActive == r.IsActive.Value);
        var total = await q.CountAsync(ct);
        var users = await q.OrderByDescending(u => u.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize).ToListAsync(ct);
        var items = new List<UserListDto>();
        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            if (!string.IsNullOrEmpty(r.Role) && !roles.Contains(r.Role)) continue;
            var orderCount = await db.RestaurantOrders.CountAsync(o => o.CustomerId == u.Id, ct)
                + await db.GroceryOrders.CountAsync(o => o.CustomerId == u.Id, ct);
            items.Add(new UserListDto(u.Id, u.FullName, u.Email!, u.PhoneNumber,
                u.IsActive, u.IsVerified, roles.ToList(), u.CreatedAt, orderCount));
        }
        return ApiResponse<PaginatedList<UserListDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetFinancialReportQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetFinancialReportQuery, ApiResponse<FinancialReportDto>>
{
    public async Task<ApiResponse<FinancialReportDto>> Handle(GetFinancialReportQuery r, CancellationToken ct)
    {
        var restRevenue = await db.RestaurantOrders
            .Where(o => o.Status == RestaurantOrderStatus.Delivered && o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1))
            .SumAsync(o => o.TotalAmount, ct);
        var grocRevenue = await db.GroceryOrders
            .Where(o => o.Status == GroceryOrderStatus.Delivered && o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1))
            .SumAsync(o => o.TotalAmount, ct);
        var svcRevenue = await db.ServiceInterventions
            .Where(o => o.Status == InterventionStatus.Completed && o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1))
            .SumAsync(o => o.TotalAmount, ct);
        var totalCommissions = await db.RestaurantOrders.Where(o => o.Status == RestaurantOrderStatus.Delivered && o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1)).SumAsync(o => o.CommissionAmount, ct)
            + await db.GroceryOrders.Where(o => o.Status == GroceryOrderStatus.Delivered && o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1)).SumAsync(o => o.CommissionAmount, ct)
            + await db.ServiceInterventions.Where(o => o.Status == InterventionStatus.Completed && o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1)).SumAsync(o => o.CommissionAmount, ct);
        var totalRefunds = await db.PaymentTransactions.Where(t => t.RefundedAt >= r.StartDate && t.RefundedAt <= r.EndDate.AddDays(1)).SumAsync(t => t.RefundedAmount ?? 0, ct);
        var totalOrders = await db.RestaurantOrders.CountAsync(o => o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1), ct)
            + await db.GroceryOrders.CountAsync(o => o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1), ct)
            + await db.ServiceInterventions.CountAsync(o => o.CreatedAt >= r.StartDate && o.CreatedAt <= r.EndDate.AddDays(1), ct);

        return ApiResponse<FinancialReportDto>.Ok(new FinancialReportDto(
            restRevenue + grocRevenue + svcRevenue, restRevenue, grocRevenue, svcRevenue,
            totalCommissions, totalRefunds, totalOrders, 0));
    }
}

public class GetAllOrdersQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetAllOrdersQuery, ApiResponse<PaginatedList<object>>>
{
    public async Task<ApiResponse<PaginatedList<object>>> Handle(GetAllOrdersQuery r, CancellationToken ct)
    {
        var items = new List<object>();
        if (r.Type == null || r.Type == "Restaurant")
        {
            var rq = db.RestaurantOrders.Include(o => o.Restaurant).Include(o => o.Customer).AsQueryable();
            if (r.From.HasValue) rq = rq.Where(o => o.CreatedAt >= r.From.Value);
            if (r.To.HasValue) rq = rq.Where(o => o.CreatedAt <= r.To.Value.AddDays(1));
            items.AddRange(await rq.OrderByDescending(o => o.CreatedAt).Take(r.PageSize)
                .Select(o => (object)new { o.Id, o.OrderNumber, Type = "Restaurant", Customer = o.Customer.FirstName + " " + o.Customer.LastName,
                    Provider = o.Restaurant.Name, Status = o.Status.ToString(), o.TotalAmount, o.CreatedAt })
                .ToListAsync(ct));
        }
        if (r.Type == null || r.Type == "Grocery")
        {
            var gq = db.GroceryOrders.Include(o => o.Store).Include(o => o.Customer).AsQueryable();
            if (r.From.HasValue) gq = gq.Where(o => o.CreatedAt >= r.From.Value);
            if (r.To.HasValue) gq = gq.Where(o => o.CreatedAt <= r.To.Value.AddDays(1));
            items.AddRange(await gq.OrderByDescending(o => o.CreatedAt).Take(r.PageSize)
                .Select(o => (object)new { o.Id, o.OrderNumber, Type = "Grocery", Customer = o.Customer.FirstName + " " + o.Customer.LastName,
                    Provider = o.Store.Name, Status = o.Status.ToString(), o.TotalAmount, o.CreatedAt })
                .ToListAsync(ct));
        }
        return ApiResponse<PaginatedList<object>>.Ok(new(items.OrderByDescending(x => ((dynamic)x).CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize).ToList(), items.Count, r.Page, r.PageSize));
    }
}

public class GetSystemLogsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetSystemLogsQuery, ApiResponse<PaginatedList<object>>>
{
    public async Task<ApiResponse<PaginatedList<object>>> Handle(GetSystemLogsQuery r, CancellationToken ct)
    {
        var q = db.AuditLogs.AsQueryable();
        if (r.From.HasValue) q = q.Where(l => l.CreatedAt >= r.From.Value);
        if (r.To.HasValue) q = q.Where(l => l.CreatedAt <= r.To.Value.AddDays(1));
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(l => l.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(l => (object)new { l.Id, l.UserId, l.Action, l.EntityType, l.EntityId, l.IpAddress, l.CreatedAt })
            .ToListAsync(ct);
        return ApiResponse<PaginatedList<object>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}
