using GURPS.Character.Entities.CharacterProperties;

namespace UsersService.Application.DTOs.Character
{
    public record CreateCharacterDto(
        string Name,
        string Base64Avatar,
        string World,
        string History,
        Appearence Appearence,
        Attributes Attributes
    );
}
