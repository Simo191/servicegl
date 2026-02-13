using MultiServices.Domain.Common;
using MultiServices.Domain.Enums;

namespace MultiServices.Domain.Interfaces.Services;

public interface IPaymentService
{
    Task<Result<string>> CreatePaymentIntentAsync(decimal amount, string currency, PaymentMethod method, Dictionary<string, string>? metadata = null);
    Task<Result> ConfirmPaymentAsync(string paymentIntentId);
    Task<Result> RefundPaymentAsync(string paymentIntentId, decimal? amount = null, string? reason = null);
    Task<Result<string>> CreateCustomerAsync(string email, string name);
    Task<Result> SaveCardAsync(string customerId, string cardToken);
    Task<Result<IReadOnlyList<object>>> GetSavedCardsAsync(string customerId);
}
