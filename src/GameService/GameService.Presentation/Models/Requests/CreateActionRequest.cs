using GURPS.Character.Enums;

namespace GameService.Presentation.Models.Requests
{
    public record CreateActionRequest(
        string Name,
        string Content,
        string RecipientLogin,
        CharacterAttribute? Attribute,
        int? Dice
    );
}
