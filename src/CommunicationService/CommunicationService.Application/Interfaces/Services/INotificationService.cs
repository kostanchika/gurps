using CommunicationService.Application.Dto.Notification;

namespace CommunicationService.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task NotifyNotificationSent(NotificationDto notificationDto, CancellationToken cancellationToken = default);
    }
}
