using MediatR;

namespace GameService.Application.Features.Game.Commands.CreateFatigue
{
    public record CreateFatigueCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        string Name,
        int FatiguePoints
    ) : IRequest<Unit>;
}
