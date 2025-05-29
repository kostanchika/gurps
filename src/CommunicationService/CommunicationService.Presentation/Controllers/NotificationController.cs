using CommunicationService.Application.Dto.Notification;
using CommunicationService.Application.Features.Notification.Commands.HideNotification;
using CommunicationService.Application.Features.Notification.Commands.ReadNotification;
using CommunicationService.Application.Features.Notification.Queries.GetNotificationsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunicationService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<NotificationDto>> GetNotifications(
            [FromQuery] bool onlyNew,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _mediator.Send(new GetNotificationsQuery(userLogin, onlyNew), cancellationToken);
        }

        [HttpPost("{id}/read")]
        [Authorize]
        public async Task<NotificationDto> ReadNotification(
            string id,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return await _mediator.Send(new ReadNotificationCommand(userLogin, id), cancellationToken);
        }

        [HttpPost("{id}/hide")]
        [Authorize]
        public async Task<NotificationDto> HideNotification(
            string id,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return await _mediator.Send(new HideNotificationCommand(userLogin, id), cancellationToken);
        }
    }
}
