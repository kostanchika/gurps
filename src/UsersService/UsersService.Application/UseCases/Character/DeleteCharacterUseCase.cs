using GURPS.Character.Entities;
using Microsoft.Extensions.Logging;
using UsersService.Application.Exceptions.Auth;
using UsersService.Application.Exceptions.Character;
using UsersService.Application.Interfaces.UseCases.Character;
using UsersService.Application.Specifications.Auth;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Character
{
    public class DeleteCharacterUseCase : IDeleteCharacterUseCase
    {
        private readonly IRepository<CharacterEntity> _characterRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly ILogger<IDeleteCharacterUseCase> _logger;

        public DeleteCharacterUseCase(
            IRepository<CharacterEntity> characterRepository,
            IRepository<UserEntity> userRepository,
            ILogger<IDeleteCharacterUseCase> logger
        )
        {
            _characterRepository = characterRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync(string login, int characterId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Start deleting character with Id = '{CharacterId}' by user with Login = '{Login}'",
                characterId,
                login
            );

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(login),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", login);

            var character = await _characterRepository.GetByIdAsync(characterId, cancellationToken)
                ?? throw new CharacterNotFoundException(characterId);

            if (character.Id != user.Id)
            {
                throw new ForbiddenCharacterOperationException(login, "delete", characterId);
            }

            await _characterRepository.RemoveAsync(character, cancellationToken);
            await _characterRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully deleted character with Id = '{CharacterId}' by user with Login = '{Login}'",
                characterId,
                login
            );
        }
    }
}
