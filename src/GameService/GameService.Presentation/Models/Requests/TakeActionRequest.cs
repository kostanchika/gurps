using GURPS.Character.Entities.CharacterProperties;

namespace GameService.Presentation.Models.Requests
{
    public record TakeActionRequest(
        List<Item> Items,
        List<Skill> Skills,
        List<Trait> Traits
    );
}
