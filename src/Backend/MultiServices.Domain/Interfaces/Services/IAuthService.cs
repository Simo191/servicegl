using MultiServices.Domain.Common;
using MultiServices.Domain.Entities.Identity;

namespace MultiServices.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<Result<(string AccessToken, string RefreshToken)>> LoginAsync(string email, string password);
    Task<Result<(string AccessToken, string RefreshToken)>> RegisterAsync(ApplicationUser user, string password);
    Task<Result<(string AccessToken, string RefreshToken)>> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<Result> RevokeTokenAsync(string userId);
    Task<Result<(string AccessToken, string RefreshToken)>> ExternalLoginAsync(string provider, string token);
    Task<Result> SendOtpAsync(string phoneNumber);
    Task<Result> VerifyOtpAsync(string phoneNumber, string otp);
    Task<Result> SendPasswordResetAsync(string email);
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
    Task<Result> Enable2FAAsync(string userId);
    Task<Result> Verify2FAAsync(string userId, string code);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
