using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Restaurant;

namespace MultiServices.Application.Features.Restaurants.Commands;

// Client
public record CreateRestaurantOrderCommand(CreateRestaurantOrderDto Dto) : IRequest<ApiResponse<RestaurantOrderDto>>;
public record CancelRestaurantOrderCommand(Guid OrderId, string? Reason) : IRequest<ApiResponse>;
public record CreateRestaurantReviewCommand(CreateReviewDto Dto) : IRequest<ApiResponse>;
public record ToggleFavoriteRestaurantCommand(Guid RestaurantId) : IRequest<ApiResponse>;

// Restaurant Owner
public record CreateRestaurantCommand(CreateRestaurantDto Dto) : IRequest<ApiResponse<Guid>>;
public record UpdateRestaurantCommand(Guid Id, CreateRestaurantDto Dto) : IRequest<ApiResponse>;
public record ToggleRestaurantOpenCommand(Guid RestaurantId) : IRequest<ApiResponse>;
public record CreateMenuCategoryCommand(Guid RestaurantId, CreateMenuCategoryDto Dto) : IRequest<ApiResponse<Guid>>;
public record UpdateMenuCategoryCommand(Guid Id, CreateMenuCategoryDto Dto) : IRequest<ApiResponse>;
public record DeleteMenuCategoryCommand(Guid Id) : IRequest<ApiResponse>;
public record CreateMenuItemCommand(Guid RestaurantId, CreateMenuItemDto Dto) : IRequest<ApiResponse<Guid>>;
public record UpdateMenuItemCommand(Guid Id, CreateMenuItemDto Dto) : IRequest<ApiResponse>;
public record DeleteMenuItemCommand(Guid Id) : IRequest<ApiResponse>;
public record ToggleMenuItemAvailabilityCommand(Guid Id) : IRequest<ApiResponse>;
public record AcceptRestaurantOrderCommand(Guid OrderId, int EstimatedMinutes) : IRequest<ApiResponse>;
public record RejectRestaurantOrderCommand(Guid OrderId, string Reason) : IRequest<ApiResponse>;
public record UpdateOrderStatusCommand(Guid OrderId, string Status) : IRequest<ApiResponse>;
public record ReplyToReviewCommand(Guid ReviewId, string Reply) : IRequest<ApiResponse>;
public record CreateRestaurantPromotionCommand(Guid RestaurantId, PromotionDto Dto) : IRequest<ApiResponse<Guid>>;
public record UploadRestaurantImageCommand(Guid RestaurantId, Stream Image, string FileName, string Type) : IRequest<ApiResponse<string>>;
