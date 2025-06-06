using AutoMapper;
using CommunicationService.Application.Dto;
using CommunicationService.Application.Dto.Chat;
using CommunicationService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Features.Chat.Queries.GetUserChats
{
    public class GetUserChatsHandler : IRequestHandler<GetUserChatsQuery, PagedResultDto<ChatDto>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserChatsHandler> _logger;

        public GetUserChatsHandler(
            IChatRepository chatRepository,
            IMapper mapper,
            ILogger<GetUserChatsHandler> logger
        )
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResultDto<ChatDto>> Handle(
            GetUserChatsQuery query,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation(
                "Start getting chats for user with id = '{Id}', page = {Page}, pageSize = {PageSize}",
                query.UserLogin,
                query.PageNumber,
                query.PageSize
            );

            var chats = await _chatRepository.GetUserChats(
                query.UserLogin,
                query.ChatName,
                query.PageNumber,
                query.PageSize,
                query.SortBy,
                query.SortType,
                cancellationToken
            );

            var chatsCount = await _chatRepository.CountUserChats(
                query.UserLogin,
                query.ChatName,
                cancellationToken
            );

            var chatDtos = _mapper.Map<IEnumerable<ChatDto>>(chats);
            _logger.LogInformation(
                "Successfully got chats for user with id = '{Id}', page = {Page}, pageSize = {PageSize}",
                query.UserLogin,
                query.PageNumber,
                query.PageSize
            );

            var pagedResultDto = new PagedResultDto<ChatDto>()
            {
                Items = chatDtos,
                TotalCount = chatsCount
            };

            return pagedResultDto;
        }
    }
}
