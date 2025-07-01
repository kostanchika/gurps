using GURPS.Character.Entities;
using MediatR;

namespace GameService.Application.Features.Character.Queries.GetCharacterById
{
    public record GetCharacterByIdQuery(
        Guid CharacterId
    ) : IRequest<CharacterEntity>;
}
