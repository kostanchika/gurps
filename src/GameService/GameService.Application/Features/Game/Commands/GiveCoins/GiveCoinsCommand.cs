using MediatR;

namespace GameService.Application.Features.Game.Commands.GiveCoins
{
    public record GiveCoinsCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        int Amount
    ) : IRequest<Unit>;
}
