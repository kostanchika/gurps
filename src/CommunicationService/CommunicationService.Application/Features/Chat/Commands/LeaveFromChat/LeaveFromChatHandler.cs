using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Features.Chat.Commands.SendMessage;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.LeaveFromChat
{
    public class LeaveFromChatHandler : IRequestHandler<LeaveFromChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatService _chatService;
        private readonly ISender _sender;
        private readonly ILogger<LeaveFromChatHandler> _logger;

        public LeaveFromChatHandler(
            IChatRepository chatRepository,
            IChatService chatService,
            ISender sender,
            ILogger<LeaveFromChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _chatService = chatService;
            _sender = sender;
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

            var sendMessageCommand = new SendMessageCommand(
                command.UserLogin,
                command.ChatId,
                ":Leave",
                null,
                true
            );

            await _sender.Send(sendMessageCommand, cancellationToken);

            existingChat.Participants.Remove(command.UserLogin);

            if (existingChat.OwnerLogin == command.UserLogin && existingChat.Participants.Count != 0)
            {
                existingChat.OwnerLogin = existingChat.Participants[0];

                var sendNewAdminMessageCommand = new SendMessageCommand(
                    command.UserLogin,
                    command.ChatId,
                    $":Admin:By:{command.UserLogin}",
                    null,
                    true
                );

                await _sender.Send(sendNewAdminMessageCommand, cancellationToken);

                _logger.LogInformation(
                    "Chat with id = {ChatId} lost it owner, new owner is {NewOwner}",
                    command.ChatId,
                    existingChat.OwnerLogin
                );
            }

            await _chatRepository.UpdateAsync(existingChat, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            await _chatService.RemoveUserFromChatAsync(command.UserLogin, command.ChatId, cancellationToken);

            _logger.LogInformation(
                "User with login = '{UserLogin}' successfully left chat with id = '{ChatId}'",
                command.UserLogin,
                command.ChatId
            );
        }
    }
}
