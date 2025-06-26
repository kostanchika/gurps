using GURPS.Character.Entities.CharacterProperties.Bonuses;
using MediatR;

namespace GameService.Application.Features.Game.Commands.GiveSkill
{
    public record GiveSkillCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        string Name,
        AttributeBonus AttributeBonus,
        int Points
    ) : IRequest<Unit>;
}
