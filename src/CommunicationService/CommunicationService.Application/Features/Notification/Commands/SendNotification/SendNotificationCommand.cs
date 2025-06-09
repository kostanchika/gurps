using CommunicationService.Domain.Entities;
using MediatR;

namespace CommunicationService.Application.Features.Notification.Commands.SendNotification
{
    public record SendNotificationCommand(
        string RecipentLogin,
        string Name,
        string Content,
        List<MetaEntity> Meta
    ) : IRequest<Unit>;
}
