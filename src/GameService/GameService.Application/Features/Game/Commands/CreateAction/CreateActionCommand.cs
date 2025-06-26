using GURPS.Character.Enums;
using MediatR;

namespace GameService.Application.Features.Game.Commands.CreateAction
{
    public record CreateActionCommand(
        Guid LobbyId,
        string Login,
        string Name,
        string RecipientLogin,
        string Content,
        CharacterAttribute? Attribute,
        int? Dice
    ) : IRequest<Unit>;
}
