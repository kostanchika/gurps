using GURPS.Character.Entities.CharacterProperties.Bonuses;

namespace GameService.Presentation.Models.Requests
{
    public record GiveSkillRequest(
        string RecipientLogin,
        string Name,
        AttributeBonus AttributeBonus,
        int Points
    );
}
