using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiServices.Application.Features.Notifications.Commands;

namespace MultiServices.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public NotificationsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult> GetNotifications([FromQuery] bool? unreadOnly, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetNotificationsQuery(unreadOnly, page, pageSize)));

    [HttpGet("unread-count")]
    public async Task<ActionResult> GetUnreadCount()
        => Ok(await _mediator.Send(new GetUnreadCountQuery()));

    [HttpPost("{id}/read")]
    public async Task<ActionResult> MarkRead(Guid id)
        => Ok(await _mediator.Send(new MarkNotificationReadCommand(id)));

    [HttpPost("read-all")]
    public async Task<ActionResult> MarkAllRead()
        => Ok(await _mediator.Send(new MarkAllNotificationsReadCommand()));

    [HttpPost("device-token")]
    public async Task<ActionResult> RegisterDevice([FromBody] RegisterDeviceTokenCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("preferences")]
    public async Task<ActionResult> UpdatePreferences([FromBody] UpdateNotificationPreferencesCommand command)
        => Ok(await _mediator.Send(command));
}
