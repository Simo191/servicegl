using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MultiServices.Domain.Common;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Interfaces.Repositories;
using MultiServices.Domain.Interfaces.Services;

namespace MultiServices.Infrastructure.Services.Auth;

public class JwtTokenService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IGenericRepository<ApplicationUser> _userRepo;
    private readonly IUnitOfWork _uow;

    public JwtTokenService(IConfiguration config, IGenericRepository<ApplicationUser> userRepo, IUnitOfWork uow)
    {
        _config = config;
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<Result<(string AccessToken, string RefreshToken)>> LoginAsync(string email, string password)
    {
        var users = await _userRepo.FindAsync(u => u.Email == email.ToLower());
        var user = users.FirstOrDefault();
        if (user == null) return Result<(string, string)>.Failure("Invalid credentials");
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            return Result<(string, string)>.Failure("Account is locked. Try again later.");
        if (!VerifyPassword(password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                user.FailedLoginAttempts = 0;
            }
            await _userRepo.UpdateAsync(user);
            await _uow.SaveChangesAsync();
            return Result<(string, string)>.Failure("Invalid credentials");
        }

        user.FailedLoginAttempts = 0;
        user.LastLoginAt = DateTime.UtcNow;
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepo.UpdateAsync(user);
        await _uow.SaveChangesAsync();

        return Result<(string, string)>.Success((accessToken, refreshToken));
    }

    public async Task<Result<(string AccessToken, string RefreshToken)>> RegisterAsync(ApplicationUser user, string password)
    {
        if (await _userRepo.AnyAsync(u => u.Email == user.Email.ToLower()))
            return Result<(string, string)>.Failure("Email already registered");

        user.Email = user.Email.ToLower();
        user.PasswordHash = HashPassword(password);
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userRepo.AddAsync(user);
        await _uow.SaveChangesAsync();

        var accessToken = GenerateAccessToken(user);
        return Result<(string, string)>.Success((accessToken, refreshToken));
    }

    public async Task<Result<(string AccessToken, string RefreshToken)>> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null) return Result<(string, string)>.Failure("Invalid token");

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Result<(string, string)>.Failure("Invalid token");

        var user = await _userRepo.GetByIdAsync(Guid.Parse(userId));
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return Result<(string, string)>.Failure("Invalid or expired refresh token");

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepo.UpdateAsync(user);
        await _uow.SaveChangesAsync();

        return Result<(string, string)>.Success((newAccessToken, newRefreshToken));
    }

    public async Task<Result> RevokeTokenAsync(string userId)
    {
        var user = await _userRepo.GetByIdAsync(Guid.Parse(userId));
        if (user == null) return Result.Failure("User not found");
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _userRepo.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        return Result.Success();
    }

    public Task<Result<(string AccessToken, string RefreshToken)>> ExternalLoginAsync(string provider, string token)
        => Task.FromResult(Result<(string, string)>.Failure("Not implemented yet"));

    public Task<Result> SendOtpAsync(string phoneNumber)
        => Task.FromResult(Result.Success());

    public Task<Result> VerifyOtpAsync(string phoneNumber, string otp)
        => Task.FromResult(Result.Success());

    public Task<Result> SendPasswordResetAsync(string email)
        => Task.FromResult(Result.Success());

    public Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
        => Task.FromResult(Result.Success());

    public Task<Result> Enable2FAAsync(string userId)
        => Task.FromResult(Result.Success());

    public Task<Result> Verify2FAAsync(string userId, string code)
        => Task.FromResult(Result.Success());

    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, 12);
    public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);

    private string GenerateAccessToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60")),
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
        var validation = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _config["Jwt:Audience"],
            ValidateIssuer = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!)),
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validation, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }
}
