using MediatR;

namespace GameService.Application.Features.Game.Commands.CreateInjury
{
    public record CreateInjuryCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        string Name,
        int HealthPoints
    ) : IRequest<Unit>;
}
