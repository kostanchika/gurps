using CommunicationService.Application.Dto.Message;
using MediatR;

namespace CommunicationService.Application.Features.Chat.Commands.DeleteMessage
{
    public record DeleteMessageCommand(
        string UserLogin,
        string MessageId
    ) : IRequest;
}
