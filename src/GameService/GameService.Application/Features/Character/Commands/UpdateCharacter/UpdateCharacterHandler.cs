using GameService.Application.Exceptions.Character;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Character.Commands.UpdateCharacter
{
    public class UpdateCharacterHandler : IRequestHandler<UpdateCharacterCommand, Guid>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IImageService _imageService;
        private readonly ILogger<UpdateCharacterHandler> _logger;

        public UpdateCharacterHandler(
            ICharacterRepository characterRepository,
            IImageService imageService,
            ILogger<UpdateCharacterHandler> logger
        )
        {
            _characterRepository = characterRepository;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Guid> Handle(UpdateCharacterCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start updating character with Id = '{Id}' for user with Login = '{Login}'",
                command.CharacterId,
                command.Login
            );

            var character = await _characterRepository.GetByIdAsync(command.CharacterId, cancellationToken)
                ?? throw new CharacterNotFoundException(command.CharacterId);

            if (character.UserLogin != command.Login)
            {
                throw new CharacterOwnershipException(command.Login, character.Id);
            }

            character.Name = command.Name;
            character.World = command.World;
            character.History = command.History;
            character.Appearence = command.Appearence;
            character.Attributes = command.Attributes;

            await _imageService.DeleteImageAsync(character.AvatarPath, cancellationToken);
            character.AvatarPath = await _imageService.SaveImageAsync(command.Base64Avatar, cancellationToken);

            _logger.LogInformation(
                "Successfully updated character with Id = '{Id}' for user with Login = '{Login}'",
                command.CharacterId,
                command.Login
            );

            return command.CharacterId;
        }
    }
}
