using MediatR;

namespace GameService.Application.Features.Lobby.Commands.DeleteLobby
{
    public record DeleteLobbyCommand(
        string Login,
        Guid LobbyId
    ) : IRequest<Unit>;
}
