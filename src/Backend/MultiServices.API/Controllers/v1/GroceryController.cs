using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.DTOs.Grocery;
using MultiServices.Application.Features.Grocery.Commands;
using MultiServices.Application.Features.Grocery.Queries;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class GroceryController : ControllerBase
{
    private readonly IMediator _mediator;
    public GroceryController(IMediator mediator) => _mediator = mediator;

    [HttpGet("stores")]
    public async Task<ActionResult> SearchStores([FromQuery] string? q, [FromQuery] double? maxDistance,
        [FromQuery] double? lat, [FromQuery] double? lng, [FromQuery] bool? hasPromo,
        [FromQuery] bool? freeDelivery, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new SearchStoresQuery(q, maxDistance, lat, lng, hasPromo, freeDelivery, page, pageSize)));

    [HttpGet("stores/{id}")]
    public async Task<ActionResult> GetStoreDetail(Guid id)
        => Ok(await _mediator.Send(new GetStoreDetailQuery(id)));

    [HttpGet("stores/{id}/categories")]
    public async Task<ActionResult> GetCategories(Guid id)
        => Ok(await _mediator.Send(new GetStoreCategoriesQuery(id)));

    [HttpGet("stores/{storeId}/products")]
    public async Task<ActionResult> SearchProducts(Guid storeId, [FromQuery] string? q,
        [FromQuery] Guid? categoryId, [FromQuery] string? brand, [FromQuery] bool? isBio,
        [FromQuery] bool? isHalal, [FromQuery] bool? onPromo, [FromQuery] string? sortBy,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new SearchProductsQuery(storeId, q, categoryId, brand, isBio, isHalal, onPromo, sortBy, page, pageSize)));

    [HttpGet("products/{id}")]
    public async Task<ActionResult> GetProductDetail(Guid id)
        => Ok(await _mediator.Send(new GetProductDetailQuery(id)));

    [HttpGet("products/scan/{barcode}")]
    public async Task<ActionResult> ScanBarcode(string barcode, [FromQuery] Guid storeId)
        => Ok(await _mediator.Send(new ScanBarcodeQuery(barcode, storeId)));

    // Orders
    [Authorize]
    [HttpPost("orders")]
    public async Task<ActionResult> CreateOrder([FromBody] CreateGroceryOrderDto dto)
        => Ok(await _mediator.Send(new CreateGroceryOrderCommand(dto)));

    [Authorize]
    [HttpGet("orders/my-orders")]
    public async Task<ActionResult> GetMyOrders([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetMyGroceryOrdersQuery(status, page, pageSize)));

    [Authorize]
    [HttpGet("orders/{id}")]
    public async Task<ActionResult> GetOrderDetail(Guid id)
        => Ok(await _mediator.Send(new GetGroceryOrderDetailQuery(id)));

    [Authorize]
    [HttpPost("orders/{id}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid id, [FromBody] string? reason)
        => Ok(await _mediator.Send(new CancelGroceryOrderCommand(id, reason)));

    [Authorize]
    [HttpPost("orders/{orderId}/items/{itemId}/substitution")]
    public async Task<ActionResult> AcceptSubstitution(Guid orderId, Guid itemId, [FromBody] bool accept)
        => Ok(await _mediator.Send(new AcceptProductSubstitutionCommand(orderId, itemId, accept)));

    // Shopping Lists
    [Authorize]
    [HttpGet("shopping-lists")]
    public async Task<ActionResult> GetShoppingLists()
        => Ok(await _mediator.Send(new GetMyShoppingListsQuery()));

    [Authorize]
    [HttpPost("shopping-lists")]
    public async Task<ActionResult> CreateShoppingList([FromBody] CreateShoppingListDto dto)
        => Ok(await _mediator.Send(new CreateShoppingListCommand(dto)));

    [Authorize]
    [HttpPut("shopping-lists/{id}")]
    public async Task<ActionResult> UpdateShoppingList(Guid id, [FromBody] CreateShoppingListDto dto)
        => Ok(await _mediator.Send(new UpdateShoppingListCommand(id, dto)));

    [Authorize]
    [HttpDelete("shopping-lists/{id}")]
    public async Task<ActionResult> DeleteShoppingList(Guid id)
        => Ok(await _mediator.Send(new DeleteShoppingListCommand(id)));

    [Authorize]
    [HttpPost("shopping-lists/{id}/to-cart")]
    public async Task<ActionResult> ConvertToCart(Guid id, [FromQuery] Guid storeId)
        => Ok(await _mediator.Send(new ConvertListToCartCommand(id, storeId)));

    [Authorize]
    [HttpPost("orders/{id}/duplicate")]
    public async Task<ActionResult> DuplicateOrder(Guid id)
        => Ok(await _mediator.Send(new DuplicatePreviousOrderCommand(id)));

    // Store Manager endpoints
    [Authorize(Policy = "StoreManager")]
    [HttpPost("stores/{storeId}/products")]
    public async Task<ActionResult> CreateProduct(Guid storeId, [FromBody] CreateProductDto dto)
        => Ok(await _mediator.Send(new CreateProductCommand(storeId, dto)));

    [Authorize(Policy = "StoreManager")]
    [HttpPut("products/{id}")]
    public async Task<ActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
        => Ok(await _mediator.Send(new UpdateProductCommand(id, dto)));

    [Authorize(Policy = "StoreManager")]
    [HttpPost("stores/{storeId}/products/bulk-import")]
    public async Task<ActionResult> BulkImport(Guid storeId, [FromBody] BulkImportProductDto dto)
        => Ok(await _mediator.Send(new BulkImportProductsCommand(storeId, dto)));

    [Authorize(Policy = "StoreManager")]
    [HttpGet("stores/{storeId}/orders")]
    public async Task<ActionResult> GetStoreOrders(Guid storeId, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetGroceryOrdersQuery(storeId, status, page, pageSize)));

    [Authorize(Policy = "StoreManager")]
    [HttpPost("orders/{id}/accept")]
    public async Task<ActionResult> AcceptOrder(Guid id)
        => Ok(await _mediator.Send(new AcceptGroceryOrderCommand(id)));

    [Authorize(Policy = "StoreManager")]
    [HttpPost("orders/{id}/ready")]
    public async Task<ActionResult> MarkReady(Guid id)
        => Ok(await _mediator.Send(new MarkOrderReadyCommand(id)));

    [Authorize(Policy = "StoreManager")]
    [HttpGet("stores/{id}/stats")]
    public async Task<ActionResult> GetStoreStats(Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _mediator.Send(new GetStoreStatsQuery(id, from, to)));
}
