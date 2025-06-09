using CommunicationService.Application.Exceptions.Notification;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Notification.Commands.ReadNotification
{
    public class ReadNotificationHandler : IRequestHandler<ReadNotificationCommand>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<ReadNotificationHandler> _logger;

        public ReadNotificationHandler(
            INotificationRepository notificationRepository,
            ILogger<ReadNotificationHandler> logger
        )
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task Handle(ReadNotificationCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start marking notification with id = '{notificationId}' as read by user with login = '{UserLogin}'",
                command.NotificationId,
                command.UserLogin
            );

            var notification = await _notificationRepository.GetByIdAsync(
                command.NotificationId,
                cancellationToken
            ) ?? throw new NotificationNotFoundException(command.NotificationId);

            if (notification.UserLogin != command.UserLogin)
            {
                throw new UnauthorizedNotificationException(command.UserLogin, command.NotificationId);
            }
            if (notification.Status > NotificationStatus.Viewed)
            {
                throw new WrongNotificationStatusException(command.UserLogin, command.NotificationId, notification.Status);
            }

            await _notificationRepository.ChangeNotificationStatus(notification, NotificationStatus.Viewed, cancellationToken);

            _logger.LogInformation(
                "Successfully marked notification with id = '{notificationId}' as read by user with login = '{UserLogin}'",
                command.NotificationId,
                command.UserLogin
            );
        }
    }
}
