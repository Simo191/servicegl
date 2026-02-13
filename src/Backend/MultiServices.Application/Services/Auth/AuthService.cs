using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MultiServices.Application.DTOs.Auth;
using MultiServices.Application.DTOs.Common;
using MultiServices.Application.Interfaces;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Enums;
using MultiServices.Domain.Interfaces.Services;

namespace MultiServices.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;
    private readonly ICacheService _cacheService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ISmsService smsService,
        IEmailService emailService,
        ICacheService cacheService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _smsService = smsService;
        _emailService = emailService;
        _cacheService = cacheService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return new AuthResponse { Succeeded = false, Errors = new List<string> { "Email already registered" } };

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            Gender = request.Gender,
            PreferredLanguage = request.PreferredLanguage,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return new AuthResponse { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };

        // Assign default role
        if (!await _roleManager.RoleExistsAsync("Client"))
            await _roleManager.CreateAsync(new ApplicationRole { Name = "Client", Description = "Client role" });
        await _userManager.AddToRoleAsync(user, "Client");

        // Send verification email
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendTemplatedEmailAsync(user.Email, "welcome", new Dictionary<string, string>
        {
            { "name", user.FirstName },
            { "token", emailToken }
        });

        // Generate tokens
        var token = await GenerateJwtTokenAsync(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponse
        {
            Succeeded = true,
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiration = DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24")),
            User = MapToUserDto(user, roles.ToList())
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return new AuthResponse { Succeeded = false, Errors = new List<string> { "Invalid credentials" } };

        if (!user.IsActive)
            return new AuthResponse { Succeeded = false, Errors = new List<string> { "Account is suspended" } };

        // Check lockout
        if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
            return new AuthResponse { Succeeded = false, Errors = new List<string> { "Account is locked. Try again later." } };

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEndDate = DateTime.UtcNow.AddMinutes(30);
                user.FailedLoginAttempts = 0;
            }
            await _userManager.UpdateAsync(user);
            return new AuthResponse { Succeeded = false, Errors = new List<string> { "Invalid credentials" } };
        }

        // Reset failed attempts
        user.FailedLoginAttempts = 0;
        user.LastLoginAt = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(request.DeviceToken))
            user.DeviceToken = request.DeviceToken;

        var token = await GenerateJwtTokenAsync(user);
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponse
        {
            Succeeded = true,
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiration = DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24")),
            User = MapToUserDto(user, roles.ToList())
        };
    }

    public async Task<AuthResponse> SocialLoginAsync(SocialLoginRequest request, CancellationToken ct = default)
    {
        // TODO: Validate social token with provider (Google/Facebook/Apple)
        // For now, placeholder implementation
        throw new NotImplementedException("Social login provider validation not yet implemented");
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var principal = GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
            return new AuthResponse { Succeeded = false, Errors = new List<string> { "Invalid token" } };

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return new AuthResponse { Succeeded = false, Errors = new List<string> { "Invalid token" } };

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return new AuthResponse { Succeeded = false, Errors = new List<string> { "Invalid or expired refresh token" } };

        var newToken = await GenerateJwtTokenAsync(user);
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponse
        {
            Succeeded = true,
            Token = newToken,
            RefreshToken = newRefreshToken,
            TokenExpiration = DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24")),
            User = MapToUserDto(user, roles.ToList())
        };
    }

    public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return ApiResponse.Ok("If email exists, reset link sent"); // Don't reveal

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendTemplatedEmailAsync(request.Email, "password-reset", new Dictionary<string, string>
        {
            { "name", user.FirstName },
            { "token", token }
        });

        return ApiResponse.Ok("If email exists, reset link sent");
    }

    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return ApiResponse.Fail("Invalid request");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded) return ApiResponse.Fail("Reset failed");

        return ApiResponse.Ok("Password reset successfully");
    }

    public async Task<ApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return ApiResponse.Fail("User not found");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded) return ApiResponse.Fail("Password change failed");

        return ApiResponse.Ok("Password changed successfully");
    }

    public async Task<ApiResponse> VerifyPhoneAsync(VerifyPhoneRequest request, CancellationToken ct = default)
    {
        var isValid = await _smsService.VerifyOtpAsync(request.PhoneNumber, request.Otp);
        if (!isValid) return ApiResponse.Fail("Invalid OTP");
        return ApiResponse.Ok("Phone verified");
    }

    public async Task<ApiResponse> SendPhoneVerificationAsync(string phoneNumber, CancellationToken ct = default)
    {
        await _smsService.SendOtpAsync(phoneNumber);
        return ApiResponse.Ok("OTP sent");
    }

    public async Task<ApiResponse> Enable2FAAsync(Guid userId, Enable2FARequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return ApiResponse.Fail("User not found");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return ApiResponse.Fail("Invalid password");

        user.TwoFactorEnabled = true;
        user.TwoFactorSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        await _userManager.UpdateAsync(user);

        return ApiResponse.Ok("2FA enabled");
    }

    public async Task<AuthResponse> Verify2FAAsync(Guid userId, Verify2FARequest request, CancellationToken ct = default)
    {
        // TODO: Implement TOTP verification
        throw new NotImplementedException();
    }

    public async Task<ApiResponse> LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return ApiResponse.Fail("User not found");

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        user.DeviceToken = null;
        await _userManager.UpdateAsync(user);

        // Blacklist current token
        await _cacheService.SetAsync($"blacklist:{userId}", true, TimeSpan.FromHours(24));

        return ApiResponse.Ok("Logged out successfully");
    }

    public async Task<ApiResponse> DeleteAccountAsync(Guid userId, string password, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return ApiResponse.Fail("User not found");

        if (!await _userManager.CheckPasswordAsync(user, password))
            return ApiResponse.Fail("Invalid password");

        user.IsActive = false;
        user.Email = $"deleted_{user.Id}@deleted.com";
        user.UserName = $"deleted_{user.Id}";
        user.PhoneNumber = null;
        user.FirstName = "Deleted";
        user.LastName = "User";
        await _userManager.UpdateAsync(user);

        return ApiResponse.Ok("Account deleted");
    }

    // ==================== PRIVATE HELPERS ====================
    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var expiration = DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24"));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }

    private static UserDto MapToUserDto(ApplicationUser user, List<string> roles) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email!,
        PhoneNumber = user.PhoneNumber,
        ProfileImageUrl = user.ProfileImageUrl,
        PreferredLanguage = user.PreferredLanguage,
        IsVerified = user.IsVerified,
        TwoFactorEnabled = user.TwoFactorEnabled,
        Roles = roles
    };
}
