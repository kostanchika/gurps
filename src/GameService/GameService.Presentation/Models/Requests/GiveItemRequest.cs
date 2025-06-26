using GURPS.Character.Entities.CharacterProperties.Bonuses;

namespace GameService.Presentation.Models.Requests
{
    public record GiveItemRequest(
        string RecipientLogin,
        string Name,
        string Description,
        string Type,
        int Quantity,
        int Price,
        float Weight,
        List<ArmorBonus> ArmorBonuses,
        List<DamageBonus> DamageBonuses
    );
}
