namespace MultiServices.Application.DTOs.Identity;

public record RegisterDto(string Email, string Password, string FirstName, string LastName, string Phone, string? CountryCode = "+212");
public record LoginDto(string Email, string Password, string? DeviceToken = null, string? Platform = null);
public record RefreshTokenDto(string AccessToken, string RefreshToken);
public record ExternalLoginDto(string Provider, string Token);
public record ForgotPasswordDto(string Email);
public record ResetPasswordDto(string Email, string Token, string NewPassword);
public record ChangePasswordDto(string CurrentPassword, string NewPassword);
public record VerifyOtpDto(string PhoneNumber, string Otp);
public record Enable2FADto(string UserId);
public record Verify2FADto(string UserId, string Code);

public record AuthResponseDto(string AccessToken, string RefreshToken, UserDto User);

public record UserDto(
    Guid Id, string Email, string FirstName, string LastName, string? ProfileImageUrl,
    string Phone, string Role, bool PhoneVerified, bool EmailVerified,
    int LoyaltyPoints, decimal WalletBalance, string? Language);

public record UpdateProfileDto(string? FirstName, string? LastName, string? Phone, string? Language);
public record AddAddressDto(string Street, string City, string PostalCode, string? Country,
    string? Apartment, string? BuildingName, double Latitude, double Longitude, string? Label, bool IsDefault);
