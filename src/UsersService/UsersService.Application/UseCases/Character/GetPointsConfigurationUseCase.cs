using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Character;
using UsersService.Application.Interfaces.Services;
using UsersService.Application.Interfaces.UseCases.Character;

namespace UsersService.Application.UseCases.Character
{
    public class GetPointsConfigurationUseCase : IGetPointsConfigurationUseCase
    {
        private readonly ICharacterManager _characterManager;
        private readonly ILogger<GetPointsConfigurationUseCase> _logger;

        public GetPointsConfigurationUseCase(
            ICharacterManager characterManager,
            ILogger<GetPointsConfigurationUseCase> logger
        )
        {
            _characterManager = characterManager;
            _logger = logger;
        }

        public async Task<PointsConfigurationDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start getting points configuration");

            var pointsConfiguration = await _characterManager.GetPointsConfigurationAsync(cancellationToken);

            _logger.LogInformation("Successfully got points configuration");

            return pointsConfiguration;
        }
    }
}
