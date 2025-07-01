using GameService.Application.DTOs.Game;
using GURPS.Character.Entities;

namespace GameService.Application.Interfaces.Services
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
