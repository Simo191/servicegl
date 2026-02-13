using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Grocery;

namespace MultiServices.Application.Features.Grocery.Commands;

// Client
public record CreateGroceryOrderCommand(CreateGroceryOrderDto Dto) : IRequest<ApiResponse<GroceryOrderDto>>;
public record CancelGroceryOrderCommand(Guid OrderId, string? Reason) : IRequest<ApiResponse>;
public record AcceptProductSubstitutionCommand(Guid OrderId, Guid ItemId, bool Accept) : IRequest<ApiResponse>;
public record ReportMissingProductCommand(Guid OrderId, Guid ItemId) : IRequest<ApiResponse>;
public record ReportDamagedProductCommand(Guid OrderId, Guid ItemId, string Description) : IRequest<ApiResponse>;
public record CreateShoppingListCommand(CreateShoppingListDto Dto) : IRequest<ApiResponse<ShoppingListDto>>;
public record UpdateShoppingListCommand(Guid Id, CreateShoppingListDto Dto) : IRequest<ApiResponse>;
public record DeleteShoppingListCommand(Guid Id) : IRequest<ApiResponse>;
public record ConvertListToCartCommand(Guid ListId, Guid StoreId) : IRequest<ApiResponse<List<GroceryCartItemDto>>>;
public record DuplicatePreviousOrderCommand(Guid OrderId) : IRequest<ApiResponse<List<GroceryCartItemDto>>>;

// Store Manager
public record CreateProductCommand(Guid StoreId, CreateProductDto Dto) : IRequest<ApiResponse<Guid>>;
public record UpdateProductCommand(Guid ProductId, UpdateProductDto Dto) : IRequest<ApiResponse>;
public record DeleteProductCommand(Guid ProductId) : IRequest<ApiResponse>;
public record BulkImportProductsCommand(Guid StoreId, BulkImportProductDto Dto) : IRequest<ApiResponse<int>>;
public record UpdateStockCommand(Guid ProductId, int Quantity) : IRequest<ApiResponse>;
public record AcceptGroceryOrderCommand(Guid OrderId) : IRequest<ApiResponse>;
public record RejectGroceryOrderCommand(Guid OrderId, string Reason) : IRequest<ApiResponse>;
public record AssignPreparerCommand(Guid OrderId, string PreparerName) : IRequest<ApiResponse>;
public record MarkProductUnavailableCommand(Guid OrderId, Guid ItemId, Guid? SubstituteProductId) : IRequest<ApiResponse>;
public record MarkOrderReadyCommand(Guid OrderId) : IRequest<ApiResponse>;
public record CreateStorePromotionCommand(Guid StoreId, StorePromotionDto Dto) : IRequest<ApiResponse<Guid>>;
