using GameService.Application.DTOs.Game;
using MediatR;

namespace GameService.Application.Features.Game.Queries.GetGameById
{
    public record GetGameByIdQuery(
        Guid LobbyId,
        string Login
    ) : IRequest<GameStateDto>;
}
