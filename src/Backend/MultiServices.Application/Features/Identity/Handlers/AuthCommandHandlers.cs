using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Identity;
using MultiServices.Application.Features.Identity.Commands;
using MultiServices.Application.Interfaces;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Infrastructure.Data;
using System.Security.Claims;

namespace MultiServices.Application.Features.Identity.Handlers;

public class LoginCommandHandler(IAuthService authService)
    : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(new DTOs.Auth.LoginRequest
        {
            Email = request.Dto.Email,
            Password = request.Dto.Password,
            DeviceToken = request.Dto.DeviceToken
        }, ct);
        if (!result.Succeeded)
            return ApiResponse<AuthResponseDto>.Fail(string.Join(", ", result.Errors));
        var dto = new AuthResponseDto(result.Token, result.RefreshToken,
            new UserDto(result.User.Id, result.User.Email, result.User.FirstName, result.User.LastName,
                result.User.ProfileImageUrl, result.User.PhoneNumber ?? "", string.Join(",", result.User.Roles),
                result.User.IsVerified, result.User.IsVerified, 0, 0, result.User.PreferredLanguage.ToString()));
        return ApiResponse<AuthResponseDto>.Ok(dto);
    }
}

public class RegisterCommandHandler(IAuthService authService)
    : IRequestHandler<RegisterCommand, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(new DTOs.Auth.RegisterRequest
        {
            FirstName = request.Dto.FirstName, LastName = request.Dto.LastName,
            Email = request.Dto.Email, Password = request.Dto.Password,
            ConfirmPassword = request.Dto.Password, PhoneNumber = request.Dto.Phone
        }, ct);
        if (!result.Succeeded)
            return ApiResponse<AuthResponseDto>.Fail(string.Join(", ", result.Errors));
        var dto = new AuthResponseDto(result.Token, result.RefreshToken,
            new UserDto(result.User.Id, result.User.Email, result.User.FirstName, result.User.LastName,
                result.User.ProfileImageUrl, result.User.PhoneNumber ?? "", string.Join(",", result.User.Roles),
                result.User.IsVerified, result.User.IsVerified, 0, 0, result.User.PreferredLanguage.ToString()));
        return ApiResponse<AuthResponseDto>.Ok(dto);
    }
}

public class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var result = await authService.RefreshTokenAsync(new DTOs.Auth.RefreshTokenRequest
        {
            Token = request.Dto.AccessToken, RefreshToken = request.Dto.RefreshToken
        }, ct);
        if (!result.Succeeded)
            return ApiResponse<AuthResponseDto>.Fail(string.Join(", ", result.Errors));
        var dto = new AuthResponseDto(result.Token, result.RefreshToken,
            new UserDto(result.User.Id, result.User.Email, result.User.FirstName, result.User.LastName,
                result.User.ProfileImageUrl, result.User.PhoneNumber ?? "", string.Join(",", result.User.Roles),
                result.User.IsVerified, result.User.IsVerified, 0, 0, result.User.PreferredLanguage.ToString()));
        return ApiResponse<AuthResponseDto>.Ok(dto);
    }
}

public class ExternalLoginCommandHandler(IAuthService authService)
    : IRequestHandler<ExternalLoginCommand, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(ExternalLoginCommand request, CancellationToken ct)
    {
        var result = await authService.SocialLoginAsync(new DTOs.Auth.SocialLoginRequest
        {
            Provider = request.Dto.Provider, AccessToken = request.Dto.Token
        }, ct);
        if (!result.Succeeded)
            return ApiResponse<AuthResponseDto>.Fail(string.Join(", ", result.Errors));
        var dto = new AuthResponseDto(result.Token, result.RefreshToken,
            new UserDto(result.User.Id, result.User.Email, result.User.FirstName, result.User.LastName,
                result.User.ProfileImageUrl, result.User.PhoneNumber ?? "", string.Join(",", result.User.Roles),
                result.User.IsVerified, result.User.IsVerified, 0, 0, result.User.PreferredLanguage.ToString()));
        return ApiResponse<AuthResponseDto>.Ok(dto);
    }
}

public class ForgotPasswordCommandHandler(IAuthService authService)
    : IRequestHandler<ForgotPasswordCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        return await authService.ForgotPasswordAsync(new DTOs.Auth.ForgotPasswordRequest { Email = request.Dto.Email }, ct);
    }
}

public class ResetPasswordCommandHandler(IAuthService authService)
    : IRequestHandler<ResetPasswordCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        return await authService.ResetPasswordAsync(new DTOs.Auth.ResetPasswordRequest
        {
            Email = request.Dto.Email, Token = request.Dto.Token,
            NewPassword = request.Dto.NewPassword, ConfirmPassword = request.Dto.NewPassword
        }, ct);
    }
}

public class ChangePasswordCommandHandler(IAuthService authService, IHttpContextAccessor http)
    : IRequestHandler<ChangePasswordCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return await authService.ChangePasswordAsync(userId, new DTOs.Auth.ChangePasswordRequest
        {
            CurrentPassword = request.Dto.CurrentPassword,
            NewPassword = request.Dto.NewPassword, ConfirmPassword = request.Dto.NewPassword
        }, ct);
    }
}

public class LogoutCommandHandler(IAuthService authService, IHttpContextAccessor http)
    : IRequestHandler<LogoutCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(LogoutCommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return await authService.LogoutAsync(userId, ct);
    }
}

public class SendOtpCommandHandler(IAuthService authService)
    : IRequestHandler<SendOtpCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(SendOtpCommand request, CancellationToken ct)
    {
        return await authService.SendPhoneVerificationAsync(request.PhoneNumber, ct);
    }
}

public class VerifyPhoneCommandHandler(IAuthService authService)
    : IRequestHandler<VerifyPhoneCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(VerifyPhoneCommand request, CancellationToken ct)
    {
        return await authService.VerifyPhoneAsync(new DTOs.Auth.VerifyPhoneRequest
        {
            PhoneNumber = request.Dto.PhoneNumber, Otp = request.Dto.Otp
        }, ct);
    }
}

public class Enable2FACommandHandler(IAuthService authService)
    : IRequestHandler<Enable2FACommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(Enable2FACommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(request.UserId);
        var result = await authService.Enable2FAAsync(userId, new DTOs.Auth.Enable2FARequest { Password = "" }, ct);
        return result.Success
            ? ApiResponse<string>.Ok("2FA enabled")
            : ApiResponse<string>.Fail(string.Join(", ", result.Errors));
    }
}

public class Verify2FACommandHandler(IAuthService authService)
    : IRequestHandler<Verify2FACommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(Verify2FACommand request, CancellationToken ct)
    {
        var userId = Guid.Parse(request.Dto.UserId);
        var result = await authService.Verify2FAAsync(userId, new DTOs.Auth.Verify2FARequest { Code = request.Dto.Code }, ct);
        return result.Succeeded ? ApiResponse.Ok("2FA verified") : ApiResponse.Fail(string.Join(", ", result.Errors));
    }
}
