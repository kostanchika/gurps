using MediatR;

namespace GameService.Application.Features.Lobby.Commands.DisconnectFromLobby
{
    public record DisconnectFromLobbyCommand(
        string Login,
        Guid LobbyId
    ) : IRequest<Guid>;
}
