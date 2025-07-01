using GameService.Application.DTOs.Game;
using GameService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Character.Queries.GetPointsConfiguration
{
    public class GetPointsConfigurationHandler : IRequestHandler<GetPointsConfigurationQuery, PointsConfigurationDto>
    {
        private readonly ICharacterManager _characterManager;
        private readonly ILogger<GetPointsConfigurationHandler> _logger;

        public GetPointsConfigurationHandler(
            ICharacterManager characterManager,
            ILogger<GetPointsConfigurationHandler> logger
        )
        {
            _characterManager = characterManager;
            _logger = logger;
        }

        public async Task<PointsConfigurationDto> Handle(GetPointsConfigurationQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start getting points configuration");

            var pointsConfigurationDto = await _characterManager.GetPointsConfigurationAsync(cancellationToken);

            _logger.LogInformation("Successfully got points configuration");

            return pointsConfigurationDto;
        }
    }
}
