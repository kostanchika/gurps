using GURPS.Character.Entities.CharacterProperties;
using MediatR;

namespace GameService.Application.Features.Character.Commands.CreateCharacter
{
    public record CreateCharacterCommand(
        string Login,
        string Name,
        string Base64Avatar,
        string World,
        string History,
        Appearence Appearence,
        Attributes Attributes
    ) : IRequest<Guid>;
}
