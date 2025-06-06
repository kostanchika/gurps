using AutoMapper;
using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.SendMessage
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;
        private readonly ILogger<SendMessageHandler> _logger;

        public SendMessageHandler(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            IAttachmentService attachmentService,
            IChatService chatService,
            IMapper mapper,
            ILogger<SendMessageHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _attachmentService = attachmentService;
            _chatService = chatService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start sending message from user with login = '{UserLogin}' to chat with id = '{ChatId}', " +
                "attachment: {HasAttachment}",
                command.UserLogin,
                command.ChatId,
                command.Attachment != null
            );

            var existingChat = await _chatRepository.GetByIdAsync(command.ChatId, cancellationToken)
                ?? throw new ChatNotFoundException(command.ChatId);

            if (!existingChat.Participants.Contains(command.UserLogin))
            {
                throw new UnauthorizedChatException(command.UserLogin, command.ChatId);
            }

            var message = new MessageEntity
            {
                Id = Guid.NewGuid().ToString(),
                SenderLogin = command.UserLogin,
                ChatId = command.ChatId,
                CanDelete = true,
                Status = MessageStatus.Created,
                Content = command.Message,
                CreatedAt = DateTime.UtcNow
            };

            if (command.Attachment != null)
            {
                var filePath = await _attachmentService.SaveAttachmentAsync(command.Attachment, cancellationToken);

                var attachment = new AttachmentEntity
                {
                    FileName = command.Attachment.FileName,
                    FilePath = filePath,
                    IsPreviewAvailable = await _attachmentService.IsImageAsync(filePath, cancellationToken),
                    FileUri = await _attachmentService.GetFileUriAsync(filePath, cancellationToken),
                    CreatedAt = DateTime.UtcNow
                };

                message.Attachment = attachment;
            }

            await _messageRepository.AddAsync(message, cancellationToken);
            await _messageRepository.SaveChangesAsync(cancellationToken);

            var messageDto = _mapper.Map<MessageDto>(message);
            await _chatService.NotifyMessageSentAsync(messageDto, cancellationToken);

            _logger.LogInformation(
                "Successfully sent message from user with login = '{UserLogin}' to chat with id = '{ChatId}', " +
                "attachment: {HasAttachment}",
                command.UserLogin,
                command.ChatId,
                command.Attachment != null
            );
        }
    }
}
