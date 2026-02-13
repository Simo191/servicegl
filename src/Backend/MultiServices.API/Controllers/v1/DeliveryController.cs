using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.DTOs.Delivery;
using MultiServices.Application.Features.Delivery.Commands;
using MultiServices.Application.Features.Delivery.Queries;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "DeliveryDriver")]
public class DeliveryController : ControllerBase
{
    private readonly IMediator _mediator;
    public DeliveryController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] DriverRegistrationDto dto)
        => Ok(await _mediator.Send(new RegisterDriverCommand(dto)));

    [HttpGet("profile")]
    public async Task<ActionResult> GetProfile()
        => Ok(await _mediator.Send(new GetDriverProfileQuery()));

    [HttpPost("go-online")]
    public async Task<ActionResult> GoOnline()
        => Ok(await _mediator.Send(new GoOnlineCommand()));

    [HttpPost("go-offline")]
    public async Task<ActionResult> GoOffline()
        => Ok(await _mediator.Send(new GoOfflineCommand()));

    [HttpPut("location")]
    public async Task<ActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        => Ok(await _mediator.Send(new UpdateDriverLocationCommand(dto)));

    [HttpGet("available")]
    public async Task<ActionResult> GetAvailableDeliveries([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radius = 10)
        => Ok(await _mediator.Send(new GetAvailableDeliveriesQuery(lat, lng, radius)));

    [HttpPost("{id}/accept")]
    public async Task<ActionResult> AcceptDelivery(Guid id, [FromQuery] string type)
        => Ok(await _mediator.Send(new AcceptDeliveryCommand(id, type)));

    [HttpPost("{id}/reject")]
    public async Task<ActionResult> RejectDelivery(Guid id, [FromQuery] string type)
        => Ok(await _mediator.Send(new RejectDeliveryCommand(id, type)));

    [HttpPost("{id}/status")]
    public async Task<ActionResult> UpdateStatus(Guid id, [FromQuery] string type, [FromBody] UpdateDeliveryStatusDto dto)
        => Ok(await _mediator.Send(new UpdateDeliveryStatusCommand(id, type, dto)));

    [HttpGet("active")]
    public async Task<ActionResult> GetActiveDelivery()
        => Ok(await _mediator.Send(new GetActiveDeliveryQuery()));

    [HttpGet("earnings")]
    public async Task<ActionResult> GetEarnings([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _mediator.Send(new GetDriverEarningsQuery(from, to)));

    [HttpGet("stats")]
    public async Task<ActionResult> GetStats()
        => Ok(await _mediator.Send(new GetDriverStatsQuery()));

    [HttpPost("payout")]
    public async Task<ActionResult> RequestPayout([FromBody] decimal amount)
        => Ok(await _mediator.Send(new RequestPayoutCommand(amount)));

    [HttpPost("sos")]
    public async Task<ActionResult> TriggerSos([FromBody] UpdateLocationDto location)
        => Ok(await _mediator.Send(new TriggerSosCommand(location.Latitude, location.Longitude)));
}
