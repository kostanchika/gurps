using AutoMapper;
using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Dto.Notification;
using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.KickUserFromChat
{
    public class KickUserFromChatHandler : IRequestHandler<KickUserFromChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IChatService _chatService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<KickUserFromChatHandler> _logger;

        public KickUserFromChatHandler(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            INotificationRepository notificationRepository,
            IChatService chatService,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<KickUserFromChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _notificationRepository = notificationRepository;
            _chatService = chatService;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(KickUserFromChatCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "User with login = '{UserLogin}' kicks user with login = '{KickeeLogin}' from chat with id = '{ChatId}'",
                command.UserLogin,
                command.KickeeLogin,
                command.ChatId
            );

            var existingChat = await _chatRepository.GetByIdAsync(command.ChatId, cancellationToken)
                ?? throw new ChatNotFoundException(command.ChatId);

            if (existingChat.OwnerLogin != command.UserLogin)
            {
                throw new UnauthorizedChatException(command.UserLogin, existingChat.Id);
            }

            if (!existingChat.Participants.Contains(command.KickeeLogin))
            {
                throw new ParticipantNotFoundException(command.KickeeLogin, existingChat.Id);
            }

            var leavingMessage = new MessageEntity
            {
                Id = Guid.NewGuid().ToString(),
                SenderLogin = command.KickeeLogin,
                Status = MessageStatus.Created,
                Attachment = null,
                CanDelete = false,
                ChatId = command.ChatId,
                Content = $":Kick:By:{command.UserLogin}",
                CreatedAt = DateTime.UtcNow,
            };

            await _messageRepository.AddAsync(leavingMessage, cancellationToken);
            await _messageRepository.SaveChangesAsync(cancellationToken);

            existingChat.Participants.Remove(command.KickeeLogin);
            await _chatRepository.UpdateAsync(existingChat, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            var messageDto = _mapper.Map<MessageDto>(leavingMessage);
            await _chatService.NotifyMessageSentAsync(messageDto, cancellationToken);

            var notification = new NotificationEntity
            {
                Name = "Kick",
                Content = $"You were kicked from chat '{existingChat.Name}'",
                Meta = [new MetaEntity { Name = "ChatId", Description = existingChat.Id }],
                Status = NotificationStatus.Created,
                CreatedAt = DateTime.UtcNow,
                ViewedAt = DateTime.MaxValue,
                UserLogin = command.KickeeLogin
            };

            var notificationDto = _mapper.Map<NotificationDto>(notification);

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _notificationRepository.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyNotificationSent(notificationDto, cancellationToken);

            await _chatService.RemoveUserFromChatAsync(command.KickeeLogin, command.ChatId, cancellationToken);

            _logger.LogInformation(
                "User with login = '{UserLogin}' successfully kicked user with login = '{KickeeLogin}' from chat with id = '{ChatId}'",
                command.UserLogin,
                command.KickeeLogin,
                command.ChatId
            );
        }
    }
}
