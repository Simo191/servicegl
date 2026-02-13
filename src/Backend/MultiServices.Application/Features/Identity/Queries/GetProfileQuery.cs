using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Identity;

namespace MultiServices.Application.Features.Identity.Queries;

public record GetProfileQuery : IRequest<ApiResponse<UserDto>>;
public record GetUserAddressesQuery : IRequest<ApiResponse<List<AddAddressDto>>>;
public record GetUserByIdQuery(Guid UserId) : IRequest<ApiResponse<UserDto>>;
