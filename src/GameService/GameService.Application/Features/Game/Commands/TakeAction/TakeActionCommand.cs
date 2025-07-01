using GURPS.Character.Entities.CharacterProperties;
using MediatR;

namespace GameService.Application.Features.Game.Commands.TakeAction
{
    public record TakeActionCommand(
        Guid LobbyId,
        string Login,
        List<Item> Items,
        List<Skill> Skills,
        List<Trait> Traits
    ) : IRequest<Unit>;
}
