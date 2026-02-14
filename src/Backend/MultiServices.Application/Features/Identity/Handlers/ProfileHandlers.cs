using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Identity;
using MultiServices.Application.Features.Identity.Commands;
using MultiServices.Application.Features.Identity.Queries;
using MultiServices.Application.Interfaces;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Identity.Handlers;

public class GetProfileQueryHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor http)
    : IRequestHandler<GetProfileQuery, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(GetProfileQuery request, CancellationToken ct)
    {
        var userId = http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return ApiResponse<UserDto>.Fail("Utilisateur introuvable");
        var roles = await userManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Ok(new UserDto(user.Id, user.Email!, user.FirstName, user.LastName,
            user.ProfileImageUrl, user.PhoneNumber ?? "", string.Join(",", roles),
            user.PhoneNumberConfirmed, user.EmailConfirmed, 
            user.LoyaltyAccount?.Points ?? 0, user.Wallet?.Balance ?? 0,
            user.PreferredLanguage.ToString()));
    }
}

public class GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return ApiResponse<UserDto>.Fail("Utilisateur introuvable");
        var roles = await userManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Ok(new UserDto(user.Id, user.Email!, user.FirstName, user.LastName,
            user.ProfileImageUrl, user.PhoneNumber ?? "", string.Join(",", roles),
            user.PhoneNumberConfirmed, user.EmailConfirmed, 0, 0,
            user.PreferredLanguage.ToString()));
    }
}

public class GetUserAddressesQueryHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<GetUserAddressesQuery, ApiResponse<List<AddAddressDto>>>
{
    public async Task<ApiResponse<List<AddAddressDto>>> Handle(GetUserAddressesQuery request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var addresses = await db.UserAddresses.Where(a => a.UserId == userId && !a.IsDeleted)
            .Select(a => new AddAddressDto(a.Street, a.City, a.PostalCode, a.Country,
                a.Apartment, a.BuildingName, a.Latitude, a.Longitude, a.Label, a.IsDefault))
            .ToListAsync(ct);
        return ApiResponse<List<AddAddressDto>>.Ok(addresses);
    }
}

public class UpdateProfileCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor http)
    : IRequestHandler<UpdateProfileCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var userId = http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return ApiResponse<UserDto>.Fail("Utilisateur introuvable");
        if (request.Dto.FirstName != null) user.FirstName = request.Dto.FirstName;
        if (request.Dto.LastName != null) user.LastName = request.Dto.LastName;
        if (request.Dto.Phone != null) user.PhoneNumber = request.Dto.Phone;
        await userManager.UpdateAsync(user);
        var roles = await userManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Ok(new UserDto(user.Id, user.Email!, user.FirstName, user.LastName,
            user.ProfileImageUrl, user.PhoneNumber ?? "", string.Join(",", roles),
            user.PhoneNumberConfirmed, user.EmailConfirmed, 0, 0,
            user.PreferredLanguage.ToString()));
    }
}

public class UploadProfileImageCommandHandler : IRequestHandler<UploadProfileImageCommand, ApiResponse<string>>
{
    public Task<ApiResponse<string>> Handle(UploadProfileImageCommand request, CancellationToken ct)
    {
        // TODO: Upload to blob storage
        var url = $"/uploads/profiles/{Guid.NewGuid()}/{request.FileName}";
        return Task.FromResult(ApiResponse<string>.Ok(url));
    }
}

public class AddUserAddressCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<AddUserAddressCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(AddUserAddressCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (request.Dto.IsDefault)
        {
            var existing = await db.UserAddresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync(ct);
            foreach (var a in existing) a.IsDefault = false;
        }
        var address = new UserAddress
        {
            UserId = userId, Label = request.Dto.Label ?? "Adresse",
            Street = request.Dto.Street, City = request.Dto.City,
            PostalCode = request.Dto.PostalCode, Country = request.Dto.Country ?? "Morocco",
            Apartment = request.Dto.Apartment, BuildingName = request.Dto.BuildingName,
            Latitude = request.Dto.Latitude, Longitude = request.Dto.Longitude,
            IsDefault = request.Dto.IsDefault
        };
        await db.UserAddresses.AddAsync(address, ct);
        await db.SaveChangesAsync(ct);
        return ApiResponse<Guid>.Ok(address.Id);
    }
}

public class UpdateUserAddressCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<UpdateUserAddressCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateUserAddressCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var address = await db.UserAddresses.FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId, ct);
        if (address == null) return ApiResponse.Fail("Adresse introuvable");
        address.Street = request.Dto.Street; address.City = request.Dto.City;
        address.PostalCode = request.Dto.PostalCode; address.Apartment = request.Dto.Apartment;
        address.BuildingName = request.Dto.BuildingName; address.Latitude = request.Dto.Latitude;
        address.Longitude = request.Dto.Longitude; address.IsDefault = request.Dto.IsDefault;
        if (request.Dto.Label != null) address.Label = request.Dto.Label;
        if (request.Dto.IsDefault)
        {
            var others = await db.UserAddresses.Where(a => a.UserId == userId && a.Id != address.Id && a.IsDefault).ToListAsync(ct);
            foreach (var o in others) o.IsDefault = false;
        }
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Adresse mise a jour");
    }
}

public class DeleteUserAddressCommandHandler(ApplicationDbContext db, IHttpContextAccessor http)
    : IRequestHandler<DeleteUserAddressCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteUserAddressCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var address = await db.UserAddresses.FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId, ct);
        if (address == null) return ApiResponse.Fail("Adresse introuvable");
        address.IsDeleted = true; address.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ApiResponse.Ok("Adresse supprimee");
    }
}

public class DeleteAccountCommandHandler(IAuthService authService, IHttpContextAccessor http)
    : IRequestHandler<DeleteAccountCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteAccountCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return await authService.DeleteAccountAsync(userId, "", ct);
    }
}
