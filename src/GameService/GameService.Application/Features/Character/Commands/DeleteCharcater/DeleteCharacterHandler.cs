using GameService.Application.Exceptions.Character;
using GameService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Character.Commands.DeleteCharcater
{
    public class DeleteCharacterHandler
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<DeleteCharacterHandler> _logger;

        public DeleteCharacterHandler(
            ICharacterRepository characterRepository,
            ILogger<DeleteCharacterHandler > logger
        )
        {
            _characterRepository = characterRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteCharacterCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start deleting character with id = '{Id}'", command.CharacterId);

            var character = await _characterRepository.GetByIdAsync(command.CharacterId, cancellationToken)
                ?? throw new CharacterNotFoundException(command.CharacterId);

            if (character.UserLogin != command.Login)
            {
                throw new CharacterOwnershipException(command.Login, command.CharacterId);
            }

            await _characterRepository.RemoveAsync(character, cancellationToken);
            await _characterRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted character with id = '{Id}'", command.CharacterId);

            return Unit.Value;
        }
    }
}
