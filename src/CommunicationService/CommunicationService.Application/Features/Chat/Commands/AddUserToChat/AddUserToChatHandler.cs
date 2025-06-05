using AutoMapper;
using CommunicationService.Application.Dto.Chat;
using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Dto.Notification;
using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.AddUserToChat
{
    public class AddUserToChatHandler : IRequestHandler<AddUserToChatCommand, ChatDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatService _chatService;
        private readonly IFriendsService _friendsService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserToChatHandler> _logger;

        public AddUserToChatHandler(
            IChatRepository chatRepository,
            INotificationRepository notificationRepository,
            IMessageRepository messageRepository,
            IChatService chatService,
            IFriendsService friendsService,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<AddUserToChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _notificationRepository = notificationRepository;
            _messageRepository = messageRepository;
            _chatService = chatService;
            _friendsService = friendsService;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ChatDto> Handle(AddUserToChatCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "User with login = '{InviterLogin}' add user with login = '{InviteeLgin}' to chat with id = '{ChatId}'",
                command.UserLogin,
                command.InviteeLogin,
                command.ChatId
            );

            var existingChat = await _chatRepository.GetByIdAsync(command.ChatId, cancellationToken)
                ?? throw new ChatNotFoundException(command.ChatId);

            if (!existingChat.Participants.Contains(command.UserLogin))
            {
                throw new UnauthorizedChatException(command.UserLogin, command.ChatId);
            }

            var areFriends = await _friendsService.AreFriendsAsync(command.UserLogin, command.InviteeLogin, cancellationToken);
            if (!areFriends)
            {
                throw new CantAddUserToChatException(command.UserLogin, command.InviteeLogin);
            }

            if (existingChat.Participants.Contains(command.InviteeLogin))
            {
                throw new DuplicateParticipantException(command.InviteeLogin, command.ChatId);
            }

            existingChat.Participants.Add(command.InviteeLogin);
            await _chatRepository.UpdateAsync(existingChat, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            var message = new MessageEntity
            {
                Id = Guid.NewGuid().ToString(),
                SenderLogin = command.InviteeLogin,
                Attachment = null,
                CanDelete = false,
                ChatId = command.ChatId,
                Content = $":Add:By:{command.UserLogin}",
                Status = MessageStatus.Created,
                CreatedAt = DateTime.UtcNow,
            };

            await _messageRepository.AddAsync(message, cancellationToken);
            await _messageRepository.SaveChangesAsync(cancellationToken);

            var messageDto = _mapper.Map<MessageDto>(message);

            await _chatService.NotifyMessageSentAsync(messageDto, cancellationToken);

            var notification = new NotificationEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Invitation",
                Content = $"You were invited to chat '{existingChat.Name}'",
                Meta = [new MetaEntity { Name = "ChatId", Description = existingChat.Id }],
                Status = NotificationStatus.Created,
                CreatedAt = DateTime.UtcNow,
                ViewedAt = DateTime.MaxValue,
                UserLogin = command.InviteeLogin
            };

            var notificationDto = _mapper.Map<NotificationDto>(notification);

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _notificationRepository.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyNotificationSent(notificationDto, cancellationToken);

            var chatDto = _mapper.Map<ChatDto>(existingChat);
            await _chatService.AddUserToChatAsync(command.InviteeLogin, command.ChatId, cancellationToken);

            _logger.LogInformation(
                "User with login = '{InviterLogin}' successfully added user with login = '{InviteeLogin}' " +
                "to chat with id = '{ChatId}'",
                command.UserLogin,
                command.InviteeLogin,
                command.ChatId
            );

            return chatDto;
        }
    }
}
