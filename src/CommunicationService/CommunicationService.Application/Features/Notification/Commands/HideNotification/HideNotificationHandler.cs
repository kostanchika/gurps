using CommunicationService.Application.Exceptions.Notification;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Notification.Commands.HideNotification
{
    public class HideNotificationHandler : IRequestHandler<HideNotificationCommand>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<HideNotificationCommand> _logger;

        public HideNotificationHandler(
            INotificationRepository notificationRepository,
            ILogger<HideNotificationCommand> logger
        )
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task Handle(HideNotificationCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start hiding notification with id = '{notificationId}' by user with login = '{UserLogin}'",
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

            await _notificationRepository.ChangeNotificationStatus(notification, NotificationStatus.Hidden, cancellationToken);

            _logger.LogInformation(
                "Successfully hid notification with id = '{notificationId}' by user with login = '{UserLogin}'",
                command.NotificationId,
                command.UserLogin
            );
        }
    }
}
