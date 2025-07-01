using MediatR;

namespace GameService.Application.Features.Game.Commands.GivePoints
{
    public record GivePointsCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        int Amount
    ) : IRequest<Unit>;
}
