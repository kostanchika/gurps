using AutoMapper;
using CommunicationService.Application.Dto.Notification;
using CommunicationService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Notification.Queries.GetNotificationsQuery
{
    public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetNotificationsHandler> _logger;

        public GetNotificationsHandler(
            INotificationRepository notificationRepository,
            IMapper mapper,
            ILogger<GetNotificationsHandler> logger
        )
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<NotificationDto>> Handle(
            GetNotificationsQuery query,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Start getting unread notifications for user with login = '{UserLogin}'", query.UserLogin);

            var unreadNotifications = await _notificationRepository.GetNotificationsByUserLoginAsync(
                query.UserLogin,
                query.OnlyNew,
                cancellationToken
            );

            var unreadNotificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(unreadNotifications);

            _logger.LogInformation("Successfully got unread notifications for user with login = '{UserLogin}'", query.UserLogin);

            return unreadNotificationDtos;
        }
    }
}
