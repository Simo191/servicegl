using System.ComponentModel.DataAnnotations;
using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Auth;

// ==================== AUTH REQUESTS ====================
public class RegisterRequest
{
    [Required, MaxLength(50)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MinLength(8)] public string Password { get; set; } = string.Empty;
    [Required, Compare(nameof(Password))] public string ConfirmPassword { get; set; } = string.Empty;
    [Required, Phone] public string PhoneNumber { get; set; } = string.Empty;
    public Gender? Gender { get; set; }
    public Language PreferredLanguage { get; set; } = Language.French;
}

public class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    public string? DeviceToken { get; set; }
}

public class SocialLoginRequest
{
    [Required] public string Provider { get; set; } = string.Empty; // "Google", "Facebook", "Apple"
    [Required] public string AccessToken { get; set; } = string.Empty;
    public string? DeviceToken { get; set; }
}

public class RefreshTokenRequest
{
    [Required] public string Token { get; set; } = string.Empty;
    [Required] public string RefreshToken { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Token { get; set; } = string.Empty;
    [Required, MinLength(8)] public string NewPassword { get; set; } = string.Empty;
    [Required, Compare(nameof(NewPassword))] public string ConfirmPassword { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    [Required] public string CurrentPassword { get; set; } = string.Empty;
    [Required, MinLength(8)] public string NewPassword { get; set; } = string.Empty;
    [Required, Compare(nameof(NewPassword))] public string ConfirmPassword { get; set; } = string.Empty;
}

public class VerifyPhoneRequest
{
    [Required, Phone] public string PhoneNumber { get; set; } = string.Empty;
    [Required, StringLength(6)] public string Otp { get; set; } = string.Empty;
}

public class Enable2FARequest
{
    [Required] public string Password { get; set; } = string.Empty;
}

public class Verify2FARequest
{
    [Required] public string Code { get; set; } = string.Empty;
}

// ==================== AUTH RESPONSES ====================
public class AuthResponse
{
    public bool Succeeded { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenExpiration { get; set; }
    public UserDto User { get; set; } = null!;
    public List<string> Errors { get; set; } = new();
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public Language PreferredLanguage { get; set; }
    public bool IsVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public List<string> Roles { get; set; } = new();
    public string FullName => $"{FirstName} {LastName}";
}
