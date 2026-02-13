namespace MultiServices.Domain.Interfaces.Services;

/// <summary>
/// Date/Time service for testability
/// </summary>
public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}

/// <summary>
/// Email service
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default);
    Task SendTemplatedEmailAsync(string to, string templateId, Dictionary<string, string> variables, CancellationToken ct = default);
}

/// <summary>
/// SMS service
/// </summary>
public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken ct = default);
    Task SendOtpAsync(string phoneNumber, CancellationToken ct = default);
    Task<bool> VerifyOtpAsync(string phoneNumber, string otp, CancellationToken ct = default);
}

/// <summary>
/// Push notification service (Firebase/APNS)
/// </summary>
public interface IPushNotificationService
{
    Task SendToUserAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
    Task SendToMultipleAsync(IEnumerable<Guid> userIds, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
    Task SendToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
}

/// <summary>
/// File storage service (Azure Blob / AWS S3)
/// </summary>
public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default);
    Task<string> UploadBase64Async(string base64, string fileName, string folder, CancellationToken ct = default);
    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string fileUrl, CancellationToken ct = default);
    string GetPublicUrl(string filePath);
}

/// <summary>
/// Redis cache service
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
}

/// <summary>
/// AI/ML recommendation service
/// </summary>
public interface IAiService
{
    Task<IEnumerable<Guid>> GetRecommendedRestaurantsAsync(Guid userId, double lat, double lon, int count = 10, CancellationToken ct = default);
    Task<IEnumerable<Guid>> GetRecommendedProductsAsync(Guid userId, Guid storeId, int count = 10, CancellationToken ct = default);
    Task<Guid?> FindOptimalDelivererAsync(double pickupLat, double pickupLon, double deliveryLat, double deliveryLon, CancellationToken ct = default);
    Task<int> PredictDemandAsync(string module, DateTime date, string? zone = null, CancellationToken ct = default);
}

// Supporting records
public record RouteInfo(double DistanceKm, int EstimatedMinutes, string? PolylineEncoded);
public record GeocodingResult(string FormattedAddress, double Latitude, double Longitude, string? City, string? PostalCode);
public record PaymentIntentResult(string PaymentIntentId, string ClientSecret, string Status);
public record RefundResult(string RefundId, decimal Amount, string Status);
public record SavedCardResult(string CardId, string Last4, string Brand, int ExpiryMonth, int ExpiryYear);
