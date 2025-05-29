using AutoMapper;
using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.LeaveFromChat
{
    public class LeaveFromChatHandler : IRequestHandler<LeaveFromChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;
        private readonly ILogger<LeaveFromChatHandler> _logger;

        public LeaveFromChatHandler(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            IChatService chatService,
            IMapper mapper,
            ILogger<LeaveFromChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _chatService = chatService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(LeaveFromChatCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "User with login = '{UserLogin}' leaves chat with id = '{ChatId}'",
                command.UserLogin,
                command.ChatId
            );

            var existingChat = await _chatRepository.GetByIdAsync(command.ChatId, cancellationToken)
                ?? throw new ChatNotFoundException(command.ChatId);

            if (!existingChat.Participants.Contains(command.UserLogin))
            {
                throw new ParticipantNotFoundException(command.UserLogin, existingChat.Id);
            }

            var leavingMessage = new MessageEntity
            {
                Id = Guid.NewGuid().ToString(),
                SenderLogin = command.UserLogin,
                Status = MessageStatus.Created,
                Attachment = null,
                CanDelete = false,
                ChatId = command.ChatId,
                Content = ":Leave",
                CreatedAt = DateTime.UtcNow,
            };

            await _messageRepository.AddAsync(leavingMessage, cancellationToken);
            await _messageRepository.SaveChangesAsync(cancellationToken);

            existingChat.Participants.Remove(command.UserLogin);

            if (existingChat.OwnerLogin == command.UserLogin && existingChat.Participants.Count != 0)
            {
                existingChat.OwnerLogin = existingChat.Participants[0];

                var newAdminMessage = new MessageEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderLogin = existingChat.OwnerLogin,
                    Status = MessageStatus.Created,
                    Attachment = null,
                    CanDelete = false,
                    ChatId = command.ChatId,
                    Content = $":Admin:By:{command.UserLogin}",
                    CreatedAt = DateTime.UtcNow,
                };

                _logger.LogInformation(
                    "Chat with id = {ChatId} lost it owner, new owner is {NewOwner}",
                    command.ChatId,
                    existingChat.OwnerLogin
                );

                await _messageRepository.AddAsync(newAdminMessage, cancellationToken);
                await _messageRepository.SaveChangesAsync(cancellationToken);

                var newAdminMessageDto = _mapper.Map<MessageDto>(newAdminMessage);

                await _chatService.NotifyMessageSentAsync(newAdminMessageDto, cancellationToken);
            }

            await _chatRepository.UpdateAsync(existingChat, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            var messageDto = _mapper.Map<MessageDto>(leavingMessage);
            await _chatService.NotifyMessageSentAsync(messageDto, cancellationToken);

            await _chatService.RemoveUserFromChatAsync(command.UserLogin, command.ChatId, cancellationToken);

            _logger.LogInformation(
                "User with login = '{UserLogin}' successfully left chat with id = '{ChatId}'",
                command.UserLogin,
                command.ChatId
            );
        }
    }
}
