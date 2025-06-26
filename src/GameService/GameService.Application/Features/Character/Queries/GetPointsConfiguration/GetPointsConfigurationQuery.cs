using GameService.Application.DTOs.Game;
using MediatR;

namespace GameService.Application.Features.Character.Queries.GetPointsConfiguration
{
    public record GetPointsConfigurationQuery(
        
    ) : IRequest<PointsConfigurationDto>;
}
