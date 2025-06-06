using MediatR;

namespace CommunicationService.Application.Features.Chat.Commands.KickUserFromChat
{
    public record KickUserFromChatCommand(
        string UserLogin,
        string KickeeLogin,
        string ChatId
    ) : IRequest;
}
