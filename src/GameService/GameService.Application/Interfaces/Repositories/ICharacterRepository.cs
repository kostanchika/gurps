using GameService.Application.Features.Character.Queries.GetCharacters;
using GURPS.Character.Entities;

namespace GameService.Application.Interfaces.Repositories
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<CharacterEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CharacterEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CharacterEntity>> GetFilteredAsync(
            GetCharactersQuery filters,
            CancellationToken cancellationToken
        );
        Task<int> CountFilteredAsync(
            GetCharactersQuery filters,
            CancellationToken cancellationToken
        );
        Task AddAsync(CharacterEntity character, CancellationToken cancellationToken = default);
        Task UpdateAsync(CharacterEntity character, CancellationToken cancellationToken = default);
        Task RemoveAsync(CharacterEntity character, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
