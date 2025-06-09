using CommunicationService.Application.Interfaces;
using MediatR;

namespace CommunicationService.Application.Features.Chat.Commands.CreateChat
{
    public record CreateChatCommand(
        string UserLogin,
        string Name,
        string[]? Participants,
        IAttachment Logo
    ) : IRequest<string>;
}
