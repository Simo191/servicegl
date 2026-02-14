using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Service;
using MultiServices.Application.Features.Services.Commands;
using MultiServices.Domain.Entities.Service;
using MultiServices.Domain.Enums;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Services.Handlers;

public class CreateServiceBookingCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<CreateServiceBookingCommand, ApiResponse<ServiceBookingDto>>
{
    public async Task<ApiResponse<ServiceBookingDto>> Handle(CreateServiceBookingCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var d = request.Dto;
        var provider = await db.ServiceProviders.FindAsync(new object[] { d.ProviderId }, ct);
        if (provider == null) return ApiResponse<ServiceBookingDto>.Fail("Prestataire introuvable");
        if (!provider.IsAvailable) return ApiResponse<ServiceBookingDto>.Fail("Prestataire indisponible");

        var service = await db.ServiceOfferings.FindAsync(new object[] { d.ServiceOfferingId }, ct);
        if (service == null) return ApiResponse<ServiceBookingDto>.Fail("Service introuvable");

        var address = await db.UserAddresses.FindAsync(new object[] { d.InterventionAddressId }, ct);
        if (address == null) return ApiResponse<ServiceBookingDto>.Fail("Adresse introuvable");

        var estimated = service.FixedPrice ?? (service.HourlyRate ?? 0) * (service.EstimatedDurationMinutes / 60m);
        var intervention = new ServiceIntervention
        {
            InterventionNumber = $"SVC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            CustomerId = userId, ProviderId = d.ProviderId, ServiceOfferingId = d.ServiceOfferingId,
            ProblemDescription = d.ProblemDescription,
            ScheduledDate = d.ScheduledDate,
            ScheduledStartTime = TimeOnly.Parse(d.ScheduledTime),
            ScheduledEndTime = TimeOnly.Parse(d.ScheduledTime).AddMinutes(service.EstimatedDurationMinutes),
            InterventionStreet = address.Street, InterventionCity = address.City,
            InterventionLatitude = address.Latitude, InterventionLongitude = address.Longitude,
            EstimatedPrice = estimated, TravelFee = provider.TravelFee,
            TotalAmount = estimated + provider.TravelFee,
            CommissionAmount = estimated * provider.CommissionRate / 100,
            PaymentMethod = d.PaymentMethod, PayBeforeIntervention = d.PayBeforeIntervention,
            Status = InterventionStatus.Reserved
        };
        intervention.StatusHistory.Add(new InterventionStatusHistory { Status = InterventionStatus.Reserved, Note = "Reservation creee" });
        await db.ServiceInterventions.AddAsync(intervention, ct);
        await db.SaveChangesAsync(ct);

        return ApiResponse<ServiceBookingDto>.Ok(new ServiceBookingDto(intervention.Id, intervention.InterventionNumber,
            intervention.Status.ToString(), provider.CompanyName, service.Name, d.ProblemDescription,
            estimated, null, d.ScheduledDate, d.ScheduledTime,
            $"{address.Street}, {address.City}", intervention.PaymentStatus.ToString(),
            null, null, null, null));
    }
}

public class CancelServiceBookingCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CancelServiceBookingCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CancelServiceBookingCommand request, CancellationToken ct)
    {
        var i = await db.ServiceInterventions.FindAsync(new object[] { request.BookingId }, ct);
        if (i == null) return ApiResponse.Fail("Reservation introuvable");
        if (i.Status > InterventionStatus.Confirmed) return ApiResponse.Fail("Annulation impossible");
        i.Status = InterventionStatus.Cancelled;
        i.CancelledAt = DateTime.UtcNow; i.CancellationReason = request.Reason;
        i.StatusHistory.Add(new InterventionStatusHistory { Status = InterventionStatus.Cancelled, Note = request.Reason });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Reservation annulee");
    }
}

public class AcceptServiceBookingCommandHandler(ApplicationDbContext db)
    : IRequestHandler<AcceptServiceBookingCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(AcceptServiceBookingCommand request, CancellationToken ct)
    {
        var i = await db.ServiceInterventions.FindAsync(new object[] { request.BookingId }, ct);
        if (i == null) return ApiResponse.Fail("Reservation introuvable");
        i.Status = InterventionStatus.Confirmed; i.ConfirmedAt = DateTime.UtcNow;
        i.StatusHistory.Add(new InterventionStatusHistory { Status = InterventionStatus.Confirmed, Note = "Acceptee" });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Reservation acceptee");
    }
}

