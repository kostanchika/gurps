using MediatR;

namespace GameService.Application.Features.Game.Commands.EndGame
{
    public record EndGameCommand(
        Guid LobbyId,
        string Login
    ) : IRequest<Unit>;
}
