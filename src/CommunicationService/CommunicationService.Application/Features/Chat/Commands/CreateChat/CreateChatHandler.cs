using AutoMapper;
using CommunicationService.Application.Dto.Chat;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Commands.CreateChat
{
    public class CreateChatHandler : IRequestHandler<CreateChatCommand, ChatDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateChatHandler> _logger;

        public CreateChatHandler(
            IChatRepository chatRepository,
            IAttachmentService attachmentService,
            IMapper mapper,
            ILogger<CreateChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _attachmentService = attachmentService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ChatDto> Handle(CreateChatCommand command, CancellationToken cancellationToken)
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

            var chatDto = _mapper.Map<ChatDto>(chat);

            _logger.LogInformation(
                "Successfuuly created chat with name = '{Name}' by user with login = '{UserLogin}'",
                command.Name,
                command.UserLogin
            );

            return chatDto;
        }
    }
}
