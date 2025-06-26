using GURPS.Character.Entities.CharacterProperties;
using MediatR;

namespace GameService.Application.Features.Character.Commands.UpdateCharacter
{
    public record UpdateCharacterCommand(
        string Login,
        Guid CharacterId,
        string Name,
        string Base64Avatar,
        string World,
        string History,
        Appearence Appearence,
        Attributes Attributes
    ) : IRequest<Guid>;
}
