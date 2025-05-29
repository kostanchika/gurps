using AutoMapper;
using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Exceptions.Message;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.DeleteMessage
{
    public class DeleteMessageHandler : IRequestHandler<DeleteMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteMessageHandler> _logger;

        public DeleteMessageHandler(
            IMessageRepository messageRepository,
            IChatService chatService,
            IMapper mapper,
            ILogger<DeleteMessageHandler> logger
        )
        {
            _messageRepository = messageRepository;
            _chatService = chatService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MessageDto> Handle(DeleteMessageCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start deleting message with id = {MessageId} by user with login = {UserLogin}",
                command.MessageId,
                command.UserLogin
            );

            var existingMessage = await _messageRepository.GetByIdAsync(command.MessageId, cancellationToken)
                ?? throw new MessageNotFoundException(command.MessageId);

            if (existingMessage.SenderLogin != command.UserLogin)
            {
                throw new UnauthorizedMessageException(command.UserLogin, command.MessageId);
            }

            if (!existingMessage.CanDelete)
            {
                throw new UnauthorizedMessageException(command.UserLogin, command.MessageId);
            }

            await _messageRepository.MarkMessageAsDeletedAsync(command.MessageId, cancellationToken);
            await _messageRepository.SaveChangesAsync(cancellationToken);

            await _chatService.NotifyMessageDeletedAsync(existingMessage.Id, existingMessage.ChatId, cancellationToken);

            var messageDto = _mapper.Map<MessageDto>(existingMessage);

            _logger.LogInformation(
                "Successfully deleted message with id = {MessageId} by user with login = {UserLogin}",
                command.MessageId,
                command.UserLogin
            );

            return messageDto;
        }
    }
}
