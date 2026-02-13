using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.Features.Payment.Commands;
using MultiServices.Domain.Enums;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create")]
    public async Task<ActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPost("confirm")]
    public async Task<ActionResult> ConfirmPayment([FromBody] string paymentIntentId)
        => Ok(await _mediator.Send(new ConfirmPaymentCommand(paymentIntentId)));

    [HttpPost("methods")]
    public async Task<ActionResult> SavePaymentMethod([FromBody] string cardToken)
        => Ok(await _mediator.Send(new SavePaymentMethodCommand(cardToken)));

    [HttpGet("methods")]
    public async Task<ActionResult> GetPaymentMethods()
        => Ok(await _mediator.Send(new GetSavedPaymentMethodsQuery()));

    [HttpDelete("methods/{id}")]
    public async Task<ActionResult> DeletePaymentMethod(Guid id)
        => Ok(await _mediator.Send(new DeletePaymentMethodCommand(id)));

    [HttpGet("wallet/balance")]
    public async Task<ActionResult> GetWalletBalance()
        => Ok(await _mediator.Send(new GetWalletBalanceQuery()));

    [HttpPost("wallet/topup")]
    public async Task<ActionResult> TopUpWallet([FromBody] TopUpWalletCommand command)
        => Ok(await _mediator.Send(command));

    [HttpGet("wallet/transactions")]
    public async Task<ActionResult> GetWalletTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetWalletTransactionsQuery(page, pageSize)));

    [HttpPost("promo/apply")]
    public async Task<ActionResult> ApplyPromoCode([FromBody] ApplyPromoCodeCommand command)
        => Ok(await _mediator.Send(command));

    [HttpGet("history")]
    public async Task<ActionResult> GetPaymentHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetPaymentHistoryQuery(page, pageSize)));
}
