using GURPS.Character.Entities;

namespace GameService.Application.Interfaces.Services
{
    public interface ICharacterService
    {
        Task<CharacterEntity> GetCharacterAsync(
            int CharacterId,
            CancellationToken cancellationToken = default
        );
    }
}
