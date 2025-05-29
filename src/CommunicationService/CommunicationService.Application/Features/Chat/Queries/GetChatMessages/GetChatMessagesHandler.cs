using AutoMapper;
using CommunicationService.Application.Dto;
using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Exceptions.Chat;
using CommunicationService.Application.Features.Chat.Queries.GetUserChats;
using CommunicationService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Queries.GetChatMessages
{
    public class GetChatMessagesHandler : IRequestHandler<GetChatMessagesQuery, PagedResultDto<MessageDto>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserChatsHandler> _logger;

        public GetChatMessagesHandler(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            IMapper mapper,
            ILogger<GetUserChatsHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResultDto<MessageDto>> Handle(
            GetChatMessagesQuery query,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation(
                "Start getting messages for chat with id = '{ChatId}' for user with login = '{UserLogin}', " +
                "page = {Page}, pageSize = {PageSize}",
                query.ChatId,
                query.UserLogin,
                query.PageNumber,
                query.PageSize
            );

            var existingChat = await _chatRepository.GetByIdAsync(
                query.ChatId,
                cancellationToken
            ) ?? throw new ChatNotFoundException(query.ChatId);

            if (!existingChat.Participants.Contains(query.UserLogin))
            {
                throw new UnauthorizedChatException(query.UserLogin, query.ChatId);
            }

            var messages = await _messageRepository.GetChatMessagesAsync(
                query.ChatId,
                query.PageNumber,
                query.PageSize,
                cancellationToken
            );

            var messagesCount = await _messageRepository.CountChatMessagesAsync(
                query.ChatId,
                cancellationToken
            );

            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);

            _logger.LogInformation(
                "Successfully got messages for chat with id = '{ChatId}' for user with login = '{UserLogin}', " +
                "page = {Page}, pageSize = {PageSize}",
                query.ChatId,
                query.UserLogin,
                query.PageNumber,
                query.PageSize
            );

            var pagedResultDto = new PagedResultDto<MessageDto>
            {
                Items = messageDtos,
                TotalCount = messagesCount
            };

            return pagedResultDto;
        }
    }
}
