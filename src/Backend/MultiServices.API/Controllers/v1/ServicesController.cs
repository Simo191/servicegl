using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.DTOs.Service;
using MultiServices.Application.Features.Services.Commands;
using MultiServices.Application.Features.Services.Queries;
using MultiServices.Domain.Enums;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ServicesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("providers")]
    public async Task<ActionResult> SearchProviders([FromQuery] ServiceCategory? category, [FromQuery] double? minRating,
        [FromQuery] string? city, [FromQuery] decimal? maxPrice, [FromQuery] string? sortBy,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new SearchServiceProvidersQuery(category, minRating, city, maxPrice, sortBy, page, pageSize)));

    [HttpGet("providers/{id}")]
    public async Task<ActionResult> GetProviderDetail(Guid id)
        => Ok(await _mediator.Send(new GetServiceProviderDetailQuery(id)));

    [HttpGet("providers/available")]
    public async Task<ActionResult> GetAvailable([FromQuery] ServiceCategory category, [FromQuery] DateTime date, [FromQuery] string city)
        => Ok(await _mediator.Send(new GetAvailableProvidersQuery(category, date, city)));

    [HttpGet("categories")]
    public async Task<ActionResult> GetCategories()
        => Ok(await _mediator.Send(new GetServiceCategoriesQuery()));

    // Client bookings
    [Authorize]
    [HttpPost("bookings")]
    public async Task<ActionResult> CreateBooking([FromBody] CreateServiceBookingDto dto)
        => Ok(await _mediator.Send(new CreateServiceBookingCommand(dto)));

    [Authorize]
    [HttpGet("bookings/my-bookings")]
    public async Task<ActionResult> GetMyBookings([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetMyServiceBookingsQuery(status, page, pageSize)));

    [Authorize]
    [HttpGet("bookings/{id}")]
    public async Task<ActionResult> GetBookingDetail(Guid id)
        => Ok(await _mediator.Send(new GetServiceBookingDetailQuery(id)));

    [Authorize]
    [HttpPost("bookings/{id}/cancel")]
    public async Task<ActionResult> CancelBooking(Guid id, [FromBody] string? reason)
        => Ok(await _mediator.Send(new CancelServiceBookingCommand(id, reason)));

    [Authorize]
    [HttpPost("bookings/{id}/review")]
    public async Task<ActionResult> ReviewBooking(Guid id, [FromBody] CreateServiceReviewCommand command)
        => Ok(await _mediator.Send(command));

    // Provider endpoints
    [Authorize(Policy = "ServiceProvider")]
    [HttpPost("providers/register")]
    public async Task<ActionResult> RegisterProvider([FromBody] CreateServiceProviderDto dto)
        => Ok(await _mediator.Send(new RegisterServiceProviderCommand(dto)));

    [Authorize(Policy = "ServiceProvider")]
    [HttpGet("providers/my-profile")]
    public async Task<ActionResult> GetMyProfile()
        => Ok(await _mediator.Send(new GetMyServiceProviderQuery()));

    [Authorize(Policy = "ServiceProvider")]
    [HttpGet("providers/{id}/bookings")]
    public async Task<ActionResult> GetProviderBookings(Guid id, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetServiceBookingsQuery(id, status, page, pageSize)));

    [Authorize(Policy = "ServiceProvider")]
    [HttpPost("bookings/{id}/accept")]
    public async Task<ActionResult> AcceptBooking(Guid id)
        => Ok(await _mediator.Send(new AcceptServiceBookingCommand(id)));

    [Authorize(Policy = "ServiceProvider")]
    [HttpPost("bookings/{id}/reject")]
    public async Task<ActionResult> RejectBooking(Guid id, [FromBody] string reason)
        => Ok(await _mediator.Send(new RejectServiceBookingCommand(id, reason)));

    [Authorize(Policy = "ServiceProvider")]
    [HttpPost("bookings/{id}/quote")]
    public async Task<ActionResult> SendQuote([FromBody] SendQuoteDto dto)
        => Ok(await _mediator.Send(new SendQuoteCommand(dto)));

    [Authorize(Policy = "ServiceProvider")]
    [HttpPost("bookings/{id}/assign-worker")]
    public async Task<ActionResult> AssignWorker(Guid id, [FromBody] Guid workerId)
        => Ok(await _mediator.Send(new AssignWorkerCommand(id, workerId)));

    [Authorize(Policy = "ServiceProvider")]
    [HttpPost("bookings/{id}/status")]
    public async Task<ActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusDto dto)
        => Ok(await _mediator.Send(new UpdateBookingStatusCommand(dto)));

    [Authorize(Policy = "ServiceProvider")]
    [HttpGet("providers/{id}/stats")]
    public async Task<ActionResult> GetStats(Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _mediator.Send(new GetProviderStatsQuery(id, from, to)));

    [Authorize(Policy = "ServiceProvider")]
    [HttpGet("providers/{id}/earnings")]
    public async Task<ActionResult> GetEarnings(Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _mediator.Send(new GetProviderEarningsQuery(id, from, to)));
}
