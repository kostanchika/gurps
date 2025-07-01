using MediatR;

namespace GameService.Application.Features.Game.Commands.RemoveFatigue
{
    public record RemoveFatigueCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        string Name
    ) : IRequest<Unit>;
}
