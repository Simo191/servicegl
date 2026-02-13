using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Domain.Enums;

namespace MultiServices.Application.Features.Payment.Commands;

public record CreatePaymentCommand(Guid OrderId, string OrderType, decimal Amount, PaymentMethod Method) : IRequest<ApiResponse<string>>;
public record ConfirmPaymentCommand(string PaymentIntentId) : IRequest<ApiResponse>;
public record SavePaymentMethodCommand(string CardToken) : IRequest<ApiResponse>;
public record DeletePaymentMethodCommand(Guid PaymentMethodId) : IRequest<ApiResponse>;
public record TopUpWalletCommand(decimal Amount, PaymentMethod Method) : IRequest<ApiResponse>;
public record ApplyPromoCodeCommand(string Code, string OrderType, decimal OrderAmount) : IRequest<ApiResponse<decimal>>;

public record GetSavedPaymentMethodsQuery : IRequest<ApiResponse<List<object>>>;
public record GetWalletBalanceQuery : IRequest<ApiResponse<decimal>>;
public record GetWalletTransactionsQuery(int Page = 1, int PageSize = 20) : IRequest<ApiResponse<object>>;
public record GetPaymentHistoryQuery(int Page = 1, int PageSize = 20) : IRequest<ApiResponse<object>>;
