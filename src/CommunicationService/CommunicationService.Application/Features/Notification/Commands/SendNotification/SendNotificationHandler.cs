using AutoMapper;
using CommunicationService.Application.Dto.Notification;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Notification.Commands.SendNotification
{
    public class SendNotificationHandler : IRequestHandler<SendNotificationCommand, Unit>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<SendNotificationHandler> _logger;

        public SendNotificationHandler(
            INotificationRepository notificationRepository,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<SendNotificationHandler> logger
        )
        {
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start sending notification with name = '{Name}' to user with login = '{Login}'",
                request.Name,
                request.RecipentLogin
            );

            var notification = new NotificationEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Content = request.Content,
                Meta = request.Meta,
                UserLogin = request.RecipentLogin,
                CreatedAt = DateTime.UtcNow,
                ViewedAt = DateTime.MaxValue.ToUniversalTime(),
                Status = NotificationStatus.Created
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _notificationRepository.SaveChangesAsync(cancellationToken);

            var notificationDto = _mapper.Map<NotificationDto>(notification);

            await _notificationService.NotifyNotificationSent(notificationDto, cancellationToken);

            _logger.LogInformation(
                "Successfully sent notification with name = '{Name}' to user with login = '{Login}'",
                request.Name,
                request.RecipentLogin
            );

            return Unit.Value;
        }
    }
}
