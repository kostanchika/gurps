using MediatR;

namespace GameService.Application.Features.Lobby.Commands.ConnectToLobby
{
    public record ConnectToLobbyCommand(
        string Login,
        Guid CharacterId,
        Guid LobbyId,
        string? Password
    ) : IRequest<Guid>;
}
