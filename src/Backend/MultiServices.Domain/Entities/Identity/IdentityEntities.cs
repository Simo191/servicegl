using Microsoft.AspNetCore.Identity;
using MultiServices.Domain.Entities.Common;
using MultiServices.Domain.Enums;
using MultiServices.Domain.ValueObjects;

namespace MultiServices.Domain.Entities.Identity;

/// <summary>
/// Application user extending ASP.NET Identity
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Language PreferredLanguage { get; set; } = Language.French;
    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public string? DeviceToken { get; set; } // FCM Token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool TwoFactorEnabled { get; set; } = false;
    public string? TwoFactorSecret { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEndDate { get; set; }

    // Navigation
    public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    public virtual ICollection<UserDocument> Documents { get; set; } = new List<UserDocument>();
    public virtual ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual WalletAccount? Wallet { get; set; }
    public virtual LoyaltyAccount? LoyaltyAccount { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<SavedPaymentMethod> SavedPaymentMethods { get; set; } = new List<SavedPaymentMethod>();

    public string FullName => $"{FirstName} {LastName}";
}

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class UserAddress : BaseEntity
{
    public Guid UserId { get; set; }
    public string Label { get; set; } = string.Empty; // "Maison", "Travail"
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "Morocco";
    public string? Apartment { get; set; }
    public string? Floor { get; set; }
    public string? BuildingName { get; set; }
    public string? AdditionalInfo { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}

public class UserDocument : BaseEntity
{
    public Guid UserId { get; set; }
    public DocumentType Type { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}

public class UserFavorite : BaseEntity
{
    public Guid UserId { get; set; }
    public string EntityType { get; set; } = string.Empty; // "Restaurant", "ServiceProvider", "GroceryStore"
    public Guid EntityId { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}

public class Review : BaseEntity
{
    public Guid UserId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public Guid? OrderId { get; set; }
    public int Stars { get; set; } // 1-5
    public string? Comment { get; set; }
    public string? Reply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public bool IsVisible { get; set; } = true;
    public List<string> ImageUrls { get; set; } = new();

    public virtual ApplicationUser User { get; set; } = null!;
}

public class WalletAccount : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal Balance { get; set; } = 0;
    public string Currency { get; set; } = "MAD";
    public bool IsActive { get; set; } = true;

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
}

public class WalletTransaction : BaseEntity
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // "Credit", "Debit"
    public string Description { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }

    public virtual WalletAccount Wallet { get; set; } = null!;
}

public class LoyaltyAccount : BaseEntity
{
    public Guid UserId { get; set; }
    public int Points { get; set; } = 0;
    public string Tier { get; set; } = "Bronze"; // Bronze, Silver, Gold, Platinum
    public int TotalPointsEarned { get; set; } = 0;
    public int TotalPointsRedeemed { get; set; } = 0;

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<LoyaltyTransaction> Transactions { get; set; } = new List<LoyaltyTransaction>();
}

public class LoyaltyTransaction : BaseEntity
{
    public Guid LoyaltyAccountId { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = string.Empty; // "Earned", "Redeemed"
    public string Description { get; set; } = string.Empty;
    public string? OrderId { get; set; }

    public virtual LoyaltyAccount LoyaltyAccount { get; set; } = null!;
}

public class SavedPaymentMethod : BaseEntity
{
    public Guid UserId { get; set; }
    public PaymentMethod Type { get; set; }
    public string? TokenizedCardId { get; set; }
    public string? Last4Digits { get; set; }
    public string? CardBrand { get; set; } // Visa, Mastercard
    public int? ExpiryMonth { get; set; }
    public int? ExpiryYear { get; set; }
    public bool IsDefault { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? Data { get; set; } // JSON payload
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}

public class Referral : BaseEntity
{
    public Guid ReferrerId { get; set; }
    public Guid? ReferredUserId { get; set; }
    public string ReferralCode { get; set; } = string.Empty;
    public bool IsUsed { get; set; } = false;
    public decimal ReferrerReward { get; set; }
    public decimal ReferredReward { get; set; }
    public DateTime? UsedAt { get; set; }

    public virtual ApplicationUser Referrer { get; set; } = null!;
    public virtual ApplicationUser? ReferredUser { get; set; }
}
