using MediatR;

namespace GameService.Application.Features.Game.Commands.GiveTrait
{
    public record GiveTraitCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        string Name,
        string Description,
        int Points
    ) : IRequest<Unit>;
}
