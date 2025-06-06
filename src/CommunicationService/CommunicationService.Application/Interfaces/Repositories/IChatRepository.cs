using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;

namespace CommunicationService.Application.Interfaces.Repositories
{
    public interface IChatRepository : IRepository<ChatEntity>
    {
        Task<IEnumerable<ChatEntity>> GetUserChats(
            string userLogin,
            string? chatName,
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            string? sortType,
            CancellationToken cancellationToken = default
        );

        Task<int> CountUserChats(
            string userLogin,
            string? chatName,
            CancellationToken cancellation = default
        );
    }
}
