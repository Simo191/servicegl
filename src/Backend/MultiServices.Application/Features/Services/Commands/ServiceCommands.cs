using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Service;

namespace MultiServices.Application.Features.Services.Commands;

// Client
public record CreateServiceBookingCommand(CreateServiceBookingDto Dto) : IRequest<ApiResponse<ServiceBookingDto>>;
public record CancelServiceBookingCommand(Guid BookingId, string? Reason) : IRequest<ApiResponse>;
public record AcceptSubstitutionCommand(Guid BookingId, bool Accept) : IRequest<ApiResponse>;
public record CreateServiceReviewCommand(Guid BookingId, int ProviderRating, int? WorkerRating,
    int QualityRating, int PunctualityRating, int OverallRating, string? Comment) : IRequest<ApiResponse>;

// Provider
public record RegisterServiceProviderCommand(CreateServiceProviderDto Dto) : IRequest<ApiResponse<Guid>>;
public record UpdateServiceProviderCommand(Guid Id, CreateServiceProviderDto Dto) : IRequest<ApiResponse>;
public record AddServiceOfferingCommand(Guid ProviderId, CreateServiceOfferingDto Dto) : IRequest<ApiResponse<Guid>>;
public record UpdateServiceOfferingCommand(Guid Id, CreateServiceOfferingDto Dto) : IRequest<ApiResponse>;
public record DeleteServiceOfferingCommand(Guid Id) : IRequest<ApiResponse>;
public record AcceptServiceBookingCommand(Guid BookingId) : IRequest<ApiResponse>;
public record RejectServiceBookingCommand(Guid BookingId, string Reason) : IRequest<ApiResponse>;
public record SendQuoteCommand(SendQuoteDto Dto) : IRequest<ApiResponse>;
public record AssignWorkerCommand(Guid BookingId, Guid WorkerId) : IRequest<ApiResponse>;
public record UpdateBookingStatusCommand(UpdateBookingStatusDto Dto) : IRequest<ApiResponse>;
public record UploadInterventionPhotosCommand(Guid BookingId, string Type, Stream Image, string FileName) : IRequest<ApiResponse<string>>;
public record AddWorkerCommand(Guid ProviderId, string FirstName, string LastName, string Phone, string? Specialization) : IRequest<ApiResponse<Guid>>;
public record UpdateAvailabilityCommand(Guid ProviderId, List<AvailabilitySlotDto> Slots) : IRequest<ApiResponse>;
public record ToggleProviderAvailabilityCommand(Guid ProviderId) : IRequest<ApiResponse>;
public record AddPortfolioItemCommand(Guid ProviderId, string Title, string? Description, Stream Image, string FileName) : IRequest<ApiResponse<Guid>>;
public record ReplyToServiceReviewCommand(Guid ReviewId, string Reply) : IRequest<ApiResponse>;
