using GameService.Application.DTOs.Game;
using GURPS.Character.Entities.CharacterProperties.Bonuses;
using MediatR;

namespace GameService.Application.Features.Game.Commands.GiveItem
{
    public record GiveItemCommand(
        Guid LobbyId,
        string Login,
        string RecipientLogin,
        string Name,
        string Description,
        string Type,
        int Quantity,
        int Price,
        float Weight,
        List<ArmorBonus> ArmorBonuses,
        List<DamageBonus> DamageBonuses
    ) : IRequest<Unit>;
}
