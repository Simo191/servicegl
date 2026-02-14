namespace MultiServices.Maui.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Aligned with backend MultiServices.Application.DTOs.Auth.AuthResponse
/// Backend fields: Succeeded, Token, RefreshToken, TokenExpiration, User, Errors
/// Also compatible with Identity.AuthResponseDto: AccessToken, RefreshToken, User
/// </summary>
public class AuthResponse
{
    // Backend Auth DTOs use "Token", Identity DTOs use "AccessToken"
    // We accept both via Newtonsoft deserialization
    public string Token { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime? TokenExpiration { get; set; }
    public bool Succeeded { get; set; }
    public List<string>? Errors { get; set; }
    public UserDto User { get; set; } = new();

    /// <summary>Returns whichever token field the backend populated</summary>
    public string GetToken() => !string.IsNullOrEmpty(Token) ? Token : AccessToken;
}

/// <summary>
/// Aligned with backend MultiServices.Application.DTOs.Auth.UserDto
/// Backend: Id, FirstName, LastName, Email, PhoneNumber, ProfileImageUrl,
///          PreferredLanguage, IsVerified, TwoFactorEnabled, Roles, FullName
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    // Backend fields
    public string PreferredLanguage { get; set; } = "French";
    public bool IsVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public List<string> Roles { get; set; } = new();

    // Client-side computed (not from backend)
    public int LoyaltyPoints { get; set; }
    public string LoyaltyTier { get; set; } = "Bronze";
}

/// <summary>
/// Aligned with backend MultiServices.Application.DTOs.Common.AddressDto
/// </summary>
public class AddressDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public string? Apartment { get; set; }
    public string? Instructions { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; }
}

/// <summary>
/// Aligned with backend MultiServices.Application.DTOs.Common.NotificationDto
/// </summary>
public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    // Backend uses "Message", keep both for compatibility
    public string Body { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public string DisplayBody => !string.IsNullOrEmpty(Body) ? Body : Message;
}

/// <summary>
/// Aligned with backend MultiServices.Application.DTOs.Common.ReviewDto
/// Backend: Id, UserName, Stars, Comment, Reply, CreatedAt
/// </summary>
public class ReviewDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserImageUrl { get; set; }
    // Backend uses "Stars", keep both
    public int Rating { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
    public string? Reply { get; set; }
    public DateTime CreatedAt { get; set; }

    public int DisplayRating => Stars > 0 ? Stars : Rating;
}

public class WalletDto
{
    public decimal Balance { get; set; }
    public List<WalletTransactionDto> RecentTransactions { get; set; } = new();
}

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
