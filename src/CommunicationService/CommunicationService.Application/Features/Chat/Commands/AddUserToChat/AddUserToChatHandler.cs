using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Features.Chat.Commands.SendMessage;
using CommunicationService.Application.Features.Notification.Commands.SendNotification;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.AddUserToChat
{
    public class AddUserToChatHandler : IRequestHandler<AddUserToChatCommand, string>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatService _chatService;
        private readonly IFriendsService _friendsService;
        private readonly ISender _sender;
        private readonly ILogger<AddUserToChatHandler> _logger;

        public AddUserToChatHandler(
            IChatRepository chatRepository,
            IChatService chatService,
            IFriendsService friendsService,
            ISender sender,
            ILogger<AddUserToChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _chatService = chatService;
            _friendsService = friendsService;
            _sender = sender;
            _logger = logger;
        }

        public async Task<string> Handle(AddUserToChatCommand command, CancellationToken cancellationToken)
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

            var sendMessageCommand = new SendMessageCommand(
                command.InviteeLogin,
                command.ChatId,
                $":Add:By:{command.UserLogin}",
                null,
                true
            );

            await _sender.Send(sendMessageCommand, cancellationToken);

            var sendNotificationCommand = new SendNotificationCommand(
                command.InviteeLogin,
                "Invitation",
                $"You were invited to chat '{existingChat.Name}'",
                [new MetaEntity { Name = "ChatId", Description = existingChat.Id }]
            );

            await _sender.Send(sendNotificationCommand, cancellationToken);

            await _chatService.AddUserToChatAsync(command.InviteeLogin, command.ChatId, cancellationToken);

            _logger.LogInformation(
                "User with login = '{InviterLogin}' successfully added user with login = '{InviteeLogin}' " +
                "to chat with id = '{ChatId}'",
                command.UserLogin,
                command.InviteeLogin,
                command.ChatId
            );

            return existingChat.Id;
        }
    }
}
