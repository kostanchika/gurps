using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Interfaces;
using MediatR;

namespace CommunicationService.Application.Features.Chat.Commands.SendMessage
{
    public record SendMessageCommand(
        string UserLogin,
        string ChatId,
        string Message,
        IAttachment? Attachment
    ) : IRequest<MessageDto>;
}
