using GURPS.Character.Entities;
using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Interfaces.Services
{
    public interface ICharacterManager
    {
        public int StartPoints { get; }
        public int StartCoins { get; }

        Task<PointsConfigurationDto> GetPointsConfigurationAsync(CancellationToken cancellationToken = default);

        Task<bool> ValidateCharacterPointsAsync(
            CharacterEntity characterEntity,
            CancellationToken cancellationToken = default
        );
    }
}
