using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Identity;

namespace MultiServices.Application.Features.Identity.Commands;

public record RegisterCommand(RegisterDto Dto) : IRequest<ApiResponse<AuthResponseDto>>;
public record LoginCommand(LoginDto Dto) : IRequest<ApiResponse<AuthResponseDto>>;
public record RefreshTokenCommand(RefreshTokenDto Dto) : IRequest<ApiResponse<AuthResponseDto>>;
public record ExternalLoginCommand(ExternalLoginDto Dto) : IRequest<ApiResponse<AuthResponseDto>>;
public record ForgotPasswordCommand(ForgotPasswordDto Dto) : IRequest<ApiResponse>;
public record ResetPasswordCommand(ResetPasswordDto Dto) : IRequest<ApiResponse>;
public record ChangePasswordCommand(ChangePasswordDto Dto) : IRequest<ApiResponse>;
public record LogoutCommand : IRequest<ApiResponse>;
public record VerifyPhoneCommand(VerifyOtpDto Dto) : IRequest<ApiResponse>;
public record SendOtpCommand(string PhoneNumber) : IRequest<ApiResponse>;
public record Enable2FACommand(string UserId) : IRequest<ApiResponse<string>>;
public record Verify2FACommand(Verify2FADto Dto) : IRequest<ApiResponse>;
public record UpdateProfileCommand(UpdateProfileDto Dto) : IRequest<ApiResponse<UserDto>>;
public record UploadProfileImageCommand(Stream Image, string FileName, string ContentType) : IRequest<ApiResponse<string>>;
public record AddUserAddressCommand(AddAddressDto Dto) : IRequest<ApiResponse<Guid>>;
public record UpdateUserAddressCommand(Guid AddressId, AddAddressDto Dto) : IRequest<ApiResponse>;
public record DeleteUserAddressCommand(Guid AddressId) : IRequest<ApiResponse>;
public record DeleteAccountCommand : IRequest<ApiResponse>;
