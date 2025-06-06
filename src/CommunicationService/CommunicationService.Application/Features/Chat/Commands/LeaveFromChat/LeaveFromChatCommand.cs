using MediatR;

namespace CommunicationService.Application.Features.Chat.Commands.LeaveFromChat
{
    public record LeaveFromChatCommand(
        string UserLogin,
        string ChatId
    ) : IRequest;
}
