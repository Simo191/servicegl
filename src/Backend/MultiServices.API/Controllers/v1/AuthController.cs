using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Identity;
using MultiServices.Application.Features.Identity.Commands;
using MultiServices.Application.Features.Identity.Queries;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
        => Ok(await _mediator.Send(new RegisterCommand(dto)));

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto dto)
        => Ok(await _mediator.Send(new LoginCommand(dto)));

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
        => Ok(await _mediator.Send(new RefreshTokenCommand(dto)));

    [HttpPost("external-login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> ExternalLogin([FromBody] ExternalLoginDto dto)
        => Ok(await _mediator.Send(new ExternalLoginCommand(dto)));

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        => Ok(await _mediator.Send(new ForgotPasswordCommand(dto)));

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordDto dto)
        => Ok(await _mediator.Send(new ResetPasswordCommand(dto)));

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordDto dto)
        => Ok(await _mediator.Send(new ChangePasswordCommand(dto)));

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse>> Logout()
        => Ok(await _mediator.Send(new LogoutCommand()));

    [HttpPost("send-otp")]
    public async Task<ActionResult<ApiResponse>> SendOtp([FromBody] string phoneNumber)
        => Ok(await _mediator.Send(new SendOtpCommand(phoneNumber)));

    [HttpPost("verify-otp")]
    public async Task<ActionResult<ApiResponse>> VerifyOtp([FromBody] VerifyOtpDto dto)
        => Ok(await _mediator.Send(new VerifyPhoneCommand(dto)));

    [Authorize]
    [HttpPost("enable-2fa")]
    public async Task<ActionResult<ApiResponse<string>>> Enable2FA()
        => Ok(await _mediator.Send(new Enable2FACommand("")));

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
        => Ok(await _mediator.Send(new GetProfileQuery()));

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateProfileDto dto)
        => Ok(await _mediator.Send(new UpdateProfileCommand(dto)));

    [Authorize]
    [HttpPost("profile/image")]
    public async Task<ActionResult<ApiResponse<string>>> UploadProfileImage(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return Ok(await _mediator.Send(new UploadProfileImageCommand(stream, file.FileName, file.ContentType)));
    }

    [Authorize]
    [HttpPost("addresses")]
    public async Task<ActionResult<ApiResponse<Guid>>> AddAddress([FromBody] AddAddressDto dto)
        => Ok(await _mediator.Send(new AddUserAddressCommand(dto)));

    [Authorize]
    [HttpPut("addresses/{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateAddress(Guid id, [FromBody] AddAddressDto dto)
        => Ok(await _mediator.Send(new UpdateUserAddressCommand(id, dto)));

    [Authorize]
    [HttpDelete("addresses/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteAddress(Guid id)
        => Ok(await _mediator.Send(new DeleteUserAddressCommand(id)));

    [Authorize]
    [HttpGet("addresses")]
    public async Task<ActionResult<ApiResponse<List<AddAddressDto>>>> GetAddresses()
        => Ok(await _mediator.Send(new GetUserAddressesQuery()));

    [Authorize]
    [HttpDelete("account")]
    public async Task<ActionResult<ApiResponse>> DeleteAccount()
        => Ok(await _mediator.Send(new DeleteAccountCommand()));
}
