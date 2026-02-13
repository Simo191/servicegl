using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.DTOs.Admin;
using MultiServices.Application.Features.Admin.Commands;
using MultiServices.Application.Features.Admin.Queries;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _mediator.Send(new GetDashboardQuery(from, to)));

    [HttpGet("users")]
    public async Task<ActionResult> GetUsers([FromQuery] string? role, [FromQuery] string? search,
        [FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetAllUsersQuery(role, search, isActive, page, pageSize)));

    [HttpPost("users/{id}/suspend")]
    public async Task<ActionResult> SuspendUser(Guid id, [FromBody] string reason)
        => Ok(await _mediator.Send(new SuspendUserCommand(id, reason)));

    [HttpPost("users/{id}/activate")]
    public async Task<ActionResult> ActivateUser(Guid id)
        => Ok(await _mediator.Send(new ActivateUserCommand(id)));

    [HttpGet("approvals")]
    public async Task<ActionResult> GetPendingApprovals([FromQuery] string? entityType, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetPendingApprovalsQuery(entityType, page, pageSize)));

    [HttpPost("approvals")]
    public async Task<ActionResult> ApproveEntity([FromBody] ApproveEntityDto dto)
        => Ok(await _mediator.Send(new ApproveEntityCommand(dto)));

    [HttpPut("commissions")]
    public async Task<ActionResult> UpdateCommissions([FromBody] CommissionSettingsDto dto)
        => Ok(await _mediator.Send(new UpdateCommissionSettingsCommand(dto)));

    [HttpGet("orders")]
    public async Task<ActionResult> GetAllOrders([FromQuery] string? type, [FromQuery] string? status,
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetAllOrdersQuery(type, status, from, to, page, pageSize)));

    [HttpPost("orders/{id}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid id, [FromQuery] string orderType, [FromBody] string reason)
        => Ok(await _mediator.Send(new CancelOrderAdminCommand(id, orderType, reason)));

    [HttpPost("orders/{id}/reassign-driver")]
    public async Task<ActionResult> ReassignDriver(Guid id, [FromQuery] string orderType, [FromBody] Guid newDriverId)
        => Ok(await _mediator.Send(new ReassignDriverCommand(id, orderType, newDriverId)));

    [HttpPost("orders/{id}/refund")]
    public async Task<ActionResult> ProcessRefund(Guid id, [FromQuery] string orderType, [FromBody] ProcessRefundCommand command)
        => Ok(await _mediator.Send(command));

    [HttpGet("finance/report")]
    public async Task<ActionResult> GetFinancialReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        => Ok(await _mediator.Send(new GetFinancialReportQuery(startDate, endDate)));

    [HttpPost("finance/export")]
    public async Task<ActionResult> ExportReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format = "xlsx")
        => Ok(await _mediator.Send(new ExportFinancialReportCommand(startDate, endDate, format)));

    [HttpPost("promotions")]
    public async Task<ActionResult> CreatePromotion([FromBody] CreatePromotionDto dto)
        => Ok(await _mediator.Send(new CreateGlobalPromotionCommand(dto)));

    [HttpPost("notifications/bulk")]
    public async Task<ActionResult> SendBulkNotification([FromBody] SendBulkNotificationCommand command)
        => Ok(await _mediator.Send(command));

    [HttpGet("logs")]
    public async Task<ActionResult> GetLogs([FromQuery] string? level, [FromQuery] DateTime? from,
        [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        => Ok(await _mediator.Send(new GetSystemLogsQuery(level, from, to, page, pageSize)));
}
