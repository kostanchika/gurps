using MediatR;

namespace CommunicationService.Application.Features.Notification.Commands.ReadNotification
{
    public record ReadNotificationCommand(
        string UserLogin,
        string NotificationId
    ) : IRequest;
}
