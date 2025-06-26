using GameService.Domain.Entities;

namespace GameService.Application.Interfaces.Repositories
{
    public interface ILobbyRepository
    {
        Task<LobbyEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<LobbyEntity?> GetByUserLoginAsync(string login, CancellationToken cancellationToken = default);
        Task<IEnumerable<LobbyEntity>> GetFilteredAsync(
            bool? isPublic,
            int? PlayersCountFrom,
            int? PlayerCountTo,
            int? page,
            int? pageSize,
            CancellationToken cancellationToken = default
        );
        Task<int> CountFilteredAsync(
            bool? isPublic,
            int? PlayersCountFrom,
            int? PlayersCountTo,
            CancellationToken cancellationToken = default
        );
        Task AddAsync(LobbyEntity lobby, CancellationToken cancellationToken = default);
        Task UpdateAsync(LobbyEntity lobby, CancellationToken cancellationToken = default);
        Task RemoveAsync(LobbyEntity lobby, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
