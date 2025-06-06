using CommunicationService.Application.Dto;
using CommunicationService.Application.Dto.Chat;
using MediatR;

namespace CommunicationService.Application.Features.Chat.Queries.GetUserChats
{
    public record GetUserChatsQuery(
        string UserLogin,
        string? ChatName,
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        string? SortType
    ) : IRequest<PagedResultDto<ChatDto>>;
}
