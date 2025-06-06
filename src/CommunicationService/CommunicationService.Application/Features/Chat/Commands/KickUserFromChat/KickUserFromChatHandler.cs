using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Features.Chat.Commands.SendMessage;
using CommunicationService.Application.Features.Notification.Commands.SendNotification;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.KickUserFromChat
{
    public class KickUserFromChatHandler : IRequestHandler<KickUserFromChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatService _chatService;
        private readonly ISender _sender;
        private readonly ILogger<KickUserFromChatHandler> _logger;

        public KickUserFromChatHandler(
            IChatRepository chatRepository,
            IChatService chatService,
            ISender sender,
            ILogger<KickUserFromChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _chatService = chatService;
            _sender = sender;
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

            var sendMessageCommand = new SendMessageCommand(
                command.KickeeLogin,
                command.ChatId,
                $":Kick:By:{command.UserLogin}",
                null,
                true
            );

            await _sender.Send(sendMessageCommand, cancellationToken);

            existingChat.Participants.Remove(command.KickeeLogin);
            await _chatRepository.UpdateAsync(existingChat, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            await _chatService.RemoveUserFromChatAsync(command.KickeeLogin, command.ChatId, cancellationToken);

            var sendNotificationCommand = new SendNotificationCommand(
                command.KickeeLogin,
                "Kick",
                $"You were kicked from chat '{existingChat.Name}'",
                [new MetaEntity { Name = "ChatId", Description = existingChat.Id }]
            );

            await _sender.Send(sendNotificationCommand, cancellationToken);

            _logger.LogInformation(
                "User with login = '{UserLogin}' successfully kicked user with login = '{KickeeLogin}' from chat with id = '{ChatId}'",
                command.UserLogin,
                command.KickeeLogin,
                command.ChatId
            );
        }
    }
}
