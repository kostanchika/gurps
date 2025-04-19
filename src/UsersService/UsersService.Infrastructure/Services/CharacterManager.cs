using GURPS.Character.Entities;
using GURPS.Character.Providers.Interfaces;
using UsersService.Application.DTOs.Character;
using UsersService.Application.Interfaces.Services;

namespace UsersService.Infrastructure.Services
{
    public class CharacterManager : ICharacterManager
    {
        private readonly ICharacterCalculator _characterCalculator;

        public CharacterManager(ICharacterCalculator characterCalculator)
        {
            _characterCalculator = characterCalculator;
        }

        public int StartPoints => _characterCalculator.Configuration.StartPoints;
        public int StartCoins => _characterCalculator.Configuration.StartCoins;

        public Task<PointsConfigurationDto> GetPointsConfigurationAsync(
            CancellationToken cancellationToken = default
        )
        {
            var configuration = _characterCalculator.Configuration;

            var pointsConfigurationDto = new PointsConfigurationDto
            {
                StartPoints = configuration.StartPoints,
                StartCoins = configuration.StartCoins,
                AttributePrices = configuration.AttributePrices,
                DefaultAttributes = configuration.DefaultAttributes,
                Coefficients = configuration.Coefficients
            };

            return Task.FromResult(pointsConfigurationDto);
        }

        public async Task<bool> ValidateCharacterPointsAsync(
            CharacterEntity characterEntity, 
            CancellationToken cancellationToken = default
        )
        {
            return await _characterCalculator.GetCharacterCostInPointsAsync(
                characterEntity,
                cancellationToken
            ) <= characterEntity.SummaryPoints;
        }
    }
}
