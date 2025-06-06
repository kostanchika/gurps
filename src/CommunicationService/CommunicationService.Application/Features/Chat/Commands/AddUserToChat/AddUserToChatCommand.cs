using CommunicationService.Application.Dto.Chat;
using MediatR;

namespace CommunicationService.Application.Features.Chat.Commands.AddUserToChat
{
    public record AddUserToChatCommand(
        string UserLogin,
        string ChatId,
        string InviteeLogin
    ) : IRequest<string>;
}
