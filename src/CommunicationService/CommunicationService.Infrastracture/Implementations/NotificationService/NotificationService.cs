using CommunicationService.Application.Dto.Notification;
using CommunicationService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CommunicationService.Infrastracture.Implementations.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNotificationSent(NotificationDto notificationDto, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.User(notificationDto.UserLogin)
                .SendAsync("ReceiveNotification", notificationDto, cancellationToken: cancellationToken);
        }
    }
}
