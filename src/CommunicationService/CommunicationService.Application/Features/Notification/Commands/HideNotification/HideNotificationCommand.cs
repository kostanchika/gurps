using CommunicationService.Application.Dto.Notification;
using MediatR;

namespace CommunicationService.Application.Features.Notification.Commands.HideNotification
{
    public record HideNotificationCommand(
        string UserLogin,
        string NotificationId
    ) : IRequest;
}
