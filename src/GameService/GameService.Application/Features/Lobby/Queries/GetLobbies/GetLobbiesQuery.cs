using GameService.Application.Models;
using GameService.Application.Models.Lobby;
using MediatR;

namespace GameService.Application.Features.Lobby.Queries.GetLobbies
{
    public record GetLobbiesQuery(
        bool? IsPublic,
        int? PlayersCountFrom,
        int? PlayersCountTo,
        int? Page,
        int? PageSize
    ) : IRequest<PagedResultDto<LobbyDto>>;
}