public class RejectServiceBookingCommandHandler(ApplicationDbContext db)
    : IRequestHandler<RejectServiceBookingCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RejectServiceBookingCommand request, CancellationToken ct)
    {
        var i = await db.ServiceInterventions.FindAsync(new object[] { request.BookingId }, ct);
        if (i == null) return ApiResponse.Fail("Reservation introuvable");
        i.Status = InterventionStatus.Cancelled; i.CancelledAt = DateTime.UtcNow;
        i.CancellationReason = request.Reason;
        i.StatusHistory.Add(new InterventionStatusHistory { Status = InterventionStatus.Cancelled, Note = $"Refusee: {request.Reason}" });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Reservation refusee");
    }
}

public class SendQuoteCommandHandler(ApplicationDbContext db)
    : IRequestHandler<SendQuoteCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(SendQuoteCommand request, CancellationToken ct)
    {
        var i = await db.ServiceInterventions.FindAsync(new object[] { request.Dto.BookingId }, ct);
        if (i == null) return ApiResponse.Fail("Reservation introuvable");
        i.QuoteRequested = true; i.QuoteAmount = request.Dto.EstimatedPrice;
        i.QuoteSentAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Devis envoye");
    }
}

public class UpdateBookingStatusCommandHandler(ApplicationDbContext db)
    : IRequestHandler<UpdateBookingStatusCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateBookingStatusCommand request, CancellationToken ct)
    {
        var i = await db.ServiceInterventions.FindAsync(new object[] { request.Dto.BookingId }, ct);
        if (i == null) return ApiResponse.Fail("Reservation introuvable");
        if (!Enum.TryParse<InterventionStatus>(request.Dto.Status, out var newStatus))
            return ApiResponse.Fail("Statut invalide");
        i.Status = newStatus;
        switch (newStatus)
        {
            case InterventionStatus.EnRoute: i.EnRouteAt = DateTime.UtcNow; break;
            case InterventionStatus.OnSite: i.ArrivedAt = DateTime.UtcNow; break;
            case InterventionStatus.InProgress: i.StartedAt = DateTime.UtcNow; break;
            case InterventionStatus.Completed: i.CompletedAt = DateTime.UtcNow; break;
        }
        i.StatusHistory.Add(new InterventionStatusHistory { Status = newStatus, Note = request.Dto.Notes });
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok($"Statut: {newStatus}");
    }
}

public class CreateServiceReviewCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<CreateServiceReviewCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CreateServiceReviewCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        // Get provider from booking
        var intervention = await db.ServiceInterventions.FindAsync(new object[] { request.BookingId }, ct);
        var providerId = intervention?.ProviderId ?? Guid.Empty;
        var review = new Domain.Entities.Identity.Review
        {
            UserId = userId, EntityType = "ServiceProvider", EntityId = providerId,
            OrderId = request.BookingId, Stars = request.OverallRating, Comment = request.Comment
        };
        await db.Reviews.AddAsync(review, ct);
        var provider = await db.ServiceProviders.FindAsync(new object[] { providerId }, ct);
        if (provider != null)
        {
            var allReviews = await db.Reviews.Where(r => r.EntityId == provider.Id && r.EntityType == "ServiceProvider").ToListAsync(ct);
            provider.AverageRating = allReviews.Average(r => r.Stars);
            provider.TotalReviews = allReviews.Count;
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Avis soumis");
    }
}

public class RegisterServiceProviderCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<RegisterServiceProviderCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(RegisterServiceProviderCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var d = request.Dto;
        var provider = new ServiceProvider
        {
            OwnerId = userId, CompanyName = d.CompanyName,
            Slug = d.CompanyName.ToLower().Replace(" ", "-"),
            Description = d.Description ?? "", Phone = d.Phone,
            PrimaryCategory = d.Categories.First(),
            AdditionalCategories = d.Categories.Skip(1).ToList(),
            YearsOfExperience = d.YearsOfExperience,
            InterventionCities = d.ServiceZoneCities,
            VerificationStatus = VerificationStatus.Pending
        };
        foreach (var svc in d.Services)
            provider.Services.Add(new ServiceOffering
            {
                Name = svc.Name, Description = svc.Description ?? "",
                Category = svc.Category, HourlyRate = svc.HourlyRate,
                FixedPrice = svc.FixedPrice, EstimatedDurationMinutes = svc.EstimatedDurationMinutes,
                PricingType = svc.FixedPrice.HasValue ? PricingType.FixedPrice : PricingType.Hourly
            });
        await db.ServiceProviders.AddAsync(provider, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(provider.Id);
    }
}

public class AssignWorkerCommandHandler(ApplicationDbContext db)
    : IRequestHandler<AssignWorkerCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(AssignWorkerCommand request, CancellationToken ct)
    {
        var i = await db.ServiceInterventions.FindAsync(new object[] { request.BookingId }, ct);
        if (i == null) return ApiResponse.Fail("Reservation introuvable");
        i.AssignedTeamMemberId = request.WorkerId;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Intervenant assigne");
    }
}
