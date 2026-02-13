using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.DTOs.Restaurant;
using MultiServices.Application.Features.Restaurants.Commands;
using MultiServices.Application.Features.Restaurants.Queries;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/restaurant-orders")]
[Authorize]
public class RestaurantOrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public RestaurantOrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] CreateRestaurantOrderDto dto)
        => Ok(await _mediator.Send(new CreateRestaurantOrderCommand(dto)));

    [HttpGet("my-orders")]
    public async Task<ActionResult> GetMyOrders([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetMyRestaurantOrdersQuery(status, page, pageSize)));

    [HttpGet("{id}")]
    public async Task<ActionResult> GetOrderDetail(Guid id)
        => Ok(await _mediator.Send(new GetRestaurantOrderDetailQuery(id)));

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid id, [FromBody] string? reason)
        => Ok(await _mediator.Send(new CancelRestaurantOrderCommand(id, reason)));

    [HttpPost("{id}/review")]
    public async Task<ActionResult> ReviewOrder(Guid id, [FromBody] CreateReviewDto dto)
        => Ok(await _mediator.Send(new CreateRestaurantReviewCommand(dto)));

    // Restaurant owner endpoints
    [Authorize(Policy = "RestaurantOwner")]
    [HttpGet("restaurant/{restaurantId}")]
    public async Task<ActionResult> GetRestaurantOrders(Guid restaurantId, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetRestaurantOrdersQuery(restaurantId, status, page, pageSize)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPost("{id}/accept")]
    public async Task<ActionResult> AcceptOrder(Guid id, [FromBody] int estimatedMinutes)
        => Ok(await _mediator.Send(new AcceptRestaurantOrderCommand(id, estimatedMinutes)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPost("{id}/reject")]
    public async Task<ActionResult> RejectOrder(Guid id, [FromBody] string reason)
        => Ok(await _mediator.Send(new RejectRestaurantOrderCommand(id, reason)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPost("{id}/status")]
    public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] string status)
        => Ok(await _mediator.Send(new UpdateOrderStatusCommand(id, status)));
}
