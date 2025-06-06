using CommunicationService.Application.Dto.Notification;
using MediatR;

namespace CommunicationService.Application.Features.Notification.Queries.GetNotificationsQuery
{
    public record GetNotificationsQuery(
        string UserLogin,
        bool OnlyNew
    ) : IRequest<IEnumerable<NotificationDto>>;
}
