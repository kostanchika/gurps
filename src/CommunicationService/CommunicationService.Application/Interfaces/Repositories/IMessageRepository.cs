using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;

namespace CommunicationService.Application.Interfaces.Repositories
{
    public interface IMessageRepository : IRepository<MessageEntity>
    {
        Task<IEnumerable<MessageEntity>> GetChatMessagesAsync(
            string chatId,
            int? page,
            int? pageSize,
            CancellationToken cancellationToken = default
        );

        Task<int> CountChatMessagesAsync(
            string chatId,
            CancellationToken cancellationToken = default
        );

        Task MarkMessageAsDeletedAsync(
            string messageId,
            CancellationToken cancellationToken = default
        );
    }
}
