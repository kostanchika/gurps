using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;
using CommunicationService.Domain.Interfaces;

namespace CommunicationService.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IRepository<NotificationEntity>
    {
        public Task<IEnumerable<NotificationEntity>> GetNotificationsByUserLoginAsync(
            string userLogin,
            bool onlyNew,
            CancellationToken cancellationToken = default
        );

        public Task ChangeNotificationStatus(
            NotificationEntity notification,
            NotificationStatus status,
            CancellationToken cancellationToken = default
        );
    }
}
