using MediatR;

namespace GameService.Application.Features.Character.Commands.DeleteCharcater
{
    public record DeleteCharacterCommand(
        string Login,
        Guid CharacterId
    ) : IRequest<Unit>;
}
