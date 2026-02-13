using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Restaurant;
using MultiServices.Application.Features.Restaurants.Commands;
using MultiServices.Application.Features.Restaurants.Queries;
using MultiServices.Domain.Enums;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly IMediator _mediator;
    public RestaurantsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult> Search([FromQuery] string? q, [FromQuery] CuisineType? cuisine,
        [FromQuery] PriceRange? price, [FromQuery] double? maxDistance, [FromQuery] double? lat,
        [FromQuery] double? lng, [FromQuery] double? minRating, [FromQuery] bool? hasPromo,
        [FromQuery] string? sortBy, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new SearchRestaurantsQuery(q, cuisine, price, maxDistance, lat, lng, minRating, hasPromo, sortBy, page, pageSize)));

    [HttpGet("{id}")]
    public async Task<ActionResult> GetDetail(Guid id)
        => Ok(await _mediator.Send(new GetRestaurantDetailQuery(id)));

    [HttpGet("nearby")]
    public async Task<ActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radius = 5)
        => Ok(await _mediator.Send(new GetNearbyRestaurantsQuery(lat, lng, radius)));

    [HttpGet("popular")]
    public async Task<ActionResult> GetPopular([FromQuery] int count = 10)
        => Ok(await _mediator.Send(new GetPopularRestaurantsQuery(count)));

    [Authorize]
    [HttpGet("favorites")]
    public async Task<ActionResult> GetFavorites()
        => Ok(await _mediator.Send(new GetFavoriteRestaurantsQuery()));

    [Authorize]
    [HttpPost("{id}/favorite")]
    public async Task<ActionResult> ToggleFavorite(Guid id)
        => Ok(await _mediator.Send(new ToggleFavoriteRestaurantCommand(id)));

    [HttpGet("{id}/menu")]
    public async Task<ActionResult> GetMenu(Guid id)
        => Ok(await _mediator.Send(new GetRestaurantMenuQuery(id)));

    [HttpGet("{id}/reviews")]
    public async Task<ActionResult> GetReviews(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetRestaurantReviewsQuery(id, page, pageSize)));

    // Restaurant Owner endpoints
    [Authorize(Policy = "RestaurantOwner")]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateRestaurantDto dto)
        => Ok(await _mediator.Send(new CreateRestaurantCommand(dto)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CreateRestaurantDto dto)
        => Ok(await _mediator.Send(new UpdateRestaurantCommand(id, dto)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPost("{id}/toggle-open")]
    public async Task<ActionResult> ToggleOpen(Guid id)
        => Ok(await _mediator.Send(new ToggleRestaurantOpenCommand(id)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpGet("my-restaurants")]
    public async Task<ActionResult> GetMyRestaurants()
        => Ok(await _mediator.Send(new GetMyRestaurantsQuery()));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPost("{id}/menu/categories")]
    public async Task<ActionResult> CreateCategory(Guid id, [FromBody] CreateMenuCategoryDto dto)
        => Ok(await _mediator.Send(new CreateMenuCategoryCommand(id, dto)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPost("{id}/menu/items")]
    public async Task<ActionResult> CreateMenuItem(Guid id, [FromBody] CreateMenuItemDto dto)
        => Ok(await _mediator.Send(new CreateMenuItemCommand(id, dto)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPut("menu/items/{itemId}")]
    public async Task<ActionResult> UpdateMenuItem(Guid itemId, [FromBody] CreateMenuItemDto dto)
        => Ok(await _mediator.Send(new UpdateMenuItemCommand(itemId, dto)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpDelete("menu/items/{itemId}")]
    public async Task<ActionResult> DeleteMenuItem(Guid itemId)
        => Ok(await _mediator.Send(new DeleteMenuItemCommand(itemId)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpPost("menu/items/{itemId}/toggle-availability")]
    public async Task<ActionResult> ToggleItemAvailability(Guid itemId)
        => Ok(await _mediator.Send(new ToggleMenuItemAvailabilityCommand(itemId)));

    [Authorize(Policy = "RestaurantOwner")]
    [HttpGet("{id}/stats")]
    public async Task<ActionResult> GetStats(Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _mediator.Send(new GetRestaurantStatsQuery(id, from, to)));
}
