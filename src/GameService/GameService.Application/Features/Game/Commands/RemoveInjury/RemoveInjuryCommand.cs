using MediatR;

namespace GameService.Application.Features.Game.Commands.RemoveInjury
{
    public record RemoveInjuryCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        string Name
    ) : IRequest<Unit>;
}
