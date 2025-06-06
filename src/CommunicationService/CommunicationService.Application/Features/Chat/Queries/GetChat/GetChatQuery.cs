using CommunicationService.Application.Dto.Chat;
using MediatR;

namespace CommunicationService.Application.Features.Chat.Queries.GetChat
{
    public record GetChatQuery(
        string UserLogin,
        string ChatId
    ) : IRequest<ChatDto>;
}
