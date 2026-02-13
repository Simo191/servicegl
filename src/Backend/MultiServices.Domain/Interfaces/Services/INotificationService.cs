using MultiServices.Domain.Common;

namespace MultiServices.Domain.Interfaces.Services;

public interface INotificationService
{
    Task<Result> SendPushAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null);
    Task<Result> SendPushToDeviceAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null);
    Task<Result> SendEmailAsync(string to, string subject, string htmlBody);
    Task<Result> SendSmsAsync(string phoneNumber, string message);
    Task<Result> SendBulkPushAsync(IEnumerable<Guid> userIds, string title, string body, Dictionary<string, string>? data = null);
}
