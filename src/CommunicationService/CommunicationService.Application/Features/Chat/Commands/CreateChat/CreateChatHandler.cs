using AutoMapper;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.CreateChat
{
    public class CreateChatHandler : IRequestHandler<CreateChatCommand, string>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<CreateChatHandler> _logger;

        public CreateChatHandler(
            IChatRepository chatRepository,
            IAttachmentService attachmentService,
            ILogger<CreateChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _attachmentService = attachmentService;
            _logger = logger;
        }

        public async Task<string> Handle(CreateChatCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start creating chat with name = '{Name}' by user with login = '{UserLogin}'",
                command.Name,
                command.UserLogin
            );

            var filePath = await _attachmentService.SaveAttachmentAsync(command.Logo, cancellationToken);

            var chat = new ChatEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = command.Name,
                OwnerLogin = command.UserLogin,
                Participants = [.. command.Participants?.Where(p => p != command.UserLogin), command.UserLogin],
                LogoUri = await _attachmentService.GetFileUriAsync(filePath, cancellationToken)
            };

            await _chatRepository.AddAsync(chat, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfuuly created chat with name = '{Name}' by user with login = '{UserLogin}'",
                command.Name,
                command.UserLogin
            );

            return chat.Id;
        }
    }
}
