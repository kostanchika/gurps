using MediatR;

namespace GameService.Application.Features.Lobby.Commands.CreateLobby
{
    public record CreateLobbyCommand(
        string Login,
        string? Password
    ) : IRequest<Guid>;
}
