using GameService.Application.Exceptions.Character;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GURPS.Character.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Character.Commands.CreateCharacter
{
    public class CreateCharacterHandler : IRequestHandler<CreateCharacterCommand, Guid>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICharacterManager _characterManager;
        private readonly IImageService _imageService;
        private readonly ILogger<CreateCharacterHandler> _logger;

        public CreateCharacterHandler(
            ICharacterRepository characterRepository,
            ICharacterManager characterManager,
            IImageService imageService,
            ILogger<CreateCharacterHandler> logger
        )
        {
            _characterRepository = characterRepository;
            _characterManager = characterManager;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start creating character with Name = '{Name}' for user with Login = '{Login}'",
                command.Name,
                command.Login
            );

            var character = new CharacterEntity
            {
                UserLogin = command.Login,
                Name = command.Name,
                Appearence = command.Appearence,
                History = command.History,
                World = command.World,
                Attributes = command.Attributes,
                Fatigues = [],
                Injuries = [],
                Inventory = [],
                Skills = [],
                Traits = [],
                CreatedAt = DateTime.UtcNow,
                SummaryPoints = _characterManager.StartPoints,
                Coins = _characterManager.StartCoins,
                AvatarPath = await _imageService.SaveImageAsync(command.Base64Avatar, cancellationToken)
            };

            if (!await _characterManager.ValidateCharacterPointsAsync(character, cancellationToken))
            {
                throw new InvalidCharacterStatsException(command.Login);
            }

            await _characterRepository.AddAsync(character, cancellationToken);
            await _characterRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully created character with Name = '{Name}' for user with Login = '{Login}'",
                command.Name,
                command.Login
            );

            return character.Id;
        }
    }
}
