using MediatR;

namespace GameService.Application.Features.Game.Commands.StartGame
{
    public record StartGameCommand(
        Guid LobbyId,
        string Login
    ) : IRequest<Unit>;
}
