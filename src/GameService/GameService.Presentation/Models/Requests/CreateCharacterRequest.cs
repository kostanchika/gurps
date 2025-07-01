using GURPS.Character.Entities.CharacterProperties;

namespace GameService.Presentation.Models.Requests
{
    public record CreateCharacterRequest(
        string Name,
        string Base64Avatar,
        string World,
        string History,
        Appearence Appearence,
        Attributes Attributes
    );
}
