using CommunicationService.Application.Dto;
using CommunicationService.Application.Dto.Message;
using MediatR;

namespace CommunicationService.Application.Features.Chat.Queries.GetChatMessages
{
    public record GetChatMessagesQuery(
        string UserLogin,
        string ChatId,
        int? PageNumber,
        int? PageSize
    ) : IRequest<PagedResultDto<MessageDto>>;
}
