using GameService.Application.Models.Lobby;
using MediatR;

namespace GameService.Application.Features.Lobby.Queries.GetLobbyById
{
    public record GetLobbyByIdQuery(
        string Login,
        Guid Id
    ) : IRequest<LobbyDto>;
}
