using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Service;
using MultiServices.Application.Features.Services.Queries;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Services.Handlers;

public class SearchServiceProvidersQueryHandler(ApplicationDbContext db)
    : IRequestHandler<SearchServiceProvidersQuery, ApiResponse<PaginatedList<ServiceProviderListDto>>>
{
    public async Task<ApiResponse<PaginatedList<ServiceProviderListDto>>> Handle(SearchServiceProvidersQuery r, CancellationToken ct)
    {
        var q = db.ServiceProviders.Where(p => p.IsActive && !p.IsDeleted).AsQueryable();
        if (r.Category.HasValue) q = q.Where(p => p.PrimaryCategory == r.Category.Value || p.AdditionalCategories.Contains(r.Category.Value));
        if (r.MinRating.HasValue) q = q.Where(p => p.AverageRating >= r.MinRating.Value);
        if (!string.IsNullOrEmpty(r.City)) q = q.Where(p => p.City == r.City || p.InterventionCities.Contains(r.City));
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(p => p.AverageRating).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(p => new ServiceProviderListDto(p.Id, p.CompanyName, p.LogoUrl,
                p.AverageRating, p.TotalReviews, p.YearsOfExperience, p.IsAvailable,
                new List<string> { p.PrimaryCategory.ToString() },
                p.Services.Min(s => s.FixedPrice ?? s.HourlyRate),
                p.InterventionCities)).ToListAsync(ct);
        return ApiResponse<PaginatedList<ServiceProviderListDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetServiceProviderDetailQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetServiceProviderDetailQuery, ApiResponse<ServiceProviderDetailDto>>
{
    public async Task<ApiResponse<ServiceProviderDetailDto>> Handle(GetServiceProviderDetailQuery r, CancellationToken ct)
    {
        var p = await db.ServiceProviders
            .Include(x => x.Services.Where(s => s.IsActive))
            .Include(x => x.Portfolio)
            .Include(x => x.Availabilities)
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct);
        if (p == null) return ApiResponse<ServiceProviderDetailDto>.Fail("Prestataire introuvable");
        var reviews = await db.Reviews.Include(rv => rv.User)
            .Where(rv => rv.EntityId == p.Id && rv.EntityType == "ServiceProvider")
            .OrderByDescending(rv => rv.CreatedAt).Take(10).ToListAsync(ct);
        return ApiResponse<ServiceProviderDetailDto>.Ok(new ServiceProviderDetailDto(p.Id, p.CompanyName, p.Description,
            p.LogoUrl, p.CoverImageUrl, p.AverageRating, p.TotalReviews, p.YearsOfExperience, p.IsAvailable, p.Phone,
            p.Services.Select(s => new ServiceOfferingDto(s.Id, s.Category, s.Name, s.Description,
                s.HourlyRate, s.FixedPrice, null, s.EstimatedDurationMinutes)).ToList(),
            p.Certifications,
            p.Portfolio.Select(i => new PortfolioItemDto(i.Id, i.Title, i.Description,
                i.AfterImageUrls.FirstOrDefault() ?? "", i.BeforeImageUrls.FirstOrDefault())).ToList(),
            reviews.Select(rv => new ServiceProviderReviewDto(rv.Id, rv.User.FirstName + " " + rv.User.LastName,
                rv.Stars, rv.Stars, rv.Stars, rv.Comment, rv.Reply, rv.CreatedAt)).ToList(),
            p.InterventionCities,
            p.Availabilities.Select(a => new AvailabilitySlotDto(a.DayOfWeek, a.StartTime.ToString(), a.EndTime.ToString(), a.IsAvailable)).ToList()));
    }
}

public class GetServiceBookingsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetServiceBookingsQuery, ApiResponse<PaginatedList<ServiceBookingDto>>>
{
    public async Task<ApiResponse<PaginatedList<ServiceBookingDto>>> Handle(GetServiceBookingsQuery r, CancellationToken ct)
    {
        var q = db.ServiceInterventions.Include(i => i.Provider).Include(i => i.ServiceOffering).AsQueryable();
        if (r.ProviderId.HasValue) q = q.Where(i => i.ProviderId == r.ProviderId.Value);
        if (!string.IsNullOrEmpty(r.Status) && Enum.TryParse<InterventionStatus>(r.Status, out var s))
            q = q.Where(i => i.Status == s);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(i => i.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(i => new ServiceBookingDto(i.Id, i.InterventionNumber, i.Status.ToString(),
                i.Provider.CompanyName, i.ServiceOffering.Name, i.ProblemDescription,
                i.EstimatedPrice, i.FinalPrice, i.ScheduledDate,
                i.ScheduledStartTime.ToString(), $"{i.InterventionStreet}, {i.InterventionCity}",
                i.PaymentStatus.ToString(), null, i.InterventionReport, null, null)).ToListAsync(ct);
        return ApiResponse<PaginatedList<ServiceBookingDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetMyServiceBookingsQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetMyServiceBookingsQuery, ApiResponse<PaginatedList<ServiceBookingDto>>>
{
    public async Task<ApiResponse<PaginatedList<ServiceBookingDto>>> Handle(GetMyServiceBookingsQuery r, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var q = db.ServiceInterventions.Include(i => i.Provider).Include(i => i.ServiceOffering)
            .Where(i => i.CustomerId == userId);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(i => i.CreatedAt).Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
            .Select(i => new ServiceBookingDto(i.Id, i.InterventionNumber, i.Status.ToString(),
                i.Provider.CompanyName, i.ServiceOffering.Name, i.ProblemDescription,
                i.EstimatedPrice, i.FinalPrice, i.ScheduledDate,
                i.ScheduledStartTime.ToString(), $"{i.InterventionStreet}, {i.InterventionCity}",
                i.PaymentStatus.ToString(), null, null, null, null)).ToListAsync(ct);
        return ApiResponse<PaginatedList<ServiceBookingDto>>.Ok(new(items, total, r.Page, r.PageSize));
    }
}

public class GetServiceBookingDetailQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetServiceBookingDetailQuery, ApiResponse<ServiceBookingDto>>
{
    public async Task<ApiResponse<ServiceBookingDto>> Handle(GetServiceBookingDetailQuery r, CancellationToken ct)
    {
        var i = await db.ServiceInterventions
            .Include(x => x.Provider).Include(x => x.ServiceOffering).Include(x => x.AssignedTeamMember)
            .FirstOrDefaultAsync(x => x.Id == r.BookingId, ct);
        if (i == null) return ApiResponse<ServiceBookingDto>.Fail("Reservation introuvable");
        WorkerInfoDto? worker = null;
        if (i.AssignedTeamMember != null)
            worker = new WorkerInfoDto(i.AssignedTeamMember.FirstName + " " + i.AssignedTeamMember.LastName,
                i.AssignedTeamMember.PhotoUrl, i.AssignedTeamMember.Phone, null, null);
        return ApiResponse<ServiceBookingDto>.Ok(new ServiceBookingDto(i.Id, i.InterventionNumber, i.Status.ToString(),
            i.Provider.CompanyName, i.ServiceOffering.Name, i.ProblemDescription,
            i.EstimatedPrice, i.FinalPrice, i.ScheduledDate,
            i.ScheduledStartTime.ToString(), $"{i.InterventionStreet}, {i.InterventionCity}",
            i.PaymentStatus.ToString(), worker, i.InterventionReport,
            i.BeforeImageUrls.Any() ? string.Join(",", i.BeforeImageUrls) : null,
            i.AfterImageUrls.Any() ? string.Join(",", i.AfterImageUrls) : null));
    }
}

public class GetServiceCategoriesQueryHandler
    : IRequestHandler<GetServiceCategoriesQuery, ApiResponse<List<object>>>
{
    public Task<ApiResponse<List<object>>> Handle(GetServiceCategoriesQuery r, CancellationToken ct)
    {
        var categories = Enum.GetValues<ServiceCategory>()
            .Select(c => (object)new { Value = (int)c, Name = c.ToString() }).ToList();
        return Task.FromResult(ApiResponse<List<object>>.Ok(categories));
    }
}
