using AutoMapper;
using CommunicationService.Application.Dto.Chat;
using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Queries.GetChat
{
    public class GetChatHandler : IRequestHandler<GetChatQuery, ChatDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetChatHandler> _logger;

        public GetChatHandler(
            IChatRepository chatRepository,
            IMapper mapper,
            ILogger<GetChatHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ChatDto> Handle(GetChatQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start getting chat with id = '{ChatId}' by user with login = '{UserLogin}'",
                query.ChatId,
                query.UserLogin
            );

            var chat = await _chatRepository.GetByIdAsync(query.ChatId, cancellationToken)
                ?? throw new ChatNotFoundException(query.ChatId);

            if (!chat.Participants.Contains(query.UserLogin))
            {
                throw new UnauthorizedChatException(query.UserLogin, query.ChatId);
            }

            var chatDto = _mapper.Map<ChatDto>(chat);

            _logger.LogInformation(
                "Successfully got chat with id = '{ChatId}' by user with login = '{UserLogin}'",
                query.ChatId,
                query.UserLogin
            );

            return chatDto;
        }
    }
}
