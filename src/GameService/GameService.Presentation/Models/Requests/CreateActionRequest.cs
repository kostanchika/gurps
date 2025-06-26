using GURPS.Character.Enums;

namespace GameService.Presentation.Models.Requests
{
    public record CreateActionRequest(
        string Name,
        string Content,
        CharacterAttribute? Attribute,
        int? Dice
    );
}
